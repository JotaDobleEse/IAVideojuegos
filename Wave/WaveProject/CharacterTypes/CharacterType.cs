using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public enum EnumeratedCharacterType
    {
        NONE, EXPLORER, MELEE, RANGED
    }
    public abstract class CharacterType
    {
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        //definir ActualHP o MAXHP
        //definir visibilidad (float?)
        //definir RAngo de ataque, Arquero tendra mayor rango

        public CharacterType(int hp = 0, int atk = 0, int def = 0)
        {
            HP = hp;
            Atk = atk;
            Def = def;
        }

        public abstract float Cost(Terrain terrain);

        public abstract float MaxVelocity(Terrain terrain);
        public abstract EnumeratedCharacterType GetCharacterType();

        public abstract void Update();
        public abstract void Attack(CharacterType character);

        public CharacterType FindEnemyNear()
        {
            //Buscamos el mejor objetivo de entre los enemigos, debemos de utilizar la visibilidad para comprobar los enemigos en el grid

            /**
             * for (int i = (int) (-visibility/2); i <= visibility/2; i++)
             *      for(int j = (int) (-visibility/2); j <= visibility/2; j++)
             *          //posicion en el grid x + i
             *          //posicion en el grid y + j
             *          //si nos salimos del grid, continue
             *          
             *          //SI LA POSICION NO ES NULA, Y SE ENCUENTRA EN EL EQUIPO CONTRARIO
             *          //AÑADE AL ENEMIGO A UNA LISTA Y SELECCIONAMOS EL MEJOR
             *          // O
             *          //ESCOJE UNO AL AZAR
             * 
             * 
             */
            throw new NotImplementedException();
        }

        public bool GoToHeal()
        {
            //Comprueba si esta en la base de tu equipo
            //No se si pasarle el equipo o pasarle la posición de la base
            //si esta en la base return true
            /*
             * if (InBase(character)){
             *      HP += healRatio;
             *      if(HP>HPMAX){
             *          HP=MAXHP
             *          GoToCover para salir de la base
             *      
             *      }
             * }
             * else GoToBase(mybase)
             * 
             *  
             */

            return true;
        }

        public void GoToEnemyBase(/*Base enemiga o tu base*/)
        {
            //Llamar al pathfinding pasandole tu posicion de origen y de destino la de la base enemiga
        }

        public void GoToMyBase()
        {

        }

        public void GoToWaypoint()
        {

        }
    }
}
