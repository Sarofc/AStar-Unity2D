using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public GameObject pawn;
    public int count = 5;
    private Saro.AStar.TileProvider tileProvider;
    private Saro.AStar.Agent agent;

    private void Start()
    {
        if (pawn != null)
        {
            agent = pawn.GetComponent<Saro.AStar.Agent>();
            tileProvider = Saro.AStar.PathRequestManager.Instance.TryGetTileProvider(agent.key_provider);
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(30, 30, 120, 50), "Spawn Agents"))
        {
            for (int i = 0; i < count; i++)
            {
                GameObject.Instantiate(pawn, this.transform);
            }
        }
    }
}
