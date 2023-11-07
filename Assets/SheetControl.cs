using UnityEngine;
using UnityEngine.UI;

public class SheetControl : MonoBehaviour
{
    public float PixelPerSecond = 128f;
    public bool TestingWithoutMusic = false;
    public float StartX = 0;
    public GameObject sheet1;
    public GameObject sheet2;

    private float time2pixel(float time)
    {
        // float original_width = GetComponent<RectTransform>().rect.width;
        // float rendered_width = GetComponent<Image>().sprite.rect.width;
        // float scale_ratio = rendered_width / original_width;
        float scale_ratio = 150f / 256f;
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

    void Start()
    {
        // GetComponent<RectTransform>().transform.Translate(Vector3.right * PixelPerSecond * CountDownBeforeStart);
    }

    void Update()
    {
        if (TestingWithoutMusic)
        {
            sheet1.GetComponent<RectTransform>().transform.Translate(Vector3.left * time2pixel(Time.deltaTime));
            sheet2.GetComponent<RectTransform>().transform.Translate(Vector3.left * time2pixel(Time.deltaTime));
        }
    }

    public void UpdateUsingMusicTime(float musicTime)
    {
        sheet1.GetComponentInChildren<Sheet>().UpdateUsingMusicTime(musicTime);
        sheet2.GetComponentInChildren<Sheet>().UpdateUsingMusicTime(musicTime);
    }
}
