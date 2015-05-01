using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings
{
    public class Cohesion : Steering
    {
        public float Radius { get; set; }

        public Cohesion()
        {
            Radius = 200;
        }

        public override SteeringOutput GetSteering()
        {
            int count = 0;
            Vector2 centerOfMass = new Vector2(0, 0);
            var targets = Kinematic.Kinematics.Where(w => (w.Position - Character.Position).Length() <= Radius);
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
