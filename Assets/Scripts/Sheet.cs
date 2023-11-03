using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheet : MonoBehaviour {
    public float PixelPerSecond;

    void Start() {

    }

    void Update() {
        GetComponent<RectTransform>().transform.Translate(Vector3.left * PixelPerSecond * Time.deltaTime);
    }
}
