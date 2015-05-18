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
using WaveProject.CharacterTypes;
using WaveProject.Steerings;

namespace WaveProject.Characters
{
    public class SteeringCharacter : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public Kinematic Kinematic { get; private set; }
        public Color Color { get; set; }
        public Steering Steering { get; set; }

        public SteeringCharacter(Kinematic kinematic, EnumeratedCharacterType type, Steering steering = null)
        {
            Kinematic = kinematic;
            Kinematic.MaxVelocity = 500;
            Steering = steering;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color.White;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float dt = (float)gameTime.TotalSeconds;
            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;

            SteeringOutput output = Steering.GetSteering();
            Kinematic.Update(dt, output);

            Transform.Position = Kinematic.Position;
            Transform.Rotation = Kinematic.Orientation;

            var width = WaveServices.Platform.ScreenWidth * 2;
            var height = WaveServices.Platform.ScreenHeight * 2;
            #region Escenario circular
            if (Transform.Position.X > width)
            {
                Transform.Position -= new Vector2(width, 0);
            }
            else if (Transform.Position.X < 0)
            {
                Transform.Position += new Vector2(width, 0);
            }
            if (Transform.Position.Y > height)
            {
                Transform.Position -= new Vector2(0, height);
            }
            else if (Transform.Position.Y < 0)
            {
                Transform.Position += new Vector2(0, height);
            }
            #endregion
        }
    }
}
