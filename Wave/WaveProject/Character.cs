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
using WaveProject.Steerings;

namespace WaveProject
{
    public enum CharacterType
    {
        BEAST, LIZARD, SOLDIER, JUGGERNAUT, NONE
    }
    public class Character : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public Kinematic Kinematic { get; private set; }
        public Color Color { get; set; }
        public Steering Steering { get; set; }

        public Character(Steering steering, Kinematic kinematic, Color color, float maxVelocity = 50)
        {
            Kinematic = kinematic;
            Kinematic.MaxVelocity = maxVelocity;
            Steering = steering;
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
            SteeringOutput output = Steering.GetSteering();
            Kinematic.Update(dt, output);

            Transform.Position = Kinematic.Position;
            Transform.Rotation = Kinematic.Orientation;

            #region Escenario circular
            if (Transform.Position.X > MyScene.TiledMap.Width())
            {
                Transform.Position -= new Vector2(MyScene.TiledMap.Width(), 0);
            }
            else if (Transform.Position.X < 0)
            {
                Transform.Position += new Vector2(MyScene.TiledMap.Width(), 0);
            }
            if (Transform.Position.Y > MyScene.TiledMap.Height())
            {
                Transform.Position -= new Vector2(0, MyScene.TiledMap.Height());
            }
            else if (Transform.Position.Y < 0)
            {
                Transform.Position += new Vector2(0, MyScene.TiledMap.Height());
            }
            #endregion
        }
    }
}
