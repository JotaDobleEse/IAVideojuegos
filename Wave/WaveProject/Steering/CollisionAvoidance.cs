using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steering
{
    class CollisionAvoidance : Steering
    {
        public EntityManager EntityManager { get; set; }
        public float MinBoxLength { get; set; }

        public CollisionAvoidance(EntityManager entityManager)
        {
            EntityManager = entityManager;
            MinBoxLength = 30;
        }

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target = null)
        {
            float minIntersection = 0;
            Entity closetObstacle = null;
            float boxLength = MinBoxLength + (origin.Speed.Length() / new Vector2(origin.MaxSpeed, origin.MaxSpeed).Length()) * MinBoxLength;
            List<Entity> objects = GetCollisionCandidates(origin).ToList();
            //Console.WriteLine(objects.Count);
            foreach (var obstacle in objects)
            {
                var localPos = ConvertLocalPos(origin.Transform, obstacle.FindComponent<Transform2D>().Position);
                //Console.WriteLine("{0} -- {1}", localPos, obstacle.FindComponent<Transform2D>().Position);
                if (localPos.X >= 0)
                {
                    var objectTransform = obstacle.FindComponent<Transform2D>();
                    float objectRadius = Math.Max(objectTransform.Rectangle.Width, objectTransform.Rectangle.Height) * 0.75f;
                    //Console.WriteLine("{0}: {1}", obstacle.Name, objectRadius);
                    float sumRadius = objectRadius + objectRadius; // +origin.Texture.Texture.Width / 2;
                    if (Math.Abs(localPos.Y) < sumRadius)
                    {
                        float sqrtPart = (float)Math.Sqrt((sumRadius * sumRadius) - (localPos.Y * localPos.Y));
                        float intersection = localPos.X - sqrtPart;
                        if (intersection <= 0)
                        {
                            intersection = localPos.X + sqrtPart;
                        }
                        //if (obstacle.Name == "look")
                        //    Console.WriteLine("{0}: {1} - {2}", obstacle.Name, minIntersection, intersection);
                        if (minIntersection < intersection)
                        {
                            //Console.WriteLine("{0}: {1} - {2}", obstacle.Name, minIntersection, intersection);
                            minIntersection = intersection;
                            closetObstacle = obstacle;
                        }

                    }
                }
            }

            if (closetObstacle != null)
            {
                Vector2 localPos = ConvertLocalPos(origin.Transform, closetObstacle.FindComponent<Transform2D>().Position);
                float x = localPos.X;// closetObstacle.FindComponent<Transform2D>().LocalPosition.X;
                float y = localPos.Y;// closetObstacle.FindComponent<Transform2D>().LocalPosition.Y;

                Vector2 steeringLocal = Vector2.Zero;
                float factorX = 0.2f;
                float factorY = 1 + (boxLength - x) / boxLength;

                var objectTransform = closetObstacle.FindComponent<Transform2D>();
                float objectRadius = Math.Max(objectTransform.Rectangle.Width, objectTransform.Rectangle.Height) * 0.75f;

                steeringLocal.X = (objectRadius - (float)Math.Abs(x)) * factorX;
                steeringLocal.Y = (objectRadius - (float)Math.Abs(y)) * factorY;

                Vector2 repulsion = ConvertGlobalPos(origin.Transform, steeringLocal);
                //origin.Speed = repulsion;
                //origin.Rotation = (float)Math.Atan2(repulsion.X, -repulsion.Y);
                Linear = repulsion;
                Linear.Normalize();
                Linear *= 0.2f;
                origin.Transform.Rotation = (float)Math.Atan2(origin.Speed.X, -origin.Speed.Y);

            }
            else
            {
                //origin.Speed = new Vector2(0, -50); ;
                //origin.Rotation = (float)Math.Atan2(origin.Speed.X, -origin.Speed.Y);
                //Linear = new Vector2(0, -50);
                //Angular = (float)Math.Atan2(Linear.X, -Linear.Y);
                origin.Transform.Rotation = (float)Math.Atan2(origin.Speed.X, -origin.Speed.Y);
            }

        }

        private IEnumerable<Entity> GetCollisionCandidates(SteeringBehavior origin)
        {
            return EntityManager.AllEntities.Where(w => w.FindComponent<SteeringBehavior>() != null && w.FindComponent<SteeringBehavior>() != origin);
            //if (origin.Speed.X > 0)
            //{
            //    if (origin.Speed.Y > 0)
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X > origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y > origin.Transform.Position.Y);
            //    }
            //    else
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X > origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y < origin.Transform.Position.Y);
            //    }
            //}
            //else
            //{
            //    if (origin.Speed.Y > 0)
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X < origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y > origin.Transform.Position.Y);
            //    }
            //    else
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X < origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y < origin.Transform.Position.Y);
            //    }
            //}
        }
        private Vector2 ConvertGlobalPos(Transform2D origin, Vector2 position)
        {
            var globalPos = position + origin.Position;
            globalPos += RotationToVector(origin.Rotation);
            return globalPos;
        }

        private Vector2 ConvertLocalPos(Transform2D origin, Vector2 position)
        {
            var localPos = position - origin.Position;
            localPos -= RotationToVector(origin.Rotation);
            return localPos;
        }

        private Vector2 RotationToVector(float rotation)
        {
            return new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
        }
    }
}
