using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Characters;
using WaveProject.DecisionManager;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class FormationType : CharacterType
    {
        public FormationType() : base(null, null)
        {
                
        }
        public override float Cost(Terrain terrain)
        {
            if (terrain == Terrain.WATER)
                return 5f;
            return 1f;
        }

        public override float MaxVelocity(Terrain terrain)
        {
            if (terrain == Terrain.WATER)
                return 10f;
            return 20f;
        }

        public override EnumeratedCharacterType GetCharacterType()
        {
            return EnumeratedCharacterType.NONE;
        }

        public override GenericAction Update()
        {
            return null;
        }

        public override void Attack(ICharacterInfo character)
        {
            throw new NotImplementedException();
        }
    }
}
