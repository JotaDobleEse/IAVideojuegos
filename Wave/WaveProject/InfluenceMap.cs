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
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class InfluenceMap
    {

        private static InfluenceMap instance = new InfluenceMap();
        public static InfluenceMap Influence { get { return instance; } }
        public EntityManager EntityManager { get; private set; }
        public const int Scale = 5;
        public const int MaxAlpha = 160;

        private const float Expand = 0.95f;

        private List<Vector2> StandardLocalSearchPattern = new List<Vector2>() { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
        public Texture2D Texture { get; private set; }

        private InfluenceMap()
        {
            Texture = new Texture2D()
                {
                    Format = PixelFormat.R8G8B8A8,
                    //Width = Map.CurrentMap.TotalWidth / Scale,
                    //Height = Map.CurrentMap.TotalWidth / Scale,
                    Levels = 1
                };
        }

        public void Initialize(EntityManager entity)
        {
            EntityManager = entity;
            Texture.Width = Map.CurrentMap.TotalWidth / Scale;
            Texture.Height = Map.CurrentMap.TotalHeight / Scale;
        }

        public List<Vector2> GetTeamCharacters(int team)
        {
            var characters = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo)
                .Where(w => w.GetTeam() == team);

            return characters.Select(s => s.GetPosition()).ToList();
        }

        private Node[,] Copy(Node[,] nodes)
        {
            Node[,] result = new Node[nodes.GetLength(0), nodes.GetLength(1)];
            Parallel.For(0, nodes.GetLength(0), (i) =>
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    var node = nodes[i, j];
                    result[i, j] = node.Clone() as Node;
                    result[i, j].InfluenceTeam[0] = node.InfluenceTeam[0];
                    result[i, j].InfluenceTeam[1] = node.InfluenceTeam[1];
                }
            });
            return result;
        }

        private List<Node> GetNeighbors(Vector2 node, List<Vector2> positions)
        {
            List<Node> neighbors = new List<Node>();
            // Para cada posición del patrón
            foreach (var posicion in positions)
            {
                // Si existe un nodo en la posición especificada se añade a la lista
                if (Map.CurrentMap.NodeMap.Exists(node + posicion))
                    neighbors.Add(Map.CurrentMap.NodeMap[node.X() + posicion.X(), node.Y() + posicion.Y()]);
            }
            return neighbors;
        }

        private void UpdateInfluenceNodes()
        {
            var team1 = GetTeamCharacters(1);
            var team2 = GetTeamCharacters(2);

            var NodeMap = Map.CurrentMap.NodeMap;
            for (int i = 0; i < NodeMap.GetLength(0); i++)
            {
                for (int j = 0; j < NodeMap.GetLength(1); j++)
                {
                    var node = NodeMap[i, j];
                    node.InfluenceTeam[0] = 0;
                    node.InfluenceTeam[1] = 0;
                }
            }

            foreach (var ch1 in team1)
            {
                var pos = Map.CurrentMap.TilePositionByWolrdPosition(ch1);
                NodeMap[pos.X(), pos.Y()].InfluenceTeam[0] = MaxAlpha;
            }

            foreach (var ch2 in team2)
            {
                var pos = Map.CurrentMap.TilePositionByWolrdPosition(ch2);
                NodeMap[pos.X(), pos.Y()].InfluenceTeam[1] = MaxAlpha;
            }

            for (int k = 0; k < 100; k++)
            {
                NodeMap = Map.CurrentMap.NodeMap;
                var nodeAux = Copy(NodeMap);

                //for (int i = 0; i < NodeMap.GetLength(0); i++)
                Parallel.For(0, NodeMap.GetLength(0), (i) =>
                {
                    for (int j = 0; j < NodeMap.GetLength(1); j++)
                    {
                        var node = nodeAux[i, j];
                        if (node.InfluenceTeam[0] == 0 && node.InfluenceTeam[1] == 0)
                            continue;
                        var neightbors = GetNeighbors(node.Position, StandardLocalSearchPattern);
                        if (node.InfluenceTeam[0] != 0)
                        {
                            foreach (var n in neightbors)
                            {
                                //n.InfluenceTeam[0] = Math.Max(node.InfluenceTeam[0] - 1, 0);
                                n.InfluenceTeam[0] = Math.Min(Math.Max(n.InfluenceTeam[0], (int)(node.InfluenceTeam[0] * Expand)), MaxAlpha);
                            }
                        }
                        if (node.InfluenceTeam[1] != 0)
                        {
                            foreach (var n in neightbors)
                            {
                                //n.InfluenceTeam[1] = Math.Max(node.InfluenceTeam[1] - 1, 0);
                                n.InfluenceTeam[1] = Math.Min(Math.Max(n.InfluenceTeam[1], (int)(node.InfluenceTeam[1] * Expand)), MaxAlpha);
                            }
                        }
                    }
                });
            }
        }

        public void GenerateInfluenteMap()
        {
            //var team1 = GetTeamCharacters(1);
            //var team2 = GetTeamCharacters(2);

            UpdateInfluenceNodes();

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Map.CurrentMap.TotalWidth / Scale, Map.CurrentMap.TotalHeight / Scale);
            System.Drawing.Graphics batch = System.Drawing.Graphics.FromImage(bitmap);
            batch.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            int recWidth = Map.CurrentMap.TileWidth / Scale;
            int recHeight = Map.CurrentMap.TileHeight / Scale;

            batch.FillRectangle(System.Drawing.Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);

            //foreach (var ch1 in team1)
            //{
            //    var position = Map.CurrentMap.TileByWolrdPosition(ch1).Position() * new Vector2(recWidth, recHeight);
            //    batch.FillRectangle(System.Drawing.Brushes.Red, position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
            //}
            //foreach (var ch2 in team2)
            //{
            //    var position = Map.CurrentMap.TileByWolrdPosition(ch2).Position() * new Vector2(recWidth, recHeight);
            //    batch.FillRectangle(System.Drawing.Brushes.Blue, position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
            //}

            //var result = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            

            for (int i = 0; i < Map.CurrentMap.NodeMap.GetLength(0); i++)
            {
                for (int j = 0; j < Map.CurrentMap.NodeMap.GetLength(1); j++)
                {
                    var node = Map.CurrentMap.NodeMap[i, j];
                    if (node.InfluenceTeam[0] == 0 && node.InfluenceTeam[1] == 0)
                        continue;
                    var position = node.Position * new Vector2(recWidth, recHeight);
                    if (node.InfluenceTeam[0] != 0)
                    {
                        batch.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(node.InfluenceTeam[0], 255, 0, 0)), position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
                    }
                    if (node.InfluenceTeam[1] != 0)
                    {
                        batch.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(node.InfluenceTeam[1], 0, 0, 255)), position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
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
