//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Saro.AStar
//{
//    [RequireComponent(typeof(Rigidbody2D))]
//    public class Agent : MonoBehaviour
//    {
//        public string key_provider;
//        public float speed = 3f;
//        public float stoppingDistance = 1f;
//        public float turnDistance = 1f;

//        public Vector3 Destination
//        {
//            set
//            {

//                if (currentRequest == null &&
//                    ((m_lastRequestPostion - value).sqrMagnitude > m_moveThreshold * m_moveThreshold
//                    /* || ((Vector3) m_rigid.position - value).sqrMagnitude > m_moveThreshold * m_moveThreshold */)
//                )
//                {

//                    m_lastRequestPostion = value;

//                    currentRequest = PathRequest.New(m_rigid.position, value, OnPathFound, tileProvider, m_currentPath);

//                    IsDone = false;
//                }
//            }
//            get
//            {
//                return m_lastRequestPostion;
//            }
//        }
//        public bool IsDone { get; protected set; }
//        public Vector3 Velocity { get { return m_rigid.velocity; } }

//        public Rigidbody2D Rigidbody { get => m_rigid; }

//        protected TileProvider tileProvider;
//        protected List<Vector2> m_currentPath;
//        protected Path m_path;

//        protected Vector3 m_desition;
//        protected Vector3 m_lastRequestPostion;
//        [SerializeField] protected float m_moveThreshold = 1f;
//        protected Rigidbody2D m_rigid;
//        private PathRequest currentRequest;

//        private int pathIndex = 0;
//        private float speedPercent = 1;

//        public virtual void Init()
//        {
//            m_rigid = GetComponent<Rigidbody2D>();
//            tileProvider = PathRequestManager.Get().TryGetTileProvider(key_provider);
//            m_lastRequestPostion = m_rigid.position;
//            IsDone = true;
//        }

//        public virtual void Tick()
//        {
//            Move();
//        }

//        public void SetDestination(Vector3 position)
//        {
//            Destination = position;
//        }

//        public void Warp(Vector3 position)
//        {
//            m_rigid.position = position;
//            m_lastRequestPostion = position;
//        }

//        public void Stopped()
//        {
//            IsDone = true;
//        }

//        private void OnPathFound(EPathProcessResult result, List<Vector2> path)
//        {
//            if (currentRequest != null)
//            {
//                currentRequest.Dispose();
//                currentRequest = null;
//            }

//            if (result == EPathProcessResult.Success)
//            {
//                m_path = new Path(path.ToArray(), m_rigid.position, turnDistance, stoppingDistance);
//                pathIndex = 1;
//            }
//            else
//            {
//                IsDone = true;

//                Debug.LogWarning(result);
//            }
//        }

//        private float dis;
//        private Vector2 dir;
//        protected void Move()
//        {

//            if (IsDone)
//            {
//                m_rigid.velocity = Vector2.zero;
//                m_path = null;
//                return;
//            }

//            if (m_path == null) return;

//            while (m_path.turnBoundaries[pathIndex].HasCrossedLine(m_rigid.position))
//            {
//                if (pathIndex == m_path.finishLineIndex)
//                {
//                    pathIndex = 0;
//                    IsDone = true;
//                    break;
//                }
//                else
//                {
//                    pathIndex++;
//                }
//            }

//            if (!IsDone)
//            {
//                dis = m_path.turnBoundaries[m_path.finishLineIndex].DistanceFromPoint(m_rigid.position);
//                //if (pathIndex >= m_path.slowDownIndex && stoppingDistance > 0)
//                //{
//                //    speedPercent = Mathf.Clamp01(dis / stoppingDistance);
//                //}

//                dir = (m_path.lookPoints[pathIndex] - m_rigid.position).normalized;

//                m_rigid.velocity = dir * speed * speedPercent;

//                if (dis <= stoppingDistance)
//                {
//                    IsDone = true;
//                    m_path = null;
//                }
//            }
//        }

//#if UNITY_EDITOR
//        private void OnDrawGizmosSelected()
//        {
//            if (m_path != null)
//            {
//                m_path.DrawGizmos(.5f, true);
//            }
//            Gizmos.color = Color.red;
//            Gizmos.DrawWireSphere(new Vector2(Destination.x, Destination.y), .5f);
//        }
//#endif
//    }
//}