using UnityEngine;
using UnityEngine.UI;

public class Sheet : MonoBehaviour {
    public float PixelPerSecond;
    public bool TestingWithoutMusic = false;
    public float StartX = 0;

    private float time2pixel(float time) {
        // float original_width = GetComponent<RectTransform>().rect.width;
        // float rendered_width = GetComponent<Image>().sprite.rect.width;
        // float scale_ratio = rendered_width / original_width;
        float scale_ratio = 1f; // 150f / 256f;
        // Debug.Log(GetComponent<RectTransform>().rect);
        // Debug.Log(GetComponent<Image>().sprite.rect);
        // Debug.Log(GetComponent<Image>().sprite.texture.width);
        // Debug.Log(GetComponent<Image>().sprite.texture.height);
        // Debug.Log(time);
        // Debug.Log(PixelPerSecond);
        // Debug.Log(time * PixelPerSecond * rendered_width / original_width);
        // Debug.Log("-----");
        return time * PixelPerSecond * scale_ratio;
    }

    void Start() {
        // GetComponent<RectTransform>().transform.Translate(Vector3.right * PixelPerSecond * CountDownBeforeStart);
    }

    void Update() {
        if (TestingWithoutMusic) {
            GetComponent<RectTransform>().transform.Translate(Vector3.left * time2pixel(Time.deltaTime));
        }
    }

    public void UpdateUsingMusicTime(float musicTime) {
        Transform tf = GetComponent<RectTransform>().transform;
        Vector3 pos = tf.position;
        tf.position = new Vector3(StartX - time2pixel(musicTime), pos.y, pos.z);
    }
}
