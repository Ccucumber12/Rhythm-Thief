using UnityEngine;
using UnityEngine.UIElements;

public class GizmosExtensions
{
    private GizmosExtensions() { }

    /// <summary>
    /// Draws a wire arc.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction">The center direction</param>
    /// <param name="angle">The angle range, in degrees.</param>
    /// <param name="radius"></param>
    /// <param name="steps">How many steps to use to draw the arc.</param>
    public static void DrawWireArc(Vector3 origin, Vector3 direction, float angle, float radius, float steps = 20)
    {
        Vector3 posA = origin;
        Vector3 center = direction.normalized * radius;
        for (int i = 0; i <= steps; i++)
        {
            float currentAngle = angle / steps * i;
            Vector3 posB = origin;
            posB += MathUtils.GetVector3FromVector2(MathUtils.GetRotatedVector2(center, angle / 2 - currentAngle));
            Gizmos.DrawLine(posA, posB);
            posA = posB;
        }
        Gizmos.DrawLine(posA, origin);
    }
}