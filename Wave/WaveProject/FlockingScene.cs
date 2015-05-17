using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Cameras;
using WaveEngine.Framework;

namespace WaveProject
{
    public class FlockingScene : Scene
    {
        protected override void CreateScene()
        {
            // Create a 2D camera
            var camera2D = new FixedCamera2D("Camera2D") { ClearFlags = ClearFlags.All, BackgroundColor = Color.LightBlue }
                .Entity.AddComponent(new CameraController());

            EntityManager.Add(camera2D);

            for (int i = 0; i < 100; i++)
            {
                EntityManager.Add(EntityFactory.FlockingRandom());
            }
        }
    }
}
