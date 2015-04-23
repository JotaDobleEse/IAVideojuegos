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
using WaveProject.Steering;

namespace WaveProject
{
    public class DebugLines : Drawable2D
    {
        private IEnumerable<SteeringBehavior> Steerings;
        private Camera2D Camera;

        private bool DEBUG = true;

        protected override void Initialize()
        {
            base.Initialize();
            try
            {
                Steerings = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is SteeringBehavior)).Select(s => s.FindComponent<SteeringBehavior>()).ToList();
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
                Vector3 mousePosition = new Vector3(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y, 0f);
                Vector3 mouse = new Vector3(mousePosition.X, mousePosition.Y, mousePosition.Z);
                Vector3 project = Camera.Unproject(ref mouse);
                Vector2 mousePositionProject;
                project.ToVector2(out mousePositionProject);
                //Console.WriteLine("Original: {0} -- Proyectada: {1}", mousePosition, mousePositionProject);

                foreach (SteeringBehavior steering in Steerings)
                {
                    lb.DrawLine(steering.Transform.Position, steering.Transform.Position + steering.Speed, Color.Red, 1f);
                    lb.DrawLine(steering.Transform.Position, steering.Transform.Position + steering.Steering.Linear, Color.Chocolate, 1f);
                }

                if (Steerings.Any(a => a.Steering is Arrive))
                {
                    Arrive arrive = (Arrive)Steerings.First(a => a.Steering is Arrive).Steering;
                    lb.DrawCircle(mousePositionProject, arrive.SlowRadius, Color.White, 1f);
                    lb.DrawCircle(mousePositionProject, arrive.TargetRadius, Color.Orange, 1f);
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
