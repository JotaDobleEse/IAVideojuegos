using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{
    public class Arrive : Steering
    {
        public Vector2 MaxSpeed { get; set; }
        public float TargetRadius { get; set; }
        public float SlowRadius { get; set; }
        public float TimeToTarget { get; set; }

        public float MaxAceleration { get; set; }

        public Arrive(bool stable = false)
            : base(stable)
        {
            MaxSpeed = new Vector2(150, 150);
            TimeToTarget = 0.1f;
            TargetRadius = 50f;
            SlowRadius = 150f;
            MaxAceleration = 0.1f;
        }

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            Vector2 direction = Target.Position - Character.Position;
            float distance = direction.Length();

            if (distance < TargetRadius)
            {
                steering.Linear = Vector2.Zero;
            }

            Vector2 targetSpeed, targetVelocity;

            if (distance > SlowRadius)
            {
                targetSpeed = MaxSpeed;
            }
            else
            {
                targetSpeed = MaxSpeed * distance / SlowRadius;
            }

            targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity *= targetSpeed;

            steering.Linear = targetVelocity - Character.Velocity;
            steering.Linear /= TimeToTarget;

            if (steering.Linear.Length() > MaxAceleration)
            {
                steering.Linear.Normalize();
                steering.Linear *= MaxAceleration;
            }

            steering.Angular = 0;
            return steering;
        }

        public override void Draw(LineBatch2D lb)
        {
            lb.DrawCircleVM(Target.Position, SlowRadius, Color.White, 1f);
            lb.DrawCircleVM(Target.Position, TargetRadius, Color.Orange, 1f);
        }
    }

     
}
