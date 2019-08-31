using Priority_Queue;
using UnityEngine;

namespace Saro.AStar
{
    public class Node : FastPriorityQueueNode
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public static Node New(int x, int y)
        {
            return new Node(x, y);
        }

        private Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            var other = (Node)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X + Y * 7;
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", X, Y);
        }

    }
}