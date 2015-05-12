using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class Map
    {
        private static Map instance = new Map();
        public static Map CurrentMap { get { return instance; } }

        public TiledMap TiledMap { get; private set; }
        public Node[,] NodeMap { get; private set; }
        public const int HealRatio = 10;

        public List<Vector2> HealPoints { get; private set; }

        public int Width { get { return TiledMap.Width; } }
        public int Height { get { return TiledMap.Height; } }

        public int TotalWidth { get { return TiledMap.Width(); } }
        public int TotalHeight { get { return TiledMap.Height(); } }

        public int TileWidth { get { return TiledMap.TileWidth; } }
        public int TileHeight { get { return TiledMap.TileHeight; } }

        public void Initialize(TiledMap map)
        {
            HealPoints = new List<Vector2>();
            TiledMap = map;

            #region Node Map Base
            // Esto es para que los valores true o false recuperados de los tiles,
            // se formateen siempre como True o False.
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            NodeMap = new Node[TiledMap.Width, TiledMap.Height];

            // Obtenemos la posición del tile objetivo para generar los valores heuristicos iniciales
            var layer = TiledMap.TileLayers.First().Value;

            // Obtenemos todos los tiles del mapa
            var tiles = layer.Tiles;
            foreach (var tile in tiles)
            {
                try
                {
                    // Generamos un nodo con las posiciones del Tile
                    Node node = new Node();
                    node.X = tile.X;
                    node.Y = tile.Y;
                    node.Temp = 0f;

                    // Obtenemos el tipo de terreno
                    string terrain = tile.TilesetTile.Properties["terrain"];
                    node.Terrain = (Terrain)System.Enum.Parse(typeof(Terrain), terrain, true);

                    // Obtenemos el valor que indica si un Tile es pasable
                    string passable = tile.TilesetTile.Properties["passable"];
                    node.Passable = bool.Parse(textInfo.ToTitleCase(passable));

                    // Obtenemos el valor que indica si un punto de curación
                    string heal = tile.TilesetTile.Properties["heal"];
                    if (bool.Parse(textInfo.ToTitleCase(heal)))
                    {
                        HealPoints.Add(new Vector2(tile.X, tile.Y));
                    }

                    // Guardamos el nodo en la matriz
                    NodeMap[node.X, node.Y] = node;
                }
                catch (Exception)
                {
                    Console.WriteLine("Property or tile could not found.");
                }
            }
            #endregion
        }

        public Vector2 TilePositionByWolrdPosition(Vector2 position)
        {
            var layer = TiledMap.TileLayers.First().Value;
            try
            {
                Vector2 endTile = layer.GetLayerTileByWorldPosition(position).Position();
                return endTile;
            }
            catch (Exception)
            {
                return new Vector2(-1, -1);
            }
        }

        public Vector2 WolrdPositionByTilePosition(Vector2 position)
        {
            return position * new Vector2(TiledMap.TileWidth, TiledMap.TileHeight);
        }

        public LayerTile TileByWolrdPosition(Vector2 position)
        {
            var layer = TiledMap.TileLayers.First().Value;
            try
            {
                return layer.GetLayerTileByWorldPosition(position);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Terrain TerrainOnWorldPosition(Vector2 position)
        {
            Vector2 tilePosition = TilePositionByWolrdPosition(position);
            return NodeMap[tilePosition.X(), tilePosition.Y()].Terrain;
        }

        public bool PositionInMap(Vector2 position)
        {
            return TiledMap.PositionInMap(position);
        }

        public void Draw(LineBatch2D lb)
        {
            foreach (var healpoint in HealPoints)
            {
                Vector2 src = WolrdPositionByTilePosition(healpoint - new Vector2(HealRatio, HealRatio));
                Vector2 dst = WolrdPositionByTilePosition(new Vector2(HealRatio, HealRatio) * 2);
                lb.DrawRectangleVM(new RectangleF(src.X, src.Y, dst.X, dst.Y), Color.Green, 1f);
            }
        }
    }
}
