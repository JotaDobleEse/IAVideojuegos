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
using WaveProject.Steerings;

namespace WaveProject
{
    public class SteeringBehavior : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }

        public Steering Steering { get; set; }
        public Kinematic Kinematic { get; set; }
        public Color Color { get; set; }
        public string Target { get; set; }
        public float MaxSpeed { get; set; }

        public SteeringBehavior(Steering steering, Color color, string target = null, Kinematic kinematic = null)
        {
            if (kinematic == null)
                Kinematic = new Kinematic(true);
            else
                Kinematic = kinematic;
            Steering = steering;
            Steering.Character = Kinematic;
            Color = color;
            Target = target;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Kinematic.Velocity = Vector2.Zero;
            Kinematic.Rotation = 0f;
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
            MaxSpeed = 50;

            Kinematic target = null;
            if (string.IsNullOrEmpty(Target))
            {
                target = new Kinematic();
                Vector2 targetMouse = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
                target.Position = targetMouse;
            }
            else
            {
                try
                {
                    target = EntityManager.Find(Target).FindComponent<SteeringBehavior>().Kinematic;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Steering.Target = target;
            //Collider.Size = new Vector2(Texture.SourceRectangle.Value.Width, Texture.SourceRectangle.Value.Height);
        }
        protected override void Update(TimeSpan gameTime)
        {
            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;
            float dt = (float)gameTime.TotalSeconds;
            if (string.IsNullOrEmpty(Target))
            {
                Vector2 targetMouse = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
                Steering.Target.Position = targetMouse;
            }

            SteeringOutput output = Steering.GetSteering();

            Kinematic.Update(output, dt);
            Transform.Position = Kinematic.Position;
            Transform.Rotation = Kinematic.Orientation;

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
