using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steering
{
    public class Collision
    {
        public Vector2 Position { get; set; }
        public Vector2 Normal { get; set; }
    };

    public class CollisionDetector
    {
        private static CollisionDetector detector = new CollisionDetector();
        public static CollisionDetector Detector { get { return detector; } }
        public IEnumerable<Wall> Walls { get; set; }

        public CollisionDetector()
        {
            Walls = new Wall[] { new Wall(100, 100, 50, 300) };
        }

        public Collision GetCollision(Vector2 position, Vector2 moveAmount)
        {
            Collision firstCollision = null;
            foreach (var wall in Walls)
            {
                RectangleF rect = new RectangleF(position.X, position.Y, moveAmount.X, moveAmount.Y);

                Ray ray = new Ray(position.ToVector3(0f), moveAmount.ToVector3(0f));
                float? result;
                BoundingBox collider = wall.WallCollider;
                ray.Intersects(ref collider, out result);

                if (result == null || float.IsNaN(result.Value))
                    continue;

                if (firstCollision == null)
                {
                    firstCollision = new Collision();
                    firstCollision.Normal = new Vector2();
                    firstCollision.Position = new Vector2();
                }
                else
                {
                    // Si la colision esta mas cerca que la ultima detectada asignar a firstCollision
                }

                RectangleF face1 = new RectangleF(wall.P1.X, wall.P1.Y, wall.P2.X, wall.P2.Y);
                RectangleF face2 = new RectangleF(wall.P2.X, wall.P2.Y, wall.P3.X, wall.P3.Y);
                RectangleF face3 = new RectangleF(wall.P3.X, wall.P3.Y, wall.P4.X, wall.P4.Y);
                RectangleF face4 = new RectangleF(wall.P4.X, wall.P4.Y, wall.P1.X, wall.P1.Y);
                if (rect.Intersects(face1))
                {

                }
                if (rect.Intersects(face2))
                {

                }
                if (rect.Intersects(face3))
                {

                }
                if (rect.Intersects(face4))
                {

                }
            }

            return firstCollision;
        }
    }
}
