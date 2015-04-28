using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveProject.Steering;

namespace WaveProject
{
    public class Kinematic
    {
        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float MaxVelocity { get; set; }

        public Kinematic()
        {
            MaxVelocity = 50;
        }

        public void Update(SteeringOutput steering, float deltaTime)
        {
            Velocity += steering.Linear * deltaTime;
            Rotation += steering.Angular * deltaTime;

            if (Velocity.X > MaxVelocity)
                Velocity = new Vector2(MaxVelocity, Velocity.Y);
            else if (Velocity.X < -MaxVelocity)
                Velocity = new Vector2(-MaxVelocity, Velocity.Y);
            if (Velocity.Y > MaxVelocity)
                Velocity = new Vector2(Velocity.X, MaxVelocity);
            else if (Velocity.Y < -MaxVelocity)
                Velocity = new Vector2(Velocity.X, -MaxVelocity);

            Position += Velocity * deltaTime;
            Orientation += Rotation * deltaTime;
        }
    }
}
