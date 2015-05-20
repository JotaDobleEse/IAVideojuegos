using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{
    public class Flee : Steering
    {
        protected float MaxAceleration = 1f;

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            steering.Linear = Character.Position - Target.Position; // Única variación con respecto al Seek
            steering.Linear.Normalize();
            steering.Linear *= MaxAceleration;

            steering.Angular = 0f;
            return steering;
        }
    }
}
