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
        public List<Vector2> Path { get; set; }
        public GameController Controller { get; set; }

        private bool DEBUG = true;
        public DebugLines(TextBlock text)
        {
            Path = new List<Vector2>();
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
                Text.Text = string.Format("Coords. ({0},{1}), FPS: {2}", WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y, 1 / (float)gameTime.TotalSeconds);
                LineBatch2D lb = RenderManager.LineBatch2D;

                foreach (var kinematic in Kinematic.Kinematics)
                {
                    lb.DrawLineVM(kinematic.Position, kinematic.Position + kinematic.Velocity, Color.Red, 1f);
                    lb.DrawLineVM(kinematic.Position, kinematic.Position + kinematic.LastOutput.Linear, Color.Orange, 1f);
                }

                foreach (var steering in Steerings)
                {
                    steering.Draw(lb);
                }

                Controller.Draw(lb);

                /*foreach (var wall in Wall.Walls)
                {
                    lb.DrawRectangleVM(wall.WallRectangle, Color.Blue, 1f);
                }*/

                /*for (int i = 0; i < Path.Count-1; i++)
                {
                    if (i == 0)
                        lb.DrawCircleVM(Path[i], 2, Color.Red, 0.5f);
                    lb.DrawLineVM(Path[i], Path[i + 1], Color.White, 0.5f);
                }*/

                Kinematic mouse = Kinematic.Mouse;
                if (Map.CurrentMap.PositionInMap(mouse.Position))
                {
                    LayerTile tile = Map.CurrentMap.TileByWolrdPosition(mouse.Position);
                    int x, y;
                    x = tile.X * Map.CurrentMap.TileWidth;
                    y = tile.Y * Map.CurrentMap.TileHeight;
                    if (Map.CurrentMap.NodeMap[tile.X, tile.Y].Passable)
                        lb.DrawRectangleVM(new RectangleF(x, y, Map.CurrentMap.TileWidth, Map.CurrentMap.TileHeight), Color.Green, 1);
                    else
                        lb.DrawRectangleVM(new RectangleF(x, y, Map.CurrentMap.TileWidth, Map.CurrentMap.TileHeight), Color.Red, 1);

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
