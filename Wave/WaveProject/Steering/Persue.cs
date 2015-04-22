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
        public float maxPrediction = 10f;

        public override void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin)
        {
            Vector2 direction = target.Transform.Position - origin.Transform.Position;
            float distance = direction.Length();

            float speed = origin.Speed.Length();

            // Idea feliz
            //Linear = direction * (speed * distance);
            //float T = distance / 0.2f;

            Linear = target.Transform.Position + target.Speed * maxPrediction;

            //Delegar en seek?
            Linear.Normalize();
            Linear *= 0.2f;

            Angular = 0f;
            //throw new NotImplementedException();
        }
        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            throw new NotImplementedException();
        }

    }

}
