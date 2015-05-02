using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings;

namespace WaveProject.SteeringsCombinados
{
    public class PrioritySteering : Steering
    {
        public BlendedSteering[] Groups { get; private set; }
        public float Epsilon { get; set; }


        public PrioritySteering(BlendedSteering[] groups, float epsilon = 0.05f)
        {
            Groups = groups;
            Epsilon = epsilon;
        }

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            foreach (var group in Groups)
            {
                steering = group.GetSteering();

                if (steering.Linear.Length() > Epsilon || steering.Angular.Abs() > Epsilon)
                {
                    return steering;
                }
            }

            return steering;
        }
    }
}
