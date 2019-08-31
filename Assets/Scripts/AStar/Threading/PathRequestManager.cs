using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
// using Grid = Saro.GridGraph;

namespace Saro.AStar
{
    public class PathRequestManager : Singleton<PathRequestManager>
    {

        // public TileProvider provider;

        [SerializeField, Range(1, 16)] private int m_threadCount = 3;
        private PathfindingThread[] m_threads;

        public object QueueLock = new object();
        public object ResultLock = new object();

        private List<PathRequest> m_pendings = new List<PathRequest>();
        private List<PathResult> m_results = new List<PathResult>();

        [Serializable]
        public struct Maps
        {
            public string mapName;
            public Map map;
        }

        [SerializeField] Maps[] m_maps;
        private Dictionary<string, TileProvider> m_TileProviderLookup = new Dictionary<string, TileProvider>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void Awake()
        {
            base.Awake();

            m_TileProviderLookup.Clear();

            foreach (var map in m_maps)
            {
                m_TileProviderLookup[map.mapName] = new CustomTileProvider(map.map);
            }

            // m_map = GetComponent<Map>();

            // provider = new CustomTileProvider(m_map);

            CreateThreads(m_threadCount);
            StartThreads();
        }

        public TileProvider TryGetTileProvider(string name)
        {
            if (name.Equals(string.Empty)) return null;
            m_TileProviderLookup.TryGetValue(name, out TileProvider tileProvider);
            return tileProvider;
        }

        public void Enqueue(PathRequest request)
        {
            if (request == null) return;

            if (!m_pendings.Contains(request)) m_pendings.Add(request);
            else Debug.LogWarning("request was already in queue");
        }

        public void AddResult(PathResult result)
        {
            lock (ResultLock)
            {
                m_results.Add(result);
            }
        }

        private void Update()
        {
            lock (ResultLock)
            {
                foreach (var item in m_results)
                {
                    item.callback?.Invoke(item.result, item.path);
                }
                m_results.Clear();
            }
        }

        private void LateUpdate()
        {
            lock (QueueLock)
            {
                foreach (var request in m_pendings)
                {
                    int lowest = int.MaxValue;
                    PathfindingThread t = null;

                    for (int i = 0; i < m_threads.Length; i++)
                    {
                        if (m_threads[i].Queue.Count < lowest)
                        {
                            lowest = m_threads[i].Queue.Count;
                            t = m_threads[i];
                        }
                    }

                    t.Queue.Enqueue(request);
                }

                m_pendings.Clear();
            }
        }

        private void CreateThreads(int count)
        {
            if (m_threads != null) return;

            m_threads = new PathfindingThread[count];
            for (int i = 0; i < count; i++)
            {
                m_threads[i] = new PathfindingThread(this, i);
            }
        }

        private void StartThreads()
        {
            if (m_threads == null) return;

            for (int i = 0; i < m_threads.Length; i++)
            {
                m_threads[i].StartThread();
            }
        }

        private void StopThreads()
        {
            if (m_threads == null) return;

            for (int i = 0; i < m_threads.Length; i++)
            {
                m_threads[i].StopThread();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            StopThreads();
        }

    }

}