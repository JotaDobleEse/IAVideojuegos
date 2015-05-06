using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings.Pathfinding
{
    public enum Terrain
    {
        WATER, PATH, PLAIN, FOREST, DESERT
    }

    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float H { get; set; }
        public float Temp { get; set; }

        public float Hup { get; set; }
        public float Hdown { get; set; }
        public float Hright { get; set; }
        public float Hleft { get; set; }
        public float Hneightbors { get; set; }

        public Terrain Terrain { get; set; }
        public bool Passable { get; set; }

        public Vector2 Position { get { return new Vector2(X, Y); } }

        public static bool operator ==(Node n1, Node n2)
        {
            int nulls = 0;
            try
            {
                int n = n1.X;
            }
            catch (Exception)
            {

                nulls++;
            }
            try
            {
                int n = n2.X;
            }
            catch (Exception)
            {

                nulls++;
            }
            if (nulls == 2)
                return true;
            try
            {
                return (n1.X == n2.X) && (n1.Y == n2.Y);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool operator !=(Node n1, Node n2)
        {
            return !(n1 == n2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                Node n2 = (Node)obj;
                return (this == n2);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "), H=" + H + ", Temp=" + Temp;
        }
    }

    public class LRTA
    {
        private Node[,] Map;
        private CharacterType CharacterType;
        private List<Vector2> StandardLocalSearchPattern = new List<Vector2>() { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
        private List<Vector2> SharpLocalSearchPattern = new List<Vector2>() { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(-1, 2), new Vector2(1, -2), new Vector2(-2, -1), new Vector2(2, -1), new Vector2(-2, 1), new Vector2(2, 1), new Vector2(-1, 2), new Vector2(1, 2) };
        public LRTA(Vector2 endPos, CharacterType characterType = CharacterType.NONE)
        {
            CultureInfo cultureInfo   = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            Map = new Node[MyScene.TiledMap.Width, MyScene.TiledMap.Height];

            var layer = MyScene.TiledMap.TileLayers.First().Value;
            Vector2 endTile = layer.GetLayerTileByWorldPosition(endPos).Position();

            var tiles = layer.Tiles;
            foreach (var tile in tiles)
            {
                try
                {
                    Node node = new Node();
                    node.X = tile.X;
                    node.Y = tile.Y;
                    node.Temp = 0f;

                    string terrain = tile.TilesetTile.Properties["terrain"];
                    node.Terrain = (Terrain)System.Enum.Parse(typeof(Terrain), terrain, true);

                    string passable = tile.TilesetTile.Properties["passable"];
                    node.Passable = bool.Parse(textInfo.ToTitleCase(passable));
                    if (node.Passable)
                        node.H = Manhatan(tile.Position(), endTile);
                    else
                        node.H = float.PositiveInfinity;
                    Map[node.X, node.Y] = node;
                }
                catch (Exception)
                {
                    Console.WriteLine("Property or tile could not found.");
                }
            }
        }
        private List<Node> GetNeighbors(Vector2 node, List<Vector2> posiciones)
        {
            List<Node> neighbors = new List<Node>();
            foreach (var posicion in posiciones)
            {
                try
                {
                    neighbors.Add(Map[(int)node.X + (int)posicion.X, (int)node.Y + (int)posicion.Y]);
                }
                catch (Exception)
                {

                }
            }
            return neighbors;
        }

        public Vector2[] Execute(Vector2 startPos, Vector2 endPos)
        {
            var layer = MyScene.TiledMap.TileLayers.First().Value;
            var startTile = layer.GetLayerTileByWorldPosition(startPos).Position();
            var endTile = layer.GetLayerTileByWorldPosition(endPos).Position();
            if (!Map[(int)endTile.X, (int)endTile.Y].Passable)
                return new Vector2[0];
            return Execute(Map[(int)startTile.X, (int)startTile.Y], Map[(int)endTile.X, (int)endTile.Y]);
        }

        private Vector2[] Execute(Node start, Node end)
        {
            // Matriz de guardado de camino
            Vector2[,] pathMatrix = new Vector2[Map.GetLength(0), Map.GetLength(1)];
            var current = start.Position;

            while (current != end.Position)
            {
                var neighbors = GetNeighbors(current, SharpLocalSearchPattern).Where(w => w != end && w.Passable);

                foreach (var u in neighbors)
                {
                    u.Temp = u.H;
                    u.H = float.PositiveInfinity;
                }

                UpdateStep(neighbors);

                do
                {
                    Node origin = Map[(int)current.X, (int)current.Y];
                    Console.WriteLine(origin);
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

                    if (pathMatrix[(int)current.X, (int)current.Y].IsNull())
                        pathMatrix[(int)current.X, (int)current.Y] = origin.Position;
                }
                while (neighbors.Any(a => a.X == (int)current.X && a.Y == (int)current.Y));
            }

            LinkedList<Vector2> path = new LinkedList<Vector2>();
            while (current != Vector2.Zero)
            {
                path.AddFirst(current);
                current = pathMatrix[(int)current.X, (int)current.Y];
            }

            path.AddFirst(start.Position);

            return StringPulling(path.ToList());
            //return path.ToArray();
        }

        public void UpdateStep(IEnumerable<Node> sLss)
        {
            while (sLss.Any(a => float.IsInfinity(a.H)))
            {
                foreach (var n in sLss)
                {

                    try
                    {
                        n.Hup = Map[n.X, n.Y - 1].H + Weigth(Map[n.X, n.Y - 1]);
                    }
                    catch (Exception)
                    {
                        n.Hup = float.PositiveInfinity;
                    }
                    try
                    {
                        n.Hdown = Map[n.X, n.Y + 1].H + Weigth(Map[n.X, n.Y + 1]);
                    }
                    catch (Exception)
                    {
                        n.Hdown = float.PositiveInfinity;
                    }
                    try
                    {
                        n.Hright = Map[n.X + 1, n.Y].H + Weigth(Map[n.X + 1, n.Y]);
                    }
                    catch (Exception)
                    {
                        n.Hright = float.PositiveInfinity;
                    }
                    try
                    {
                        n.Hleft = Map[n.X - 1, n.Y].H + Weigth(Map[n.X - 1, n.Y]);
                    }
                    catch (Exception)
                    {
                        n.Hleft = float.PositiveInfinity;
                    }
                    n.Hneightbors = Math.Min(n.Hup, Math.Min(n.Hdown, Math.Min(n.Hleft, n.Hright)));
                }

                var v = sLss.Where(w => float.IsInfinity(w.H)).OrderBy(o => Math.Max(o.Hneightbors, o.Temp)).First();
                v.H = Math.Max(v.Hneightbors, v.Temp);
                if (float.IsInfinity(v.H))
                    break;
            }
        }

        public float Weigth(Node target)
        {
            switch (CharacterType)
            {
                case CharacterType.BEAST:
                case CharacterType.JUGGERNAUT:
                case CharacterType.LIZARD:
                case CharacterType.SOLDIER:
                case CharacterType.NONE:
                    return 1;
                default:
                    break;
            }
            return 1;
        }

        public Vector2[] StringPulling(List<Vector2> path)
        {
            for (int i = 0; i < path.Count-2; i++)
            {
                if (CanDelete(path[i], path[i + 1], path[i + 2]))
                    path.RemoveAt(i + 1);
            }
            return path.ToArray();
        }

        public float Manhatan(Vector2 i, Vector2 j, float minValue = 1f)
        {
            //h (n) = D * (abs (n.x-goal.x) + abs (n.y-goal.y)).
            return minValue * (Math.Abs(i.X - j.X) + Math.Abs(i.Y - j.Y));
        }

        public float Chebychev(Vector2 i, Vector2 j, float minValue = 1f)
        {
            //h (n) = D * max (abs (N.X-goal.x), abs (N.Y-goal.y))
            return minValue * Math.Max(Math.Abs(i.X - j.X), Math.Abs(i.Y - j.Y));
        }

        public float Euclidean(Vector2 i, Vector2 j, float minValue = 1f)
        {
            //h (n) = D * sqrt ((N.X-goal.x) ^ 2 + (N.Y-goal.y) ^ 2)
            return minValue * (float)Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
        }

        public float Manhatan(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * (abs (n.x-goal.x) + abs (n.y-goal.y)).
            return minValue * (Math.Abs(i.X - j.X) + Math.Abs(i.Y - j.Y));
        }

        public float Chebychev(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * max (abs (N.X-goal.x), abs (N.Y-goal.y))
            return minValue * Math.Max(Math.Abs(i.X - j.X), Math.Abs(i.Y - j.Y));
        }

        public float Euclidean(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * sqrt ((N.X-goal.x) ^ 2 + (N.Y-goal.y) ^ 2)
            return minValue * (float)Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
        }

        public bool IsAdjacent(Vector2 p1, Vector2 p2)
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

        public bool CanDelete(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if (IsAdjacent(p1,p2) && IsAdjacent(p2,p3))
            {
                Vector2 newPos = (p1 - p2) + (p3 - p2);
                newPos += p2;
                if (p1 != newPos)
                    return (Map[(int)newPos.X, (int)newPos.Y].Passable);
            }
            return false;
        }
    }
}
