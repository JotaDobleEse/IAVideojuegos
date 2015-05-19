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
    public class ExplorerCharacter : CharacterType
    {
        public ExplorerCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 180, 20, 30, 300)
        {

        }

        public override float Cost(Terrain terrain)
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
                    return 8f;
            }
            return 1;
        }

        public override float MaxVelocity(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 35;
                case Terrain.FOREST:
                    return 40;
                case Terrain.PATH:
                    return 20;
                case Terrain.PLAIN:
                    return 25;
                case Terrain.WATER:
                    return 10;
            }
            return 30;
        }

        public override EnumeratedCharacterType GetCharacterType()
        {
            return EnumeratedCharacterType.EXPLORER;
        }

        public override GenericAction Update()
        {
            //ATAQUE
            if (HP >= MaxHP * 0.75)
            {
                var enemy = FindEnemyNear();
                if (enemy != null)
                {
                    return new GenericAction(1f, 1, true, AttackEnemyNear);
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    return new GenericAction(1f, 1, true, GoToEnemyBase);
                }
            }
            //DEFENSA
            else// if (HP < HP * 0.75)
            {
                var enemy = FindEnemyNear();
                //SI ENCONTRAMOS UN ENEMIGO Y
                if (enemy != null)
                {
                    return new GenericAction(1f, 1, true, GoToWaypoint);
                }

                //else
                //SI NO ENCONTRAMOS ENEMIGO CERCA Y LA DISTANCIA PARA IR A LA BASE ES PEQUEÑA
                return new GenericAction(1f, 1, false, GoToHeal);
                //SI NO ENCONTRAMOS ENEMIGOS CERCA Y LA DISTANCIA PARA IR A LA BASE ES BASTANTE, VETE A UN WAYPOINT
                //GoToNextWaypoint()
            }
        }


        public override void Attack(ICharacterInfo character)
        {
            if (character == null)
                return;
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

            var myPos = Map.CurrentMap.TilePositionByWolrdPosition(MyInfo.GetPosition());
            var isInAttackPos = ps.Any(a => a == myPos);

            if (isInAttackPos)
            {
                character.ReceiveAttack(base.Atk);
            }
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

                //Console.WriteLine(attackPoint);
                // PATHFINDING
                MyInfo.SetPathFinding(attackPoint);
            }
        }
    }
}
