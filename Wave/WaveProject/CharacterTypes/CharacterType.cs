using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Managers;
using WaveProject.Characters;
using WaveProject.DecisionManager;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    // Enumerado con los tipos de personajes
    public enum EnumeratedCharacterType
    {
        NONE, EXPLORER, MELEE, RANGED
    }

    // Clase base para el tipo de personaje
    public abstract class CharacterType
    {
        public EntityManager EntityManager { get; set; }
        // Salud máxima
        public int MaxHP { get; set; }
        // Salud actual
        public int HP { get; set; }
        // Ataque
        public int Atk { get; set; }
        // Defensa
        public int Def { get; set; }
        // Radio de visibilidad
        public float VisibilityRadius { get; set; }
        // 
        public ICharacterInfo MyInfo { get; set; }
        private Vector2 LastWaypoint = Vector2.Zero;
        //definir RAngo de ataque, Arquero tendra mayor rango

        public CharacterType(ICharacterInfo myInfo, EntityManager entityManager, int hp = 0, int atk = 0, int def = 0, float visibilityRadius = 0f)
        {
            Random r = new Random();
            MyInfo = myInfo;
            EntityManager = entityManager;
            MaxHP = HP = hp + (r.Next(21)); // Asignamos la vida con un factor aleatorio
            Atk = atk + (r.Next(6)); // Asignamos el ataque con un factor aleatorio
            Def = def + (r.Next(6)); // Asignamos la defensa con un factor aleatorio
            VisibilityRadius = visibilityRadius;
        }

        public abstract float Cost(Terrain terrain);

        public abstract float MaxVelocity(Terrain terrain);
        public abstract EnumeratedCharacterType GetCharacterType();

        public abstract GenericAction Update();
        public abstract void Attack(ICharacterInfo character);

        public void AttackEnemyNear()
        {
            Attack(FindEnemyNear());
        }

        public ICharacterInfo FindEnemyNear()
        {
            // Obtenemos todos los personajes del otro equipo
            var characters = EntityManager.AllCharactersByTeam((MyInfo.GetTeam() % 2) + 1);

            //Buscamos el mejor objetivo de entre los enemigos, debemos de utilizar la visibilidad para comprobar los enemigos en el grid
            var enemy = characters.Where(w => (w.GetPosition() - MyInfo.GetPosition()).Length() <= VisibilityRadius)
                .OrderBy(o => (o.GetPosition() - MyInfo.GetPosition()).Length())
                .FirstOrDefault();

            return enemy;
        }

        public void GoToHeal()
        {
            // Obtenemos el mejor punto de curación
            var healpoint = Map.CurrentMap.GetBestHealPoinPosition(MyInfo);

            // Cogemos las posiciones del radio exterior del área de curación
            List<Vector2> ps = new List<Vector2>();
            ps.Add(healpoint + new Vector2(Map.HealRatio, Map.HealRatio));
            ps.Add(healpoint + new Vector2(Map.HealRatio, -Map.HealRatio));
            ps.Add(healpoint + new Vector2(-Map.HealRatio, Map.HealRatio));
            ps.Add(healpoint + new Vector2(-Map.HealRatio, -Map.HealRatio));
            ps.Add(healpoint + new Vector2(0, Map.HealRatio));
            ps.Add(healpoint + new Vector2(0, -Map.HealRatio));
            ps.Add(healpoint + new Vector2(Map.HealRatio, 0));
            ps.Add(healpoint + new Vector2(-Map.HealRatio, 0));

            // Buscamos el mejor candidato
            float minLength = float.PositiveInfinity;
            foreach (var pos in ps)
            {
                if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                {
                    var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                    var length = (worldPos - MyInfo.GetPosition()).Length();
                    if (length < minLength && !EntityManager.PositionOcupped(MyInfo))
                    {
                        minLength = length;
                        healpoint = worldPos;
                    }
                }
            }

            // Establecemos el mejor punto como destino
            MyInfo.SetPathFinding(healpoint);
            
        }

        public void GoToEnemyBase()
        {
            // Buscamos el mejor punto de la base enemiga
            var enemyBase = Map.CurrentMap.HealPoints.Where(w => w.Team != MyInfo.GetTeam())
                .Select(s => s.Position)
                .OrderBy(o => (Map.CurrentMap.WorldPositionByTilePosition(o) - MyInfo.GetPosition()).Length())
                .FirstOrDefault();

            // Establecemos los posibles puntos alrededor de la base
            List<Vector2> ps = new List<Vector2>();
            ps.Add(enemyBase + new Vector2(1, 1));
            ps.Add(enemyBase + new Vector2(1, -1));
            ps.Add(enemyBase + new Vector2(-1, 1));
            ps.Add(enemyBase + new Vector2(-1, -1));
            ps.Add(enemyBase + new Vector2(0, 1));
            ps.Add(enemyBase + new Vector2(0, -1));
            ps.Add(enemyBase + new Vector2(1, 0));
            ps.Add(enemyBase + new Vector2(-1, 0));

            // Cogemos el candidato más prometedor
            float minLength = float.PositiveInfinity;
            foreach (var pos in ps)
            {
                if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                {
                    var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                    var length = (worldPos - MyInfo.GetPosition()).Length();
                    if (length < minLength && !EntityManager.PositionOcupped(MyInfo))
                    {
                        minLength = length;
                        enemyBase = worldPos;
                    }
                }
            }

            // Vamos mejor punto
            MyInfo.SetPathFinding(enemyBase);
        }

        public void GoToMyBase()
        {
            // Buscamos el mejor punto de nuestra base
            var myBase = Map.CurrentMap.GetBestHealPoinPosition(MyInfo);

            // Establecemos los posibles puntos alrededor de la base
            List<Vector2> ps = new List<Vector2>();
            ps.Add(myBase + new Vector2( 1,  1));
            ps.Add(myBase + new Vector2( 1, -1));
            ps.Add(myBase + new Vector2(-1,  1));
            ps.Add(myBase + new Vector2(-1, -1));
            ps.Add(myBase + new Vector2( 0,  1));
            ps.Add(myBase + new Vector2( 0, -1));
            ps.Add(myBase + new Vector2( 1,  0));
            ps.Add(myBase + new Vector2(-1,  0));

            // Cogemos el candidato más prometedor
            float minLength = float.PositiveInfinity;
            foreach (var pos in ps)
            {
                if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                {
                    var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                    var length = (worldPos - MyInfo.GetPosition()).Length();
                    if (length < minLength && !EntityManager.PositionOcupped(MyInfo))
                    {
                        minLength = length;
                        myBase = worldPos;
                    }
                }
            }

            // Vamos mejor punto
            MyInfo.SetPathFinding(myBase);
        }

        public void GoToWaypoint()
        {
            // Se obtiene el tile en el que se encuentra el personaje
            var mapPos = Map.CurrentMap.TilePositionByWolrdPosition(MyInfo.GetPosition());

            // Obtenemos el waypoint mas cercano
            var waypoint = Map.CurrentMap.Waypoints.Where(w => mapPos != w)
                .OrderBy(o => (o - mapPos).Length())
                .FirstOrDefault();

            // Vamos hacia él
            MyInfo.SetPathFinding(Map.CurrentMap.WorldPositionByTilePosition(waypoint));
        }
    }
}
