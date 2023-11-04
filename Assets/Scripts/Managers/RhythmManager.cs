using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    private static RhythmManager _instance;
    public static RhythmManager Instance { get => _instance; }

    public StageMusicData musicData;
    public float tolerance;

    [Header("Events")]
    public OnLightOnEvent onLightsOn;
    public OnLightOffEvent onLightsOff;
    public OnGateOpenEvent onGateOpen;
    public OnGateCloseEvent onGateClose;

    private AudioSource music;
    private TimestampManager timestamps = new TimestampManager();

    private bool isLightsOnTriggered;
    private bool isLightsOffTriggered;
    private bool isGateOpenTriggered;
    private bool isGateCloseTriggered;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);

        music = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ParseMusicData();
        music.Play();
    }

    private void Update()
    {
        timestamps.UpdateIndex(music.time, tolerance);
        CheckEventInvoke();
    }

    public bool CheckMove()
    {
        return Mathf.Abs(Time.time - timestamps.GetMoveTimestamp()) < tolerance;
    }

    public bool IsBellRinging()
    {
        return timestamps.GetBellRingTimestamp() > timestamps.GetBellStopTimestamp();
    }

    private void ParseMusicData()
    {
        music.clip = musicData.audioClip;
        timestamps.timestamp = JsonUtility.FromJson<Timestamp>(musicData.timestamp.text);
    }

    private void CheckEventInvoke()
    {
        float time = music.time;
        if (time >= timestamps.GetLightsOffTimestamp())
        {
            if (!isLightsOffTriggered)
            {
                onLightsOff.Invoke();
                isLightsOffTriggered = true;
            }
        }
        else
        {
            isLightsOffTriggered = false;
        }

        if (time >= timestamps.GetLightsOnTimestamp())
        {
            if (!isLightsOnTriggered)
            {
                onLightsOn.Invoke();
                isLightsOnTriggered = true;
            }
        }
        else
        {
            isLightsOnTriggered = false;
        }

        if (time >= timestamps.GetGateOpenTimestamp())
        {
            if (!isGateOpenTriggered)
            {
                onGateOpen.Invoke();
                isGateOpenTriggered = true;
            }
        }
        else
        {
            isGateOpenTriggered = false;
        }

        if (time >= timestamps.GetGateCloseTimestamp())
        {
            if (!isGateCloseTriggered)
            {
                onGateClose.Invoke();
                isGateCloseTriggered = true;
            }
        }
        else
        {
            isGateCloseTriggered = false;
        }
    }

    [System.Serializable]
    public class OnGateOpenEvent : UnityEvent { }

    [System.Serializable]
    public class OnGateCloseEvent : UnityEvent { }

    [System.Serializable]
    public class OnLightOffEvent : UnityEvent { }

    [System.Serializable]
    public class OnLightOnEvent : UnityEvent { }
}

[System.Serializable]
public class Timestamp
{
    public List<float> move;
    public List<float> bellRing;
    public List<float> bellStop;
    public List<float> lightsOff;
    public List<float> lightsOn;
    public List<float> gateOpen;
    public List<float> gateClose;
}

public class TimestampManager
{
    public Timestamp timestamp;

    private int moveIndex;
    private int bellRingIndex;
    private int bellStopIndex;
    private int lightsOffIndex;
    private int lightsOnIndex;
    private int gateOpenIndex;
    private int gateCloseIndex;
    
    public void UpdateIndex(float time, float tolerance)
    {
        if (time - GetMoveTimestamp() > tolerance)
            moveIndex += 1;
        if (time - GetBellRingTimestamp() > tolerance)
            bellRingIndex += 1;
        if (time - GetBellStopTimestamp() > tolerance)
            bellStopIndex += 1;
        if (time - GetLightsOffTimestamp() > tolerance)
            lightsOffIndex += 1;
        if (time - GetLightsOnTimestamp() > tolerance)
            lightsOnIndex += 1;
        if (time - GetGateCloseTimestamp() > tolerance)
            gateCloseIndex += 1;
        if (time - GetGateOpenTimestamp() > tolerance)
            gateOpenIndex += 1;
    }

    public float GetMoveTimestamp()
    {
        return moveIndex < timestamp.move.Count ? timestamp.move[moveIndex] : Mathf.Infinity;
    }

    public float GetBellRingTimestamp()
    {
        return bellRingIndex < timestamp.bellRing.Count ? timestamp.bellRing[bellRingIndex] : Mathf.Infinity;
    }

    public float GetBellStopTimestamp()
    {
        return bellStopIndex < timestamp.bellStop.Count ? timestamp.bellStop[bellStopIndex] : Mathf.Infinity;
    }

    public float GetLightsOffTimestamp()
    {
        return lightsOffIndex < timestamp.lightsOff.Count ? timestamp.lightsOff[lightsOffIndex] : Mathf.Infinity;
    }

    public float GetLightsOnTimestamp()
    {
        return lightsOnIndex < timestamp.lightsOn.Count ? timestamp.lightsOn[lightsOnIndex] : Mathf.Infinity;
    }

    public float GetGateOpenTimestamp()
    {
        return gateOpenIndex < timestamp.gateOpen.Count ? timestamp.gateOpen[gateOpenIndex] : Mathf.Infinity;
    }

    public float GetGateCloseTimestamp()
    {
        return gateCloseIndex < timestamp.gateClose.Count ? timestamp.gateClose[gateCloseIndex] : Mathf.Infinity;
    }
}