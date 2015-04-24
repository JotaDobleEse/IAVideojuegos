using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    class CollisionAvoidance : Steering
    {
        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            throw new NotImplementedException();
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            throw new NotImplementedException();
        }
    }
}
