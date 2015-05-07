using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveProject.Steerings.Delegated
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

        public override SteeringOutput GetSteering()
        {
            WanderOrientation += BinomialRandom() * WanderRate;

            float targetOrientation = WanderOrientation + Character.Orientation;

            Vector2 targetPosition = Character.Position + WanderOffset * Character.RotationAsVector();
            targetPosition += WanderRadius * targetOrientation.RotationToVector();

            Face face = new Face();
            face.Character = Character;
            face.Target = new Kinematic() { Position = targetPosition };
            SteeringOutput steering = new SteeringOutput() { Linear = MaxAcceleration * Character.RotationAsVector() };
            return face.GetSteering() + steering;
        }

        public float BinomialRandom()
        {
            return (float)(WaveServices.Random.NextDouble() - WaveServices.Random.NextDouble());
        }
    }
}
