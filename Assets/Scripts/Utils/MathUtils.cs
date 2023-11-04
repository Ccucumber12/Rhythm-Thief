using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MathUtils
{
    /// <summary>
    /// Returns a Vector3 on the x-y surface that angle from (1, 0, 0) to itself is <paramref name="angle"/>.
    /// <paramref name="angle"/> should be specified in degrees [0, 360).
    /// </summary>
    /// <param name="angle"> is the desired angle in degrees [0, 360).</param>
    /// <returns>The desired Vector3.</returns>
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float rad = Mathf.Deg2Rad * angle;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }

    /// <summary>
    /// Returns the angle from (1, 0, 0) to <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>angle in degrees.</returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        return Vector3.SignedAngle(Vector3.right, vector, Vector3.forward);
    }

    /// <summary>
    /// Returns the angle from (1, 0) to <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>angle in degrees.</returns>
    public static float GetAngleFromVector(Vector2 vector)
    {
        return Vector3.SignedAngle(Vector3.right, vector, Vector3.forward);
    }

    public static Vector3 GetVector3FromVector2(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector2 GetVector2FromVector3(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2 GetRotatedVector2(Vector2 vector, float angle)
    {
        return GetVectorFromAngle(GetAngleFromVector(vector) + angle).normalized * vector.magnitude;
    }
}
