using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Saro.AStar
{

    public class PathfindingThread
    {
        private const int IDLE_SLEEP = 5;

        public Thread thread;
        public int ThreadNumber;

        public bool Run { get; private set; }

        public Queue<PathRequest> Queue = new Queue<PathRequest>();
        public Pathfinding pathfinding;
        public long LatestTime;
        public float AproximateWork;
        public long TimeThisSecond;

        private PathRequestManager m_pathRequestMgr;
        private System.Diagnostics.Stopwatch watch;
        private System.Diagnostics.Stopwatch Watch2rd;

        public PathfindingThread(PathRequestManager mgr, int number)
        {
            this.ThreadNumber = number;
            this.m_pathRequestMgr = mgr;
            this.pathfinding = new Pathfinding();
            this.watch = new System.Diagnostics.Stopwatch();
            this.Watch2rd = new System.Diagnostics.Stopwatch();
        }

        public void StartThread()
        {
            if (Run == true) return;

            Run = true;
            thread = new Thread(new ParameterizedThreadStart(RunThread));
            thread.Start(ThreadNumber);
        }

        public void StopThread()
        {
            Run = false;
        }

        private void RunThread(object v)
        {
            int number = (int)v;

            Debug.Log("Started pathfinding thread #" + number);

            Watch2rd.Start();

            while (Run)
            {
                try
                {
                    if (Watch2rd.ElapsedMilliseconds >= 1000)
                    {
                        Watch2rd.Reset();
                        Watch2rd.Start();
                        AproximateWork = Mathf.Clamp01(TimeThisSecond / 1000f);
                        TimeThisSecond = 0;
                    }

                    int count = Queue.Count;
                    if (count == 0) Thread.Sleep(IDLE_SLEEP);
                    else
                    {
                        PathRequest request;
                        lock (m_pathRequestMgr.QueueLock)
                        {
                            request = Queue.Dequeue();
                        }

                        if (request == null) continue;

                        if (request.callback == null) continue;

                        watch.Reset();
                        watch.Start();
                        var result = pathfinding.FindPath(request.pathStart, request.pathEnd, request.provider/*  m_pathRequestMgr.provider */, request.ExistingList, out List<Vector2> path);
                        watch.Stop();
                        LatestTime = watch.ElapsedMilliseconds;
                        TimeThisSecond += LatestTime;

                        m_pathRequestMgr.AddResult(new PathResult(result, path, request.callback));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Execption in pathfinding thread #" + number);
                    Debug.LogError(e);
                }
            }

            Watch2rd.Stop();
            Debug.Log("Stopped pathfinding thread #" + number);
        }
    }
}