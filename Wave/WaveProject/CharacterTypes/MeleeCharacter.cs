using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class MeleeCharacter : CharacterType
    {
        public MeleeCharacter()
            : base(120, 60, 45)
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
    }
}
