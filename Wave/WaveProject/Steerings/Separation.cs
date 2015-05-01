using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steerings
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
            Threshold = 30f;
            DecayCoefficient = 1f;
            MaxAcceleration = 0.1f;
        }


        public override SteeringOutput GetSteering()
        {
            var linearAcc = Vector2.Zero;

            //var prueba = EntityManager.AllEntities;
            IEnumerable<Kinematic> Steerings = Kinematic.Kinematics.Where(w => (w.Position - Character.Position).Length() <= Threshold && w != Character);
            foreach (var targets in Steerings)
            {
                Vector2 direction = targets.Position - Character.Position;
                var distance = direction.Length();
                //Calculo de strength
                float strength = Math.Min(DecayCoefficient / (distance*distance), MaxAcceleration);

                //añadir la aceleracion
                direction.Normalize();
                // LinearAcc += strength * direction;
                linearAcc += strength * direction;
            }
            //A la salida del foreach tendremos en LinearAcc el vector resultante de la suma de los vectores 
            return new SteeringOutput() { Linear = linearAcc };
        }
    }
}
