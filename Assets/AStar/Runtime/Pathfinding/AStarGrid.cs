using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Saro.AStar
{
    [CreateAssetMenu(fileName = "AStarGrid")]
    public class AStarGrid : ScriptableObject
    {
        public Vector2 GridSize = new Vector2(5, 5);
        public Vector2 GridCenter;
        public float NodeSize = 1f;

        public EDiagonalMovement MovementType = EDiagonalMovement.NotCutCorner;
        public LayerMask checkLayer;

        [Saro.Attributes.ReadOnly] public byte[] Tiles;

        [SerializeField] private int m_GridX;
        [SerializeField] private int m_GridY;

        public int GridX { get => m_GridX; private set => m_GridX = value; }
        public int GridY { get => m_GridY; private set => m_GridY = value; }

        private void OnValidate()
        {
            GridX = Mathf.RoundToInt(GridSize.x / NodeSize);
            GridY = Mathf.RoundToInt(GridSize.y / NodeSize);
        }

        public Vector2Int World2TilePosition(Vector2 worldPosition)
        {
            var percentX = (worldPosition.x - GridCenter.x + GridSize.x / 2) / GridSize.x;
            var percentY = (worldPosition.y - GridCenter.y + GridSize.y / 2) / GridSize.y;

            var x = Mathf.RoundToInt((GridX - 1) * percentX);
            var y = Mathf.RoundToInt((GridY - 1) * percentY);

            return new Vector2Int(x, y);
        }

        public Vector2 Tile2WorldPosition(int x, int y)
        {
            var percentX = (float)x / (GridX - 1);
            var percentY = (float)y / (GridY - 1);

            var newX = GridSize.x * percentX /*+ GridCenter.x*/;
            var newY = GridSize.y * percentY /*+ GridCenter.y*/;

            return new Vector2(newX, newY);
        }

        public int GetValue(int x, int y)
        {
            return Tiles[GetIndex(x, y)];
        }

        public bool IsTileWalkable(int x, int y)
        {
            return IsInBounds(x, y) && Tiles[GetIndex(x, y)] < 255;
        }

        public bool IsInBounds(int x, int y)
        {
            return (x >= 0 && x < GridX && y >= 0 && y < GridY);
        }

        public void DrawGrid()
        {
            Gizmos.DrawWireCube(GridCenter, new Vector3(GridSize.x, GridSize.y, 0));

            if (Tiles != null && Tiles.Length == GridX * GridY)
            {
                for (int i = 0; i < GridX; i++)
                {
                    for (int j = 0; j < GridY; j++)
                    {
                        byte n = Tiles[GetIndex(i, j)];
                        Gizmos.color = new Color(1 - n / 255f, 1 - n / 255f, 1 - n / 255f, 1 - n / 255f);
                        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, .4f);
                        Gizmos.DrawCube(new Vector2((i + 0.5f) * NodeSize + GridCenter.x - GridSize.x / 2f, (j + 0.5f) * NodeSize + GridCenter.y - GridSize.y / 2f), Vector2.one * NodeSize * .9f);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetIndex(int x, int y)
        {
            return x + GridX * y;
        }

#if UNITY_EDITOR

        public byte[] GenerateGrid()
        {
            GridX = Mathf.RoundToInt(GridSize.x / NodeSize);
            GridY = Mathf.RoundToInt(GridSize.y / NodeSize);

            Tiles = new byte[GridX * GridY];

            //var obstacles = GameObject.FindObjectsOfType<Design.AStarObstacle>();
            //foreach (var obstacle in obstacles)
            //{
            //    var pos = obstacle.transform.position;

            //    var tilePos = World2TilePosition(new Vector2(pos.x, pos.y));

            //    Tiles[GetIndex(tilePos.x, tilePos.y)] = obstacle.cost;
            //}

            Vector2 worldBottomLeft = GridCenter - Vector2.right * GridSize.x / 2 - Vector2.up * GridSize.y / 2;
            for (int x = 0; x < GridX; x++)
            {
                for (int y = 0; y < GridY; y++)
                {
                    Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * NodeSize + NodeSize / 2f) + Vector2.up * (y * NodeSize + NodeSize / 2f);

                    byte cost = 0;
                    Collider2D hit = Physics2D.OverlapBox(worldPoint, new Vector2(NodeSize * .5f, NodeSize * .5f), 0, checkLayer);
                    // if (hit.gameObject.layer == LayerMask.NameToLayer ("Obstacle")) {
                    // }
                    // else {
                    //     lookupWeights.TryGetValue (hit.gameObject.layer, out movementPenalty);
                    // }
                    if (hit)
                    {
                        var obstacle = hit.GetComponent<Design.AStarObstacle>();
                        if (obstacle)
                            cost = obstacle.cost;
                    }
                    Tiles[GetIndex(x, y)] = cost;
                }
            }

            return Tiles;
        }

        [CustomEditor(typeof(AStarGrid))]
        public class MapEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.BeginVertical();
                if (GUILayout.Button("Create Map Data"))
                {
                    var data = ((AStarGrid)target).GenerateGrid();
                    ((AStarGrid)target).Tiles = data;
                    //string str = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                    //Debug.Log(str);

                    //try
                    //{
                    //    if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);
                    //    using (var sw = new StreamWriter(fullPath, false))
                    //    {
                    //        sw.Write(str);
                    //    }
                    //}
                    //catch (System.Exception e)
                    //{
                    //    throw new System.Exception(e.Message);
                    //}

                    //AssetDatabase.Refresh();

                    //mapDataProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + fileNameProp.stringValue);
                }

                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
            }
        }
#endif
    }
}