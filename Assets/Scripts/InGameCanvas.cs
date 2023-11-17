using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCanvas : MonoBehaviour
{
    private RhythmManager rhythmManager;
    private Image blackImage;

    private void Awake()
    {
        blackImage = GetComponent<Image>();
    }

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onLightsOff.AddListener(SetLightsOff);
        rhythmManager.onLightsOn.AddListener(SetLightsOn);
    }

    private void OnDestroy()
    {
        rhythmManager.onLightsOff.RemoveListener(SetLightsOff);
        rhythmManager.onLightsOn.RemoveListener(SetLightsOn);
    }

    public void SetLightsOff()
    {
        Color color = blackImage.color;
        color.a = 0.7f;
        blackImage.color = color;
    }

    public void SetLightsOn()
    {
        Color color = blackImage.color;
        color.a = 0;
        blackImage.color = color;
    }
}
