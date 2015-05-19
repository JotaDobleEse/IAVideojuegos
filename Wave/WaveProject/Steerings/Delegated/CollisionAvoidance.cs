using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steerings.Delegated
{
    class CollisionAvoidance : Steering
    {
        public float MinBoxLength { get; set; }
        public float MaxAcceleration { get; set; }

        private List<Vector2> PositionsLocals = new List<Vector2>();
        private float BoxLenght = 0f;

        public CollisionAvoidance(bool stable = false) : base(stable)
        {
            MinBoxLength = 50;
            MaxAcceleration = 20f;
        }

        public override SteeringOutput GetSteering()
        {
            PositionsLocals.Clear();
            // HACK
            if (Character.Velocity == Vector2.Zero)
                return new SteeringOutput();
            // END HACK

            float minIntersection = 0;
            Obstacle closetObstacle = null;
            float boxLength = MinBoxLength + (Character.Velocity.Length() / new Vector2(Character.MaxVelocity, Character.MaxVelocity).Length()) * MinBoxLength;
            BoxLenght = boxLength;
            List<Obstacle> obstacles = ObstaclesInRange(boxLength);
            foreach (var obstacle in obstacles)
            {
                var localPos = Character.ConvertToLocalPos(obstacle.Position);
                if (localPos.X >= 0)
                {
                    PositionsLocals.Add(localPos);
                    float objectRadius = obstacle.BRadius;
                    float sumRadius = objectRadius + Character.BRadius;
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
                Console.WriteLine(localPos);

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
                //Character.Orientation = Character.Velocity.ToRotation(); //Despues

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

        private List<Obstacle> ObstaclesInRange(float boxLength)
        {
            return Obstacle.Obstacles.Where(w => Character.ConvertToLocalPos(w.Position).X > 0 && Character.ConvertToLocalPos(w.Position).X < 0).ToList();
        }

        public override void Draw(LineBatch2D lb)
        {
            foreach (var position in PositionsLocals)
            {
                lb.DrawCircleVM(position, 12, Color.Cyan, 1f);
            }
            lb.DrawLineVM(Vector2.Zero, new Vector2(BoxLenght, 0), Color.Green, 1f);
        }
    }
}
