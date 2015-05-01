using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{
    class Evade : Steering
    {
        public float maxPrediction = 10f;

        public override SteeringOutput GetSteering()
        {
            Vector2 direction = Target.Position - Character.Position;

            float distance = direction.Length();

            float speed = Character.Velocity.Length();

            float prediction = 0f;

            if (speed <= distance / maxPrediction)
                prediction = maxPrediction;
            else
                prediction = distance / speed;

            //Delegar en Flee
            Steering flee = new Flee();

            flee.Character = Character;
            flee.Target = new Kinematic() { Position = Target.Velocity * prediction };
            return flee.GetSteering();
        }
    }
}
