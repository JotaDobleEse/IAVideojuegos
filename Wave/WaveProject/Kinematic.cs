using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveProject.Steerings;

namespace WaveProject
{
    public class Kinematic
    {
        private static List<Kinematic> kinematics = new List<Kinematic>();
        public static List<Kinematic> Kinematics { get { return kinematics; } }

        private static Kinematic mouse = new Kinematic();
        /// <summary>
        /// Kinematic con la posición del ratón, solo se actualiza si algún kinematic pasa por el método Update.
        /// </summary>
        public static Kinematic MouseKinematic { get { return mouse; } }

        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float MaxVelocity { get; set; }
        public SteeringOutput LastOutput { get; private set; }

        public bool IsStable { get; private set; }


        /// <summary>
        /// Crea un Kinematic.
        /// </summary>
        /// <param name="stable">Establece si el Kinematic se introduce en la colección de muros del sistema.</param>
        public Kinematic(bool stable = false)
        {
            IsStable = stable;
            if (stable)
                kinematics.Add(this);
            MaxVelocity = 50;
        }

        ~Kinematic()
        {
            kinematics.Remove(this);
        }

        public void Update(SteeringOutput steering, float deltaTime)
        {
            LastOutput = steering;
            Velocity += steering.Linear * deltaTime;
            Rotation += steering.Angular * deltaTime;

            if (Velocity.X > MaxVelocity)
                Velocity = new Vector2(MaxVelocity, Velocity.Y);
            else if (Velocity.X < -MaxVelocity)
                Velocity = new Vector2(-MaxVelocity, Velocity.Y);
            if (Velocity.Y > MaxVelocity)
                Velocity = new Vector2(Velocity.X, MaxVelocity);
            else if (Velocity.Y < -MaxVelocity)
                Velocity = new Vector2(Velocity.X, -MaxVelocity);

            Position += Velocity * deltaTime;
            Orientation += Rotation * deltaTime;

            #region Update mouse
            if (CameraController.CurrentCamera != null)
            {
                Vector2 targetMouse = WaveServices.Input.MouseState.PositionRelative(CameraController.CurrentCamera);
                mouse.Position = targetMouse;
            }
            #endregion
        }

        public Kinematic Clone()
        {
            return new Kinematic(IsStable) { Position = this.Position, LastOutput = this.LastOutput, MaxVelocity = this.MaxVelocity, Orientation = this.Orientation, Rotation = this.Rotation, Velocity = this.Velocity };
        }

        public Kinematic Clone(bool stable)
        {
            return new Kinematic(stable) { Position = this.Position, LastOutput = this.LastOutput, MaxVelocity = this.MaxVelocity, Orientation = this.Orientation, Rotation = this.Rotation, Velocity = this.Velocity };
        }
    }
}
