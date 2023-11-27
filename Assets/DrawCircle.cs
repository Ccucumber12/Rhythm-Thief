using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public LineRenderer circleRenderer;
    public float centerX;
    public float centerY;
    public float radiusX; // ratio
    public float radiusY; // ratio
    public float width;
    // Start is called before the first frame update
    void Start()
    {
        draw_circle(100, 1f);
        circleRenderer.startWidth = width;
        circleRenderer.endWidth = width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void draw_circle(int steps, float scale)
    {
        circleRenderer.positionCount = steps;
        circleRenderer.startColor = new Vector4(1, 1, 1, 1 - scale);
        circleRenderer.endColor = new Vector4(1, 1, 1, 1 - scale);
        for (int i = 0; i < steps; i++)
        {
            float degree = (float)i / steps;
            float radian = degree * 2 * Mathf.PI;

            float x = Mathf.Cos(radian) * radiusX * scale;
            float y = Mathf.Sin(radian) * radiusY * scale;

            Vector3 point = new Vector3(x + centerX, y + centerY, 0);
            circleRenderer.SetPosition(i, point);

        }
    }
}
