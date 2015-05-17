using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Managers;
using WaveProject.Characters;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public enum EnumeratedCharacterType
    {
        NONE, EXPLORER, MELEE, RANGED
    }
    public abstract class CharacterType
    {
        public EntityManager EntityManager { get; set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public float VisibilityRadius { get; set; }
        public ICharacterInfo MyInfo { get; set; }
        private Vector2 LastWaypoint = Vector2.Zero;
        //definir RAngo de ataque, Arquero tendra mayor rango

        public CharacterType(ICharacterInfo myInfo, EntityManager entityManager, int hp = 0, int atk = 0, int def = 0, float visibilityRadius = 0f)
        {
            Random r = new Random();
            MyInfo = myInfo;
            EntityManager = entityManager;
            MaxHP = HP = hp + (r.Next(20));
            Atk = atk + (r.Next(5));
            Def = def + (r.Next(5));
            VisibilityRadius = visibilityRadius;
        }

        public abstract float Cost(Terrain terrain);

        public abstract float MaxVelocity(Terrain terrain);
        public abstract EnumeratedCharacterType GetCharacterType();

        public abstract Action Update();
        public abstract void Attack(ICharacterInfo character);

        public void AttackEnemyNear()
        {
            Attack(FindEnemyNear());
        }

        public ICharacterInfo FindEnemyNear()
        {
            var characters = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo)
                .Where(w => w.GetTeam() == (MyInfo.GetTeam() % 2) + 1);

            //Buscamos el mejor objetivo de entre los enemigos, debemos de utilizar la visibilidad para comprobar los enemigos en el grid
            var enemy = characters.Where(w => (w.GetPosition() - MyInfo.GetPosition()).Length() <= VisibilityRadius)
                .OrderBy(o => (o.GetPosition() - MyInfo.GetPosition()).Length())
                .FirstOrDefault();

            return enemy;
        }

        public void GoToHeal()
        {
            var healpoint = Map.CurrentMap.GetBestHealPoinPosition(MyInfo);

            if ((Map.CurrentMap.WorldPositionByTilePosition(healpoint) - MyInfo.GetPosition()).Length() < Map.HealRatio)
            {
                HP = Math.Max(HP + 5, MaxHP);
            }
            else
            {
                List<Vector2> ps = new List<Vector2>();
                ps.Add(healpoint + new Vector2(Map.HealRatio, Map.HealRatio));
                ps.Add(healpoint + new Vector2(Map.HealRatio, -Map.HealRatio));
                ps.Add(healpoint + new Vector2(-Map.HealRatio, Map.HealRatio));
                ps.Add(healpoint + new Vector2(-Map.HealRatio, -Map.HealRatio));
                ps.Add(healpoint + new Vector2(0, Map.HealRatio));
                ps.Add(healpoint + new Vector2(0, -Map.HealRatio));
                ps.Add(healpoint + new Vector2(Map.HealRatio, 0));
                ps.Add(healpoint + new Vector2(-Map.HealRatio, 0));

                float minLength = float.PositiveInfinity;
                foreach (var pos in ps)
                {
                    if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                    {
                        var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                        var length = (worldPos - MyInfo.GetPosition()).Length();
                        if (length < minLength)
                        {
                            minLength = length;
                            healpoint = worldPos;
                        }
                    }
                }

                //Console.WriteLine(healpoint);
                // PATHFINDING
                MyInfo.SetPathFinding(healpoint);
            }
        }

        public void GoToEnemyBase()
        {
            var enemyBase = Map.CurrentMap.HealPoints.Where(w => w.Team != MyInfo.GetTeam())
                .Select(s => s.Position)
                .OrderBy(o => (Map.CurrentMap.WorldPositionByTilePosition(o) - MyInfo.GetPosition()).Length())
                .FirstOrDefault();

            List<Vector2> ps = new List<Vector2>();
            ps.Add(enemyBase + new Vector2(1, 1));
            ps.Add(enemyBase + new Vector2(1, -1));
            ps.Add(enemyBase + new Vector2(-1, 1));
            ps.Add(enemyBase + new Vector2(-1, -1));
            ps.Add(enemyBase + new Vector2(0, 1));
            ps.Add(enemyBase + new Vector2(0, -1));
            ps.Add(enemyBase + new Vector2(1, 0));
            ps.Add(enemyBase + new Vector2(-1, 0));

            float minLength = float.PositiveInfinity;
            foreach (var pos in ps)
            {
                if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                {
                    var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                    var length = (worldPos - MyInfo.GetPosition()).Length();
                    if (length < minLength)
                    {
                        minLength = length;
                        enemyBase = worldPos;
                    }
                }
            }

            //Console.WriteLine(healpoint);
            // PATHFINDING
            MyInfo.SetPathFinding(enemyBase);
        }

        public void GoToMyBase()
        {
            var myBase = Map.CurrentMap.GetBestHealPoinPosition(MyInfo);

            if ((myBase - MyInfo.GetPosition()).Length() < Map.HealRatio)
            {
                HP = Math.Max(HP + 1, MaxHP);
            }
            else
            {
                List<Vector2> ps = new List<Vector2>();
                ps.Add(myBase + new Vector2( 1,  1));
                ps.Add(myBase + new Vector2( 1, -1));
                ps.Add(myBase + new Vector2(-1,  1));
                ps.Add(myBase + new Vector2(-1, -1));
                ps.Add(myBase + new Vector2( 0,  1));
                ps.Add(myBase + new Vector2( 0, -1));
                ps.Add(myBase + new Vector2( 1,  0));
                ps.Add(myBase + new Vector2(-1,  0));

                float minLength = float.PositiveInfinity;
                foreach (var pos in ps)
                {
                    if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                    {
                        var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                        var length = (worldPos - MyInfo.GetPosition()).Length();
                        if (length < minLength)
                        {
                            minLength = length;
                            myBase = worldPos;
                        }
                    }
                }

                //Console.WriteLine(healpoint);
                // PATHFINDING
                MyInfo.SetPathFinding(myBase);
            }
        }

        public void GoToWaypoint()
        {
            var mapPos = Map.CurrentMap.TilePositionByWolrdPosition(MyInfo.GetPosition());
            if (Map.CurrentMap.Waypoints.Any(a => mapPos == a))
                LastWaypoint = mapPos;

            var waypoint = Map.CurrentMap.Waypoints.Where(w => mapPos != w && LastWaypoint != w)
                .OrderBy(o => (o - mapPos).Length())
                .FirstOrDefault();

            MyInfo.SetPathFinding(Map.CurrentMap.WorldPositionByTilePosition(waypoint));
        }
    }
}
