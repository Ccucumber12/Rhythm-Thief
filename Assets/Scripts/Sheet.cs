using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheet : MonoBehaviour {
    public float PixelPerSecond;
    public float CountDownBeforeStart;

    void Start() {
        GetComponent<RectTransform>().transform.Translate(Vector3.right * PixelPerSecond * CountDownBeforeStart);
    }

    void Update() {
        GetComponent<RectTransform>().transform.Translate(Vector3.left * PixelPerSecond * Time.deltaTime);
    }
}
