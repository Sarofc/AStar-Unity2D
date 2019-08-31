using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Saro.AStar
{

    [ExecuteInEditMode]
    public class Map : MonoBehaviour
    {
        public Vector2 GridSize = new Vector2(5, 5);
        public Vector2 GridCenter;
        public float NodeSize = 1f;

        public DiagonalMovement MovementType = DiagonalMovement.NotCutCorner;
        public LayerMask checkLayer;
        // public int obstacleProximityPenalty = 10;
        // public Weights[] weights;
        // Dictionary<int, int> lookupWeights = new Dictionary<int, int> ();

        public TileData[,] Tiles;
        public TextAsset mapData;
        public int GridX { get; private set; }
        public int GridY { get; private set; }

        private void Awake()
        {
            GridX = Mathf.RoundToInt(GridSize.x / NodeSize);
            GridY = Mathf.RoundToInt(GridSize.y / NodeSize);

            // GenerateGrid ();

            if (mapData)
            {
                Tiles = Newtonsoft.Json.JsonConvert.DeserializeObject<TileData[,]>(mapData.text);
                if (Tiles.Length != GridX * GridY) throw new System.Exception("Map Data wasn't corrent!");
            }
        }

        // public void SetPenalty (Vector2 worldPosition, int penalty) {
        //     var tile = World2TilePosition (worldPosition);
        //     if (IsInBounds (tile.x, tile.y)) {
        //         Tiles[tile.x, tile.y].Penalty = penalty;
        //     }
        // }

        public Vector2Int World2TilePosition(Vector2 worldPosition)
        {
            float percentX = ((worldPosition.x - GridCenter.x) + GridSize.x / 2) / GridSize.x;
            float percentY = ((worldPosition.y - GridCenter.y) + GridSize.y / 2) / GridSize.y;

            // if ((percentX < 0 || percentX > 1) || (percentY < 0 || percentY > 1)) return default;

            int x = Mathf.RoundToInt((GridX - 1) * percentX);
            int y = Mathf.RoundToInt((GridY - 1) * percentY);

            //  return Tiles[x, y];
            return new Vector2Int(x, y);
        }

        public bool IsWalkableAt(int x, int y)
        {
            return IsInBounds(x, y) && Tiles[x, y].Penalty != -1;
        }

        public bool IsInBounds(int x, int y)
        {
            return (x >= 0 && x < GridX && y >= 0 && y < GridY);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(GridCenter, new Vector3(GridSize.x, GridSize.y, 0));

            if (Tiles != null)
            {
                foreach (var n in Tiles)
                {
                    Gizmos.color = (n.Penalty != -1) ? Color.white : Color.red;
                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, .4f);
                    Gizmos.DrawCube(n.WorldPosition, Vector2.one * NodeSize * .9f);
                }
            }
        }

#if UNITY_EDITOR

        public string fileName = "Scenes/MapData.txt";

        public TileData[,] GenerateGrid()
        {

            GridX = Mathf.RoundToInt(GridSize.x / NodeSize);
            GridY = Mathf.RoundToInt(GridSize.y / NodeSize);

            Tiles = new TileData[GridX, GridY];
            Vector2 worldBottomLeft = GridCenter - Vector2.right * GridSize.x / 2 - Vector2.up * GridSize.y / 2;

            for (int x = 0; x < GridX; x++)
            {
                for (int y = 0; y < GridY; y++)
                {
                    Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * NodeSize + NodeSize / 2f) + Vector2.up * (y * NodeSize + NodeSize / 2f);

                    int movementPenalty = 0;
                    Collider2D hit = Physics2D.OverlapBox(worldPoint, new Vector2(NodeSize * .5f, NodeSize * .5f), 0, checkLayer);
                    // if (hit.gameObject.layer == LayerMask.NameToLayer ("Obstacle")) {
                    // }
                    // else {
                    //     lookupWeights.TryGetValue (hit.gameObject.layer, out movementPenalty);
                    // }
                    if (hit)
                    {
                        movementPenalty = -1;

                    }
                    Tiles[x, y] = new TileData(worldPoint, movementPenalty);
                }
            }

            return Tiles;
        }

        [CustomEditor(typeof(Map))]
        public class MapEditor : Editor
        {

            private SerializedProperty fileNameProp;
            private SerializedProperty mapDataProp;

            private string fullPath;
            private string fullDir;

            private void OnEnable()
            {

                fileNameProp = serializedObject.FindProperty("fileName");
                mapDataProp = serializedObject.FindProperty("mapData");

                fullPath = string.Format("{0}/{1}", Application.dataPath, fileNameProp.stringValue);

                var firstSlash = fullPath.IndexOf('/');
                var lastSlash = fullPath.LastIndexOf('/');

                if (firstSlash != lastSlash) fullDir = fullPath.Substring(0, lastSlash);

                Debug.Log(fullPath);
                Debug.Log(fullDir);
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.BeginVertical();
                if (GUILayout.Button("Create Map Data"))
                {
                    var data = ((Map)target).GenerateGrid();

                    string str = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                    Debug.Log(str);

                    try
                    {
                        if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);
                        using (var sw = new StreamWriter(fullPath, false))
                        {
                            sw.Write(str);
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw new System.Exception(e.Message);
                    }

                    AssetDatabase.Refresh();

                    mapDataProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + fileNameProp.stringValue) as TextAsset;
                }

                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
            }
        }
#endif
    }
}