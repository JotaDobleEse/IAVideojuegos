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
using WaveProject.Steerings;
using WaveProject.Steerings.Combined;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class PlayableCharacter : Behavior, IWalker
    {
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

            //Steering = new PredictivePathFollowing(true) { Character = Kinematic, PredictTime = 0.6f };
            //Steering = new FollowPath() { Character = Kinematic };

            BehaviorAndWeight[] behaviors = SteeringsFactory.PathFollowing(Kinematic);
            Steering = new BlendedSteering(behaviors);
            PathFollowing = (PredictivePathFollowing)(behaviors.Select(s => s.Behavior).FirstOrDefault(f => f is PredictivePathFollowing) ?? new PredictivePathFollowing());

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
            lb.DrawCircleVM(Kinematic.Position, 35, Color.Red, 1f);
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

        public float Cost(Terrain terrain)
        {
            switch (Type)
            {
                case CharacterType.RANGED:
                    return CostRanged(terrain);
                case CharacterType.EXPLORER:
                    return CostExplorer(terrain);
                case CharacterType.MELEE:
                    return CostMelee(terrain);
            }
            return 1;
        }

        private float CostRanged(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 1f;
                case Terrain.FOREST:
                    return 5f;
                case Terrain.PATH:
                    return 2f;
                case Terrain.PLAIN:
                    return 0.7f;
                case Terrain.WATER:
                    return float.PositiveInfinity;
            }
            return 1;
        }

        private float CostExplorer(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 1.5f;
                case Terrain.FOREST:
                    return 0.5f;
                case Terrain.PATH:
                    return 5f;
                case Terrain.PLAIN:
                    return 3f;
                case Terrain.WATER:
                    return float.PositiveInfinity;
            }
            return 1;
        }

        private float CostMelee(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 2f;
                case Terrain.FOREST:
                    return 5f;
                case Terrain.PATH:
                    return 0.6f;
                case Terrain.PLAIN:
                    return 1.5f;
                case Terrain.WATER:
                    return float.PositiveInfinity;
            }
            return 1;
        }
    }
}
