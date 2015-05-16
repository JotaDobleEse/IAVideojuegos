using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveProject.CharacterTypes;

namespace WaveProject
{
    public static class EntityFactory
    {
        private static string[] Textures = new string[] { "malabestia", "soldado", "lagarto", "juggernaut" };
        private static System.Random Rand = new System.Random();

        public static Entity Shoot(Vector2 position, Vector2 target)
        {
            return new Entity()
                .AddComponent(new Transform2D() { Position = position })
                .AddComponent(new Sprite(@"Content\Textures\labala"))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new Shoot(position, target));
        }

        public static Entity CharacterRandom(int team)
        {
            float x = Rand.Next(1000);
            float y = Rand.Next(800);

            Kinematic position = new Kinematic(true) { Position = new Vector2(x, y) };
            string texture = Textures[(int)y % Textures.Length];
            EnumeratedCharacterType type = EnumeratedCharacterType.NONE;
            switch (texture)
            {
                case "malabestia":
                    type = EnumeratedCharacterType.MELEE;
                    break;
                case "soldado":
                    type = EnumeratedCharacterType.RANGED;
                    break;
                case "lagarto":
                    type = EnumeratedCharacterType.EXPLORER;
                    break;
                case "juggernaut":
                    type = EnumeratedCharacterType.MELEE;
                    break;
            }

            Entity character = new Entity()
                 .AddComponent(new Transform2D() { Position = position.Position })
                 .AddComponent(new Sprite("Content/Textures/" + texture))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new CharacterNPC(position, type, team));

            return character;
        }

        public static Entity Character(float x, float y, int team, EnumeratedCharacterType type)
        {
            Kinematic position = new Kinematic(true) { Position = new Vector2(x, y) };
            string texture = "";
            switch (type)
            {
                case EnumeratedCharacterType.MELEE:
                    texture = "malabestia";
                    break;
                case EnumeratedCharacterType.RANGED:
                    texture = "soldado";
                    break;
                case EnumeratedCharacterType.EXPLORER:
                    texture = "lagarto";
                    break;
                case EnumeratedCharacterType.NONE:
                    texture = "juggernaut";
                    break;
            }

            Entity character = new Entity()
                 .AddComponent(new Transform2D() { Position = position.Position })
                 .AddComponent(new Sprite("Content/Textures/" + texture))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new CharacterNPC(position, type, team));

            return character;
        }
    }
}
