using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class GameController : Behavior
    {
        Kinematic Mouse;
        Vector2 LastMousePosition;
        Vector2 LastStartTile;
        Vector2 LastEndTile;

        Button LRTAManhattan;
        Button LRTAChevychev;
        Button LRTAEuclidean;
        DistanceAlgorith CurrentLrtaAlgorithm = DistanceAlgorith.MANHATTAN;
        public DebugLines Debug { get; set; }
        private PlayableCharacter SelectedCharacter;

        public GameController(Kinematic mouse)
        {
            Mouse = mouse;
            LastMousePosition = mouse.Position;
            LastStartTile = LastEndTile = Vector2.Zero;
        }

        protected override void Initialize()
        {
            LRTAManhattan = EntityManager.Find<Button>("LRTA_Manhattan");
            LRTAChevychev = EntityManager.Find<Button>("LRTA_Chevychev");
            LRTAEuclidean = EntityManager.Find<Button>("LRTA_Euclidean");
            
            float width = WaveServices.ViewportManager.ScreenWidth;
            float height = WaveServices.ViewportManager.ScreenHeight;
            LRTAManhattan.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAManhattan.Width - 10, 50);
            LRTAChevychev.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAManhattan.Width - 10, 100);
            LRTAEuclidean.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAManhattan.Width - 10, 150);

            LRTAManhattan.Click += LRTAManhattan_Click;
            LRTAChevychev.Click += LRTAChevychev_Click;
            LRTAEuclidean.Click += LRTAEuclidean_Click;

            LRTAManhattan.IsVisible = false;
            //base.Initialize();
        }

        void LRTAEuclidean_Click(object sender, EventArgs e)
        {
            CurrentLrtaAlgorithm = DistanceAlgorith.EUCLIDEAN;
            LRTAManhattan.IsVisible = true;
            LRTAChevychev.IsVisible = true;
            LRTAEuclidean.IsVisible = false;
        }

        void LRTAChevychev_Click(object sender, EventArgs e)
        {
            CurrentLrtaAlgorithm = DistanceAlgorith.CHEVYCHEV;
            LRTAManhattan.IsVisible = true;
            LRTAChevychev.IsVisible = false;
            LRTAEuclidean.IsVisible = true;
        }

        void LRTAManhattan_Click(object sender, EventArgs e)
        {
            CurrentLrtaAlgorithm = DistanceAlgorith.MANHATTAN;
            LRTAManhattan.IsVisible = false;
            LRTAChevychev.IsVisible = true;
            LRTAEuclidean.IsVisible = true;
        }

        protected override void Update(TimeSpan gameTime)
        {
            Mouse.Update((float)gameTime.TotalMilliseconds, new Steerings.SteeringOutput());
            if (WaveServices.Input.MouseState.LeftButton == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                IEnumerable<PlayableCharacter> characters = EntityManager.AllEntities
                    .Where(w => w.FindComponent<PlayableCharacter>() != null)
                    .Select(s => s.FindComponent<PlayableCharacter>());

                SelectedCharacter = characters
                    .FirstOrDefault(f => Mouse.Position.IsContent(f.Kinematic.Position, new Vector2(f.Texture.Texture.Width, f.Texture.Texture.Height)));
            }
            if (SelectedCharacter != null)
            {
                if (WaveServices.Input.MouseState.RightButton == WaveEngine.Common.Input.ButtonState.Pressed && MyScene.TiledMap.PositionInMap(Mouse.Position))
                {
                    LRTA lrta = new LRTA(SelectedCharacter.Kinematic.Position, Mouse.Position, algorithm: CurrentLrtaAlgorithm);
                    if (LastStartTile != lrta.StartPos || LastEndTile != lrta.EndPos)
                    {
                        LastStartTile = lrta.StartPos;
                        LastEndTile = lrta.EndPos;
                        List<Vector2> path = lrta.Execute();
                        SelectedCharacter.SetPath(path);
                        Debug.Path = path;
                    }
                }
            }
            Debug.SelecterCharacter = SelectedCharacter;
            LastMousePosition = Mouse.Position;
        }
    }
}
