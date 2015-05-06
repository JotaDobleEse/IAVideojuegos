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
        Button LRTAManhattan;
        Button LRTAChevychev;
        Button LRTAEuclidean;
        DistanceAlgorith CurrentLrtaAlgorithm = DistanceAlgorith.MANHATTAN;
        public DebugLines Debug { get; set; }

        public GameController(Kinematic mouse)
        {
            Mouse = mouse;
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
            if (MyScene.TiledMap.PositionInMap(Mouse.Position))
            {
                LRTA lrta = new LRTA(new Vector2(300, 300), Mouse.Position, algorithm: CurrentLrtaAlgorithm);
                Vector2[] path = lrta.Execute();
                Debug.Path = path;
            }
        }
    }
}
