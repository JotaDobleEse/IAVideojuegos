using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings.Delegated
{
    class WallAvoidance : Steering
    {
        public CollisionDetector CollisionDetector { get; set; }
        public float AvoidDistance { get; set; }
        public float LookAhead { get; set; }

        Vector2 Ray1, Ray2, RayCenter;

        public WallAvoidance() : base(true)
        {
            CollisionDetector = new CollisionDetector();
            AvoidDistance = 40;
            LookAhead = 80;
        }

        public override SteeringOutput GetSteering()
        {
            // HACK
            if (Character.Velocity == Vector2.Zero)
                return new SteeringOutput();
            // END HACK

            var rayVector = Character.Velocity;
            rayVector.Normalize();
            rayVector *= LookAhead;

            Ray1 = rayVector.RotateVector((float)(45 * Math.PI / 180)) * 0.75f;
            Ray2 = rayVector.RotateVector((float)(-45 * Math.PI / 180)) * 0.75f;
            RayCenter = rayVector * 1.25f;

            Collision collision1 = CollisionDetector.GetCollision(Character.Position, RayCenter);
            Collision collision2 = CollisionDetector.GetCollision(Character.Position, Ray1);
            Collision collision3 = CollisionDetector.GetCollision(Character.Position, Ray2);
            if (collision1 != null)
            {
                var target = collision1.Position + collision1.Normal * AvoidDistance;
                //Console.WriteLine("{0} {1}",collision.Position, target);

                Seek seek = new Seek();
                seek.Character = Character;
                seek.Target = new Kinematic() { Position = target };
                return seek.GetSteering();
            }
            if (collision2 != null)
            {
                var target = Ray1 * AvoidDistance;
                //Console.WriteLine("{0} {1}",collision.Position, target);

                Seek seek = new Seek();
                seek.Character = Character;
                seek.Target = new Kinematic() { Position = target };
                return seek.GetSteering();
            }
            if (collision3 != null)
            {
                var target = Ray2 * AvoidDistance;
                //Console.WriteLine("{0} {1}",collision.Position, target);

                Seek seek = new Seek();
                seek.Character = Character;
                seek.Target = new Kinematic() { Position = target };
                return seek.GetSteering();
            }
            return new SteeringOutput();
        }

        public override void Draw(LineBatch2D lb)
        {
            if (!Ray1.IsNull() && !Ray2.IsNull())
            {
                lb.DrawLineVM(Character.Position, Character.Position + Ray1, Color.Cyan, 1f);
                lb.DrawLineVM(Character.Position, Character.Position + Ray2, Color.Cyan, 1f);
                lb.DrawLineVM(Character.Position, Character.Position + RayCenter, Color.Cyan, 1f);
            }
        }
    }
}
