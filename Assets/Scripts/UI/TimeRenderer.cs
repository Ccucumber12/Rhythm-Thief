using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeRenderer : MonoBehaviour
{
    private const string timeTextPrefix = "TIME: ";

    private TextMeshProUGUI timeText;
    private RhythmManager rhythmManager;
    private float musicLength;


    private void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        musicLength = rhythmManager.GetMusicLength();
    }

    private void Update()
    {
        int remainingTime = Mathf.FloorToInt((musicLength - rhythmManager.GetMusicCurrentTime()));
        timeText.text = timeTextPrefix + remainingTime.ToString("D3");
    }


}
