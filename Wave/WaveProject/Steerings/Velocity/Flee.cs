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
    [Obsolete("Usar Steerings.Flee.")]
    public class Flee : SteeringBehavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }

        [RequiredComponent]
        public Sprite Texture { get; private set; }

        public Flee(float maxVel = 300f, string entityTarget = null, Color? color = null)
            : base(maxVel, entityTarget, color)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            Mass = 1f;
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
        }

        Steering FleeVelocity(Vector2 target)
        {
            Vector2 velocity = Transform.Position - target;
            velocity.Normalize();

            return Steering.Create(velocity * MaxSpeed);
        }

        protected override void Update(TimeSpan gameTime)
        {
            Vector2 target = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
            if (!string.IsNullOrEmpty(EntityTarget))
            {
                Entity e = EntityManager.Find(EntityTarget);
                target = e.FindComponent<Transform2D>().Position;
            }
            double dist = Math.Sqrt(Math.Pow(target.X - Transform.Position.X, 2) + Math.Pow(target.Y - Transform.Position.Y, 2));
            if (dist <= 200f)
            {
                Steering flee = FleeVelocity(target);
                Transform.Position += flee.Velocity * (float)gameTime.TotalSeconds;
                Transform.Rotation = flee.Rotation;
            }

            //Transform.Position = new Vector2(Math.Abs(Transform.Position.X), Math.Abs(Transform.Position.Y));
            //Transform.Position = new Vector2((Transform.Position.X > WaveServices.Platform.ScreenWidth ? WaveServices.Platform.ScreenWidth : Transform.Position.X), (Transform.Position.Y > WaveServices.Platform.ScreenHeight ? WaveServices.Platform.ScreenHeight : Transform.Position.Y));
            #region Escenario circular
            if (Transform.Position.X > WaveServices.Platform.ScreenWidth)
            {
                Transform.Position -= new Vector2(WaveServices.Platform.ScreenWidth, 0);
            }
            else if (Transform.Position.X < 0)
            {
                Transform.Position += new Vector2(WaveServices.Platform.ScreenWidth, 0);
            }
            if (Transform.Position.Y > WaveServices.Platform.ScreenHeight)
            {
                Transform.Position -= new Vector2(0, WaveServices.Platform.ScreenHeight);
            }
            else if (Transform.Position.Y < 0)
            {
                Transform.Position += new Vector2(0, WaveServices.Platform.ScreenHeight);
            }
            #endregion

        }
    }
}
