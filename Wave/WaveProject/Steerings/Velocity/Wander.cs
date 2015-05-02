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
    [Obsolete("Use Steerings.Delegated.Wander.")]
    public class Wander : SteeringBehavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }

        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public float MaxRotation { get; private set; }

        private int SameDirection = 0;
        private const int LOOPCOUNT = 15;
        private Vector2 SameVelocity;

        public Wander(float maxVel = 300f, Color? color = null)
            : base(maxVel, null, color)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            Mass = 1f;
            MaxRotation = (float)(60 * Math.PI / 180);
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
        }

        public float BinomialRandom()
        {
            return (float)(WaveServices.Random.NextDouble() - WaveServices.Random.NextDouble());
        }

        Steering WanderVelocity()
        {
            Vector2 velocity = new Vector2((float)Math.Sin(Transform.Rotation), -(float)Math.Cos(Transform.Rotation)) * MaxSpeed;
            Steering steering = Steering.Create(velocity);
            float rand = BinomialRandom();
            steering.Rotation = rand * MaxRotation;
            return steering;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (SameDirection == 0)
            {
                Steering wander = WanderVelocity();
                SameVelocity = wander.Velocity;
                Transform.Rotation += wander.Rotation * (float)gameTime.TotalSeconds*60;
            }
            Transform.Position += SameVelocity * (float)gameTime.TotalSeconds;

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
            SameDirection = (SameDirection + 1) % LOOPCOUNT;
        }
    }
}
