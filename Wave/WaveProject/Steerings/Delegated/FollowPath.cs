using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
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

        public void SetPath(List<Vector2> path)
        {
            CurrentParam = 0;
            Path.SetPath(path);
            CurrentParam = Path.GetParam(Character.Position, CurrentParam);
        }

        public override SteeringOutput GetSteering()
        {
            if (Path.Length == 0)
                return new SteeringOutput();

            CurrentParam = Path.GetParam(Character.Position, CurrentParam);
            int targetParam = CurrentParam + PathOffset;

            var targetPosition = Path.GetPosition(targetParam);
            if (targetPosition == Vector2.Zero)
                return new SteeringOutput();

            if (Path.Length - 1 == CurrentParam)
            {
                // HACK
                if ((targetPosition - Character.Position).Length() < 5f)
                {
                    Character.Velocity = Vector2.Zero;
                    Character.Rotation = 0f;
                    return new SteeringOutput();
                }
                // END HACK

                Arrive arrive = new Arrive();
                Face face = new Face();
                arrive.Character = face.Character = Character;
                arrive.Target = face.Target = new Kinematic() { Position = targetPosition };

                return arrive.GetSteering() + face.GetSteering();
            }
            else
            {
                Seek seek = new Seek();
                Face face = new Face();
                seek.Character = face.Character = Character;
                seek.Target = face.Target = new Kinematic() { Position = targetPosition };

                return seek.GetSteering() + face.GetSteering();
            }
        }

        public override void Draw(LineBatch2D lb)
        {
            Path.DrawPath(lb, Character.Position, CurrentParam);
        }
    }
}
