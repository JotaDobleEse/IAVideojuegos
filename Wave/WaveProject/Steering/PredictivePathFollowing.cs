using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
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

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target = null)
        {
            Vector2 futurePos = origin.Transform.Position + origin.Speed * PredictTime;

            CurrentParam = Path.GetParam(futurePos, CurrentParam);
            int targetParam = CurrentParam + PathOffset;

            Transform2D seekTarget = new Transform2D();
            seekTarget.Position = Path.GetPosition(targetParam);

            //Console.WriteLine("Curr: {0}, Pos: {1}",targetParam, seekTarget.Position);

            Seek seek = new Seek();
            seek.SteeringCalculation(seekTarget, origin.Transform);
            Linear = seek.Linear;

            //Arrive arrive = new Arrive();
            //arrive.SteeringCalculation(seekTarget, origin.Transform, origin.Speed);
            //Linear = arrive.Linear;

            Face face = new Face();
            face.SteeringCalculation(origin, seekTarget);
            Angular = face.Angular;
        }
    }
}
