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
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace WaveProject.Steering
{
    public class BlendedSteering : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }
        [RequiredComponent]
        public RectangleCollider Collider { get; private set; }

        private float Mass;
        public Steering[] Steerings { get; private set; }
        public Vector2 Speed { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public string Target { get; set; }
        public float MaxSpeed { get; private set; }

        public BlendedSteering(Steering[] steerings, Color color, string target = null)
        {
            
            Steerings = steerings;
            Color = color;
            Target = target;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Mass = 1f;
            Speed = Vector2.Zero;
            Rotation = 0f;
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
            Collider.Center = Vector2.Center;
            Collider.Transform2D = Transform;
            MaxSpeed = 50;
            //Collider.Size = new Vector2(Texture.SourceRectangle.Value.Width, Texture.SourceRectangle.Value.Height);
        }

        protected override void Update(TimeSpan gameTime)
        {
            float dt = (float)gameTime.TotalSeconds;
            SteeringBehavior target = null;
            SteeringBehavior final = new SteeringBehavior(null, Color.Beige);

            foreach (var x in Steerings)
            {
                final = x.SteeringCalculation(tran)
            }
            

            if (string.IsNullOrEmpty(Target))
            {
                Transform2D targetTransform = new Transform2D();
                Vector2 targetMouse = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
                targetTransform.Position = targetMouse;
                target = new SteeringBehavior(null, Color.Brown) { Transform = targetTransform };
            }
            else
            {
                try
                {
                    target = EntityManager.Find(Target).FindComponent<SteeringBehavior>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Steering.SteeringCalculation(this, target);
            Transform.Position += Speed * dt;
            Transform.Rotation += Rotation * dt;

            if (Speed.X > MaxSpeed)
                Speed = new Vector2(50, Speed.Y);
            else if (Speed.X < -MaxSpeed)
                Speed = new Vector2(-MaxSpeed, Speed.Y);
            if (Speed.Y > MaxSpeed)
                Speed = new Vector2(Speed.X, MaxSpeed);
            else if (Speed.Y < -MaxSpeed)
                Speed = new Vector2(Speed.X, -MaxSpeed);
            //Console.WriteLine(Speed);

            Speed += Steering.Linear * dt;
            Rotation += Steering.Angular * dt;
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
