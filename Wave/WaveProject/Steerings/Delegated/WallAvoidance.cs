using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings.Delegated
{
    class WallAvoidance : Steering
    {
        public CollisionDetector CollisionDetector { get; set; }
        public float AvoidDistance { get; set; }
        public float LookAhead { get; set; }

        public WallAvoidance()
        {
            CollisionDetector = CollisionDetector.Detector;
            AvoidDistance = 100;
            LookAhead = 100;
        }

        public override SteeringOutput GetSteering()
        {
            var rayVector = Character.Velocity;
            rayVector.Normalize();
            rayVector *= LookAhead;

            Collision collision = CollisionDetector.GetCollision(Character.Position, rayVector);
            if (collision != null)
            {
                var target = collision.Position + collision.Normal * AvoidDistance;
                //Console.WriteLine("{0} {1}",collision.Position, target);

                Seek seek = new Seek();
                seek.Character = Character;
                seek.Target = new Kinematic() { Position = target };
                return seek.GetSteering();
            }
            //if (Character.Velocity == Vector2.Zero)
            //{
            //    return new SteeringOutput() { Linear = new Vector2(5000, 50) };
            //}
            //return new SteeringOutput() { Linear = Character.Velocity };
            return new SteeringOutput();
        }
    }
}
