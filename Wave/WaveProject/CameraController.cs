using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveProject
{
    class CameraController : Behavior
    {
        [RequiredComponent]
        public Camera2D Camera { get; set; }
        private float Velocity = 50f;

        protected override void Initialize()
        {
            base.Initialize();
            Camera.Zoom = Vector2.One / 2f;
        }

        protected override void Update(TimeSpan gameTime)
        {
            Vector2 Speed = Vector2.Zero;
            // Keyboard
            var keyboard = WaveServices.Input.KeyboardState;
            if (keyboard.W == ButtonState.Pressed)
            {
                Speed -= new Vector2(0f, Velocity);
            }
            else if (keyboard.S == ButtonState.Pressed)
            {
                Speed += new Vector2(0f, Velocity);
            }
            if (keyboard.A == ButtonState.Pressed)
            {
                Speed -= new Vector2(Velocity, 0f);
            }
            else if (keyboard.D == ButtonState.Pressed)
            {
                Speed += new Vector2(Velocity, 0f);
            }

            Camera.Position += new Vector3(Speed * (float)gameTime.TotalSeconds, 0f);
        }
    }
}
