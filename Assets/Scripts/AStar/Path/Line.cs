using UnityEngine;

namespace Saro
{
    // TODO fix
    public struct Line
    {
        private const float verticalLineGradient = 1e5f;

        private float m_gradient;
        private float m_interceptY;
        private Vector2 m_pointOnLine1;
        private Vector2 m_pointOnLine2;

        private float m_gradientPerpendicular;
        private bool m_approachSide;

        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            float dx = pointOnLine.x - pointPerpendicularToLine.x;
            float dy = pointOnLine.y - pointPerpendicularToLine.y;

            if (dx == 0) m_gradientPerpendicular = verticalLineGradient;
            else m_gradientPerpendicular = dy / dx;

            if (m_gradientPerpendicular == 0) m_gradient = verticalLineGradient;
            else m_gradient = -1 / m_gradientPerpendicular;

            m_interceptY = pointOnLine.y - m_gradient * pointOnLine.x;
            m_pointOnLine1 = pointOnLine;
            m_pointOnLine2 = pointOnLine + new Vector2(1, m_gradient);

            m_approachSide = false;
            m_approachSide = GetSide(pointPerpendicularToLine);
        }

        private bool GetSide(Vector2 p)
        {
            return (p.x - m_pointOnLine1.x) * (m_pointOnLine2.y - m_pointOnLine1.y) >=
                (p.y - m_pointOnLine1.y) * (m_pointOnLine2.x - m_pointOnLine1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != m_approachSide;
        }

        public float DistanceFromPoint(Vector2 p)
        {
            float interceptPerpendicularY = p.y - m_gradientPerpendicular * p.x;
            float intersectX = (interceptPerpendicularY - m_interceptY) / (m_gradient - m_gradientPerpendicular);
            float intersectY = m_gradient * intersectX + m_interceptY;
            return Vector2.Distance(p, new Vector2(intersectX, intersectY));
        }

        public void DrawWithGizmos(float length)
        {
            // perpendicular to line
            Vector2 lineDir = new Vector2(1, m_gradient).normalized;
            Vector2 lineCenter = new Vector2(m_pointOnLine1.x, m_pointOnLine1.y);
            Gizmos.DrawLine(lineCenter - lineDir * length / 2f, lineCenter + lineDir * length / 2f);

        }

    }
}