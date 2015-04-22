using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    public abstract class Steering
    {
        /// <summary>
        /// Get
        /// </summary>
        public Vector2 Linear { get; protected set; }
        /// <summary>
        /// Get
        /// </summary>
        public float Angular { get; protected set; }

        public Steering()
        {
            Linear = Vector2.Zero;
            Angular = 0f;
        }

        public abstract void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null);
        public abstract void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin);

        #region Operadores
        public static Steering operator +(Steering s1, Steering s2)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear + s2.Linear;
            result.Angular = s1.Angular + s2.Angular;
            return result;
        }
        public static Steering operator +(Steering s1, float d)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear + new Vector2(d,d);
            result.Angular = s1.Angular + d;
            return result;
        }

        public static Steering operator -(Steering s1, Steering s2)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear - s2.Linear;
            result.Angular = s1.Angular - s2.Angular;
            return result;
        }
        public static Steering operator -(Steering s1, float d)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear - new Vector2(d, d);
            result.Angular = s1.Angular - d;
            return result;
        }

        public static Steering operator *(Steering s1, Steering s2)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear * s2.Linear;
            result.Angular = s1.Angular * s2.Angular;
            return result;
        }
        public static Steering operator *(Steering s1, float d)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear * new Vector2(d, d);
            result.Angular = s1.Angular * d;
            return result;
        }
        public static Steering operator /(Steering s1, Steering s2)
        {
            Steering result = new NonFuncionalSteering();
            Vector2 aux;
            if (s2.Linear == Vector2.Zero)
            {
                aux = Vector2.One;
            }
            else if (s2.Linear.X == 0)
            {
                aux = s2.Linear + new Vector2(1, 0);
            }
            else if (s2.Linear.Y == 0)
            {
                aux = s2.Linear + new Vector2(0, 1);
            }
            result.Linear = s1.Linear / (s2.Linear == Vector2.Zero ? Vector2.One : s2.Linear);
            result.Angular = s1.Angular / (s2.Angular == 0 ? 1f : s2.Angular);
            return result;
        }
        public static Steering operator /(Steering s1, float d)
        {
            Steering result = new NonFuncionalSteering();
            result.Linear = s1.Linear / (d == 0 ? Vector2.One : new Vector2(d, d));
            result.Angular = s1.Angular / (d == 0 ? 1 : d);
            return result;
        }
        #endregion

        #region SteeringContenedor
        public class NonFuncionalSteering : Steering
        {
            public override void SteeringCalculation(Transform2D target, Transform2D origin, Vector2? characterSpeed = null)
            {
                return;
            }

            public override void SteeringCalculation(SteeringBehavior target, SteeringBehavior origin)
            {
                return;
            }
        }
        #endregion
    }
}
