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

namespace WaveProject.SteeringVelocidad
{
    public class Arrive : SteeringBehavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }

        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public float Radius { get; private set; }
        public float TimeToTarget { get; private set; }

        public Arrive(float maxVel = 300f, string entityTarget = null, Color? color = null)
            : base(maxVel, entityTarget, color)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            Mass = 1f;
            TimeToTarget = 0.25f;
            Radius = 200f;
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
        }

        Steering ArriveVelocity(Vector2 target, Color? color = null)
        {
            Vector2 velocity = target - Transform.Position;

            float dist = (float)Math.Sqrt(Math.Pow(target.X - Transform.Position.X, 2) + Math.Pow(target.Y - Transform.Position.Y, 2));
            
            if (dist > Radius)
            {
                velocity.Normalize();
                return Steering.Create(velocity * MaxSpeed);
            }
            else
            {
                velocity.Normalize();
                return Steering.Create(velocity * (MaxSpeed * (dist / Radius)));
            }

            #region VersionApuntes
            //if (velocity.Length() < Radius)
            //    return Vector2.Zero;

            //velocity /= TimeToTarget;

            //if (velocity.Length() > MaxSpeed)
            //{
            //    velocity.Normalize();
            //    velocity *= MaxSpeed;
            //}

            //return velocity;
            #endregion
        }

        protected override void Update(TimeSpan gameTime)
        {
            Vector2 target = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
            if (!string.IsNullOrEmpty(EntityTarget))
            {
                Entity e = EntityManager.Find(EntityTarget);
                target = e.FindComponent<Transform2D>().Position;
            }
            Steering arrive = ArriveVelocity(target);
            if (arrive.Velocity == Vector2.Zero)
                return;
            Transform.Position += arrive.Velocity * (float)gameTime.TotalSeconds;
            Transform.Rotation = arrive.Rotation;
        }
    }
}
