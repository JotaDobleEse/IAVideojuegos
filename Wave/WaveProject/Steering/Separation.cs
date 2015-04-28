using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steering
{
    public class Separation : Steering
    {

       //public SteeringBehavior[] listaObjetivos { get; set; }

        //Variable que nos indica si un target esta close
        protected float Threshold = 0.1f;

        //Variable positiva, coeficiente de repulsion
        protected float DecayCoefficient = 1f;
        protected float MaxAcceleration = 0.1f;

        //Variable donde se iran acumulando la velocidad linear de cada uno de los targets
        public static Vector2 LinearAcc { get; set; }

        public EntityManager EntityManager { get; set; }

        public Separation(EntityManager entityManager)
        {
            this.EntityManager = entityManager;
        }


        public override SteeringOutput GetSteering()
        {
            LinearAcc = new Vector2();

            //var prueba = EntityManager.AllEntities;
            IEnumerable<SteeringBehavior> Steerings = EntityManager.AllEntities.Where(w => w.Components.Any(a => a is SteeringBehavior)).Select(s => s.FindComponent<SteeringBehavior>()).ToList();
            foreach (var targets in Steerings)
            {
                Vector2 direction = targets.Transform.Position - Character.Position;
                float distance = direction.Length();

                if (distance < Threshold)
                {
                    //Calculo de strength
                    float strength = Math.Min((float)(DecayCoefficient / Math.Pow(distance, 2)), MaxAcceleration);

                    //añadir la aceleracion
                    direction.Normalize();
                    // LinearAcc += strength * direction;
                    LinearAcc += strength * direction;
                }
            }
            //A la salida del foreach tendremos en LinearAcc el vector resultante de la suma de los vectores 
            return new SteeringOutput() { Linear = LinearAcc };
        }
    }
}
