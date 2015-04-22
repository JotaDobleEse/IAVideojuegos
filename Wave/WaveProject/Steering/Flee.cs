using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public class Flee : Steering
    {
        protected float MaxAceleration = 0.2f;

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = origin.Position - target.Position;
            Linear.Normalize();
            Linear *= MaxAceleration;

            Angular = 0f;
        }

        public override void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin)
        {
            SteeringCalculation(target.Transform, origin.Transform);
        }
    }
}
