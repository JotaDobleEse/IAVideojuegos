using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveProject.Characters;
using WaveProject.CharacterTypes;
using WaveProject.Steerings.Combined;

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

        public static Entity PlayableCharacterRandom(int team)
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
                 .AddComponent(new PlayableCharacter(position, type, team));

            return character;
        }

        public static Entity Character(float x, float y, int team, EnumeratedCharacterType type, TextBlock textBlock = null)
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
                 .AddComponent(new CharacterNPC(position, type, team) { Text = textBlock });

            return character;
        }

        public static Entity FlockingRandom()
        {
            float x = Rand.Next(WaveServices.Platform.ScreenWidth) + WaveServices.Platform.ScreenWidth / 2;
            float y = Rand.Next(WaveServices.Platform.ScreenHeight) + WaveServices.Platform.ScreenHeight / 2;

            Kinematic position = new Kinematic(true) { Position = new Vector2(x, y)/*, Rotation = (float)Math.PI/2*/ };
           
            Entity character = new Entity()
                 .AddComponent(new Transform2D() { Position = position.Position, Rotation = position.Rotation })
                 .AddComponent(new Sprite("Content/Textures/soldado"))
                 .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                 .AddComponent(new SteeringCharacter(position, EnumeratedCharacterType.RANGED, new BlendedSteering(SteeringsFactory.Flocking(position))));

            return character;
        }

        public static TextBlock GetTextBlock()
        {
            TextBlock text1 = new TextBlock()
            {
                Foreground = Color.Cyan,
                //BorderColor = Color.Black,
                //IsBorder = true,
                FontPath = "Content/Fonts/verdana.wpk",
                IsVisible = true
            };
            return text1;
        }
    }
}
