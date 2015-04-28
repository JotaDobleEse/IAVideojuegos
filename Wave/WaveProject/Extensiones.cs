using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

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

        public static float ToRotation(this Vector2 rotation)
        {
            return (float)Math.Atan2(rotation.X, -rotation.Y);
        }
    }
}
