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

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Vector2 direction = target.Position - origin.Position;
            float distance = direction.Length();

            float speed = ((Vector2)characterSpeed).Length();
            throw new NotImplementedException();
        }
    }
}
