using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveProject.Steerings;

namespace WaveProject
{
    public class Shoot : Behavior
    {
        [RequiredComponent]
        public Transform2D Transform { get; set; }
        public Steering Steering { get; set; }
        public Kinematic Kinematic { get; set; }
        public Vector2 Origin { get; set; }

        public Shoot(Vector2 origin, Vector2 target)
        {
            Origin = origin;
            Kinematic = new Kinematic() { Position = origin, MaxVelocity = 600f };
            Steering = new Seek() { Character = Kinematic, Target = new Kinematic() { Position = target } };       
        }

        protected override void Update(TimeSpan gameTime)
        {
            if ((Kinematic.Position - Origin).Length() >= (Steering.Target.Position - Origin).Length())
            {
                Steering.Dispose();
                EntityManager.Remove(EntityManager.AllEntities.First(f => f.FindComponent<Shoot>() == this));
                return;
            }
            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;

            float dt = (float)gameTime.TotalSeconds;
            SteeringOutput output = Steering.GetSteering();
            Kinematic.Update(dt, output);

            Transform.Position = Kinematic.Position;
            Transform.Rotation = Kinematic.Orientation;
        }
    }
}
