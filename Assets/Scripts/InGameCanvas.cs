using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    private RhythmManager rhythmManager;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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
        canvasGroup.alpha = 1;
    }

    public void SetLightsOn()
    {
        canvasGroup.alpha = 0;
    }
}
