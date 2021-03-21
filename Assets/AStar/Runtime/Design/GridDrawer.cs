using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saro.AStar.Design
{
    public class GridDrawer : MonoBehaviour
    {
        public AStarGrid grid;

        private void OnDrawGizmos()
        {
            if (grid)
            {
                grid.DrawGrid();
            }
        }
    }

}