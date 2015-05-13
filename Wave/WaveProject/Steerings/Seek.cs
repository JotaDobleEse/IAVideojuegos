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
        protected float MaxAceleration = 1f;

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            #region Millington
            /*steering.Linear = Target.Position - Character.Position;
            steering.Linear.Normalize();
            steering.Linear *= MaxAceleration;*/
            #endregion
            
            #region Craig
            var direction = Target.Position - Character.Position;
            direction.Normalize();
            var desiredVelocity = direction * Character.MaxVelocity;
            steering.Linear = desiredVelocity - Character.Velocity;
            #endregion

            steering.Angular = 0f;
            return steering;
        }
    }
}
