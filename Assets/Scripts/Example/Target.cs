using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Saro.AStar.Agent agent;
    public Transform target;

    void Start()
    {
        if (agent == null) agent = GetComponent<Saro.AStar.Agent>();
        if (agent) agent.Init();
    }

    void Update()
    {
        if (target != null)
        {
            agent.Destination = target.position;
        }

        // tick agent
        //if (agent) agent.Tick();
    }
}
