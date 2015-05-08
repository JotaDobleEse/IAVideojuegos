using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public interface IWalker
    {
        float Cost(Terrain terrain);
    }
}
