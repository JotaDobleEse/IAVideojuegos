using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings.Pathfinding
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float H { get; set; }
        public float Temp { get; set; }
        public int Steps { get; set; }

        public static bool operator ==(Node n1, Node n2)
        {
            return (n1.X == n2.X) && (n1.Y == n2.Y);
        }

        public static bool operator !=(Node n1, Node n2)
        {
            return !(n1 == n2);
        }
    }

    public class LRTA
    {
        private Node[] GetNeighbors(Node node)
        {
            return null;
        }

        public Node[] Execute(Vector2 startPos, Vector2 endPos)
        {
            return Execute(new Node() { X = (int)startPos.X, Y = (int)startPos.Y }, new Node() { X = (int)endPos.X, Y = (int)endPos.Y });
        }

        public Node[] Execute(Node start, Node end)
        {
            List<Node> path = new List<Node>();
            Node current = start;
            var neighbors = GetNeighbors(current);
            float min = int.MaxValue;
            foreach (var neighbor in neighbors)
            {
                var temp = current.Steps + 1 + Manhatan(neighbor, end);
                if (temp < min)
                    temp = min;
            }
            current.H = neighbors.Min(m => m.H);

            return path.ToArray();
        }

        public void UpdateStep(Node[] sLss)
        {
            foreach (var u in sLss)
            {
                u.Temp = u.H;
                u.H = float.MaxValue;
            }
            while (sLss.Any(a => a.H == float.MaxValue))
            {
                var v = sLss.OrderBy(o => o.H).First();
                Math.Max(v.Temp, 0);
                break;
            }
        }

        public float w(Node u, Node a)
        {
            //TODO coger peso al pasar del nodo u al nodo a
            return 1;
        }

        public float Manhatan(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * (abs (n.x-goal.x) + abs (n.y-goal.y)).
            return minValue * (Math.Abs(i.X - j.X) + Math.Abs(i.Y - j.Y));
        }

        public float Chebychev(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * max (abs (N.X-goal.x), abs (N.Y-goal.y))
            return minValue * Math.Max(Math.Abs(i.X - j.X), Math.Abs(i.Y - j.Y));
        }

        public float Euclidean(Node i, Node j, float minValue = 1f)
        {
            //h (n) = D * sqrt ((N.X-goal.x) ^ 2 + (N.Y-goal.y) ^ 2)
            return minValue * (float)Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
        }
    }
}
