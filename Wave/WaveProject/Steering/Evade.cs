using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    class Evade : Steering
    {
        public float maxPrediction = 10f;

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

            //Delegar en Flee
            Steering flee = new Flee();

            Transform2D targetT = target.Transform.Clone() as Transform2D;

            targetT.Position += target.Speed * prediction;
            flee.SteeringCalculation(targetT, origin.Transform);

            Linear = flee.Linear;
            Angular = 0f;
           // Console.WriteLine("{0}", speed);
        }
        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }
    }
}
