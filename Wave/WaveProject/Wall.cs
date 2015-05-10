using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject
{
    public class Wall : IDisposable
    {
        private static int InstancesCounter = 0;
        public int Id { get; private set; }

        private static List<Wall> walls = new List<Wall>();
        public static List<Wall> Walls { get { return walls; } }

        public BoundingBox WallCollider { get; set; }
        public RectangleF WallRectangle { get; set; }
        /// <summary>
        /// P1 (X, Y)
        /// </summary>
        public Vector2 P1 { get { return new Vector2(WallRectangle.X, WallRectangle.Y); } }
        /// <summary>
        /// P2 (X + Width, Y)
        /// </summary>
        public Vector2 P2 { get { return new Vector2(WallRectangle.X + WallRectangle.Width, WallRectangle.Y); } }
        /// <summary>
        /// P3 (X, Y + Height)
        /// </summary>
        public Vector2 P3 { get { return new Vector2(WallRectangle.X, WallRectangle.Y + WallRectangle.Height); } }
        /// <summary>
        /// P4 (X + Width, Y + Height)
        /// </summary>
        public Vector2 P4 { get { return new Vector2(WallRectangle.X + WallRectangle.Width, WallRectangle.Y + WallRectangle.Height); } }

        /// <summary>
        /// Construye un Muro.
        /// </summary>
        /// <param name="rectangle">Rectangulo que define el muro.</param>
        /// <param name="stable">Establece si el Muro se introduce en la colección de muros del sistema.</param>
        public Wall(RectangleF rectangle, bool stable = false)
        {
            if (stable)
            {
                Id = ++InstancesCounter;
                Wall Clon = this.Clone();
                Clon.Id = Id;
                walls.Add(Clon);
            }
            WallRectangle = rectangle;
            WallCollider = new BoundingBox(new Vector3(WallRectangle.X, WallRectangle.Y, 0f), new Vector3(WallRectangle.X + WallRectangle.Width, WallRectangle.Y + WallRectangle.Height, 0f));
        }

        /// <summary>
        /// Construye un Muro.
        /// </summary>
        /// <param name="x">Establece la posición en el eje-X de la esquina superior izquierda del muro.</param>
        /// <param name="y">Establece la posición en el eje-Y de la esquina superior izquierda del muro.</param>
        /// <param name="width">Establece el ancho del muro a lo largo del eje-X.</param>
        /// <param name="height">Establece el alto del muro a lo largo del eje-Y.</param>
        /// <param name="stable">Establece si el Muro se introduce en la colección de muros del sistema.</param>
        public Wall(float x = 0, float y = 0, float width = 0, float height = 0, bool stable = false)
        {
            if (stable)
                walls.Add(this);
            WallRectangle = new RectangleF(x, y, width, height);
            WallCollider = new BoundingBox(new Vector3(x, y, 0f), new Vector3(x + width, y + height, 0f));
        }

        public Wall Clone()
        {
            Wall w = new Wall(WallRectangle);
            return w;
        }

        public void Dispose()
        {
            Wall w = walls.FirstOrDefault(f => f.Id == Id);
            if (w != null)
                walls.Remove(w);
        }
    }
}
