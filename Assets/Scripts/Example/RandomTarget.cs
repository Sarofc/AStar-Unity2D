using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTarget : MonoBehaviour
{
    public Saro.AStar.Agent agent;

    void Start()
    {
        if (agent == null) agent = GetComponent<Saro.AStar.Agent>();
        if (agent) agent.Init();
    }

    void FixedUpdate()
    {
        if (agent)
        {
            if (agent.IsDone)
            {
                agent.Destination = new Vector2(Random.Range(0, 50), Random.Range(0, 30));
            }

            agent.Tick();
        }

    }
}
