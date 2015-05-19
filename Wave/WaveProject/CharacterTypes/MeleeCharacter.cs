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
    public class MeleeCharacter : CharacterType
    {
        public MeleeCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 120, 40, 45, 280) // Párametros iniciales
        {
            // HP    = 120
            // Atk   =  40
            // Def   =  45
            // Radio = 280
        }

        // Coste del melee al pasar por un terreno
        public override float Cost(Terrain terrain)
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
                    return 8f;
            }
            return 1;
        }

        // Velocidad máxima del melee al pasar por un terreno
        public override float MaxVelocity(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 17;
                case Terrain.FOREST:
                    return 15;
                case Terrain.PATH:
                    return 30;
                case Terrain.PLAIN:
                    return 25;
                case Terrain.WATER:
                    return 10;
            }
            return 20;
        }

        public override EnumeratedCharacterType GetCharacterType()
        {
            return EnumeratedCharacterType.MELEE;
        }

        // Árbol de decisión
        public override GenericAction Update()
        {
            //ATAQUE
            if (HP >= MaxHP * 0.40)
            {
                var enemy = FindEnemyNear();

                //SI ENCONTRAMOS UN ENEMIGO
                if (enemy != null)
                {
                    return new GenericAction(1f, 1, false, AttackEnemyNear);
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA
                    return new GenericAction(1f, 1, false, GoToEnemyBase);
                }
            }
            //DEFENSA
            else if (HP > MaxHP*0.40 && HP > MaxHP*0.20)
            {
                var enemy = FindEnemyNear();

                //SI ENCONTRAMOS UN ENEMIGO
                if (enemy != null && enemy.GetCharacterType() == EnumeratedCharacterType.EXPLORER)
                {
                    return new GenericAction(1f, 1, false, AttackEnemyNear);
                }
                //SI NO ENCONTRAMOS ENEMIGO CERCA VAMOS A UN WAYPOINT
                if (Map.CurrentMap.IsInWaypoint(MyInfo.GetPosition()))
                    return new GenericAction(20f, 1, true, GoToHeal);
                return new GenericAction(1f, 1, false, GoToWaypoint);
            }
            return new GenericAction(1f, 1, true, GoToHeal);
        }

        public override void Attack(ICharacterInfo character)
        {
            if (character == null)
                return;
            // Buscamos los tiles alrededor del objetivo
            var attackPoint = Map.CurrentMap.TilePositionByWolrdPosition(character.GetPosition());
            List<Vector2> ps = new List<Vector2>();
            ps.Add(attackPoint + new Vector2(1, 1));
            ps.Add(attackPoint + new Vector2(1, -1));
            ps.Add(attackPoint + new Vector2(-1, 1));
            ps.Add(attackPoint + new Vector2(-1, -1));
            ps.Add(attackPoint + new Vector2(0, 1));
            ps.Add(attackPoint + new Vector2(0, -1));
            ps.Add(attackPoint + new Vector2(1, 0));
            ps.Add(attackPoint + new Vector2(-1, 0));

            // Comprobamos si estamos en uno de ellos
            var myPos = Map.CurrentMap.TilePositionByWolrdPosition(MyInfo.GetPosition());
            var isInAttackPos = ps.Any(a => a == myPos);

            // Si lo estamos atacamos
            if (isInAttackPos)
            {
                character.ReceiveAttack(base.Atk);
            }
            // Sino buscamos el punto mas cercano para atacar
            else
            {
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
                            attackPoint = worldPos;
                        }
                    }
                }

                // Vamos hacia el enemigo
                MyInfo.SetPathFinding(attackPoint);
            }
        }
    }
}
