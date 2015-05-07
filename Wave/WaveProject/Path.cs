using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject
{
    public class Path
    {
        public List<Vector2> Points { get; private set; }
        public int Length { get { return Points.Count; } }

        public Path()
        {
            Points = new List<Vector2>();
        }

        public int GetParam(Vector2 position, int lastParam)
        {
            float dist1 = (position - GetPosition(lastParam)).Length();
            float dist2 = (position - GetPosition(lastParam + 1)).Length();
            if (dist1 > dist2)
            {
                return Math.Min((lastParam + 1),  Points.Count - 1);
            }
            return lastParam;
        }

        public Vector2 GetPosition(int param)
        {
            if (Length == 0)
                return Vector2.Zero;
            if (param >= Length)
                return Points[Length - 1];
            return Points[param];
        }

        public void AddPosition(Vector2 position)
        {
            Points.Add(position);
        }

        public void RemovePosition(Vector2 position)
        {
            Points.Remove(position);
        }

        public void RemovePosition(int index)
        {
            Points.RemoveAt(index);
        }

        public void SetPath(List<Vector2> path)
        {
            Points = path;
        }

        public void DrawPath(LineBatch2D batch)
        {
            for (int i = 1; i < Points.Count; i++)
            {
                Vector2 pos1 = Points[i - 1];
                Vector2 pos2 = Points[i % Points.Count];
                batch.DrawLineVM(pos1, pos2, Color.IndianRed, 1f);
            }
        }
    }
}
