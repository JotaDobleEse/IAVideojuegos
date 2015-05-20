using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steerings.Group
{
    public class Separation : Steering
    {

        //Variable que nos indica si un target esta close
        public float Threshold { get; set; } 

        //Variable positiva, coeficiente de repulsion
        public float DecayCoefficient { get; set; }
        public float MaxAcceleration { get; set; } 

        public Separation()
        {
            Threshold = 100f;
            DecayCoefficient = 1000f;
            MaxAcceleration = 20f;
        }


        public override SteeringOutput GetSteering()
        {
            // HACK
            if (Character.Velocity == Vector2.Zero)
                return new SteeringOutput();
            // END HACK
            var linearAcc = Vector2.Zero;
            // Enemigos en el Threshold
            IEnumerable<Kinematic> Steerings = Kinematic.Kinematics.Where(w => (w.Position - Character.Position).Length() <= Threshold && w != Character);
            foreach (var targets in Steerings)
            {
                Vector2 direction = targets.Position - Character.Position;
                var distance = direction.Length();
                // Calculo de strength
                float strength = Math.Min(DecayCoefficient / (distance * distance), MaxAcceleration) *-1;

                // Añadir la aceleracion
                direction.Normalize();
                linearAcc += strength * direction;
            }
            // A la salida del foreach tendremos en LinearAcc el vector resultante de la suma de los vectores 
            return new SteeringOutput() { Linear = linearAcc };
        }
    }
}
