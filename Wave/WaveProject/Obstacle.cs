using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject
{
    public class Obstacle
    {
        private static List<Obstacle> obstacles = new List<Obstacle>();
        public static List<Obstacle> Obstacles { get { return obstacles; } }
        public Vector2 Position { get; set; }
        public float BRadius { get; set; }

        public Obstacle(bool stable = false)
        {
            if (stable)
                obstacles.Add(this);
        }

        ~Obstacle()
        {
            obstacles.Remove(this);
        }
    }
}
