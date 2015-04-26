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

        public override void SteeringCalculation(Transform2D origin, Transform2D target, Vector2? characterSpeed = null)
        {
            Linear = origin.Position - target.Position;
            Linear.Normalize();
            Linear *= MaxAceleration;

            Angular = 0f;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            SteeringCalculation(origin.Transform, target.Transform);
        }
    }
}
