using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    class Face : Steering
    {

        public override SteeringOutput GetSteering()
        {
            var direction = Target.Position - Character.Position;

            if (direction.Length() == 0)
            {
                return new SteeringOutput();
            }

            Align align = new Align();
            align.Character = Character;
            align.Target = new Kinematic() { Orientation = direction.ToRotation() };
            return align.GetSteering();
        }
    }
}
