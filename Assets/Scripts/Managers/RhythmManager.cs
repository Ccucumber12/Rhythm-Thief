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

    public GameObject SheetObject;

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
        float time = music.time;
        if (time > timestamps.GetMoveTimestamp())
            timestamps.moveIndex += 1;
        if (time > timestamps.GetBellRingTimestamp())
            timestamps.bellRingIndex += 1;
        if (time > timestamps.GetBellStopTimestamp())
            timestamps.bellStopIndex += 1;
        if (time > timestamps.GetLightsOffTimestamp())
        {
            timestamps.lightsOffIndex += 1;
            onLightsOff.Invoke();
        }
        if (time > timestamps.GetLightsOnTimestamp())
        {
            timestamps.lightsOnIndex += 1;
            onLightsOn.Invoke();
        }
        if (time > timestamps.GetGateCloseTimestamp())
        {
            timestamps.gateCloseIndex += 1;
            onGateClose.Invoke();
        }
        if (time > timestamps.GetGateOpenTimestamp())
        {
            timestamps.gateOpenIndex += 1;
            onGateOpen.Invoke();
        }

        if (SheetObject != null) {
            SheetObject.GetComponentInChildren<SheetControl>().UpdateUsingMusicTime(music.time);
        } else {
            Debug.LogWarning("SheetObject is not set in RhythmManager.");
        }
    }

    public bool CheckMove()
    {
        float time = music.time;
        if (timestamps.moveIndex == 0)
            return Mathf.Abs(time - timestamps.timestamp.move[0]) <= tolerance;
        float delta1 = Mathf.Abs(time - timestamps.timestamp.move[timestamps.moveIndex - 1]);
        float delta2 = Mathf.Abs(time - timestamps.GetMoveTimestamp());
        return Mathf.Min(delta1, delta2) <= tolerance;
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

    public int moveIndex;
    public int bellRingIndex;
    public int bellStopIndex;
    public int lightsOffIndex;
    public int lightsOnIndex;
    public int gateOpenIndex;
    public int gateCloseIndex;

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
