using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{
    public class PredictivePathFollowing : Steering
    {
        public Path Path { get; set; }
        public int PathOffset { get; set; }

        public int CurrentParam { get; set; }
        public float PredictTime { get; set; }

        public PredictivePathFollowing()
        {
            Path = new Path();
            PredictTime = 0.1f;
            PathOffset = 1;
        }

        public override SteeringOutput GetSteering()
        {
            Vector2 futurePos = Character.Position + Character.Velocity * PredictTime;

            CurrentParam = Path.GetParam(futurePos, CurrentParam);
            int targetParam = CurrentParam + PathOffset;

            Seek seek = new Seek();
            Face face = new Face();
            seek.Character = face.Character = Character;
            seek.Target = face.Target = new Kinematic() { Position = Path.GetPosition(targetParam) };

            return seek.GetSteering() + face.GetSteering();
        }
    }
}
