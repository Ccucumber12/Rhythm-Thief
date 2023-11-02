using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MathUtils
{
    /// <summary>
    /// Returns a Vector3 on the x-y surface that has an angle of <paramref name="angle"/> to (1, 0, 0).
    /// <paramref name="angle"/> should be specified in degrees [0, 360).
    /// </summary>
    /// <param name="angle"> is the desired angle in degrees [0, 360).</param>
    /// <returns>The desired Vector3.</returns>
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float rad = Mathf.Deg2Rad * angle;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    /// <summary>
    /// Returns the angle from (1, 0, 0) to <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>angle in degrees.</returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        return Vector3.Angle(vector, Vector3.right);
    }

    public static Vector3 Vector2ToVector3(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector2 Vector3ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }
}
