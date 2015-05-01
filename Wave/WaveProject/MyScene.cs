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
using WaveProject.Steerings;
using WaveEngine.TiledMap;
using WaveEngine.Framework.Physics2D;
using WaveProject.SteeringsCombinados;
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
            EntityManager.Add(text);

            #region SteeringsBehaviors
            Entity wallAvoidance = new Entity("wallAvoidance")
                .AddComponent(new Transform2D() { Position = new Vector2(20, 220) })
                .AddComponent(new Sprite("Content/Textures/juggernaut"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                //.AddComponent(new SteeringBehavior(new WallAvoidance(), Color.DarkMagenta));
                .AddComponent(new SteeringBehavior(new Cohesion(), Color.DarkMagenta));

            Entity collisionAvoidance = new Entity("collisionAvoidance")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 100, (WaveServices.Platform.ScreenHeight / 2f) - 200) })
                .AddComponent(new Sprite("Content/Textures/lagarto"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new CollisionAvoidanceRT(), Color.Pink));

            Entity wander = new Entity("wander")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/soldado"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Wander(), Color.White));

            Entity face = new Entity("face")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 200, (WaveServices.Platform.ScreenHeight / 2f) - 150) })
                .AddComponent(new Sprite("Content/Textures/soldado"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Face(), Color.Blue, "arrive"));

            Entity lookWhereYouGoing = new Entity("lookWhereYouGoing")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 200, (WaveServices.Platform.ScreenHeight / 2f) - 150) })
                .AddComponent(new Sprite("Content/Textures/juggernaut"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new LookWhereYouGoing(), Color.Salmon, "arrive"));

            Entity align = new Entity("align")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f), (WaveServices.Platform.ScreenHeight / 2f)) })
                .AddComponent(new Sprite("Content/Textures/soldado"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Align(), Color.Crimson, "look"));

            Entity antiAlign = new Entity("antialign")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 100, (WaveServices.Platform.ScreenHeight / 2f) - 50) })
                .AddComponent(new Sprite("Content/Textures/soldado"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new AntiAlign(), Color.Navy, "look"));

            Entity arrive = new Entity("arrive")
                .AddComponent(new Transform2D() { Position = new Vector2(50, 50) })
                .AddComponent(new Sprite("Content/Textures/lagarto"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Arrive(), Color.Green));

            Entity flee = new Entity("flee")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) + 100, (WaveServices.Platform.ScreenHeight / 2f)) })
                .AddComponent(new Sprite("Content/Textures/malabestia"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Flee(), Color.Gold));

            Entity mouserFollower = new Entity("follower")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/lagarto"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Seek(), Color.Salmon));

            Entity persue = new Entity("persue")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) + 100, (WaveServices.Platform.ScreenHeight / 2f)) })
                .AddComponent(new Sprite("Content/Textures/lagarto"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new Persue(), Color.Green, "arrive"));

            Entity velocityMatching = new Entity("velocity")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite("Content/Textures/malabestia"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new VelocityMatching(), Color.Khaki, "flee"));

            Entity lookMouse = new Entity("look")
                .AddComponent(new Transform2D() { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 100, (WaveServices.Platform.ScreenHeight / 2f) + 50) })
                .AddComponent(new Sprite("Content/Textures/triangle"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(Steering.LookMouse, Color.MediumVioletRed));

            Path path = new Path();
            path.AddPosition(new Vector2(450, 200));
            path.AddPosition(new Vector2(450, 500));
            path.AddPosition(new Vector2(550, 475));
            path.AddPosition(new Vector2(700, 550));
            path.AddPosition(new Vector2(850, 225));

            Entity pathFollowing = new Entity("pathFollowing")
                .AddComponent(new Transform2D() { Position = new Vector2(450, 200) })
                .AddComponent(new Sprite("Content/Textures/malabestia"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new SteeringBehavior(new PredictivePathFollowing() { Path = path }, Color.Ivory));
            #endregion


            this.tiledMap = new TiledMap("Content/Maps/mapa.tmx")
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = -0
                };

            Entity map = new Entity("mapa")
                .AddComponent(new Transform2D())
                .AddComponent(this.tiledMap);

            #region Flocking
            Kinematic character = new Kinematic(true) { Position = new Vector2(100, 100), MaxVelocity = 30f };

            Entity flocko1 = new Entity("flocko1")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/juggernaut"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new BlendedSteering(SteeringsFactory.Flocking(character), character, Color.White));

            character = new Kinematic(true) { Position = new Vector2(150, 100), MaxVelocity = 50f };
            Entity flocko2 = new Entity("flocko2")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/malabestia"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new BlendedSteering(SteeringsFactory.Flocking(character), character, Color.White));

            character = new Kinematic(true) { Position = new Vector2(100, 150), MaxVelocity = 40f };
            Entity flocko3 = new Entity("flocko3")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/malabestia"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new BlendedSteering(SteeringsFactory.Flocking(character), character, Color.White));

            character = new Kinematic(true) { Position = new Vector2(150, 150), MaxVelocity = 60f };
            Entity flocko4 = new Entity("flocko4")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/malabestia"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new BlendedSteering(SteeringsFactory.Flocking(character), character, Color.White));


            EntityManager.Add(flocko1);
            EntityManager.Add(flocko2);
            EntityManager.Add(flocko3);
            EntityManager.Add(flocko4);

            #endregion

            //EntityManager.Add(wallAvoidance);
            //EntityManager.Add(collisionAvoidance);
            //EntityManager.Add(wander);
            //EntityManager.Add(align);
            ////EntityManager.Add(face);
            //EntityManager.Add(lookWhereYouGoing);
            ////EntityManager.Add(antiAlign);
            //EntityManager.Add(arrive);
            ////EntityManager.Add(flee);
            ////EntityManager.Add(mouserFollower);
            //EntityManager.Add(persue);
            ////EntityManager.Add(velocityMatching);
            //EntityManager.Add(lookMouse);
            //EntityManager.Add(pathFollowing);
            EntityManager.Add(map);

        }

        protected override void Start()
        {
            base.Start();
            // This method is called after the CreateScene and Initialize methods and before the first Update.

            if (tiledMap.ObjectLayers.Count > 0)
            {
                //TiledMapObjectLayer layer = tiledMap.ObjectLayers.First(f => f.Value.ObjectLayerName == "Agua").Value;
                foreach (var layer in tiledMap.ObjectLayers)
                {
                    foreach (var wall in layer.Value.Objects)
                    {
                        new Wall(wall.X, wall.Y, wall.Width, wall.Height, true);
                    }
                }
            }

        }
    }
}
