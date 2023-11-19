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
    private int remainingTime;


    private void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        musicLength = rhythmManager.GetMusicLength();
        remainingTime = Mathf.FloorToInt(musicLength);
    }

    private void Update()
    {
        remainingTime = Mathf.Min(remainingTime, Mathf.FloorToInt(musicLength - rhythmManager.GetMusicCurrentTime()));
        timeText.text = timeTextPrefix + remainingTime.ToString("D3");
    }


}
