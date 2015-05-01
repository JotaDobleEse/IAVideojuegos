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

namespace WaveProject.Steerings
{
    class CollisionAvoidance : Steering
    {
        public EntityManager EntityManager { get; set; }
        public float MinBoxLength { get; set; }
        public float MaxAcceleration { get; set; }

        public CollisionAvoidance(EntityManager entityManager)
        {
            EntityManager = entityManager;
            MinBoxLength = 80;
            MaxAcceleration = 20f;
        }

        public override SteeringOutput GetSteering()
        {

            float minIntersection = 0;
            Obstacle closetObstacle = null;
            float boxLength = MinBoxLength + (Character.Velocity.Length() / new Vector2(Character.MaxVelocity, Character.MaxVelocity).Length()) * MinBoxLength;
            List<Obstacle> obstacles = GetCollisionCandidates(Character).ToList();
            foreach (var obstacle in obstacles)
            {
                var localPos = Character.ConvertToLocalPos(obstacle.Position);
                if (localPos.X >= 0)
                {
                    float objectRadius = obstacle.BRadius;
                    float sumRadius = objectRadius + objectRadius;
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
                Vector2 localPos = Character.ConvertToLocalPos(closetObstacle.Position);
                float x = localPos.X;
                float y = localPos.Y;

                Vector2 steeringLocal = Vector2.Zero;
                float factorX = 0.2f;
                float factorY = 1 + (boxLength - x) / boxLength;

                float objectRadius = closetObstacle.BRadius;


                steeringLocal.X = (objectRadius - x) * factorX;
                steeringLocal.Y = (objectRadius - y) * factorY;

                Vector2 repulsion = ConvertGlobalRotation(Character, steeringLocal);
                repulsion.Normalize();

                SteeringOutput steering = new SteeringOutput();
                steering.Linear = repulsion * MaxAcceleration;
                Character.Orientation = Character.Velocity.ToRotation(); //Despues

                steering.Angular = 0;
                return steering;

            }
            return new SteeringOutput();

        }

        private Vector2 ConvertGlobalRotation(Kinematic Character, Vector2 position)
        {
            var globalPos = position + Character.RotationAsVector();
            return globalPos;
        }

        private IEnumerable<Obstacle> GetCollisionCandidates(Kinematic Character)
        {
            return Obstacle.Obstacles;
            //return EntityManager.AllEntities.Where(w => w.FindComponent<SteeringBehavior>() != null);
        }
    }
}
