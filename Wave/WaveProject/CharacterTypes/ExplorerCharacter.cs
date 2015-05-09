using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class ExplorerCharacter : CharacterType
    {
        public ExplorerCharacter()
            : base(150, 20, 30)
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
                    return float.PositiveInfinity;
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
    }
}
