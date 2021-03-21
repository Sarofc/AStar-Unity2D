using Saro.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saro.AStar
{

    public sealed class Pathfinder
    {

        public const int MAX = 1000;
        public const float DIAGONAL_DST = 1.41421356237f;

        private SBinaryMinHeap<Node> m_OpenSet;
        private Dictionary<Node, Node> m_CameFrom;
        private Dictionary<Node, float> m_CostSoFar;
        private List<Node> m_Neighbours;

        public Pathfinder()
        {
            m_OpenSet = new SBinaryMinHeap<Node>(MAX);
            m_CameFrom = new Dictionary<Node, Node>();
            m_CostSoFar = new Dictionary<Node, float>();
            m_Neighbours = new List<Node>();
        }

        public EPathProcessResult FindPath(AStarGrid grid, Vector2 pathStart, Vector2 pathEnd, List<Vector2> cachePath, out List<Vector2> path)
        {

            if (grid == null)
            {
                path = null;
                return EPathProcessResult.Error_Grid_NotFound;
            }

            var start = grid.World2TilePosition(pathStart);
            var target = grid.World2TilePosition(pathEnd);

            if (!grid.IsTileWalkable(start.x, start.y))
            {
                path = null;
                return EPathProcessResult.Error_Start_NotWalkable;
            }

            if (!grid.IsTileWalkable(target.x, target.y))
            {
                path = null;
                return EPathProcessResult.Error_End_NotWalkable;
            }

            Clear();

            var startNode = Node.New(start.x, start.y, 0);
            var targetNode = Node.New(target.x, target.y, 0);

            if (startNode.Equals(targetNode))
            {
                path = null;
                return EPathProcessResult.Error_Start_IsEnd;
            }

            m_OpenSet.Push(startNode);
            m_CameFrom[startNode] = startNode;
            m_CostSoFar[startNode] = 0f;

            int count;
            while ((count = m_OpenSet.Count) > 0)
            {

                if (count >= MAX - 8)
                {
                    path = null;
                    return EPathProcessResult.Error_Path_Too_Long;
                }

                var current = m_OpenSet.Pop();

                if (current.Equals(targetNode))
                {
                    path = RetracePath(targetNode, cachePath, grid);
                    return EPathProcessResult.Success;
                }

                List<Node> list = GetNeighbours(current, grid);
                for (int i = 0; i < list.Count; i++)
                {
                    Node neighbour = list[i];
                    //float newCost = m_CostSoFar[current] + GetGCost(current, neighbour) +
                    //    (grid.IsTileWalkable(neighbour.X, neighbour.Y) ?
                    //        grid.GetValue(neighbour.X, neighbour.Y) :
                    //        0f);

                    float newCost = m_CostSoFar[current]
                        + GetGCost(current, neighbour)
                        + grid.GetValue(neighbour.X, neighbour.Y);

                    if (!m_CostSoFar.ContainsKey(neighbour) || newCost < m_CostSoFar[neighbour])
                    {
                        m_CostSoFar[neighbour] = newCost;
                        float priority = newCost + Heuristic(neighbour, targetNode);

                        neighbour.Cost = priority;
                        m_OpenSet.Push(neighbour);

                        m_CameFrom[neighbour] = current;
                    }
                }
            }

            path = null;
            return EPathProcessResult.Error_Path_Not_Found;
        }

        private List<Vector2> RetracePath(Node endNode, List<Vector2> path, AStarGrid grid)
        {
            if (path == null) path = new List<Vector2>();
            else path.Clear();

            Node currentNode = endNode;

            while (true)
            {
                Node prev = m_CameFrom[currentNode];

                //var tileData = provider.GetTile(currentNode.X, currentNode.Y);
                //path.Add(tileData.WorldPosition);

                path.Add(grid.Tile2WorldPosition(currentNode.X, currentNode.Y));
                //path.Add(new Vector2(currentNode.X, currentNode.Y));

                if (prev != null && currentNode != prev) currentNode = prev;
                else break;
            }
            path.Reverse();
            return path;
        }

        private void Clear()
        {
            m_CameFrom.Clear();
            m_Neighbours.Clear();
            m_CostSoFar.Clear();
            m_OpenSet.Clear();
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
        private List<Node> GetNeighbours(Node node, AStarGrid grid)
        {
            m_Neighbours.Clear();

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
            if (grid.IsTileWalkable(x, y - 1))
            {
                m_Neighbours.Add(Node.New(x, y - 1));
                s0 = true;
            }

            // →
            if (grid.IsTileWalkable(x + 1, y))
            {
                m_Neighbours.Add(Node.New(x + 1, y));
                s1 = true;
            }

            // ↓
            if (grid.IsTileWalkable(x, y + 1))
            {
                m_Neighbours.Add(Node.New(x, y + 1));
                s2 = true;
            }

            // ←
            if (grid.IsTileWalkable(x - 1, y))
            {
                m_Neighbours.Add(Node.New(x - 1, y));
                s3 = true;
            }

            if (grid.MovementType == EDiagonalMovement.Never) return m_Neighbours;

            else if (grid.MovementType == EDiagonalMovement.NotCutCorner)
            {
                d0 = s0 && s3;
                d1 = s0 && s1;
                d2 = s1 && s2;
                d3 = s2 && s3;
            }
            else if (grid.MovementType == EDiagonalMovement.CutCorner)
            {
                d0 = s0 || s3;
                d1 = s0 || s1;
                d2 = s1 || s2;
                d3 = s2 || s3;
            }

            // ↖
            if (d0 && grid.IsTileWalkable(x - 1, y - 1))
                m_Neighbours.Add(Node.New(x - 1, y - 1));

            // ↗
            if (d1 && grid.IsTileWalkable(x + 1, y - 1))
                m_Neighbours.Add(Node.New(x + 1, y - 1));

            // ↘
            if (d2 && grid.IsTileWalkable(x + 1, y + 1))
                m_Neighbours.Add(Node.New(x + 1, y + 1));

            // ↙
            if (d3 && grid.IsTileWalkable(x - 1, y + 1))
                m_Neighbours.Add(Node.New(x - 1, y + 1));

            return m_Neighbours;
        }
    }

}