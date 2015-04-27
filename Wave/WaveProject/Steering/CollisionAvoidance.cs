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
            MinBoxLength = 100;
        }

        public override void SteeringCalculation(Transform2D origin, Transform2D target, Vector2? characterSpeed = null)
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
            foreach (var obstacle in objects)
            {
                var localPos = origin.Transform.ConvertToLocalPos(obstacle.FindComponent<Transform2D>().Position);
                if (localPos.X >= 0)
                {
                    float objectRadius = obstacle.BRadius();
                    float sumRadius = objectRadius + objectRadius; // +origin.Texture.Texture.Width / 2;
                    if (Math.Abs(localPos.Y) < sumRadius)
                    {
                        float sqrtPart = (float)Math.Sqrt((sumRadius * sumRadius) - (localPos.Y * localPos.Y));
                        float intersection = localPos.X - sqrtPart;
                        if (intersection <= 0)
                        {
                            intersection = localPos.X + sqrtPart;
                        }

                        if (minIntersection < intersection)
                        {
                            minIntersection = intersection;
                            closetObstacle = obstacle;
                        }

                    }
                }
            }

            if (closetObstacle != null)
            {
                Vector2 localPos = origin.Transform.ConvertToLocalPos(closetObstacle.FindComponent<Transform2D>().Position);
                float x = localPos.X;
                float y = localPos.Y;

                Vector2 steeringLocal = Vector2.Zero;
                float factorX = 0.2f;
                float factorY = 1 + (boxLength - x) / boxLength;

                float objectRadius = closetObstacle.BRadius();

                steeringLocal.X = (objectRadius - x) * factorX;
                steeringLocal.Y = (objectRadius - y) * factorY;

                Vector2 repulsion = ConvertGlobalRotation(origin.Transform, steeringLocal);
                Linear = repulsion;
                origin.Transform.Rotation = origin.Speed.ToRotation();

            }
            else
            {
                if (Linear == Vector2.Zero)
                    Linear = new Vector2(0, -50);   
                origin.Transform.Rotation = origin.Speed.ToRotation();
            }

        }

        private IEnumerable<Entity> GetCollisionCandidates(SteeringBehavior origin)
        {
            return EntityManager.AllEntities.Where(w => w.FindComponent<SteeringBehavior>() != null && w.FindComponent<SteeringBehavior>() != origin);
        }

        private Vector2 ConvertGlobalRotation(Transform2D origin, Vector2 position)
        {
            var globalPos = position + origin.RotationAsVector();
            return globalPos;
        }
    }
}
