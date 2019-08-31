using UnityEngine;

namespace Saro
{
    // TODO fix
    public class Path
    {
        public readonly Vector2[] lookPoints;
        public readonly Line[] turnBoundaries;
        public readonly int finishLineIndex;
        public readonly int slowDownIndex;

        public Path(Vector2[] waypoints, Vector2 startPos, float turnDst, float stoppingDst)
        {
            lookPoints = waypoints;
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            Vector2 prevPoint = startPos;
            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector2 currentPoint = lookPoints[i];
                Vector2 dirToCurrentPoint = (currentPoint - prevPoint).normalized;
                Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;

                turnBoundaries[i] = new Line(turnBoundaryPoint, prevPoint - dirToCurrentPoint * turnDst);
                prevPoint = turnBoundaryPoint;
            }

            float dstFromEndPoint = 0;
            for (int i = lookPoints.Length - 1; i > 0; i--)
            {
                dstFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
                if (dstFromEndPoint > stoppingDst)
                {
                    slowDownIndex = i;
                    break;
                }
            }
        }

        public void DrawGizmos(float size = .5f, bool drawLine = false)
        {

            foreach (var p in lookPoints)
            {
                Gizmos.color = new Color(0, 1, 0, .5f);
                Gizmos.DrawCube(p, Vector2.one * size);
            }

            if (!drawLine) return;

            Gizmos.color = Color.white;
            foreach (var line in turnBoundaries)
            {
                line.DrawWithGizmos(.2f);
            }
        }
    }
}