﻿using System;
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

        public float Hup { get; set; }
        public float Hdown { get; set; }
        public float Hright { get; set; }
        public float Hleft { get; set; }
        public float Hneightbors { get; set; }

        public Terrain Terrain { get; set; }
        public bool Passable { get; set; }

        public Vector2 Position { get { return new Vector2(X, Y); } }

        public static bool operator ==(Node n1, Node n2)
        {
            int nulls = 0;
            try
            {
                int n = n1.X;
            }
            catch (Exception)
            {

                nulls++;
            }
            try
            {
                int n = n2.X;
            }
            catch (Exception)
            {

                nulls++;
            }
            if (nulls == 2)
                return true;
            try
            {
                return (n1.X == n2.X) && (n1.Y == n2.Y);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool operator !=(Node n1, Node n2)
        {
            return !(n1 == n2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                Node n2 = (Node)obj;
                return (this == n2);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (X * 937 + Y * 37) / ((X + Y) * 13);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "), H=" + H + ", Temp=" + Temp;
        }
    }
}
