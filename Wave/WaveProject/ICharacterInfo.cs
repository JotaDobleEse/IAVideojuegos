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
        Vector2 GetPostion();
        int GetTeam();
        EnumeratedCharacterType GetType();
    }
}
