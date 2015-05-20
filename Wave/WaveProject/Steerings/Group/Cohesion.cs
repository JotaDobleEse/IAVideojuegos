using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings.Group
{
    public class Cohesion : Steering
    {
        public float Threshold { get; set; }

        public Cohesion()
        {
            Threshold = 300;
        }

        public override SteeringOutput GetSteering()
        {
            int count = 0;
            Vector2 centerOfMass = new Vector2(0, 0);
            // Enemigos en el Threshold
            var targets = Kinematic.Kinematics.Where(w => (w.Position - Character.Position).Length() <= Threshold && w != Character);
            foreach (var target in targets)
            {
                centerOfMass += target.Position;
                count++;
            }

            if (count == 0)
                return new SteeringOutput();

            centerOfMass /= count;
            Seek seek = new Seek();
            seek.Character = Character;
            seek.Target = new Kinematic() { Position = centerOfMass};
            return seek.GetSteering();
        }
    }
}
