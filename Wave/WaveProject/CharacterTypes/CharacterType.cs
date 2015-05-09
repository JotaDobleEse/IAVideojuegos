using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public abstract class CharacterType
    {
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }

        public CharacterType(int hp = 0, int atk = 0, int def = 0)
        {
            HP = hp;
            Atk = atk;
            Def = def;
        }

        public abstract float Cost(Terrain terrain);

        public abstract float MaxVelocity(Terrain terrain);
    }
}
