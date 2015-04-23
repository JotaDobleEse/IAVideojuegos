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

namespace WaveProject.Steering
{
    public class SteeringBehavior : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        private Sprite Texture;

        private float Mass;
        public Steering Steering { get; private set; }
        public Vector2 Speed { get; private set; }
        public float Rotation { get; private set; }
        public Color Color { get; set; }
        public string Target { get; set; }

        public SteeringBehavior(Steering steering, Color color, string target = null)
        {
            Steering = steering;
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
        }
        protected override void Update(TimeSpan gameTime)
        {
            float dt = (float)gameTime.TotalSeconds;
            SteeringBehavior target = null;
            
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
 
            Steering.SteeringCalculation(target, this); 
            Transform.Position += Speed * dt;
            Transform.Rotation += Rotation * dt;
            Speed += Steering.Linear * dt;
            if (this.Speed.X > 100)
                this.Speed = new Vector2(100, this.Speed.Y);
            if (this.Speed.Y > 100)
                this.Speed = new Vector2(this.Speed.X, 100);
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
