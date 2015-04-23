using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public class Align : Steering
    {
        public float MaxAngularAcceleration { get; set; }
        public float MaxRotation { get; set; }
        public float TargetRadius { get; set; }
        public float SlowRadius { get; set; }
        public float TimeToTarget { get; set; }
        
        public Align()
        {
            MaxRotation = (float)Math.PI;
            MaxAngularAcceleration = 0.2f;
            TimeToTarget = 0.1f;
            TargetRadius = (float)(15 * Math.PI / 180);
            SlowRadius = (float)(75 * Math.PI / 180);
        }

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Angular = 0;
            Linear = Vector2.Zero;
        }

        public override void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin)
        {
            Angular = 0;
            float rotationOriginal = target.Transform.Rotation - origin.Transform.Rotation;
            float rotation = MapToRange(rotationOriginal);
            float rotationSize = Math.Abs(rotation);

            float targetRotation;
            if (rotationSize < TargetRadius)
            {
                Angular = 0;
                Linear = Vector2.Zero;
                return;
            }

            if (rotationSize > SlowRadius)
            {
                targetRotation = MaxRotation;
            }
            else
            {
                targetRotation = rotationSize / SlowRadius;
            }

            targetRotation *= rotation / rotationSize;

            Angular = targetRotation - origin.Rotation;
            Angular /= TimeToTarget;

            float angularAcceleration = Math.Abs(Angular);
            if (angularAcceleration > MaxAngularAcceleration)
            {
                Angular /= angularAcceleration;
                Angular *= MaxAngularAcceleration;
            }

            Linear = Vector2.Zero;
        }

        private float MapToRange(float rotation)
        {
            float r = rotation;
            float Pi = (float)Math.PI;
            if (rotation > Pi)
            {
                return r - 2 * Pi;
            }
            else if (rotation < -Pi)
            {
                return r + 2 * Pi;
            }
            else return r;
        }
    }
}
