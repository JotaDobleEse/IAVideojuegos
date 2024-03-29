﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings.Delegated
{
    // Datos de una colisión, posición de colisión y normal perpendicular al punto
    public class Collision
    {
        public Vector2 Position { get; set; }
        public Vector2 Normal { get; set; }

        public override string ToString()
        {
            return string.Format("Pos: {0}, Norm: {1}", Position, Normal);
        }
    };

    public class CollisionDetector
    {
        private Vector2 posicion = Vector2.Zero;
        private Vector2 movimiento = Vector2.Zero;
        private Vector2 interseccion = Vector2.Zero;
        private Vector2 normal1 = Vector2.Zero;

        public CollisionDetector()
        {
        }

        public Collision GetCollision(Vector2 position, Vector2 moveAmount)
        {
            Collision firstCollision = null;
            // Para cada muro
            foreach (var wall in Wall.Walls)
            {
                // Creamos una lista de puntos de intersección
                Vector2 v;
                List<Vector2> intersections = new List<Vector2>();
                // Creamos variable para saber cuantos lados del muro colisionan
                int face1, face2, face3, face4;
                face1 = face2 = face3 = face4 = 0;

                // Comprobamos si interseca con  cada pared
                if (LineSegementsIntersect(position, position + moveAmount, wall.P1, wall.P2, out v))
                {
                    intersections.Add(v.Clone());
                    face1 = intersections.Count;
                }
                if (LineSegementsIntersect(position, position + moveAmount, wall.P2, wall.P4, out v))
                {
                    intersections.Add(v.Clone());
                    face2 = intersections.Count;
                }
                if (LineSegementsIntersect(position, position + moveAmount, wall.P4, wall.P3, out v))
                {
                    intersections.Add(v.Clone());
                    face3 = intersections.Count;
                }
                if (LineSegementsIntersect(position, position + moveAmount, wall.P3, wall.P1, out v))
                {
                    intersections.Add(v.Clone());
                    face4 = intersections.Count;
                }

                posicion = position;
                movimiento = posicion + moveAmount;

                // Si hay interseccioes
                if (intersections.Count > 0)
                {
                    // Obtenemos la posición de la intersección mas cercana al personaje
                    var pos = intersections.OrderBy(o => (o - position).Length()).First();
                    // Sabemos el lado que interseca por su posición en la lista +1
                    int face = intersections.IndexOf(pos) + 1;
                    // Creamos la normal
                    Vector2 normal = Vector2.Zero;

                    // Calculamos la rotación
                    float rotation = (position - pos).ToRotation();

                    // Si choca con el lado 1 o el 2
                    if (face == face1 || face == face2)
                    {
                        // Sacamos las dos normales posibles del lado
                        var vect = pos - wall.P2;
                        var norm1 = vect.Norm1();
                        var norm2 = vect.Norm2();

                        // Nos quedamos con la que mira hacia nosotros
                        float r1 = ((norm1 + pos) - position).Length();
                        float r2 = ((norm2 + pos) - position).Length();
                        if (r1 < r2)
                        {
                            normal = norm1;
                        }
                        else
                        {
                            normal = norm2;
                        }
                    }
                    // Si choca con el lado 3 o el 4
                    else if (face == face3 || face == face4)
                    {
                        // Sacamos las dos normales posibles del lado
                        var vect = pos - wall.P3;
                        var norm1 = vect.Norm1();
                        var norm2 = vect.Norm2();

                        // Nos quedamos con la que mira hacia nosotros
                        float r1 = ((norm1 + pos) - position).Length();
                        float r2 = ((norm2 + pos) - position).Length();
                        if (r1 < r2)
                        {
                            normal = norm1;
                        }
                        else
                        {
                            normal = norm2;
                        }
                    }
                    // Guardamos el punto de intersección y la normal
                    interseccion = pos;
                    normal1 = normal;

                    // Nos quedamos con la mejor colisión
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

        // Codigo de intersección sacado de internet
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

        public void Draw(LineBatch2D lb)
        {
            lb.DrawLineVM(interseccion, normal1 + interseccion, Color.Green, 1);
        }

    }
}
