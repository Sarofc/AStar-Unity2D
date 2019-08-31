using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saro.AStar
{
    public class PathRequest
    {

        private static Queue<PathRequest> Pooled = new Queue<PathRequest>();

        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<PathProcessResult, List<Vector2>> callback;
        public TileProvider provider;
        public List<Vector2> ExistingList { get; private set; }

        public static PathRequest New(Vector2 pathStart, Vector2 pathEnd, Action<PathProcessResult, List<Vector2>> callback, TileProvider provider, List<Vector2> existingList)
        {
            PathRequest request;
            if (Pooled.Count > 0)
            {
                request = Pooled.Dequeue();
                request.pathStart = pathStart;
                request.pathEnd = pathEnd;
                request.callback = callback;
                request.provider = provider;
                request.ExistingList = null;
            }
            else
            {
                request = new PathRequest(pathStart, pathEnd, callback, provider, existingList);
            }

            if (PathRequestManager.Instance != null) PathRequestManager.Instance.Enqueue(request);

            return request;
        }

        private PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<PathProcessResult, List<Vector2>> callback, TileProvider provider, List<Vector2> existingList)
        {

            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callback = callback;
            this.provider = provider;
            this.ExistingList = existingList;
        }

        public void Dispose()
        {
            callback = null;
            if (!Pooled.Contains(this)) Pooled.Enqueue(this);
        }
    }
}