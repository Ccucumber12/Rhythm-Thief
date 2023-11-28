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
        //circleRenderer.startColor = new Vector4(1, 1, 1, 1 - scale);
        //circleRenderer.endColor = new Vector4(1, 1, 1, 1 - scale);
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
    public void draw_arrow(int direction)
    {
        float [,] arrows_x = {
            {  0f,   0.5f,  0f, -0.5f },  // up
            {  0f,   - 0.5f, 0f, 0.5f },  // down
            { -1.5f, -0.5f,  1.5f, -0.5f }, // left
            {  1.5f,  0.5f, -1.5f,  0.5f }  // right
        };

        float[,] arrows_y = {
            { 1f,  0.5f, -1f,   0.5f } ,  // up
            { -1f, -0.5f,  1f, -0.5f },  // down
            {  0f,   0.5f,  0f, -0.5f }, // left
            {  0f,   - 0.5f, 0f, 0.5f }  // right
        };

        circleRenderer.positionCount = 6;
        //circleRenderer.startColor = new Vector4(1, 1, 1, 1 - scale);
        //circleRenderer.endColor = new Vector4(1, 1, 1, 1 - scale);
        for (int i = 1; i < 4; i++)
        {
            
            circleRenderer.SetPosition(i * 2 - 2, new Vector3(arrows_x[direction, 0], arrows_y[direction, 0], 0) * 0.6f);
            circleRenderer.SetPosition(i * 2 - 1, new Vector3(arrows_x[direction, i], arrows_y[direction, i], 0) * 0.6f);

        }
    }

    public void set_color(Vector4 color)
    {
        circleRenderer.startColor = color;
        circleRenderer.endColor = color;
    }
}
