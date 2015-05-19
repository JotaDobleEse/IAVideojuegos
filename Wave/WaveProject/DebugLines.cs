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
        // Singleton
        private static DebugLines debug = new DebugLines();
        public static DebugLines Debug { get { return debug; } }

        // Texto de estadísticas
        public TextBlock Text { get; set; }
        // Camino opcional
        public List<Vector2> Path { get; set; }
        // Controlador de juego
        public GameController Controller { get; set; }
        // Ganador
        public int Victory { get; set; }
        // Indica si debe mostrar información de Debug
        public bool IsDebugging { get; set; }

        private DebugLines()
        {
            IsDebugging = true;
            Path = new List<Vector2>();
            Victory = 0;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            Text.Foreground = Color.White;
        }

        public override void Draw(TimeSpan gameTime)
        {
            if (Victory != 0)
                Text.Text = string.Format("Ganador equipo {0}", (Victory == 1 ? "Azul" : "Rojo"));
            else 
                Text.Text = string.Format("Coords. ({0},{1}), FPS: {2}", WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y, 1 / (float)gameTime.TotalSeconds);

            LineBatch2D lb = RenderManager.LineBatch2D;

            // Dibuja selecciones y rectágulo del ratón
            Controller.Draw(lb);

            if (IsDebugging)
            {
                // Dibuja velocidad y aceleración
                foreach (var kinematic in Kinematic.Kinematics)
                {
                    lb.DrawLineVM(kinematic.Position, kinematic.Position + kinematic.Velocity, Color.Red, 1f);
                    lb.DrawLineVM(kinematic.Position, kinematic.Position + kinematic.LastOutput.Linear, Color.Orange, 1f);
                }

                // Dibuja algunos steerings mas representativos de todos los creados
                foreach (var steering in Steering.Steerings)
                {
                    steering.Draw(lb);
                }

                // Dibuja un camino alternativo (no pertenciente a un PathFollow)
                for (int i = 0; i < Path.Count - 1; i++)
                {
                    lb.DrawCircleVM(Path[i], 3, Color.Cyan, 0.5f);
                    lb.DrawLineVM(Path[i], Path[i + 1], Color.Green, 0.5f);
                }

                // Dibuja el tile en el que se encuentra el mouse y varia de color.
                // - Verde: Pasable
                // - Rojo: No pasable
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

                // Dibuja los waypoints y las áreas de curación
                Map.CurrentMap.Draw(lb);
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
