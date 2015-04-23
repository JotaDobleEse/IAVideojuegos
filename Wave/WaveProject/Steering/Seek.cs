using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public class Seek : Steering
    {
        protected float MaxAceleration = 0.1f;

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = target.Position - origin.Position;
            Linear.Normalize();
            Linear *= MaxAceleration;

            Angular = 0f;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            SteeringCalculation(target.Transform, origin.Transform);
        }
    }
}
