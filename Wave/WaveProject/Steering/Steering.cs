using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public abstract class Steering
    {
        /// <summary>
        /// Get
        /// </summary>
        public Vector2 Linear { get; protected set; }
        /// <summary>
        /// Get
        /// </summary>
        public float Angular { get; protected set; }

        public Steering()
        {
            Linear = Vector2.Zero;
            Angular = 0f;
        }

        public abstract void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null);
        public abstract void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin, Vector2? characterSpeed = null);
    }
}
