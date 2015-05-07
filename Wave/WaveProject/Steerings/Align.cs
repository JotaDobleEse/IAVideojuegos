using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
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
            MaxRotation = (float)Math.PI / 4;
            MaxAngularAcceleration = (float)Math.PI;// 0.2f;
            TimeToTarget = 0.1f;
            TargetRadius = (float)(15 * Math.PI / 180);
            SlowRadius = (float)(75 * Math.PI / 180);
        }

        public override SteeringOutput GetSteering()
        {
            SteeringOutput steering = new SteeringOutput();
            float rotationOriginal = Target.Orientation - Character.Orientation;
            float rotation = MapToRange(rotationOriginal);
            float rotationSize = Math.Abs(rotation);

            float targetRotation;
            if (rotationSize < TargetRadius)
            {
                return steering;
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
            steering.Angular = targetRotation - Character.Rotation;
            steering.Angular /= TimeToTarget;

            float angularAcceleration = Math.Abs(steering.Angular);
            if (angularAcceleration > MaxAngularAcceleration)
            {
                steering.Angular /= angularAcceleration;
                steering.Angular *= MaxAngularAcceleration;
            }

            steering.Linear = Vector2.Zero;
            return steering;
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
