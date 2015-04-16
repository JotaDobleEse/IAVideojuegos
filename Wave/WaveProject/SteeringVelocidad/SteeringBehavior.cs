using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;

namespace WaveProject.SteeringVelocidad
{
    public abstract class SteeringBehavior : Behavior
    {
        public float MaxSpeed { get; private set; }
        public float Mass { get; protected set; }
        public string EntityTarget { get; private set; }
        public Color Color { get; private set; }
        public bool HaveTarget { get { return !string.IsNullOrEmpty(EntityTarget); } }

        public SteeringBehavior(float maxVel = 300f, string entityTarget = null, Color? color = null)
        {
            EntityTarget = entityTarget;
            MaxSpeed = maxVel;
            Color = (color == null ? Color.White : (Color)color);
        }
    }
}
