using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings.Velocity
{
    [Obsolete("Usar Steerings.Steering.")]
    class Steering
    {
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }

        public static Steering Create(Vector2 vel)
        {
            return new Steering() { Velocity = vel, Rotation = (float)Math.Atan2(vel.X, -vel.Y) };
        }
    }
}
