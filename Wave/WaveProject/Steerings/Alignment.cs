using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings
{
    public class Alignment : Steering
    {
        public float Radius { get; set; }

        public Alignment()
        {
            Radius = 200;
        }

        public override SteeringOutput GetSteering()
        {
            int count = 0;
            Vector2 heading = new Vector2(0, 0);
            var targets = Kinematic.Kinematics.Where(w => (w.Position - Character.Position).Length() <= Radius);
            foreach (var target in targets)
            {
                heading += target.Position + target.Velocity;
                count++;
            }

            if (count == 0)
                return new SteeringOutput();

            heading /= count;

            Align align = new Align();
            VelocityMatching velocityMatching = new VelocityMatching();
            align.Character = velocityMatching.Character = Character;
            align.Target = velocityMatching.Target = new Kinematic() { Position = heading };

            return velocityMatching.GetSteering() + align.GetSteering();
        }
    }
}
