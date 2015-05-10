using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings;
using WaveProject.Steerings.Delegated;

namespace WaveProject.Steerings.Combined
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
        public static implicit operator PredictivePathFollowing(BlendedSteering steering)
        {
            var s = Steerings.FirstOrDefault(f => f is PredictivePathFollowing);
            if (s != null)
                return (PredictivePathFollowing)s;
            return null;
        }
    }
}
