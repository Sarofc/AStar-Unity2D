using System;
using System.Runtime.CompilerServices;

namespace Saro.AStar
{
    public struct Node : IComparable<Node>
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public float Cost { get; set; }

        public static Node New(int x, int y, float cost = 0f)
        {
            return new Node(x, y, cost);
        }

        public Node(int x, int y, float cost)
        {
            this.X = x;
            this.Y = y;
            this.Cost = cost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Node lhs, Node rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Node lhs, Node rhs)
        {
            return !(lhs == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Node))
            {
                return false;
            }

            return Equals((Node)other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Node other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2);
        }

        public override string ToString()
        {
            return string.Format("({0} , {1})", X, Y);
        }

        public int CompareTo(Node other)
        {
            return Cost.CompareTo(other.Cost);
        }
    }
}