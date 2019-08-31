using UnityEngine;

namespace Saro.AStar
{
    public abstract class TileProvider
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public DiagonalMovement MovementType = DiagonalMovement.NotCutCorner;

        protected TileProvider(int width, int height, DiagonalMovement movementType)
        {
            Width = width;
            Height = height;
            MovementType = movementType;
        }

        // public abstract TileData[, ] TileData { get; }

        public abstract Vector2Int World2TilePosition(Vector2 worldPosition);

        public abstract TileData GetTile(int x, int y);
        public abstract bool IsTileWalkable(int x, int y);
    }

    public enum DiagonalMovement
    {
        Never,
        CutCorner,
        NotCutCorner
    }
}