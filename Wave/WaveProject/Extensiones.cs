using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace WaveProject
{
    public static class Extensiones
    {
        public static float BRadius(this Entity entity)
        {
            var objectTexture = entity.FindComponent<Sprite>();
            if (objectTexture == null)
            {
                var objectTransform = entity.FindComponent<Transform2D>();
                return Math.Max(objectTransform.Rectangle.Width, objectTransform.Rectangle.Height) * 0.8f;
            }
            return Math.Max(objectTexture.Texture.Width, objectTexture.Texture.Height) * 0.8f;
        }

        public static Vector2 ConvertToLocalPos(this Kinematic origin, Vector2 position)
        {
            var localPos = position - origin.Position;
            localPos -= origin.Rotation.RotationToVector();
            return localPos;
        }

        public static Vector2 ConvertToGlobalPos(this Kinematic origin, Vector2 position)
        {
            var localPos = position + origin.Position;
            localPos += origin.RotationAsVector();
            return localPos;
        }
        public static Vector2 ConvertToLocalPos(this Transform2D origin, Vector2 position)
        {
            var localPos = position - origin.Position;
            localPos -= origin.Rotation.RotationToVector();
            return localPos;
        }

        public static Vector2 ConvertToGlobalPos(this Transform2D origin, Vector2 position)
        {
            var localPos = position + origin.Position;
            localPos += origin.RotationAsVector();
            return localPos;
        }

        public static Vector2 RotationToVector(this float rotation)
        {
            return new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
        }

        public static Vector2 RotationAsVector(this Transform2D transform)
        {
            return new Vector2((float)Math.Sin(transform.Rotation), -(float)Math.Cos(transform.Rotation));
        }

        public static Vector2 RotationAsVector(this Kinematic transform)
        {
            return new Vector2((float)Math.Sin(transform.Rotation), -(float)Math.Cos(transform.Rotation));
        }

        public static float ToRotation(this Vector2 rotation)
        {
            return (float)Math.Atan2(rotation.X, -rotation.Y);
        }

        public static float MapToRange(this float rotation)
        {
            float r = rotation;
            float Pi = (float)Math.PI;
            if (rotation > Pi)
            {
                return r - 2 * Pi;
            }
            else if (rotation < -Pi)
            {
                return r + 2 * Pi;
            }
            else return r;
        }

        public static float Abs(this float rotation)
        {
            return (float)Math.Abs(rotation);
        }

        private const double Epsilon = 1e-10;
        public static bool IsZero(this float d)
        {
            return Math.Abs(d) < Epsilon;
        }

        public static Vector2 Clone(this Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2 RotateVector(this Vector2 vector, float radians)
        {
            float angle = vector.ToRotation() + radians;
            return angle.MapToRange().RotationToVector() * vector.Length();
        }
        public static Vector2 Norm1(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }
        public static Vector2 Norm2(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }
        public static Vector2 Position(this MouseState mouse)
        {
            return new Vector2(mouse.X, mouse.Y);
        }
        public static Vector2 Position(this LayerTile tile)
        {
            return new Vector2(tile.X, tile.Y);
        }
        public static bool IsNull(this Vector2 v)
        {
            return (float.IsNaN(v.X) || float.IsNaN(v.Y)) || (v == Vector2.Zero);
        }

        /// <summary>
        /// Devuelve la posición del ratón relativa a la cámara especificada.
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="camera">Cámara en base a la que se va a rectificar la posición del ratón.</param>
        /// <returns></returns>
        public static Vector2 PositionRelative(this MouseState mouse, Camera camera)
        {
            Vector3 mousePosition = new Vector3(mouse.Position(), 0f);
            Vector3 project = camera.Unproject(ref mousePosition);
            Vector2 projectMouse = project.ToVector2();
            WaveServices.ViewportManager.RecoverPosition(ref projectMouse);
            return projectMouse;
        }

        public static int Width(this TiledMap map)
        {
            return map.Width * map.TileWidth;
        }

        public static int Height(this TiledMap map)
        {
            return map.Height * map.TileHeight;
        }
    }
}
