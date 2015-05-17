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
using WaveProject.DecisionManager;
using WaveProject.Steerings;
using WaveProject.Steerings.Combined;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class CharacterNPC : Behavior, ICharacterInfo
    {
        private bool disposed = false;
        [RequiredComponent]
        public Transform2D Transform { get; private set; }
        [RequiredComponent]
        public Sprite Texture { get; private set; }
        public Kinematic Kinematic { get; private set; }
        public Color Color { get; set; }
        public Steering Steering { get; set; }
        public FollowPath PathFollowing { get; set; }
        public CharacterType Type { get; private set; }
        public ActionManager ActionManager { get; set; }
        public int Team { get; set; }

        public CharacterNPC(Kinematic kinematic, EnumeratedCharacterType type, int team/*, Color color*/)
        {
            Kinematic = kinematic;
            Kinematic.MaxVelocity = 30;
            //Steering = steering;
            Team = team;
            ActionManager = new ActionManager();
            switch (type)
            {
                case EnumeratedCharacterType.EXPLORER:
                    Type = new ExplorerCharacter(this, EntityManager);
                    break;
                case EnumeratedCharacterType.MELEE:
                    Type = new MeleeCharacter(this, EntityManager);
                    break;
                case EnumeratedCharacterType.RANGED:
                    Type = new RangedCharacter(this, EntityManager);
                    break;
                case EnumeratedCharacterType.NONE:
                    Type = new MeleeCharacter(this, EntityManager);
                    break;
            }

            if (team == 1)
                Color = Color.Cyan;
            if (team == 2)
                Color = Color.Red;
            //Color = color;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Transform.Origin = Vector2.Center;
            Texture.TintColor = Color;
            Type.EntityManager = EntityManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float dt = (float)gameTime.TotalSeconds;
            Action newAction = Type.Update();
            ActionManager.ScheduleAction(new GenericAction(1f, 1, true, newAction));
            ActionManager.Execute(dt);

            if (Steering == null)
                return;
            Kinematic.Position = Transform.Position;
            Kinematic.Orientation = Transform.Rotation;

            Terrain terrain = Map.CurrentMap.TerrainOnWorldPosition(Kinematic.Position);
            Kinematic.MaxVelocity = Type.MaxVelocity(terrain);

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

        public Vector2 GetPosition()
        {
            return Kinematic.Position;
        }

        public int GetTeam()
        {
            return Team;
        }

        public EnumeratedCharacterType GetCharacterType()
        {
            return Type.GetCharacterType();
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
                Type = null;
            }

            disposed = true;
        }

        public Vector2 GetVelocity()
        {
            return Kinematic.Velocity;
        }

        public void SetTarget(Kinematic target)
        {
            Steering.SetTarget(target);
        }
        
        public void SetPathFinding(Vector2 target)
        {
            if (Steering != null)
                Steering.Dispose();
            BehaviorAndWeight[] behaviors = SteeringsFactory.PathFollowing(Kinematic);
            Steering = new BlendedSteering(behaviors);
            PathFollowing = (FollowPath)behaviors.Select(s => s.Behavior).FirstOrDefault(f => f is FollowPath);

            LRTA lrta = new LRTA(Kinematic.Position, target, Type, DistanceAlgorith.CHEVYCHEV);
            var path = lrta.Execute();
            PathFollowing.SetPath(path);
        }

        public void ReceiveHeal(int hp)
        {
            Type.HP = Math.Min(Type.HP + hp, Type.MaxHP);
        }

        public void ReceiveAttack(int atk)
        {
            float damage = (atk / (float)Type.Def) * 10;
            Type.HP = Math.Max(Type.HP - (int)damage, 0);
            Console.WriteLine(Type.HP);
        }

        public bool IsDead()
        {
            return Type.HP <= 0;
        }
    }
}
