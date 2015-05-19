using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework.Managers;
using WaveProject.Characters;
using WaveProject.DecisionManager;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class RangedCharacter : CharacterType
    {
        public RangedCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 100, 25, 34, 200) // Párametros iniciales
        {
            // HP    = 100
            // Atk   =  25
            // Def   =  34
            // Radio = 200
        }

        // Coste del ranged al pasar por un terreno
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

        // Velocidad máxima del ranged al pasar por un terreno
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

        public override EnumeratedCharacterType GetCharacterType()
        {
            return EnumeratedCharacterType.RANGED;
        }

        // Árbol de decisión
        public override GenericAction Update()
        {
            //ATAQUE
            if (HP > MaxHP *0.60)
            {
                var enemy = FindEnemyNear();

                if (enemy != null)
                {
                    return new GenericAction(1f, 1, false, AttackEnemyNear);
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    return new GenericAction(1f, 1, false, GoToEnemyBase);
                }
            }
            //DEFENSA
            else if (HP <= MaxHP * 0.60 && HP > MaxHP * 0.40)
            {
                var enemy =  FindEnemyNear();

                //SI ENCONTRAMOS UN ENEMIGO Y NO ES UN ENEMIGO QUE ATAQUE CON RANGO
                if (enemy != null && enemy.GetCharacterType() != EnumeratedCharacterType.RANGED)
                {
                    return new GenericAction(1f, 1, false, AttackEnemyNear);
                }
                //SI NO ENCONTRAMOS ENEMIGO CERCA Y LA DISTANCIA PARA IR A LA BASE ES PEQUEÑA
                return new GenericAction(1f, 1, false, GoToWaypoint);
            }
            // EN OTRO CASO VOLVEMOS A LA BASE
            return new GenericAction(1f, 1, true, GoToMyBase);
        }

        public override void Attack(ICharacterInfo character)
        {
            if (character == null)
                return;
            // Si tenemos un objetivo es que está a nuestro alcance, creamos una bala
            EntityManager.Add(EntityFactory.Shoot(MyInfo.GetPosition(), character.GetPosition()));
            // Atacamos al enemigo
            character.ReceiveAttack(base.Atk);
            // Anulamos el pathfinding mandándolo hacia nuestra propia posición
            MyInfo.SetPathFinding(MyInfo.GetPosition());
        }
    }
}
