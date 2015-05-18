using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{
    public class VelocityMatching : Steering
    {
        protected float MaxAceleration = 0.2f;
        public float TimeToTarget { get; set; }

        public VelocityMatching()
        {
            TimeToTarget = 1f;
        }

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            steering.Linear = Target.Velocity - Character.Velocity;
            steering.Linear /= TimeToTarget;

            if (steering.Linear.Length() > MaxAceleration)
            {
                steering.Linear.Normalize();
                steering.Linear *= MaxAceleration;
            }

            steering.Angular = 0f;
            return steering;
        }
    }
}
