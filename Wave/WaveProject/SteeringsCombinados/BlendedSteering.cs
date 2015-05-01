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

namespace WaveProject.SteeringsCombinados
{
    public class BlendedSteering : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }

        public Steering[] Steerings { get; private set; }
        public Kinematic Kinematic { get; set; }

        public Color Color { get; set; }
        public string Target { get; set; }
        public float MaxSpeed { get; private set; }

        public BlendedSteering(Steering[] steerings, Color color, string target = null)
        {
            Kinematic = new Kinematic(true);
            Steerings = steerings;
           
            Color = color;
            Target = target;
        }

        protected override void Initialize()
        {
            base.Initialize();

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
            foreach (var steering in Steerings)
            {
                steering.Character = Kinematic;
                steering.Target = target;
            }

        }

        protected override void Update(TimeSpan gameTime)
        {

            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;

            float dt = (float)gameTime.TotalSeconds;
            
            SteeringOutput final = new SteeringOutput();

            foreach (var x in Steerings)
            {
                final = final + (x.GetSteering() * x.Weight);
            }

            final += new LookWhereYouGoing() { Character = Kinematic, Target = new Kinematic() { Position = Kinematic.Position + Kinematic.Velocity } }.GetSteering();

            Kinematic.Update(final, dt);

            Transform.Position += Kinematic.Position * dt;
            Transform.Rotation += Kinematic.Rotation * dt;

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
