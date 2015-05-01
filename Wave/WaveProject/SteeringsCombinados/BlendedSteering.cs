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

        public BehaviorAndWeight[] Behaviors { get; private set; }
        public Kinematic Kinematic { get; set; }

        public Color Color { get; set; }
        public string Target { get; set; }

        public BlendedSteering(BehaviorAndWeight[] behaviors, Kinematic kinematic, Color color)
        {
            Kinematic = kinematic;
            Behaviors = behaviors;
           
            Color = color;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
        }

        protected override void Update(TimeSpan gameTime)
        {

            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;

            float dt = (float)gameTime.TotalSeconds;
            
            SteeringOutput final = new SteeringOutput();

            foreach (var x in Behaviors)
            {
                final = final + (x.Behavior.GetSteering() * x.Weight);
            }

            Kinematic.Update(final, dt);

            Transform.Position = Kinematic.Position;
            Transform.Rotation = Kinematic.Rotation;

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
