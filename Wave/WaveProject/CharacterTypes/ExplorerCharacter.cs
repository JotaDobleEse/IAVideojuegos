﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework.Managers;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class ExplorerCharacter : CharacterType
    {
        public ExplorerCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 180, 20, 30)
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

        public override Action Update()
        {
            //ATAQUE
            if (HP >= MaxHP*0.75)
            {
                var enemy = FindEnemyNear();
                if (enemy != null)
                {
                    return AttackEnemyNear;
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    return GoToEnemyBase;
                }
            }
            //DEFENSA
            else// if (HP < HP * 0.75)
            {
                var enemy = FindEnemyNear();
                //SI ENCONTRAMOS UN ENEMIGO Y
                if (enemy !=null )
                {
                    return GoToWaypoint;
                }

                //else
                //SI NO ENCONTRAMOS ENEMIGO CERCA Y LA DISTANCIA PARA IR A LA BASE ES PEQUEÑA
                return GoToHeal;
                //SI NO ENCONTRAMOS ENEMIGOS CERCA Y LA DISTANCIA PARA IR A LA BASE ES BASTANTE, VETE A UN WAYPOINT
                //GoToNextWaypoint()
                


            }
        }


        public override void Attack(ICharacterInfo character)
        {
            throw new NotImplementedException();
        }
    }
}
