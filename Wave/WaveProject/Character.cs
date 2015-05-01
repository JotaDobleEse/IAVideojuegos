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
using WaveProject.SteeringVelocidad;

namespace WaveProject
{
    public class Character : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public Kinematic Kinematic { get; private set; }
        public Color Color { get; set; }
        public SteeringBehavior Steering { get; set; }

        public Character(SteeringBehavior steering, Color color, float MaxSpeed = 50, string target = null)
        {
            Kinematic = new Kinematic(true);
            Kinematic.MaxVelocity = 50;
            Steering = steering;
            Color = color;
            //Target = target;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;

            //Kinematic target = null;
            //if (string.IsNullOrEmpty(Steering.Target))
            //{
            //    target = new Kinematic();
            //    Vector2 targetMouse = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
            //    target.Position = targetMouse;
            //}
            //else
            //{
            //    try
            //    {
            //        target = EntityManager.Find(Target).FindComponent<SteeringBehavior>().Kinematic;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
            //}
            //Steering.Target = target;
            
        }

        protected override void Update(TimeSpan gameTime)
        {

        }
    }
}
