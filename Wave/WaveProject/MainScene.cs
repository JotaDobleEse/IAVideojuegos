#region Using Statements
using System;
using System.Linq;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveProject.Steerings;
using WaveEngine.TiledMap;
using WaveEngine.Framework.Physics2D;
using WaveProject.Steerings.Combined;
using WaveProject.Steerings.Group;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;
using System.Globalization;
using System.Threading;
using WaveProject.CharacterTypes;
using WaveEngine.Materials;
using System.Collections.Generic;
#endregion

namespace WaveProject
{
    public class MainScene : Scene
    {
        TiledMap TiledMap;

        protected override void CreateScene()
        {
            // Controlador principal
            var gameController = new Entity("Controller")
                .AddComponent(new GameController(Kinematic.Mouse));
            EntityManager.Add(gameController);

            // Botones
            Button LRTAManhattan = new Button("LRTA_Manhattan") { BackgroundColor = Color.Gray, Text = "LRTA Manhattan", Width = 210, IsBorder = false };
            Button LRTAChevychev = new Button("LRTA_Chevychev") { BackgroundColor = Color.Gray, Text = "LRTA Chevychev", Width = 210, IsBorder = false };
            Button LRTAEuclidean = new Button("LRTA_Euclidean") { BackgroundColor = Color.Gray, Text = "LRTA Euclidean", Width = 210, IsBorder = false };
            Button FormationMode = new Button("FormationMode") { BackgroundColor = Color.Green, Text = "Enable Formation Mode", Width = 210, IsBorder = false };
            Button DecisionalIA = new Button("DecisionalIA") { BackgroundColor = Color.Green, Text = "Enable Decisional IA", Width = 210, IsBorder = false };
            Button FinalBattle = new Button("FinalBattle") { BackgroundColor = Color.Gray, Text = "Final Battle", Width = 210, IsBorder = false };
            Button SetDebug = new Button("SetDebug") { BackgroundColor = Color.Red, Text = "Disable Debug", Width = 210, IsBorder = false };

            EntityManager.Add(LRTAManhattan);
            EntityManager.Add(LRTAChevychev);
            EntityManager.Add(LRTAEuclidean);
            EntityManager.Add(FormationMode);
            EntityManager.Add(DecisionalIA);
            EntityManager.Add(FinalBattle);
            EntityManager.Add(SetDebug);

            // Mapa de juego
            Entity map = new Entity("mapa")
                .AddComponent(new Transform2D())
                .AddComponent(TiledMap = new TiledMap("Content/Maps/mapa.tmx")
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = -0
                });
            EntityManager.Add(map);

            // Texto superior, muestra estadísticas y ganador
            TextBlock text = new TextBlock("axis")
            {
                Margin = new Thickness((WaveServices.Platform.ScreenWidth / 2f) - 100, 10, 0, 0),
                FontPath = "Content/Fonts/verdana.wpk",
                Height = 130,
            };
            text.IsVisible = true;

            // Asignamos el texto a la clase Debug
            DebugLines.Debug.Text = text;

            // Cámara del juego
            var camera2D = new FixedCamera2D("Camera2D") { ClearFlags = ClearFlags.All, BackgroundColor = Color.Black }
                .Entity.AddComponent(DebugLines.Debug) // Debug sobre la cámara
                .AddComponent(new CameraController()); // Controlador de Cámara

            EntityManager.Add(camera2D);
            EntityManager.Add(text);

            // Jugadores manejables
            EntityManager.Add(EntityFactory.PlayableCharacter(168, 72, 1, EnumeratedCharacterType.RANGED));
            EntityManager.Add(EntityFactory.PlayableCharacter(264, 72, 1, EnumeratedCharacterType.EXPLORER));
            EntityManager.Add(EntityFactory.PlayableCharacter(136, 168, 1, EnumeratedCharacterType.EXPLORER));
            EntityManager.Add(EntityFactory.PlayableCharacter(296, 168, 1, EnumeratedCharacterType.MELEE));
            EntityManager.Add(EntityFactory.PlayableCharacter(360, 120, 1, EnumeratedCharacterType.MELEE));

            // Jugadores NPC, no manejables. Se les añade un componenete de texto para mostrar acciones
            TextBlock text1, text2, text3, text4, text5;
            EntityManager.Add(text1 = EntityFactory.GetTextBlock());
            EntityManager.Add(text2 = EntityFactory.GetTextBlock());
            EntityManager.Add(text3 = EntityFactory.GetTextBlock());
            EntityManager.Add(text4 = EntityFactory.GetTextBlock());
            EntityManager.Add(text5 = EntityFactory.GetTextBlock());
            EntityManager.Add(EntityFactory.Character(1160, 808, 2, EnumeratedCharacterType.RANGED, text1));
            EntityManager.Add(EntityFactory.Character(1000, 872, 2, EnumeratedCharacterType.MELEE, text2));
            EntityManager.Add(EntityFactory.Character(1000, 664, 2, EnumeratedCharacterType.MELEE, text3));
            EntityManager.Add(EntityFactory.Character(888, 840, 2, EnumeratedCharacterType.EXPLORER, text4));
            EntityManager.Add(EntityFactory.Character(1352, 616, 2, EnumeratedCharacterType.EXPLORER, text5));

            // Zona de Render del mapa de influencia
            Entity influenceMap = new Entity("InfluenceMap")
                .AddComponent(new Transform2D())
                .AddComponent(new Sprite(InfluenceMap.Influence.Texture))
                .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            EntityManager.Add(influenceMap);

        }

        protected override void Start()
        {
            base.Start();

            // Incialización de los mapaa de juego e influencia
            Map.CurrentMap.Initialize(TiledMap);
            InfluenceMap.Influence.Initialize();

            // Establecimiento del tamaño y la posición de la zona de renderizado del mapa de influencia
            float width = WaveServices.ViewportManager.ScreenWidth;
            float height = WaveServices.ViewportManager.ScreenHeight;
            var entityMap = EntityManager.Find("InfluenceMap");
            var sprite = entityMap.FindComponent<Sprite>();
            sprite.SourceRectangle = new Rectangle(0, 0, TiledMap.Width() / InfluenceMap.Scale, TiledMap.Height() / InfluenceMap.Scale);
            sprite.Transform2D.Position = new Vector2(width - sprite.SourceRectangle.Value.Width - 20, height - sprite.SourceRectangle.Value.Height - 20);
        }
    }
}
