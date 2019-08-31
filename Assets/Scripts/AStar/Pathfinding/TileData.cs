using Priority_Queue;
using UnityEngine;

namespace Saro.AStar
{
    public class TileData
    {
        public Vector2 WorldPosition { get; set; }
        public int Penalty { get; set; } //movement cost. -1 mean obstacle, otherwise, the higher cost, the lower priority.


        public TileData(Vector2 worldPosition, int penalty)
        {
            this.WorldPosition = worldPosition;
            this.Penalty = penalty;
        }
    }
}