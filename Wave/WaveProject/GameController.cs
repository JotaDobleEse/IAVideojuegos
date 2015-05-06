using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class GameController : Behavior
    {
        Kinematic Mouse;
        public DebugLines Debug { get; set; }

        public GameController(Kinematic mouse)
        {
            Mouse = mouse;
        }

        protected override void Update(TimeSpan gameTime)
        {
            Mouse.Update((float)gameTime.TotalMilliseconds, new Steerings.SteeringOutput());
            if (MyScene.TiledMap.PositionInMap(Mouse.Position))
            {
                LRTA lrta = new LRTA(new Vector2(300, 300), Mouse.Position);
                Vector2[] path = lrta.Execute();
                Debug.Path = path;
            }
        }
    }
}
