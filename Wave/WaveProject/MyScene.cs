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
using WaveProject.Steerings.Combined;
using WaveProject.Steerings.Group;
using WaveProject.Steerings.Delegated;
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
                .Entity.AddComponent(new DebugLines(text))
                .AddComponent(new CameraController()); // Transparent background need this clearFlags.
            EntityManager.Add(camera2D);
            EntityManager.Add(text);

            Entity map = new Entity("mapa")
                .AddComponent(new Transform2D())
                .AddComponent(this.tiledMap = new TiledMap("Content/Maps/mapa.tmx")
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = -0
                });
            EntityManager.Add(map);

            #region Basic Steerings
            //Kinematic wallAvoidanceChar = new Kinematic(true) { Position = new Vector2(20, 220) };
            //Kinematic collisionAvoidanceChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 100, (WaveServices.Platform.ScreenHeight / 2f) - 200) };
            //Kinematic wanderChar = new Kinematic(true);
            //Kinematic faceChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 200, (WaveServices.Platform.ScreenHeight / 2f) - 150) };
            //Kinematic lookWhereYouGoingChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 200, (WaveServices.Platform.ScreenHeight / 2f) - 150) };
            //Kinematic alignChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 200, (WaveServices.Platform.ScreenHeight / 2f) - 150) };
            //Kinematic antiAlignChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 100, (WaveServices.Platform.ScreenHeight / 2f) - 50) };
            //Kinematic arriveChar = new Kinematic(true) { Position = new Vector2(50, 50) };
            //Kinematic fleeChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) + 100, (WaveServices.Platform.ScreenHeight / 2f)) };
            //Kinematic mouserFollowerChar = new Kinematic(true);
            //Kinematic persueChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) + 100, (WaveServices.Platform.ScreenHeight / 2f)) };
            //Kinematic velocityMatchingChar = new Kinematic(true);
            //Kinematic lookChar = new Kinematic(true) { Position = new Vector2((WaveServices.Platform.ScreenWidth / 2f) - 100, (WaveServices.Platform.ScreenHeight / 2f) + 50) };
            //Kinematic pathFollowingChar = new Kinematic(true);
            
            //Entity wallAvoidance = new Entity("wallAvoidance")
            //    .AddComponent(new Transform2D() { Position = wallAvoidanceChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/juggernaut"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    //.AddComponent(new Character(new WallAvoidance() { Character = wallAvoidanceChar }, wallAvoidanceChar, Color.DarkMagenta));
            //    .AddComponent(new Character(new Cohesion() { Character = wallAvoidanceChar }, wallAvoidanceChar , Color.DarkMagenta));

            //Entity collisionAvoidance = new Entity("collisionAvoidance")
            //    .AddComponent(new Transform2D() { Position = collisionAvoidanceChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/lagarto"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new CollisionAvoidanceRT() { Character = collisionAvoidanceChar }, collisionAvoidanceChar, Color.Pink));

            //Entity wander = new Entity("wander")
            //    .AddComponent(new Transform2D() { Position = wanderChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/soldado"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Wander() { Character = wanderChar }, wanderChar, Color.White));

            //Entity face = new Entity("face")
            //    .AddComponent(new Transform2D() { Position = faceChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/soldado"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Face() { Character = faceChar, Target = arriveChar }, faceChar, Color.Blue, "arrive"));

            //Entity lookWhereYouGoing = new Entity("lookWhereYouGoing")
            //    .AddComponent(new Transform2D() { Position = lookWhereYouGoingChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/juggernaut"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new LookWhereYouGoing() { Character = lookWhereYouGoingChar, Target = arriveChar }, lookWhereYouGoingChar, Color.Salmon));

            //Entity align = new Entity("align")
            //    .AddComponent(new Transform2D() { Position = alignChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/soldado"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Align() { Character = alignChar, Target = lookChar }, alignChar, Color.Crimson));

            //Entity antiAlign = new Entity("antialign")
            //    .AddComponent(new Transform2D() { Position = antiAlignChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/soldado"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new AntiAlign() { Character = antiAlignChar, Target = lookChar }, antiAlignChar, Color.Navy));

            //Entity arrive = new Entity("arrive")
            //    .AddComponent(new Transform2D() { Position = arriveChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/lagarto"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Arrive() { Character = arriveChar, Target = Kinematic.MouseKinematic }, arriveChar, Color.Green));

            //Entity flee = new Entity("flee")
            //    .AddComponent(new Transform2D() { Position = fleeChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/malabestia"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Flee() { Character = fleeChar, Target = Kinematic.MouseKinematic }, fleeChar, Color.Gold));

            //Entity mouserFollower = new Entity("follower")
            //    .AddComponent(new Transform2D() { Position = mouserFollowerChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/lagarto"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Seek() { Character = mouserFollowerChar, Target = Kinematic.MouseKinematic }, mouserFollowerChar, Color.Salmon));

            //Entity persue = new Entity("persue")
            //    .AddComponent(new Transform2D() { Position = persueChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/lagarto"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Persue() { Character = persueChar, Target = arriveChar }, persueChar, Color.Green));

            //Entity velocityMatching = new Entity("velocity")
            //    .AddComponent(new Transform2D() { Position = velocityMatchingChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/malabestia"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new VelocityMatching() { Character = velocityMatchingChar, Target = fleeChar }, velocityMatchingChar, Color.Khaki));

            //Entity lookMouse = new Entity("look")
            //    .AddComponent(new Transform2D() { Position = lookChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/triangle"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new Steering.LookMouseSteering() { Character = lookChar, Target = Kinematic.MouseKinematic }, lookChar, Color.MediumVioletRed));

            //Path path = new Path();
            //path.AddPosition(new Vector2(450, 200));
            //path.AddPosition(new Vector2(450, 500));
            //path.AddPosition(new Vector2(550, 475));
            //path.AddPosition(new Vector2(700, 550));
            //path.AddPosition(new Vector2(850, 225));

            //pathFollowingChar.Position = path.GetPosition(0);
            //Entity pathFollowing = new Entity("pathFollowing")
            //    .AddComponent(new Transform2D() { Position = pathFollowingChar.Position })
            //    .AddComponent(new Sprite("Content/Textures/malabestia"))
            //    .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //    .AddComponent(new Character(new PredictivePathFollowing() { Character = pathFollowingChar, Path = path }, pathFollowingChar, Color.Ivory));

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
            #endregion

            #region Flocking
            Kinematic character = new Kinematic(true) { Position = new Vector2(100, 100), MaxVelocity = 30f };

            Entity flocko1 = new Entity("flocko1")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/juggernaut"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new Character(new BlendedSteering(SteeringsFactory.Flocking(character)), character, Color.White));

            character = new Kinematic(true) { Position = new Vector2(150, 100), MaxVelocity = 50f };
            Entity flocko2 = new Entity("flocko2")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/malabestia"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new Character(new BlendedSteering(SteeringsFactory.Flocking(character)), character, Color.White));

            character = new Kinematic(true) { Position = new Vector2(100, 150), MaxVelocity = 40f };
            Entity flocko3 = new Entity("flocko3")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/malabestia"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new Character(new BlendedSteering(SteeringsFactory.Flocking(character)), character, Color.White));

            character = new Kinematic(true) { Position = new Vector2(150, 150), MaxVelocity = 60f };
            Entity flocko4 = new Entity("flocko4")
                 .AddComponent(new Transform2D() { Position = character.Position })
                 .AddComponent(new Sprite("Content/Textures/malabestia"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new Character(new BlendedSteering(SteeringsFactory.Flocking(character)), character, Color.White));


            EntityManager.Add(flocko1);
            EntityManager.Add(flocko2);
            EntityManager.Add(flocko3);
            EntityManager.Add(flocko4);

            #endregion

            #region Follow the lider
            //Kinematic leader = new Kinematic(true) { Position = new Vector2(400, 500), MaxVelocity = 30f };

            //Entity leaderEntity = new Entity("leader")
            //     .AddComponent(new Transform2D() { Position = leader.Position })
            //     .AddComponent(new Sprite("Content/Textures/malabestia"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new Wander() { Character = leader }, leader, Color.White));

            //Kinematic character = new Kinematic(true) { Position = new Vector2(100, 100), MaxVelocity = 50f };
            //Entity follower1 = new Entity("follower1")
            //     .AddComponent(new Transform2D() { Position = character.Position })
            //     .AddComponent(new Sprite("Content/Textures/soldado"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new BlendedSteering(SteeringsFactory.LeaderFollowing(character, leader)), character, Color.White));

            //character = new Kinematic(true) { Position = new Vector2(200, 200), MaxVelocity = 50f };
            //Entity follower2 = new Entity("follower2")
            //     .AddComponent(new Transform2D() { Position = character.Position })
            //     .AddComponent(new Sprite("Content/Textures/soldado"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new BlendedSteering(SteeringsFactory.LeaderFollowing(character, leader)), character, Color.White));

            //character = new Kinematic(true) { Position = new Vector2(600, 300), MaxVelocity = 50f };
            //Entity follower3 = new Entity("follower3")
            //     .AddComponent(new Transform2D() { Position = character.Position })
            //     .AddComponent(new Sprite("Content/Textures/soldado"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new BlendedSteering(SteeringsFactory.LeaderFollowing(character, leader)), character, Color.White));

            //EntityManager.Add(leaderEntity);
            //EntityManager.Add(follower1);
            //EntityManager.Add(follower2);
            //EntityManager.Add(follower3);
            #endregion

            #region Priority
            //Kinematic leader = new Kinematic(true) { Position = new Vector2(400, 500), MaxVelocity = 30f };

            //Entity leaderEntity = new Entity("leader")
            //     .AddComponent(new Transform2D() { Position = leader.Position })
            //     .AddComponent(new Sprite("Content/Textures/malabestia"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new Wander() { Character = leader }, leader, Color.White));

            //Kinematic character = new Kinematic(true) { Position = new Vector2(100, 100), MaxVelocity = 50f };
            //Entity follower1 = new Entity("follower1")
            //     .AddComponent(new Transform2D() { Position = character.Position })
            //     .AddComponent(new Sprite("Content/Textures/soldado"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new PrioritySteering(SteeringsFactory.PriorityGroup(character, leader)), character, Color.White));

            //character = new Kinematic(true) { Position = new Vector2(200, 200), MaxVelocity = 50f };
            //Entity follower2 = new Entity("follower2")
            //     .AddComponent(new Transform2D() { Position = character.Position })
            //     .AddComponent(new Sprite("Content/Textures/soldado"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new PrioritySteering(SteeringsFactory.PriorityGroup(character, leader)), character, Color.White));

            //character = new Kinematic(true) { Position = new Vector2(600, 300), MaxVelocity = 50f };
            //Entity follower3 = new Entity("follower3")
            //     .AddComponent(new Transform2D() { Position = character.Position })
            //     .AddComponent(new Sprite("Content/Textures/soldado"))
            //     .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            //     .AddComponent(new Character(new PrioritySteering(SteeringsFactory.PriorityGroup(character, leader)), character, Color.White));

            //EntityManager.Add(leaderEntity);
            //EntityManager.Add(follower1);
            //EntityManager.Add(follower2);
            //EntityManager.Add(follower3);
            #endregion

        }

        protected override void Start()
        {
            base.Start();

            // Carga todas las capas de objeto del mapa como Muros
            if (tiledMap.ObjectLayers.Count > 0)
            {
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
