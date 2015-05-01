using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{
    public class Seek : Steering
    {
        protected float MaxAceleration = 0.1f;

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            steering.Linear = Target.Position - Character.Position;
            steering.Linear.Normalize();
            steering.Linear *= MaxAceleration;

            steering.Angular = 0f;
            return steering;
        }
    }
}
