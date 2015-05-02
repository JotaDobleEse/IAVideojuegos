using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings;

namespace WaveProject.SteeringsCombinados
{
    public class BlendedSteering : Steering
    {
        public BehaviorAndWeight[] Behaviors { get; private set; }


        public BlendedSteering(BehaviorAndWeight[] behaviors)
        {
            Behaviors = behaviors;
        }

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();

            foreach (var behavior in Behaviors)
            {
                steering = steering + (behavior.Behavior.GetSteering() * behavior.Weight);
            }

            return steering;
        }
    }
}
