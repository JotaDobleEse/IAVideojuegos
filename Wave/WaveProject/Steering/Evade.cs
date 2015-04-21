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

        public override void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin, Vector2? characterSpeed = null)
        {
            Vector2 direction = origin.Transform.Position - target.Transform.Position;
            float distance = direction.Length();

            float speed = ((Vector2)characterSpeed).Length();

            // Idea feliz
            Linear = target.Transform.Position + target.Speed * maxPrediction;

            //Delegar en Flee
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
