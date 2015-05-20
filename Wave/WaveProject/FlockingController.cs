using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveProject.Characters;

namespace WaveProject
{
    // Controlador para la escena del Flocking
    public class FlockingController : Behavior
    {
        private float TimeToPress = 0f;
        protected override void Update(TimeSpan gameTime)
        {
            TimeToPress -= (float)gameTime.TotalSeconds;
            if (TimeToPress <= 0)
            {
                // Añadir Flockings
                if (WaveServices.Input.KeyboardState.Z == WaveEngine.Common.Input.ButtonState.Pressed)
                {
                    EntityManager.Add(EntityFactory.FlockingRandom());
                    TimeToPress = 0.05f;
                }
                // Quitar Flockings
                else if (WaveServices.Input.KeyboardState.X == WaveEngine.Common.Input.ButtonState.Pressed)
                {
                    if (EntityManager.AllEntities.Count(c => c.FindComponent<SteeringCharacter>() != null) > 0)
                        EntityManager.Remove(EntityManager.AllEntities.First(f => f.FindComponent<SteeringCharacter>() != null));
                    TimeToPress = 0.05f;
                }
            }
        }
    }
}
