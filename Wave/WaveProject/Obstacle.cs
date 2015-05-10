using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject
{
    public class Obstacle : IDisposable
    {
        private static List<Obstacle> obstacles = new List<Obstacle>();
        public static List<Obstacle> Obstacles { get { return obstacles; } }
        public Vector2 Position { get; private set; }
        public float BRadius { get; private set; }

        public Obstacle(Vector2 position, float bRadius, bool stable = false)
        {
            Position = position;
            BRadius = bRadius;
            if (stable)
            {
                obstacles.Add(this);
            }
        }

        public Obstacle Clone()
        {
            Obstacle o = new Obstacle(Position, BRadius);
            return o;
        }

        public void Dispose()
        {
            obstacles.Remove(this);
        }
    }
}
