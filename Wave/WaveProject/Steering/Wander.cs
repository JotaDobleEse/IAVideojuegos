using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveProject.Steering
{
    class Wander : Steering
    {
        public float WanderOffset { get; set; }
        public float WanderRadius { get; set; }
        public float WanderRate { get; set; }
        public float WanderOrientation { get; set; }
        public float MaxAcceleration { get; set; }

        public Wander()
        {
            WanderOffset = 100;
            WanderRadius = 30;
            WanderRate = (float)(45 * Math.PI / 180);
            MaxAcceleration = 25f;
        }

        public float BinomialRandom()
        {
            return (float)(WaveServices.Random.NextDouble() - WaveServices.Random.NextDouble());
        }

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target = null)
        {
            WanderOrientation += BinomialRandom() * WanderRate;

            float targetOrientation = WanderOrientation + origin.Transform.Rotation;

            Vector2 targetPosition = origin.Transform.Position + WanderOffset * RotationToVector(origin.Transform.Rotation);
            targetPosition += WanderRadius * RotationToVector(targetOrientation);

            Transform2D faceTarget = new Transform2D();
            faceTarget.Position = targetPosition;

            Face face = new Face();
            face.SteeringCalculation(origin, faceTarget);
            Angular = face.Angular;

            Linear = MaxAcceleration * RotationToVector(origin.Transform.Rotation);
        }

        private Vector2 RotationToVector(float rotation)
        {
            return new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
        }
    }
}
