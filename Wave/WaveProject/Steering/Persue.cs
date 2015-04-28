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
        public float maxPrediction = 1f;

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


            //Delegar en Seek
            Seek seek = new Seek();
            seek.Character = Character;
            seek.Target = new Kinematic() { Position = Target.Position + Target.Velocity * prediction };
            return seek.GetSteering();
        }
    }

}
