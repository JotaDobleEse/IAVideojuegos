using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject
{
    public class Wall
    {
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

        public Wall(RectangleF rectangle)
        {
            WallRectangle = rectangle;
            WallCollider = new BoundingBox(new Vector3(WallRectangle.X, WallRectangle.Y, 0f), new Vector3(WallRectangle.X + WallRectangle.Width, WallRectangle.Y + WallRectangle.Height, 0f));
        }

        public Wall(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            WallRectangle = new RectangleF(x, y, width, height);
            WallCollider = new BoundingBox(new Vector3(x, y, 0f), new Vector3(x + width, y + height, 0f));
        }
    }
}
