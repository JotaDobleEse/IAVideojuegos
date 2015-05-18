using System;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
using WaveProject.Characters;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class InfluenceNode
    {
        public int Team1 { get; set; }
        public int Team2 { get; set; }

        public InfluenceNode Clone()
        {
            return new InfluenceNode() { Team1 = this.Team1, Team2 = this.Team2 };
        }
    };

    public class InfluenceMap
    {

        private static InfluenceMap instance = new InfluenceMap();
        public static InfluenceMap Influence { get { return instance; } }
        public const int Scale = 5;
        public const int MaxAlpha = 160;
        public Entity[] Entities { get; set; }
        private InfluenceNode[,] NodeMap;

        private const float Expand = 0.95f;

        private List<Vector2> StandardLocalSearchPattern = new List<Vector2>() { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
        public Texture2D Texture { get; private set; }

        private InfluenceMap()
        {
            Texture = new Texture2D()
                {
                    Format = PixelFormat.R8G8B8A8,
                    Levels = 1
                };
        }

        public void Initialize()
        {
            Texture.Width = Map.CurrentMap.TotalWidth / Scale;
            Texture.Height = Map.CurrentMap.TotalHeight / Scale;
        }

        public List<Vector2> GetTeamCharacters(int team)
        {
            try
            {
                //var entities = EntityManager.AllEntities.ToArray();
                var characters = Entities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                    .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo)
                    .Where(w => w.GetTeam() == team).ToArray();

                return characters.Select(s => s.GetPosition()).ToList();
            } catch (Exception)
            {
                return new List<Vector2>();
            }
        }

        private InfluenceNode[,] Copy(InfluenceNode[,] nodes)
        {
            InfluenceNode[,] result = new InfluenceNode[nodes.GetLength(0), nodes.GetLength(1)];
            Parallel.For(0, nodes.GetLength(0), (i) =>
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    var node = nodes[i, j];
                    result[i, j] = node.Clone();
                }
            });
            return result;
        }

        private List<InfluenceNode> GetNeighbors(Vector2 node, List<Vector2> positions)
        {
            List<InfluenceNode> neighbors = new List<InfluenceNode>();
            // Para cada posición del patrón
            foreach (var posicion in positions)
            {
                // Si existe un nodo en la posición especificada se añade a la lista
                if (Map.CurrentMap.Exists(node + posicion))
                    neighbors.Add(NodeMap[node.X() + posicion.X(), node.Y() + posicion.Y()]);
            }
            return neighbors;
        }

        private void UpdateInfluenceNodes()
        {
            var team1 = GetTeamCharacters(1);
            var team2 = GetTeamCharacters(2);

            NodeMap = Copy(Map.CurrentMap.InfluenceMap);
            for (int i = 0; i < NodeMap.GetLength(0); i++)
            {
                for (int j = 0; j < NodeMap.GetLength(1); j++)
                {
                    var node = NodeMap[i, j];
                    node.Team1 = 0;
                    node.Team2 = 0;
                }
            }

            foreach (var ch1 in team1)
            {
                var pos = Map.CurrentMap.TilePositionByWolrdPosition(ch1);
                NodeMap[pos.X(), pos.Y()].Team1 = MaxAlpha;
            }

            foreach (var ch2 in team2)
            {
                var pos = Map.CurrentMap.TilePositionByWolrdPosition(ch2);
                NodeMap[pos.X(), pos.Y()].Team2 = MaxAlpha;
            }

            for (int k = 0; k < 40; k++)
            {
                //var nodeAux = Copy(NodeMap);

                //for (int i = 0; i < NodeMap.GetLength(0); i++)
                Parallel.For(0, NodeMap.GetLength(0), (i) =>
                {
                    for (int j = 0; j < NodeMap.GetLength(1); j++)
                    {
                        var node = NodeMap[i, j];
                        if (node.Team1 == 0 && node.Team2 == 0)
                            continue;
                        var neightbors = GetNeighbors(new Vector2(i, j), StandardLocalSearchPattern);
                        if (node.Team1 != 0)
                        {
                            foreach (var n in neightbors)
                            {
                                //n.InfluenceTeam[0] = Math.Max(node.InfluenceTeam[0] - 1, 0);
                                n.Team1 = Math.Min(Math.Max(n.Team1, (int)(node.Team1 * Expand)), MaxAlpha);
                            }
                        }
                        if (node.Team2 != 0)
                        {
                            foreach (var n in neightbors)
                            {
                                //n.InfluenceTeam[1] = Math.Max(node.InfluenceTeam[1] - 1, 0);
                                n.Team2 = Math.Min(Math.Max(n.Team2, (int)(node.Team2 * Expand)), MaxAlpha);
                            }
                        }
                    }
                });
            }
            Map.CurrentMap.InfluenceMap = Copy(NodeMap);
        }

        public void GenerateInfluenteMap()
        {
            UpdateInfluenceNodes();

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Map.CurrentMap.TotalWidth / Scale, Map.CurrentMap.TotalHeight / Scale);
            System.Drawing.Graphics batch = System.Drawing.Graphics.FromImage(bitmap);
            batch.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            int recWidth = Map.CurrentMap.TileWidth / Scale;
            int recHeight = Map.CurrentMap.TileHeight / Scale;

            batch.FillRectangle(System.Drawing.Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);

            for (int i = 0; i < Map.CurrentMap.InfluenceMap.GetLength(0); i++)
            {
                for (int j = 0; j < Map.CurrentMap.InfluenceMap.GetLength(1); j++)
                {
                    var node = Map.CurrentMap.InfluenceMap[i, j];
                    if (node.Team1 == 0 && node.Team2 == 0)
                        continue;
                    var position = new Vector2(i, j) * new Vector2(recWidth, recHeight);
                    System.Drawing.RectangleF rectangle = new System.Drawing.RectangleF(position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
                    if (node.Team1 != 0)
                    {
                        batch.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(node.Team1, 255, 0, 0)), rectangle);
                    }
                    if (node.Team2 != 0)
                    {
                        batch.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(node.Team2, 0, 0, 255)), rectangle);
                    }
                }
            }

            byte[] values = new byte[bitmap.Width * bitmap.Height * 4];
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Seek(54, SeekOrigin.Begin);
                stream.Read(values, 0, values.Length);
            }
            bitmap.Dispose();

            Texture.Data = new byte[1][][];   // only 1 texture part
            Texture.Data[0] = new byte[1][];  // 1 mipmap level
            Texture.Data[0][0] = new byte[values.Length];
            Texture.Data[0][0] = values;

            if (!Texture.IsUploaded)
            {
                try
                {
                    WaveServices.GraphicsDevice.Graphics.TextureManager.UploadTexture(Texture);
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //Console.WriteLine("Updated");
            }

        }
    }
}
