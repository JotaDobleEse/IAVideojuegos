using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;
using WaveProject.Characters;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public static class Extensiones
    {
        #region Entity
        /// <summary>
        /// Obtiene el radio ocupado por una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static float BRadius(this Entity entity)
        {
            var objectTexture = entity.FindComponent<Sprite>();
            if (objectTexture == null)
            {
                var objectTransform = entity.FindComponent<Transform2D>();
                return Math.Max(objectTransform.Rectangle.Width, objectTransform.Rectangle.Height) * 0.95f;
            }
            return Math.Max(objectTexture.Texture.Width, objectTexture.Texture.Height) * 0.95f;
        }
        #endregion

        #region Transform2D
        /// <summary>
        /// Devuelve la rotación como un vector de dos coordenadas.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector2 RotationAsVector(this Transform2D transform)
        {
            return new Vector2((float)Math.Sin(transform.Rotation), -(float)Math.Cos(transform.Rotation));
        }

        /// <summary>
        /// Transporta una posicion de dos coordenadas global a otra posición, tomando como origen (0,0) la posición del Transform2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 ConvertToLocalPos(this Transform2D origin, Vector2 position)
        {
            var localPos = position - origin.Position;
            localPos -= origin.Rotation.RotationToVector();
            return localPos;
        }

        /// <summary>
        /// Transporta una posicion de dos coordenadas local a otra posición, tomando como origen (0,0) el origen de coordenadas del mundo.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 ConvertToGlobalPos(this Transform2D origin, Vector2 position)
        {
            var localPos = position + origin.Position;
            localPos += origin.RotationAsVector();
            return localPos;
        }
        #endregion

        #region Vector2
        /// <summary>
        /// Transforma un Vector2 en su valor de rotación en radianes.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static float ToRotation(this Vector2 rotation)
        {
            return (float)Math.Atan2(rotation.X, -rotation.Y);
        }

        /// <summary>
        /// Copia un Vector2
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Clone(this Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        /// <summary>
        /// Rota un Vector2 en un ángulo especificado en radianes.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="radians">Radianes a rotar.</param>
        /// <returns></returns>
        public static Vector2 RotateVector(this Vector2 vector, float radians)
        {
            float angle = vector.ToRotation() + radians;
            return angle.MapToRange().RotationToVector() * vector.Length();
        }

        /// <summary>
        /// Obtiene una de las dos normales perpendiculares del vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 Norm1(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        /// <summary>
        /// Obtiene una de las dos normales perpendiculares del vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 Norm2(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        /// <summary>
        /// Devuelve la coordenada X como número entero.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static int X(this Vector2 vector)
        {
            return (int)vector.X;
        }

        /// <summary>
        /// Devuelve la coordenada Y como número entero.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static int Y(this Vector2 vector)
        {
            return (int)vector.Y;
        }

        /// <summary>
        /// Comprueba si un vector es nulo: (NaN, NaN), (X, NaN), (NaN, Y) ó (0, 0).
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsNull(this Vector2 v)
        {
            return (float.IsNaN(v.X) || float.IsNaN(v.Y)) || (v == Vector2.Zero);
        }

        /// <summary>
        /// Comprueba si dos vectores son iguales.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="maxRelativeError"></param>
        /// <returns></returns>
        public static bool Equal(this Vector2 v1, Vector2 v2, float maxRelativeError = 0.01f)
        {
            return (v1.X.Equal(v2.X, maxRelativeError) && v1.Y.Equal(v2.Y, maxRelativeError));
        }

        /// <summary>
        /// Comprueba si un punto esta contenido en un rectangulo
        /// </summary>
        /// <param name="center"></param>
        /// <param name="widthHeight"></param>
        /// <returns></returns>
        public static bool IsContent(this Vector2 v1, Vector2 center, Vector2 widthHeight)
        {
            bool ejeX = false;
            bool ejeY = false;
            ejeX = (center.X - widthHeight.X / 2 < v1.X && v1.X < center.X + widthHeight.X / 2);
            ejeY = (center.Y - widthHeight.Y / 2 < v1.Y && v1.Y < center.Y + widthHeight.Y / 2);
            return ejeX && ejeY;
        }

        /// <summary>
        /// Devuelve la posición del vector relativa a la cámara especificada.
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="camera">Cámara en base a la que se va a rectificar la posición del vector.</param>
        /// <returns></returns>
        public static Vector2 PositionUnproject(this Vector2 vector, Camera camera)
        {
            Vector3 mousePosition = new Vector3(vector, 0f);
            Vector3 project = camera.Project(ref mousePosition);
            Vector2 projectMouse = project.ToVector2();
            WaveServices.ViewportManager.RecoverPosition(ref projectMouse);
            return projectMouse;
        }
        #endregion

        #region float
        /// <summary>
        /// Convierte el ángulo especificado en radianes a Vector2.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector2 RotationToVector(this float rotation)
        {
            return new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
        }

        /// <summary>
        /// Transforma un ángulo en radianes a valores entre PI y -PI.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static float MapToRange(this float rotation)
        {
            float r = rotation;
            float Pi = (float)Math.PI;
            if (rotation > Pi)
            {
                return r - 2 * Pi;
            }
            else if (rotation < -Pi)
            {
                return r + 2 * Pi;
            }
            else return r;
        }

        /// <summary>
        /// Devuelve el valor absoluto.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static float Abs(this float rotation)
        {
            return (float)Math.Abs(rotation);
        }

        private const double Epsilon = 1e-10;
        /// <summary>
        /// Devuelve true si el valor es 0.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsZero(this float d)
        {
            return Math.Abs(d) < Epsilon;
        }
        #endregion

        #region MouseState
        /// <summary>
        /// Devuelve la posición como Vector2.
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        public static Vector2 Position(this MouseState mouse)
        {
            return new Vector2(mouse.X, mouse.Y);
        }

        /// <summary>
        /// Devuelve la posición del ratón relativa a la cámara especificada.
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="camera">Cámara en base a la que se va a rectificar la posición del ratón.</param>
        /// <returns></returns>
        public static Vector2 PositionRelative(this MouseState mouse, Camera camera)
        {
            Vector3 mousePosition = new Vector3(mouse.Position(), 0f);
            Vector3 project = camera.Unproject(ref mousePosition);
            Vector2 projectMouse = project.ToVector2();
            WaveServices.ViewportManager.RecoverPosition(ref projectMouse);
            return projectMouse;
        }
        #endregion

        #region TiledMap
        /// <summary>
        /// Ancho del mapa, Número de Tiles de ancho * Ancho del Tile.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static int Width(this TiledMap map)
        {
            return map.Width * map.TileWidth;
        }

        /// <summary>
        /// Alto del mapa, Número de Tiles de alto * Alto del Tile.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static int Height(this TiledMap map)
        {
            return map.Height * map.TileHeight;
        }

        /// <summary>
        /// Devuelve true si hay un Tile en la posición especificada.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position">Posición a comprobar si pertenece al mapa.</param>
        /// <returns></returns>
        public static bool PositionInMap(this TiledMap map, Vector2 position)
        {
            if (position.X() < 0 || position.Y() < 0 || position.X() >= map.Width() || position.Y() >= map.Height())
                return false;
            return true;
        }
        #endregion

        #region LayerTile
        /// <summary>
        /// Devuelve la posición del Tile en un Vector2.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static Vector2 Position(this LayerTile tile)
        {
            return new Vector2(tile.X, tile.Y);
        }
        #endregion

        #region Node[,]
        /// <summary>
        /// Devuelve un nodo si la posición (i, j) existe en la matriz, en otro caso devuelve el valor por defecto.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="i">Posición i de [i, j].</param>
        /// <param name="j">Posición j de [i, j].</param>
        /// <param name="defaultValue">Valor a devolver si no existe la posición (i, j) en la matriz.</param>
        /// <returns></returns>
        public static float GetValueOrDefault(this Node[,] array, int i, int j, float defaultValue)
        {
            if (i < 0 || j < 0 || i >= array.GetLength(0) || j >= array.GetLength(1))
                return defaultValue;
            return array[i,j].H;
        }

        /// <summary>
        /// Si la posición (i, j) está en la matriz devuelve true, sino false.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="i">Posición i de [i, j].</param>
        /// <param name="j">Posición j de [i, j].</param>
        /// <returns></returns>
        public static bool Exists(this Node[,] array, int i, int j)
        {
            if (i < 0 || j < 0 || i >= array.GetLength(0) || j >= array.GetLength(1))
                return false;
            return true;
        }

        /// <summary>
        /// Si la posición parámetro está en la matriz devuelve true, sino false.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="position">Vector2 (x, y) => [x, y].</param>
        /// <returns></returns>
        public static bool Exists(this Node[,] array, Vector2 position)
        {
            return array.Exists(position.X(), position.Y());
        }
        #endregion

        #region EntityManager
        /// <summary>
        /// Devuelve todas las entidades que contienen un componente del tipo especificado.
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="type">Tipo de componente.</param>
        /// <returns></returns>
        public static Entity[] AllEntitiesByComponentType(this EntityManager entityManager, Type type)
        {
            var chars = entityManager.AllEntities.Where(w => w.Components.Any(a => a.GetType() == type));
            return chars.ToArray();
        }

        /// <summary>
        /// Devuelve todos los jugadores.
        /// </summary>
        /// <param name="entityManager"></param>
        /// <returns></returns>
        public static ICharacterInfo[] AllCharacters(this EntityManager entityManager)
        {
            var chars = entityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo);
            return chars.ToArray();
        }

        /// <summary>
        /// Devuelve todas las entidades que contienen un componente jugador.
        /// </summary>
        /// <param name="entityManager"></param>
        /// <returns></returns>
        public static Entity[] AllCharactersEntity(this EntityManager entityManager)
        {
            var chars = entityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo));
            return chars.ToArray();
        }

        /// <summary>
        /// Devuelve todos los personajes de un bando.
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="team">Bando del jugador.</param>
        /// <returns></returns>
        public static ICharacterInfo[] AllCharactersByTeam(this EntityManager entityManager, int team)
        {
            var chars = entityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo)
                .Where(w => w.GetTeam() == team);
            return chars.ToArray();
        }

        /// <summary>
        /// Indica si una posición ya está ocupada.
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="character">Jugador que solicita la información.</param>
        /// <param name="position">Posición que se quiere evaluar</param>
        /// <returns></returns>
        public static bool PositionOcupped(this EntityManager entityManager, ICharacterInfo character, Vector2 position)
        {
            var pos = Map.CurrentMap.TilePositionByWolrdPosition(position);
            var characters = entityManager.AllCharacters().Where(w => w != character).ToList();
            foreach (var ch in characters)
            {
                if (pos == Map.CurrentMap.TilePositionByWolrdPosition(ch.GetPosition()))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
