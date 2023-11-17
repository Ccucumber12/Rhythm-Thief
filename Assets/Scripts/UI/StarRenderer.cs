using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarRenderer : MonoBehaviour
{
    public Sprite star;
    public Sprite emptyStar;

    private Image[] stars;
    private InGameManager inGameManager;

    private void Awake()
    {
        stars = GetComponentsInChildren<Image>();
    }

    private void Start()
    {

        if (stars.Length != 3)
            Debug.LogError("Number of stars under StarRenderer is not equal to 3.");
        foreach (Image img in stars)
            img.sprite = emptyStar;

        inGameManager = InGameManager.Instance;
        inGameManager.onPlayerCollectStar.AddListener(StarCollected);
    }

    private void OnDestroy()
    {
        inGameManager.onPlayerCollectStar.RemoveListener(StarCollected);
    }

    public void StarCollected(int uid)
    {
        stars[uid].sprite = star;
    }
}
