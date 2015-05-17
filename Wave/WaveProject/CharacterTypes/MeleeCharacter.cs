﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Managers;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class MeleeCharacter : CharacterType
    {
        public MeleeCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 120, 60, 45, 200)
        {

        }

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

        public override Action Update()
        {
            //ATAQUE
            if (HP >= MaxHP * 0.40)
            {
                var enemy = FindEnemyNear();

                //SI ENCONTRAMOS UN ENEMIGO
                if (enemy != null)
                {
                    return AttackEnemyNear;
                }
                else
                {
                    return GoToEnemyBase;
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    //GoToBase(otherTeam)
                }
            }
            //DEFENSA
            else if (HP > MaxHP*0.40 && HP > MaxHP*0.20)
            {
                var enemy = FindEnemyNear();

                //SI ENCONTRAMOS UN ENEMIGO
                if (enemy != null)
                {
                    return AttackEnemyNear;
                    //Attack(enemy);
                }

                //else
                //SI NO ENCONTRAMOS ENEMIGO CERCA Y LA DISTANCIA PARA IR A LA BASE ES PEQUEÑA
                //GoToBase(myteam)
                return GoToWaypoint;
                //SI NO ENCONTRAMOS ENEMIGOS CERCA Y LA DISTANCIA PARA IR A LA BASE ES CERCA, VETE A UN WAYPOINT
                //GoToNextWaypoint()
            }
            return GoToHeal;
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
                character.Attack(base.Atk);
                Console.WriteLine("Attacked");
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
