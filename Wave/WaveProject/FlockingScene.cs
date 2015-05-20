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
            var controller = new Entity()
                .AddComponent(new FlockingController());
            EntityManager.Add(controller);

            // Cámara de juego
            var camera2D = new FixedCamera2D("Camera2D") { ClearFlags = ClearFlags.All, BackgroundColor = Color.LightBlue }
                .Entity.AddComponent(new CameraController()); // Controlador de cámara

            EntityManager.Add(camera2D);

            // Añadimos los personajes
            for (int i = 0; i < 300; i++)
            {
                EntityManager.Add(EntityFactory.FlockingRandom());
            }
        }
    }
}
