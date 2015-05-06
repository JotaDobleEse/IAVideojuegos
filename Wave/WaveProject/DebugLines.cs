using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;
using WaveProject.Steerings;
using WaveProject.Steerings.Delegated;

namespace WaveProject
{
    public class DebugLines : Drawable2D
    {
        private IEnumerable<Steering> Steerings;
        private Camera2D Camera;
        public TextBlock Text { get; private set; }
        public Vector2[] Path { get; set; }

        private bool DEBUG = true;
        public DebugLines(TextBlock text)
        {
            Path = new Vector2[0];
            Text = text;
            Text.Foreground = Color.White;
        }

        protected override void Initialize()
        {
            base.Initialize();
            try
            {
                Steerings = Steering.Steerings;
                Camera = EntityManager.AllEntities.First(f => f.Name == "Camera2D").FindComponent<Camera2D>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                DEBUG = false;
            }
        }

        public override void Draw(TimeSpan gameTime)
        {
            if (DEBUG)
            {
                Text.Text = string.Format("Coords. ({0},{1}), dt: {2}", WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y, (float)gameTime.TotalSeconds);
                LineBatch2D lb = RenderManager.LineBatch2D;

                foreach (var kinematic in Kinematic.Kinematics)
                {
                    lb.DrawLineVM(kinematic.Position, kinematic.Position + kinematic.Velocity, Color.Red, 1f);
                    lb.DrawLineVM(kinematic.Position, kinematic.Position + kinematic.LastOutput.Linear, Color.Orange, 1f);
                }

                foreach (var steering in Steerings.Where(a => a is Arrive))
                {
                    Arrive arrive = (Arrive)steering;
                    lb.DrawCircleVM(arrive.Target.Position, arrive.SlowRadius, Color.White, 1f);
                    lb.DrawCircleVM(arrive.Target.Position, arrive.TargetRadius, Color.Orange, 1f);
                }

                foreach (var steering in Steerings.Where(w => w is PredictivePathFollowing))
                {
                    var pathFollowing = steering as PredictivePathFollowing;
                    pathFollowing.Path.DrawPath(lb);
                }

                foreach (var wall in Wall.Walls)
                {
                    lb.DrawRectangleVM(wall.WallRectangle, Color.Blue, 1);
                }

                for (int i = 0; i < Path.Length-1; i++)
                {
                    if (i == 0)
                        lb.DrawCircleVM(Path[i], 2, Color.Red, 1);
                    lb.DrawLineVM(Path[i], Path[i + 1], Color.White, 1f);
                }

                CollisionDetector.Detector.Draw(lb);

                Kinematic mouse = Kinematic.Mouse;
                if (MyScene.TiledMap.PositionInMap(mouse.Position))
                {
                    LayerTile tile = MyScene.TiledMap.TileLayers.First().Value.GetLayerTileByWorldPosition(mouse.Position);
                    int x, y;
                    x = tile.X * MyScene.TiledMap.TileWidth;
                    y = tile.Y * MyScene.TiledMap.TileHeight;
                    lb.DrawRectangleVM(new RectangleF(x, y, MyScene.TiledMap.TileWidth, MyScene.TiledMap.TileHeight), Color.Green, 1);
                }
            }
        }

        protected override void DrawDebugLines()
        {
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
