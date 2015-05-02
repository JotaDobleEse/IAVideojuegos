using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveProject.Steerings;

namespace WaveProject
{
    public class DebugLines : Drawable2D
    {
        private IEnumerable<Steering> Steerings;
        private Camera2D Camera;

        private bool DEBUG = true;

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
                    //lb.DrawCircleVM(wall.P1, 10, Color.Green, 1);
                    //lb.DrawCircleVM(wall.P2, 10, Color.Blue, 1);
                    //lb.DrawCircleVM(wall.P3, 10, Color.Red, 1);
                    //lb.DrawCircleVM(wall.P4, 10, Color.Pink, 1);
                }

                CollisionDetector.Detector.Draw(lb);
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
