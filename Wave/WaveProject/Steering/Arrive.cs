using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public class Arrive : Steering
    {
        public Vector2 MaxSpeed { get; set; }
        public float TargetRadius { get; set; }
        public float SlowRadius { get; set; }
        public float TimeToTarget { get; set; }

        protected float MaxAceleration = 0.1f;
        
        public Arrive()
        {
            MaxSpeed = new Vector2(150, 150);
            TimeToTarget = 0.1f;
            TargetRadius = 50f;
            SlowRadius = 150f;
        }

        public override void SteeringCalculation(Transform2D origin, Transform2D target, Vector2? characterSpeed)
        {
            if (characterSpeed == null)
                throw new NotSupportedException("characterSpeed not optional");
            
            Vector2 direction = target.Position - origin.Position;
            float distance = direction.Length();

            if (distance < TargetRadius)
            {
                Linear = Vector2.Zero;
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

            Linear = targetVelocity - (Vector2)characterSpeed;
            Linear /= TimeToTarget;

            if (Linear.Length() > MaxAceleration)
            {
                Linear.Normalize();
                Linear *= MaxAceleration;
            }

            Angular = 0;
        }
        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            SteeringCalculation(origin.Transform, target.Transform, origin.Speed);
        }
    }

     
}
