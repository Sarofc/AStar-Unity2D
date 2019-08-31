using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace Saro.AStar
{

    public class Pathfinding
    {

        public const int MAX = 1000;
        public const float DIAGONAL_DST = 1.41421356237f;

        private FastPriorityQueue<Node> m_openSet = new FastPriorityQueue<Node>(MAX);
        private Dictionary<Node, Node> m_cameFrom = new Dictionary<Node, Node>();
        private Dictionary<Node, float> m_costSoFar = new Dictionary<Node, float>();
        private List<Node> m_near = new List<Node>();

        public Pathfinding() { }

        public PathProcessResult FindPath(Vector2 pathStart, Vector2 pathEnd, TileProvider provider, List<Vector2> cachePath, out List<Vector2> path)
        {

            if (provider == null)
            {
                path = null;
                return PathProcessResult.Error_Grid_NotFound;
            }

            var start = provider.World2TilePosition(pathStart);
            var target = provider.World2TilePosition(pathEnd);

            if (!provider.IsTileWalkable(start.x, start.y))
            {
                path = null;
                return PathProcessResult.Error_Start_NotWalkable;
            }

            if (!provider.IsTileWalkable(target.x, target.y))
            {
                path = null;
                return PathProcessResult.Error_End_NotWalkable;
            }

            Clear();

            var startNode = Node.New(start.x, start.y);
            var targetNode = Node.New(target.x, target.y);

            if (startNode.Equals(targetNode))
            {
                path = null;
                return PathProcessResult.Error_Start_IsEnd;
            }

            m_openSet.Enqueue(startNode, 0f);
            m_cameFrom[startNode] = startNode;
            m_costSoFar[startNode] = 0f;

            int count;
            while ((count = m_openSet.Count) > 0)
            {

                if (count >= MAX - 8)
                {
                    path = null;
                    return PathProcessResult.Error_Path_Too_Long;
                }

                var current = m_openSet.Dequeue();

                if (current.Equals(targetNode))
                {
                    path = RetracePath(targetNode, cachePath, provider);
                    return PathProcessResult.Success;
                }

                foreach (Node neighbour in GetNear(current, provider))
                {

                    float newCost = m_costSoFar[current] + GetGCost(current, neighbour) +
                        (provider.IsTileWalkable(neighbour.X, neighbour.Y) ?
                            provider.GetTile(neighbour.X, neighbour.Y).Penalty :
                            0f);

                    if (!m_costSoFar.ContainsKey(neighbour) || newCost < m_costSoFar[neighbour])
                    {
                        m_costSoFar[neighbour] = newCost;
                        float priority = newCost + Heuristic(neighbour, targetNode);

                        m_openSet.Enqueue(neighbour, priority);

                        m_cameFrom[neighbour] = current;
                    }
                }
            }

            path = null;
            return PathProcessResult.Error_Path_Not_Found;
        }

        private List<Vector2> RetracePath(Node endNode, List<Vector2> path, TileProvider provider)
        {
            if (path == null) path = new List<Vector2>();
            else path.Clear();

            Node currentNode = endNode;

            while (true)
            {
                Node prev = m_cameFrom[currentNode];
                var tileData = provider.GetTile(currentNode.X, currentNode.Y);
                path.Add(tileData.WorldPosition);
                // path.Add (new Vector2 (currentNode.X, currentNode.Y));
                if (prev != null && currentNode != prev) currentNode = prev;
                else break;
            }
            path.Reverse();
            return path;
        }

        private void Clear()
        {
            m_cameFrom.Clear();
            m_near.Clear();
            m_costSoFar.Clear();
            m_openSet.Clear();
        }

        private float Heuristic(Node a, Node b)
        {
            // Manhattan Distance
            return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);

            // Euclidean Distance
            // TODO
        }

        private float GetGCost(Node a, Node b)
        {
            if (Mathf.Abs(a.X - b.X) == 1 && a.Y == b.Y) return 1;
            if (Mathf.Abs(a.Y - b.Y) == 1 && a.X == b.X) return 1;
            return DIAGONAL_DST;
        }

        //      s0
        //  s3      s1
        //      s2

        //  d0      d1
        //
        //  d3      d2
        private List<Node> GetNear(Node node, TileProvider provider)
        {
            m_near.Clear();

            int x = node.X;
            int y = node.Y;

            bool s0 = false;
            bool s1 = false;
            bool s2 = false;
            bool s3 = false;

            bool d0 = false;
            bool d1 = false;
            bool d2 = false;
            bool d3 = false;

            // ↑
            if (provider.IsTileWalkable(x, y - 1))
            {
                m_near.Add(Node.New(x, y - 1));
                s0 = true;
            }

            // →
            if (provider.IsTileWalkable(x + 1, y))
            {
                m_near.Add(Node.New(x + 1, y));
                s1 = true;
            }

            // ↓
            if (provider.IsTileWalkable(x, y + 1))
            {
                m_near.Add(Node.New(x, y + 1));
                s2 = true;
            }

            // ←
            if (provider.IsTileWalkable(x - 1, y))
            {
                m_near.Add(Node.New(x - 1, y));
                s3 = true;
            }

            if (provider.MovementType == DiagonalMovement.Never) return m_near;

            else if (provider.MovementType == DiagonalMovement.NotCutCorner)
            {
                d0 = s0 && s3;
                d1 = s0 && s1;
                d2 = s1 && s2;
                d3 = s2 && s3;
            }
            else if (provider.MovementType == DiagonalMovement.CutCorner)
            {
                d0 = s0 || s3;
                d1 = s0 || s1;
                d2 = s1 || s2;
                d3 = s2 || s3;
            }

            // ↖
            if (d0 && provider.IsTileWalkable(x - 1, y - 1))
                m_near.Add(Node.New(x - 1, y - 1));

            // ↗
            if (d1 && provider.IsTileWalkable(x + 1, y - 1))
                m_near.Add(Node.New(x + 1, y - 1));

            // ↘
            if (d2 && provider.IsTileWalkable(x + 1, y + 1))
                m_near.Add(Node.New(x + 1, y + 1));

            // ↙
            if (d3 && provider.IsTileWalkable(x - 1, y + 1))
                m_near.Add(Node.New(x - 1, y + 1));

            return m_near;
        }
    }

}