using UnityEngine;

namespace Saro.AStar
{
    public class CustomTileProvider : TileProvider
    {
        private Map map;

        public CustomTileProvider(Map map) : base(map.GridX, map.GridY, map.MovementType)
        {
            this.map = map;
        }

        public override TileData GetTile(int x, int y)
        {
            return map.Tiles[x, y];
        }

        public override bool IsTileWalkable(int x, int y)
        {
            return map.IsWalkableAt(x, y);
        }

        public override Vector2Int World2TilePosition(Vector2 worldPosition)
        {
            return map.World2TilePosition(worldPosition);
        }
    }
}