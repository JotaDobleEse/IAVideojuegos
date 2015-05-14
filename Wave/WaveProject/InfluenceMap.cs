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

namespace WaveProject
{
    public class InfluenceMap
    {

        private static InfluenceMap instance = new InfluenceMap();
        public static InfluenceMap Influence { get { return instance; } }
        public EntityManager EntityManager { get; private set; }
        public const int Scale = 5;
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
            Texture.Height = Map.CurrentMap.TotalWidth / Scale;
        }

        public List<Vector2> GetTeamCharacters(int team)
        {
            var characters = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo)
                .Where(w => w.GetTeam() == team);

            return characters.Select(s => s.GetPostion()).ToList();
        }

        public void GenerateInfluenteMap()
        {
            var team1 = GetTeamCharacters(1);
            var team2 = GetTeamCharacters(2);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Map.CurrentMap.TotalWidth / Scale, Map.CurrentMap.TotalHeight / Scale);
            System.Drawing.Graphics batch = System.Drawing.Graphics.FromImage(bitmap);

            int recWidth = Map.CurrentMap.TileWidth / Scale;
            int recHeight = Map.CurrentMap.TileHeight / Scale;

            batch.FillRectangle(System.Drawing.Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);

            foreach (var ch1 in team1)
            {
                var position = Map.CurrentMap.TileByWolrdPosition(ch1).Position() * new Vector2(recWidth, recHeight);
                batch.FillRectangle(System.Drawing.Brushes.Red, position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
            }
            foreach (var ch2 in team2)
            {
                var position = Map.CurrentMap.TileByWolrdPosition(ch2).Position() * new Vector2(recWidth, recHeight);
                batch.FillRectangle(System.Drawing.Brushes.Blue, position.X, Math.Abs(position.Y - bitmap.Height), recWidth, recHeight);
            }

            byte[] values = new byte[bitmap.Width * bitmap.Height * 4];
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Seek(54, SeekOrigin.Begin);
                stream.Read(values, 0, values.Length);
            }

            Texture.Data = new byte[1][][];   // only 1 texture part
            Texture.Data[0] = new byte[1][];  // 1 mipmap level
            Texture.Data[0][0] = new byte[values.Length];
            Texture.Data[0][0] = values;

            if (!Texture.IsUploaded)
            {
                WaveServices.GraphicsDevice.Graphics.TextureManager.UploadTexture(Texture);
                //Console.WriteLine("Updated");
            }

        }
    }
}
