using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework.Managers;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class MeleeCharacter : CharacterType
    {
        public MeleeCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 120, 60, 45)
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

        public override void Update()
        {
            CharacterType enemy = null;// FindEnemyNear();

            //ATAQUE
            if (HP >= HP*0.40)
            {
                //SI ENCONTRAMOS UN ENEMIGO
                if (enemy != null)
                {
                    //Attack(enemy);
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    //GoToBase(otherTeam)
                }
            }
            //DEFENSA
            else if (HP < HP*0.40)
            {

                //SI ENCONTRAMOS UN ENEMIGO
                if (enemy != null)
                {
                    //Attack(enemy);
                }

                //else
                //SI NO ENCONTRAMOS ENEMIGO CERCA Y LA DISTANCIA PARA IR A LA BASE ES PEQUEÑA
                //GoToBase(myteam)
                //SI NO ENCONTRAMOS ENEMIGOS CERCA Y LA DISTANCIA PARA IR A LA BASE ES CERCA, VETE A UN WAYPOINT
                //GoToNextWaypoint()



            }
        }

        public override void Attack(CharacterType character)
        {
            throw new NotImplementedException();
        }
    }
}
