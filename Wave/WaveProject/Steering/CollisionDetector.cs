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
        public CollisionDetector()
        {
            new Wall(200, 200, 50, 150, true);
            new Wall(0, 50, 200, 10, true);
            new Wall(0, 500, 200, 10, true);
        }

        public Collision GetCollision(Vector2 position, Vector2 moveAmount)
        {
            Collision firstCollision = null;
            foreach (var wall in Wall.Walls)
            {
                Vector2 v;
                List<Vector2> intersections = new List<Vector2>();
                int face1, face2, face3, face4;
                face1 = face2 = face3 = face4 = 0;
                if (LineSegementsIntersect(position, position + moveAmount, wall.P1, wall.P2, out v))
                {
                    intersections.Add(new Vector2(v.X, v.Y));
                    face1 = intersections.Count;
                }
                if (LineSegementsIntersect(position, position + moveAmount, wall.P2, wall.P4, out v))
                {
                    intersections.Add(new Vector2(v.X, v.Y));
                    face2 = intersections.Count;
                }
                if (LineSegementsIntersect(position, position + moveAmount, wall.P4, wall.P3, out v))
                {
                    intersections.Add(new Vector2(v.X, v.Y));
                    face3 = intersections.Count;
                }
                if (LineSegementsIntersect(position, position + moveAmount, wall.P3, wall.P1, out v))
                {
                    intersections.Add(new Vector2(v.X, v.Y));
                    face4 = intersections.Count;
                }

                if (intersections.Count > 0)
                {
                    var pos = intersections.OrderBy(o => (o - position).Length()).First();
                    int face = intersections.IndexOf(pos) + 1;
                    Console.WriteLine("f1: {0}, f2: {1}, f3: {2}, f4: {3}, actual: {4}", face1, face2, face3, face4, face);
                    Vector2 normal = Vector2.Zero;

                    float rotation = (position - pos).ToRotation();

                    if (face == face1 || face == face2)
                    {
                        var vect = wall.P2 - pos;
                        var norm1 = vect + ((float)Math.PI / 2).RotationToVector();
                        var norm2 = vect - ((float)Math.PI / 2).RotationToVector(); ;
                        //var norm1 = new Vector2(vect.X, -vect.Y) / (float)Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y) * 5;
                        //var norm2 = new Vector2(vect.X, -vect.Y) / (float)Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y) * -5;
                        float r1 = (norm1.ToRotation().MapToRange() - rotation.Abs()).MapToRange().Abs();
                        float r2 = (norm2.ToRotation().MapToRange() - rotation.Abs()).MapToRange().Abs();
                        if (r1 < r2)
                        {
                            normal = norm1;
                        }
                        else
                        {
                            normal = norm2;
                        }
                    }
                    else if (face == face3 || face == face4)
                    {
                        var vect = wall.P3 - pos;
                        var norm1 = vect + ((float)Math.PI / 2).RotationToVector();
                        var norm2 = vect - ((float)Math.PI / 2).RotationToVector(); ;
                        //var norm1 = new Vector2(vect.X, -vect.Y) / (float)Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y) * 5;
                        //var norm2 = new Vector2(vect.X, -vect.Y) / (float)Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y) * -5;
                        float r1 = (norm1.ToRotation().MapToRange() - rotation.Abs()).MapToRange().Abs();
                        float r2 = (norm2.ToRotation().MapToRange() - rotation.Abs()).MapToRange().Abs();
                        if (r1 < r2)
                        {
                            normal = norm1;
                        }
                        else
                        {
                            normal = norm2;
                        }
                    }

                    if (firstCollision == null)
                    {
                        firstCollision = new Collision() { Position = pos, Normal = normal };
                    }
                    else if ((firstCollision.Position - position).Length() < (pos - position).Length())
                    {
                        firstCollision.Position = pos;
                        firstCollision.Normal = normal;
                    }
                }
            }
            return firstCollision;
        }

        /// <summary>
        /// Test whether two line segments intersect. If so, calculate the intersection point.
        /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
        /// </summary>
        /// <param name="p">Vector to the start point of p.</param>
        /// <param name="p2">Vector to the end point of p.</param>
        /// <param name="q">Vector to the start point of q.</param>
        /// <param name="q2">Vector to the end point of q.</param>
        /// <param name="intersection">The point of intersection, if any.</param>
        /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
        /// </param>
        /// <returns>True if an intersection point was found.</returns>
        public static bool LineSegementsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2,
            out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = Vector2.Zero;

            var r = p2 - p;
            var s = q2 - q;
            var rxs = Vector2.Cross(r, s);
            var qpxr = Vector2.Cross((q - p), r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                //if (considerCollinearOverlapAsIntersect)
                //    if ((0 <= ((q - p) * r) && ((q - p) * r) <= (r * r)) || (0 <= ((p - q) * s) && ((p - q) * s) <= (s * s)))
                //        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = Vector2.Cross((q - p), s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = Vector2.Cross((q - p), r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

    }
}
