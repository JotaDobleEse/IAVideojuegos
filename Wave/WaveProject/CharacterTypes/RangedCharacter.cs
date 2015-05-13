using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class RangedCharacter : CharacterType
    {
        public RangedCharacter()
            : base(100, 60, 34)
        {

        }

        public override float Cost(Terrain terrain)
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
                    return 8f;
            }
            return 1;
        }

        public override float MaxVelocity(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 25;
                case Terrain.FOREST:
                    return 18;
                case Terrain.PATH:
                    return 22;
                case Terrain.PLAIN:
                    return 30;
                case Terrain.WATER:
                    return 10;
            }
            return 25;
        }

        public override EnumeratedCharacterType GetType()
        {
            return EnumeratedCharacterType.RANGED;
        }

        public override void Update()
        {
            //ATAQUE
            if (HP > HP *0.60)
            {
                CharacterType enemy = FindEnemyNear();

                if (enemy != null)
                {
                    Attack(enemy);
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    //GoToBase(otherTeam)
                }
            }
            //DEFENSA
            else if (HP <= HP *0.60)
            {
                CharacterType enemy = FindEnemyNear();

                //SI ENCONTRAMOS UN ENEMIGO Y NO ES UN ENEMIGO QUE ATAQUE CON RANGO
                if (enemy !=null && enemy.GetType() != EnumeratedCharacterType.RANGED)
                {
                    Attack(enemy);
                }

                //else
                //SI NO ENCONTRAMOS ENEMIGO CERCA Y LA DISTANCIA PARA IR A LA BASE ES PEQUEÑA
                //GoToBase(myteam)
                //SI NO ENCONTRAMOS ENEMIGOS CERCA Y LA DISTANCIA PARA IR A LA BASE ES BASTANTE, VETE A UN WAYPOINT
                //GoToNextWaypoint()
                


            }
        }


    }
}
