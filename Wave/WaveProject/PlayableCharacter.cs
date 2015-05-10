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
using WaveProject.CharacterTypes;
using WaveProject.Steerings;
using WaveProject.Steerings.Combined;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class PlayableCharacter : Behavior, IDisposable
    {
        private bool disposed = false;
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public Kinematic Kinematic { get; private set; }
        public Color Color { get; set; }
        public Steering Steering { get; set; }
        public PredictivePathFollowing PathFollowing { get; set; }
        public CharacterType Type { get; private set; }

        public PlayableCharacter(Kinematic kinematic, CharacterType type, Color color, float maxVelocity = 50)
        {
            Kinematic = kinematic;
            Kinematic.MaxVelocity = maxVelocity;
            Type = type;

            //Steering = new PredictivePathFollowing(true) { Character = Kinematic, PredictTime = 0.3f };
            ////Steering = new FollowPath() { Character = Kinematic };
            //PathFollowing = (PredictivePathFollowing)Steering;

            BehaviorAndWeight[] behaviors = SteeringsFactory.PathFollowing(Kinematic);
            Steering = new BlendedSteering(behaviors);
            PathFollowing = (PredictivePathFollowing)behaviors.Select(s => s.Behavior).FirstOrDefault(f => f is PredictivePathFollowing);
            

            Color = color;
        }

        public void SetPath(List<Vector2> path)
        {
            PathFollowing.SetPath(path);
        }

        protected override void Initialize()
        {
            base.Initialize();
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
        }

        public void Draw(LineBatch2D lb)
        {
            lb.DrawCircleVM(Kinematic.Position, Math.Max(Texture.Texture.Width, Texture.Texture.Height) / 1.8f, Color.Cyan, 1f);
        }

        protected override void Update(TimeSpan gameTime)
        {
            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;

            Terrain terrain = Map.CurrentMap.TerrainOnWorldPosition(Kinematic.Position);
            Kinematic.MaxVelocity = Type.MaxVelocity(terrain);

            float dt = (float)gameTime.TotalSeconds;
            SteeringOutput output = Steering.GetSteering();
            Kinematic.Update(dt, output);

            Transform.Position = Kinematic.Position;
            Transform.Rotation = Kinematic.Orientation;
            
            #region Escenario circular
            if (Transform.Position.X > Map.CurrentMap.TotalWidth)
            {
                Transform.Position -= new Vector2(Map.CurrentMap.TotalWidth, 0);
            }
            else if (Transform.Position.X < 0)
            {
                Transform.Position += new Vector2(Map.CurrentMap.TotalWidth, 0);
            }
            if (Transform.Position.Y > Map.CurrentMap.TotalHeight)
            {
                Transform.Position -= new Vector2(0, Map.CurrentMap.TotalHeight);
            }
            else if (Transform.Position.Y < 0)
            {
                Transform.Position += new Vector2(0, Map.CurrentMap.TotalHeight);
            }
            #endregion
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Kinematic.Dispose();
                Kinematic = null;
                Steering = null;
                PathFollowing = null;
                Type = null;
            }

            disposed = true;
        }
    }
}
