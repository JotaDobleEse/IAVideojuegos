using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public class VelocityMatching : Steering
    {
        protected float MaxAceleration = 0.2f;
        public float TimeToTarget { get; set; }

        public VelocityMatching()
        {
            TimeToTarget = 0.1f;
        }

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            Linear = target.Speed - origin.Speed;
            Linear /= TimeToTarget;

            if (Linear.Length() > MaxAceleration)
            {
                Linear.Normalize();
                Linear *= MaxAceleration;
            }

            Angular = 0f;
        }
    }
}
