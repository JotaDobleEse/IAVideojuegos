using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steerings
{

    public struct SteeringOutput 
    {
        public Vector2 Linear { get; set; }
        public float Angular { get; set; }

        public static SteeringOutput operator +(SteeringOutput s1, SteeringOutput s2)
        {
            SteeringOutput result = new SteeringOutput();
            result.Linear = s1.Linear + s2.Linear;
            result.Angular = s1.Angular + s2.Angular;
            return result;
        }
        public static SteeringOutput operator -(SteeringOutput s1, SteeringOutput s2)
        {
            SteeringOutput result = new SteeringOutput();
            result.Linear = s1.Linear - s2.Linear;
            result.Angular = s1.Angular - s2.Angular;
            return result;
        }
        public static SteeringOutput operator *(SteeringOutput s1, SteeringOutput s2)
        {
            SteeringOutput result = new SteeringOutput();
            result.Linear = s1.Linear * s2.Linear;
            result.Angular = s1.Angular * s2.Angular;
            return result;
        }
        public static SteeringOutput operator /(SteeringOutput s1, SteeringOutput s2)
        {
            SteeringOutput result = new SteeringOutput();
            result.Linear = s1.Linear / s2.Linear;
            result.Angular = s1.Angular / s2.Angular;
            return result;
        }
        public static SteeringOutput operator *(SteeringOutput s1, float s2)
        {
            SteeringOutput result = new SteeringOutput();
            result.Linear = s1.Linear * s2;
            result.Angular = s1.Angular * s2;
            return result;
        }
        public static SteeringOutput operator /(SteeringOutput s1, float s2)
        {
            SteeringOutput result = new SteeringOutput();
            result.Linear = s1.Linear / s2;
            result.Angular = s1.Angular / s2;
            return result;
        }
    };

    public abstract class Steering
    {
        public static LookMouseSteering LookMouse { get { return new LookMouseSteering(); } }

        public Kinematic Character { get; set; }

        public Kinematic Target { get; set; }

        public float Weight { get; set; }

        public Steering(float weight = 1)
        {

            Weight = weight;
        }

        public abstract SteeringOutput GetSteering();

        #region SteeringContenedor
        public class LookMouseSteering : Steering
        {
            public override SteeringOutput GetSteering()
            {
                var direction = Target.Position - Character.Position;
                Character.Orientation = direction.ToRotation();
                return new SteeringOutput();
            }
        }
        #endregion
    }
}
