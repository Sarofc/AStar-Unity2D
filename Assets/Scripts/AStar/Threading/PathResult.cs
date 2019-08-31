using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saro.AStar
{
    public class PathResult
    {
        public PathProcessResult result;
        public List<Vector2> path;
        public Action<PathProcessResult, List<Vector2>> callback;

        public PathResult(PathProcessResult reault, List<Vector2> path, Action<PathProcessResult, List<Vector2>> callback)
        {
            this.result = reault;
            this.path = path;
            this.callback = callback;
        }
    }
}