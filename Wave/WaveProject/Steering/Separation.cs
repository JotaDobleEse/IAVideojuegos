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

        public SteeringBehavior[] listaObjetivos { get; set; }

        //Variable que nos indica si un target esta close
        protected float threshold = 0.1f;

        //Variable positiva, coeficiente de repulsion
        protected float decayCoefficient = 1f;
        protected float maxAcceleration = 0.1f;

        //Variable donde se iran acumulando la velocidad linear de cada uno de los targets
        public static Vector2 linearAcc { get; set; }

        public EntityManager entityManager { get; set; }

        public Separation(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
        {
            Vector2 direction = target.Position - origin.Position;
            float distance = direction.Length();

            if (distance < threshold)
            {
                //Calculo de strength
                float strength = Math.Min((float)(decayCoefficient / Math.Pow(distance, 2)), maxAcceleration);

                //añadir la aceleracion
                direction.Normalize();
                // linearAcc += strength * direction;
                linearAcc += strength * direction;
            }
        }


        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {
            linearAcc = new Vector2();
            
            //var prueba = entityManager.AllEntities;
            IEnumerable<SteeringBehavior> Steerings = entityManager.AllEntities.Where(w => w.Components.Any(a => a is SteeringBehavior)).Select(s => s.FindComponent<SteeringBehavior>()).ToList();
            foreach(var targets in Steerings){
                
                    SteeringCalculation(targets.Transform, origin.Transform);
            }
            //A la salida del foreach tendremos en linearAcc el vector resultante de la suma de los vectores 
            Linear = linearAcc;
            Angular = 0f;
        }

    }
}
