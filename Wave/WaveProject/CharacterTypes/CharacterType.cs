using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Managers;
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
        //definir RAngo de ataque, Arquero tendra mayor rango

        public CharacterType(ICharacterInfo myInfo, EntityManager entityManager, int hp = 0, int atk = 0, int def = 0)
        {
            MyInfo = myInfo;
            EntityManager = entityManager;
            MaxHP = HP = hp;
            Atk = atk;
            Def = def;
        }

        public abstract float Cost(Terrain terrain);

        public abstract float MaxVelocity(Terrain terrain);
        public abstract EnumeratedCharacterType GetCharacterType();

        public abstract void Update();
        public abstract void Attack(CharacterType character);

        public ICharacterInfo FindEnemyNear()
        {
            var characters = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is ICharacterInfo))
                .Select(s => s.Components.First(f => f is ICharacterInfo) as ICharacterInfo)
                .Where(w => w.GetTeam() == (MyInfo.GetTeam() % 2) + 1);

            //Buscamos el mejor objetivo de entre los enemigos, debemos de utilizar la visibilidad para comprobar los enemigos en el grid
            var enemy = characters.Where(w => (w.GetPostion() - MyInfo.GetPostion()).Length() <= VisibilityRadius)
                .FirstOrDefault();

            return enemy;
        }

        public void GoToHeal()
        {
            var healpoint = Map.CurrentMap.HealPoints.Where(w => w.Team == MyInfo.GetTeam())
                .OrderBy(o => (o.Position - MyInfo.GetPostion()).Length())
                .Select(s => s.Position)
                .FirstOrDefault();

            healpoint += new Vector2(Map.HealRatio, -Map.HealRatio);

            if ((healpoint - MyInfo.GetPostion()).Length() < Map.HealRatio)
            {
                HP = Math.Max(HP + 1, MaxHP);
            }
            else
            {
                List<Vector2> ps = new List<Vector2>();
                ps.Add(healpoint + new Vector2(Map.HealRatio, Map.HealRatio));
                ps.Add(healpoint + new Vector2(Map.HealRatio, -Map.HealRatio));
                ps.Add(healpoint + new Vector2(-Map.HealRatio, Map.HealRatio));
                ps.Add(healpoint + new Vector2(-Map.HealRatio, -Map.HealRatio));

                foreach (var p in ps)
                {
                    var pos = Map.CurrentMap.TilePositionByWolrdPosition(p);
                    if (pos != new Vector2(-1, -1) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                    {
                        healpoint = pos;
                        break;
                    }
                }

                // PATHFINDING
                MyInfo.SetPathFinding(healpoint);
            }
        }

        public void GoToEnemyBase()
        {
            var healpoint = Map.CurrentMap.HealPoints.Where(w => w.Team != MyInfo.GetTeam())
                .OrderBy(o => (o.Position - MyInfo.GetPostion()).Length())
                .Select(s => s.Position)
                .FirstOrDefault();

            healpoint += new Vector2(Map.HealRatio, -Map.HealRatio);

            List<Vector2> ps = new List<Vector2>();
            ps.Add(healpoint + new Vector2(Map.HealRatio, Map.HealRatio));
            ps.Add(healpoint + new Vector2(Map.HealRatio, -Map.HealRatio));
            ps.Add(healpoint + new Vector2(-Map.HealRatio, Map.HealRatio));
            ps.Add(healpoint + new Vector2(-Map.HealRatio, -Map.HealRatio));

            foreach (var p in ps)
            {
                var pos = Map.CurrentMap.TilePositionByWolrdPosition(p);
                if (pos != new Vector2(-1, -1) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                {
                    healpoint = pos;
                    break;
                }
            }

            // PATHFINDING
            MyInfo.SetPathFinding(healpoint);
        }

        public void GoToMyBase()
        {
            GoToHeal();
        }

        public void GoToWaypoint()
        {

        }
    }
}
