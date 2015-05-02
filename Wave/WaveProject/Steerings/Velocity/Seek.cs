using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveProject.Steerings.Velocity
{
    [Obsolete("Usar Steerings.Seek.")]
    public class Seek : SteeringBehavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }

        [RequiredComponent]
        public Sprite Texture { get; private set; }

        public Seek(float maxVel = 300f, string entityTarget = null, Color? color = null)
            : base(maxVel, entityTarget, color)
        { }

        Steering SeekVelocity(Vector2 target)
        {
            Vector2 velocity = target - Transform.Position;
            velocity.Normalize();
            return Steering.Create(velocity * MaxSpeed);
        }

        protected override void Initialize()
        {
            base.Initialize();
            Mass = 1f;
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
        }

        protected override void Update(TimeSpan gameTime)
        {
            Vector2 target = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
            if (HaveTarget)
            {
                Entity e = EntityManager.Find(EntityTarget);
                target = e.FindComponent<Transform2D>().Position;
            }
            Steering seek = SeekVelocity(target);
            if (Math.Abs((target - Transform.Position).X) > 10F)
            {
                Transform.Position += seek.Velocity * (float)gameTime.TotalSeconds;
                Transform.Rotation = seek.Rotation;// *(float)gameTime.TotalSeconds;
            }
        }
    }
}
