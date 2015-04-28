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

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            steering.Linear = Character.Position - Target.Position;
            steering.Linear.Normalize();
            steering.Linear *= MaxAceleration;

            steering.Angular = 0f;
            return steering;
        }
    }
}
