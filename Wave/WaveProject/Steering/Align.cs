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

        int n = 0;
        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            float rotation = target.Rotation - origin.Rotation;
            rotation = MapToRange(rotation);
            float rotationSize = Math.Abs(rotation);
            
            float targetRotation;
            if (rotationSize < TargetRadius)
            {
                //Console.WriteLine("Rotation size: {0}, Target Radius: {1}", rotationSize, TargetRadius);
                targetRotation = 0f;
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
            //Console.WriteLine(Angular);
            Angular /= TimeToTarget;

            float angularAcceleration = Math.Abs(Angular);
            if (angularAcceleration > MaxAngularAcceleration)
            {
                Angular /= angularAcceleration;
                Angular *= MaxAngularAcceleration;
            }

            Linear = Vector2.Zero;
        }

        public override void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin, Vector2? characterSpeed = null)
        {
            throw new NotImplementedException();
        }

        private float MapToRange(float rotation)
        {
            float r = rotation;
            float Pi = (float)Math.PI;
            if (rotation > Pi)
            {
                r -= 2 * Pi;
            }
            else if (rotation < -Pi)
            {
                r += 2 * Pi;
            }
            return r;
        }
    }
}
