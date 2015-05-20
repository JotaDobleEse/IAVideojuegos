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
        private float Velocity = 150f;
        private bool PressedMouse = false;
        private Vector2 LastPositionMouse;

        public static Camera2D CurrentCamera { get; private set; }
        
        protected override void Initialize()
        {
            base.Initialize();
            CurrentCamera = Camera;
            Camera.Zoom *= 1.75f;
            Camera.Position += new Vector3(WaveServices.Platform.ScreenWidth * 0.25f, WaveServices.Platform.ScreenHeight * 0.25f, 0);
        }

        protected override void Update(TimeSpan gameTime)
        {
            Vector2 Speed = Vector2.Zero;
            // Keyboard
            var keyboard = WaveServices.Input.KeyboardState;
            if (keyboard.W == ButtonState.Pressed)
            {
                Speed -= new Vector2(0f, Velocity) * Camera.Zoom;
            }
            else if (keyboard.S == ButtonState.Pressed)
            {
                Speed += new Vector2(0f, Velocity) * Camera.Zoom;
            }
            if (keyboard.A == ButtonState.Pressed)
            {
                Speed -= new Vector2(Velocity, 0f) * Camera.Zoom;
            }
            else if (keyboard.D == ButtonState.Pressed)
            {
                Speed += new Vector2(Velocity, 0f) * Camera.Zoom;
            }

            if (WaveServices.Input.MouseState.Wheel > 0)
            {
                Camera.Zoom /= 1.1f;
            }
            else if (WaveServices.Input.MouseState.Wheel < 0)
            {
                Camera.Zoom *= 1.1f;
            }

            if (WaveServices.Input.MouseState.MiddleButton == ButtonState.Pressed)
            {
                if (!PressedMouse)
                {
                    PressedMouse = true;
                    LastPositionMouse = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
                }
                else
                {
                    Vector2 position = new Vector2(WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y);
                    Camera.Position += new Vector3((position - LastPositionMouse), 0f);
                    LastPositionMouse = position;
                }
            }
            else if (WaveServices.Input.MouseState.MiddleButton == ButtonState.Release)
            {
                if (PressedMouse)
                {
                    PressedMouse = false;
                }
            }

            Camera.Position += new Vector3(Speed * (float)gameTime.TotalSeconds, 0f);
        }
    }
}
