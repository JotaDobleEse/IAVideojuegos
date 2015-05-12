using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveProject.CharacterTypes;

namespace WaveProject.Steerings.Pathfinding
{
    public enum Terrain
    {
        WATER, PATH, PLAIN, FOREST, DESERT
    }
    public enum DistanceAlgorith
    {
        MANHATTAN, CHEVYCHEV, EUCLIDEAN
    }

    public class LRTA
    {
        private Node[,] NodeMap;
        private CharacterType Character;
        private List<Vector2> StandardLocalSearchPattern = new List<Vector2>() { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
        private List<Vector2> SharpLocalSearchPattern = new List<Vector2>() { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(-1, 2), new Vector2(1, -2), new Vector2(-2, -1), new Vector2(2, -1), new Vector2(-2, 1), new Vector2(2, 1), new Vector2(-1, 2), new Vector2(1, 2) };
        private int ScaleWidth;
        private int ScaleHeight;
        public Vector2 StartPos { get; private set; }
        public Vector2 EndPos { get; private set; }

        public LRTA(Vector2 startPos, Vector2 endPos, CharacterType character, DistanceAlgorith algorithm = DistanceAlgorith.EUCLIDEAN)
        {
            Character = character;
            StartPos = startPos;
            EndPos = endPos;
            
            // Esto es para que los valores true o false recuperados de los tiles,
            // se formateen siempre como True o False.
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            // Creamos la matriz de nodos con el ancho y alto del mapa
            NodeMap = new Node[Map.CurrentMap.Width, Map.CurrentMap.Height];
            ScaleWidth = Map.CurrentMap.TileWidth;
            ScaleHeight = Map.CurrentMap.TileHeight;
            
            // Obtenemos la posición del tile objetivo para generar los valores heuristicos iniciales
            Vector2 endTile = Map.CurrentMap.TilePositionByWolrdPosition(endPos);

            for (int i = 0; i < NodeMap.GetLength(0); i++)
            {
                for (int j = 0; j < NodeMap.GetLength(1); j++)
                {
                    Node node = Map.CurrentMap.NodeMap[i, j].Clone() as Node;
                    if (node.Passable)
                        switch(algorithm)
                        { 
                            case DistanceAlgorith.MANHATTAN:
                                node.H = Manhatan(node.Position, endTile);
                                break;
                            case DistanceAlgorith.CHEVYCHEV:
                                node.H = Chebychev(node.Position, endTile);
                                break;
                            case DistanceAlgorith.EUCLIDEAN:
                                node.H = Euclidean(node.Position, endTile);
                                break;
                            default:
                                node.H = Euclidean(node.Position, endTile);
                                break;
                        }
                    NodeMap[i, j] = node;
                }
            }
        }

        /// <summary>
        /// Obtiene el camino aplicando el algoritmo LRTA.
        /// </summary>
        /// <returns></returns>
        public List<Vector2> Execute()
        {
            try
            {
                var startTile = Map.CurrentMap.TilePositionByWolrdPosition(StartPos);
                var endTile = Map.CurrentMap.TilePositionByWolrdPosition(EndPos);
                if (!NodeMap[startTile.X(), startTile.Y()].Passable || !NodeMap[endTile.X(), endTile.Y()].Passable)
                    return new List<Vector2>();
                return Execute(NodeMap[startTile.X(), startTile.Y()], NodeMap[endTile.X(), endTile.Y()]);
            }
            catch(Exception)
            {
                return new List<Vector2>();
            }
        }

        /// <summary>
        /// Obtiene el camino aplicando el algoritmo LRTA para un nodo origen y otro destino.
        /// </summary>
        /// <param name="start">Nodo origen.</param>
        /// <param name="end">Nodo destino.</param>
        /// <returns></returns>
        private List<Vector2> Execute(Node start, Node end)
        {
            // Matriz de guardado de camino
            Vector2[,] pathMatrix = new Vector2[NodeMap.GetLength(0), NodeMap.GetLength(1)];
            pathMatrix[start.X, start.Y] = start.Position;
            var current = start.Position;

            // Mientrasque no estamos en el nodo final
            while (current != end.Position)
            {
                // Obtenemos todos los nodos vecinos segun nuestro patrón de espacio de búsqueda local,
                // depués eliminamos los nodos final y no pasables.
                var neighbors = GetNeighbors(current, StandardLocalSearchPattern)
                    .Where(w => w != end && w.Passable);

                // Para cada nodo entre los vecinos guardamos su valor heurístico en Temp,
                // y ponemos H a infinito.
                foreach (var u in neighbors)
                {
                    u.Temp = u.H;
                    u.H = float.PositiveInfinity;
                }

                // Actualizamos los valores de los vecinos.
                UpdateStep(neighbors);

                // Mientras estemos en el espacio de búsqueda local
                do
                {
                    // Obtenemos el nodo actual
                    Node origin = NodeMap[current.X(), current.Y()];

                    // Pasamos a la mejor posición entre los vecinos
                    current = BestCandidate(origin);

                    // Si no hemos pasado por la casilla actual, almacenamos la posición
                    // desde la que hemos llegado al nuevo nodo actual.
                    if (pathMatrix[current.X(), current.Y()].IsNull())
                        pathMatrix[current.X(), current.Y()] = origin.Position;
                }
                while (neighbors.Any(a => a.X == current.X() && a.Y == current.Y()));
            }

            // Reconstruimos el camino de atrás hacia adelante.
            LinkedList<Vector2> path = new LinkedList<Vector2>();
            while (current != start.Position) // Mientras no estemos en la posición inicial
            {
                // Añadimos la posición actual
                path.AddFirst(current);
                // Retrocedemos a la posición desde la que hemos llegado a la actual
                current = pathMatrix[current.X(), current.Y()];
            }
            
            // Añadimos la posición del nodo inicial
            path.AddFirst(start.Position);

            // Recortamos esquinas del camino con el algoritmo String Pulling,
            // después convetimos las coordenadas de Tile a coordenadas de Mundo.
            return StringPulling(path.ToList())
                .Select(s => new Vector2(ScaleWidth * s.X + ScaleWidth / 2, ScaleHeight * s.Y + ScaleHeight / 2))
                .ToList();
        }

        #region Funciones auxiliares del LRTA*
        /// <summary>
        /// Obtiene los vecinos del nodo parámetro en base a un patrón de posicines.
        /// </summary>
        /// <param name="node">Nodo desde el que buscar.</param>
        /// <param name="positions">Posiciones que conforman el patrón, relativas al nodo origen</param>
        /// <returns></returns>
        private List<Node> GetNeighbors(Vector2 node, List<Vector2> positions)
        {
            List<Node> neighbors = new List<Node>();
            // Para cada posición del patrón
            foreach (var posicion in positions)
            {
                // Si existe un nodo en la posición especificada se añade a la lista
                if (NodeMap.Exists(node + posicion))
                    neighbors.Add(NodeMap[node.X() + posicion.X(), node.Y() + posicion.Y()]);
            }
            return neighbors;
        }

        /// <summary>
        /// Actualiza las heurísticas locales que los nodos del espacio de búsqueda perciben de sus vecinos.
        /// </summary>
        /// <param name="sLss">Nodos actuales del espacio de búsqueda local.</param>
        private void UpdateStep(IEnumerable<Node> sLss)
        {
            // Mientras haya nodos en el espaco de busqueda local con su heurística a infinito
            while (sLss.Any(a => float.IsInfinity(a.H)))
            {
                // Recorremos todos los nodos actualizando las heurísticas locales que tienen
                // de sus vecinos.
                foreach (var n in sLss)
                {
                    // Vecino de arriba
                    n.Hup = NodeMap.GetValueOrDefault(n.X, n.Y - 1, float.PositiveInfinity) + Weigth(n, NodeMap[n.X, n.Y - 1]);
                    // Vecino de abajo
                    n.Hdown = NodeMap.GetValueOrDefault(n.X, n.Y + 1, float.PositiveInfinity) + Weigth(n, NodeMap[n.X, n.Y + 1]);
                    // Vecino derecho
                    n.Hright = NodeMap.GetValueOrDefault(n.X + 1, n.Y, float.PositiveInfinity) + Weigth(n, NodeMap[n.X + 1, n.Y]);
                    // Vecino izquierdo
                    n.Hleft = NodeMap.GetValueOrDefault(n.X - 1, n.Y, float.PositiveInfinity) + Weigth(n, NodeMap[n.X - 1, n.Y]);

                    // Mejor heurístico local de los vecinos
                    n.Hneightbors = Math.Min(n.Hup, Math.Min(n.Hdown, Math.Min(n.Hleft, n.Hright)));
                }

                // Del espacio de búsqueda local nos quedamos con los que tienen el valor heurístico
                // a infinito, los ordenamos de menor a mayor por el heurístico local de sus vecinos
                // o su heurístico termporal, el que sea mayor. Finalmente nos quedamos con el primero.
                var v = sLss.Where(w => float.IsInfinity(w.H))
                    .OrderBy(o => Math.Max(o.Hneightbors, o.Temp))
                    .First();

                // Asignamos a nuestro mejor nodo seleccionado el máximo entre el heurístico local
                // de sus vecinos y su heurístico temporal.
                v.H = Math.Max(v.Hneightbors, v.Temp);

                // Si después de actualizar el heurístico sigue siendo Infinito, salimos del bucle
                if (float.IsInfinity(v.H))
                    break;
            }
        }

        /// <summary>
        /// Obtiene el mejor candito para continuar el camino desde un nodo.
        /// </summary>
        /// <param name="origin">Nodo desde el que nos vamos a mover.</param>
        /// <returns></returns>
        private Vector2 BestCandidate(Node origin)
        {
            Vector2 current = Vector2.Zero;// = origin.Position;
            // Comprobamos, basándonos en el heurístico local de los vecinos del nodo,
            // cual de sus vecinos es el que tiene ese valor y nos quedamos con el.
            if (origin.Hneightbors.Equal(origin.Hup))
            {
                current = new Vector2(origin.X, origin.Y - 1);
            }
            else if (origin.Hneightbors.Equal(origin.Hdown))
            {
                current = new Vector2(origin.X, origin.Y + 1);
            }
            else if (origin.Hneightbors.Equal(origin.Hright))
            {
                current = new Vector2(origin.X + 1, origin.Y);
            }
            else if (origin.Hneightbors.Equal(origin.Hleft))
            {
                current = new Vector2(origin.X - 1, origin.Y);
            }
            return current;
        }

        /// <summary>
        /// Coste de llegar al nodo objetivo.
        /// </summary>
        /// <param name="target">Nodo al que nos vamos a mover.</param>
        /// <returns></returns>
        private float Weigth(Node origin, Node target)
        {
            float n1 = Character.Cost(origin.Terrain);
            float n2 = Character.Cost(target.Terrain);
            return (n1 + n2) / 2;
        }
        #endregion

        #region My String Pulling
        /// <summary>
        /// Elimina puntos del camino para quitar esquinas.
        /// </summary>
        /// <param name="path">Camino original.</param>
        /// <returns></returns>
        private List<Vector2> StringPulling(List<Vector2> path)
        {
            for (int i = 0; i < path.Count-2; i++)
            {
                while ((i < path.Count - 2) && CanGoDirectly(path[i], path[i + 1], path[i + 2]))
                //if (CanDelete(path[i], path[i + 1], path[i + 2]))
                {
                    path.Remove(path[i + 1]);
                }
            }
            return path;
        }

        /// <summary>
        /// Comprueba si una posición es adyacente a otra.
        /// </summary>
        /// <param name="p1">Posición 1.</param>
        /// <param name="p2">Posición 2.</param>
        /// <returns></returns>
        private bool IsAdjacent(Vector2 p1, Vector2 p2)
        {
            if (p1 == new Vector2(p2.X + 1, p2.Y))
                return true;
            else if (p1 == new Vector2(p2.X - 1, p2.Y))
                return true;
            else if (p1 == new Vector2(p2.X, p2.Y + 1))
                return true;
            else if (p1 == new Vector2(p2.X, p2.Y - 1))
                return true;
            return false;
        }

        /// <summary>
        /// Indica si se puede eliminar el punto central de entre tres puntos.
        /// </summary>
        /// <param name="p1">Punto inicial.</param>
        /// <param name="p2">Punto intermedio, candidato a ser eliminado.</param>
        /// <param name="p3">Punto final.</param>
        /// <returns></returns>
        private bool CanDelete(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if (IsAdjacent(p1,p2) && IsAdjacent(p2,p3))
            {
                Vector2 newPos = (p1 - p2) + (p3 - p2);
                newPos += p2;
                if (p2 != newPos)
                {
                    return (NodeMap[newPos.X(), newPos.Y()].Passable);
                }
                else return true;
            }
            return false;
        }

        private bool CanGoDirectly(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var diff = p3 - p1;
            var max = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
            //diff.Normalize();
            Vector2 factor = new Vector2(diff.X / max, diff.Y / max);

            float lastCost = Character.Cost(NodeMap[p1.X(), p2.Y()].Terrain);
            Vector2 pAux = p1 + factor;
            Vector2 pAuxTile = new Vector2((int)Math.Round(pAux.X, 0), (int)Math.Round(pAux.Y, 0));

            while (pAuxTile != p3)
            {
                if (NodeMap[pAux.X(), pAux.Y()].Passable)
                {
                    float cost = Character.Cost(NodeMap[pAux.X(), pAux.Y()].Terrain);
                    if (cost <= lastCost /*|| cost == Character.Cost(NodeMap[p3.X(), p3.Y()].Terrain)*/)
                    {
                        lastCost = cost;
                        pAux += factor;
                        pAuxTile = new Vector2((int)Math.Round(pAux.X, 0), (int)Math.Round(pAux.Y, 0));
                    }
                    else
                        return false;
                }
                else 
                    return false;
            }

            return true;
        }
        #endregion

        #region Medidas de Distancia
        private float Manhatan(Vector2 i, Vector2 j, float minValue = 1f)
        {
            //h (n) = D * (abs (n.x-goal.x) + abs (n.y-goal.y)).
            return minValue * (Math.Abs(i.X - j.X) + Math.Abs(i.Y - j.Y));
        }

        private float Chebychev(Vector2 i, Vector2 j, float minValue = 1f)
        {
            //h (n) = D * max (abs (N.X-goal.x), abs (N.Y-goal.y))
            return minValue * Math.Max(Math.Abs(i.X - j.X), Math.Abs(i.Y - j.Y));
        }

        private float Euclidean(Vector2 i, Vector2 j, float minValue = 1f)
        {
            //h (n) = D * sqrt ((N.X-goal.x) ^ 2 + (N.Y-goal.y) ^ 2)
            return minValue * (float)Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
        }

        private float Manhatan(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * (abs (n.x-goal.x) + abs (n.y-goal.y)).
            return minValue * (Math.Abs(i.X - j.X) + Math.Abs(i.Y - j.Y));
        }

        private float Chebychev(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * max (abs (N.X-goal.x), abs (N.Y-goal.y))
            return minValue * Math.Max(Math.Abs(i.X - j.X), Math.Abs(i.Y - j.Y));
        }

        private float Euclidean(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * sqrt ((N.X-goal.x) ^ 2 + (N.Y-goal.y) ^ 2)
            return minValue * (float)Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
        }
        #endregion
    }
}
