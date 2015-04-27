using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    class WallAvoidance : Steering
    {
        public CollisionDetector CollisionDetector { get; set; }
        public float AvoidDistance { get; set; }
        public float LookAhead { get; set; }

        public WallAvoidance()
        {
            CollisionDetector = CollisionDetector.Detector;
            AvoidDistance = 50;
            LookAhead = 20;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target = null)
        {
            var rayVector = origin.Speed;
            rayVector.Normalize();
            rayVector *= LookAhead;

            Collision collision = CollisionDetector.GetCollision(origin.Transform.Position, rayVector);
            if (collision != null)
            {
                var targ = collision.Position + collision.Normal * AvoidDistance;
                Transform2D targetTransform = new Transform2D();
                targetTransform.Position = targ;

                Seek seek = new Seek();
                seek.SteeringCalculation(origin.Transform, targetTransform);
                Linear = seek.Linear;
                Angular = seek.Angular;
            }
            else
            {
                Linear = new Vector2(-50, 0);
                origin.Transform.Rotation = origin.Speed.ToRotation();
            }
        }
    }
}
