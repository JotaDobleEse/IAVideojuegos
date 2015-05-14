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
    public static class EntityFactory
    {
        public static Entity Shoot(Vector2 position, Vector2 target)
        {
            return new Entity()
                .AddComponent(new Transform2D() { Position = position })
                .AddComponent(new Sprite(@"Content\Textures\labala"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new Shoot(position, target));
        }
    }
}
