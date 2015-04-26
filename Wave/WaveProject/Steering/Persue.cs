using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public class Persue : Steering
    {
        public float maxPrediction = 1f;

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            Vector2 direction = target.Transform.Position - origin.Transform.Position;

            float distance = direction.Length();

            float speed = origin.Speed.Length();

            float prediction = 0f;

            if (speed <= distance / maxPrediction)
                prediction = maxPrediction;
            else
                prediction = distance / speed;


            //Delegar en Seek
            Seek seek = new Seek();

            Transform2D targetT = target.Transform.Clone() as Transform2D;
           
            targetT.Position += target.Speed * prediction;
            seek.SteeringCalculation(origin.Transform, targetT);

            Linear = seek.Linear;
            Angular = 0f;
            //Console.WriteLine("{0}", speed);

        }
        public override void SteeringCalculation(Transform2D origin, Transform2D target, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }

    }

}
