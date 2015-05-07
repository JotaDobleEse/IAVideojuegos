using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework.Graphics;
using WaveProject.Steerings.Delegated;

namespace WaveProject.Steerings.Delegated
{
    public class FollowPath: Steering
    {
        public Path Path { get; set; }
        public int PathOffset { get; set; }

        public int CurrentParam { get; set; }
        public FollowPath(bool stable = false) : base(stable)
        {
            Path = new Path();
            PathOffset = 1;
        }

        public override SteeringOutput GetSteering()
        {
            if (Path.Length == 0)
                return new SteeringOutput();

            CurrentParam = Path.GetParam(Character.Position, CurrentParam);
            int targetParam = CurrentParam + PathOffset;

            if (Path.Length - 1 == CurrentParam)
            {
                Arrive arrive = new Arrive();
                Face face = new Face();
                arrive.Character = face.Character = Character;
                arrive.Target = face.Target = new Kinematic() { Position = Path.GetPosition(targetParam) };

                return arrive.GetSteering() + face.GetSteering();
            }
            else
            {
                Seek seek = new Seek();
                Face face = new Face();
                seek.Character = face.Character = Character;
                seek.Target = face.Target = new Kinematic() { Position = Path.GetPosition(targetParam) };

                return seek.GetSteering() + face.GetSteering();
            }
        }

        public override void Draw(LineBatch2D lb)
        {
            Path.DrawPath(lb);
        }
    }
}
