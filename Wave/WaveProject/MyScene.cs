#region Using Statements
using System;
using System.Linq;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveProject.Steering;
using WaveEngine.TiledMap;
#endregion

namespace WaveProject
{
    public class MyScene : Scene
    {
        private TiledMap tiledMap;
        protected override void CreateScene()
        {
            TextBlock text = new TextBlock("axis")
            {
                Margin = new Thickness((WaveServices.Platform.ScreenWidth / 2f) - 100, 10, 0, 0),
                FontPath = "Content/Fonts/verdana.wpk",
                Height = 130,
            };

            text.IsVisible = true;

            // Create a 2D camera
            var camera2D = new FixedCamera2D("Camera2D") { ClearFlags = ClearFlags.All, BackgroundColor = Color.Black }
                .Entity.AddComponent(new InfoDebug(text))
                .AddComponent(new DebugLines())
                .AddComponent(new CameraController()); // Transparent background need this clearFlags.
            EntityManager.Add(camera2D);

            Entity align = new Entity("align")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/triangle"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new SteeringBehavior(new Align(), Color.Crimson, "follower"));

            Entity seek = new Entity("arrive")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/triangle"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new SteeringBehavior(new Arrive(), Color.Green));

            Entity flee = new Entity("flee")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/triangle"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new SteeringBehavior(new Flee(), Color.Gold));

            Entity mouserFollower = new Entity("follower")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/triangle"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new SteeringVelocidad.Seek(color: Color.Salmon));

            Entity map = new Entity("mapa")
                .AddComponent(new Transform2D())
                .AddComponent(this.tiledMap = new TiledMap("Content/Maps/mapa.tmx")
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = -0
                });

            EntityManager.Add(align);
            EntityManager.Add(seek);
            EntityManager.Add(flee);
            EntityManager.Add(mouserFollower);
            EntityManager.Add(map);

        }

        protected override void Start()
        {
            base.Start();
            // This method is called after the CreateScene and Initialize methods and before the first Update.
            Entity arrive = EntityManager.Find("arrive");
            arrive.FindComponent<Transform2D>().Position = new Vector2(50, 50);
            Entity align = EntityManager.Find("align");
            align.FindComponent<Transform2D>().Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f), (WaveServices.Platform.ScreenHeight / 2f));
            Entity flee = EntityManager.Find("flee");
            flee.FindComponent<Transform2D>().Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) + 100, (WaveServices.Platform.ScreenHeight / 2f));
        }
    }
}
