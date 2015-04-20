using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveProject.Steering;

namespace WaveProject
{
    public class DebugLines : Drawable2D
    {
        private IEnumerable<SteeringBehavior> Steerings;

        private bool DEBUG = true;

        protected override void Initialize()
        {
            base.Initialize();
            try
            {
                Steerings = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is SteeringBehavior)).Select(s => s.FindComponent<SteeringBehavior>()).ToList();
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
                Vector2 mousePosition = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
                foreach (SteeringBehavior steering in Steerings)
                {
                    lb.DrawLine(steering.Transform.Position, steering.Transform.Position + steering.Speed, Color.Red, 1f);
                    lb.DrawLine(steering.Transform.Position, steering.Transform.Position + steering.Steering.Linear, Color.Chocolate, 1f);
                }

                if (Steerings.Any(a => a.Steering is Arrive))
                {
                    Arrive arrive = (Arrive)Steerings.First(a => a.Steering is Arrive).Steering;
                    lb.DrawCircle(mousePosition, arrive.SlowRadius, Color.White, 1f);
                    lb.DrawCircle(mousePosition, arrive.TargetRadius, Color.Orange, 1f);
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
