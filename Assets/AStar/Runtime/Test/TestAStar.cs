using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saro.AStar.Tests
{
    public class TestAStar : MonoBehaviour
    {
        public AStarGrid grid;

        public Pathfinder pathfinder;

        public Vector2 start, end;

        private List<Vector2> path;

        private void Awake()
        {
            pathfinder = new Pathfinder();

            pathfinder.FindPath(grid, start, end, null, out path);


        }

        private void Update()
        {
            //pathfinder.FindPath(grid, start, end, )
        }

        private void OnDrawGizmos()
        {
            if (path != null)
            {
                Gizmos.color = Color.green;
                foreach (var v in path)
                {
                    Gizmos.DrawCube(v, Vector2.one * 0.4f);
                }
            }
        }
    }
}