using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveProject.CharacterTypes;

namespace WaveProject
{
    public interface ICharacterInfo
    {
        Vector2 GetPosition();
        Vector2 GetVelocity();
        int GetTeam();
        EnumeratedCharacterType GetCharacterType();
        void SetTarget(Kinematic target);
        void SetPathFinding(Vector2 target);
    }
}
