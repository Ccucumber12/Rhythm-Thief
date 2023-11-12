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
        if (time > timestamps.GetNextMoveTimestamp())
            timestamps.IncrMoveIndex();
        if (time > timestamps.GetNextBellRingTimestamp())
            timestamps.IncrBellRingIndex();
        if (time > timestamps.GetNextBellStopTimestamp())
            timestamps.IncrBellStopIndex();
        if (time > timestamps.GetNextLightsOffTimestamp())
        {
            timestamps.IncrLightsOffIndex();
            onLightsOff.Invoke();
        }
        if (time > timestamps.GetNextLightsOnTimestamp())
        {
            timestamps.IncrLightsOnIndex();
            onLightsOn.Invoke();
        }
        if (time > timestamps.GetNextGateCloseTimestamp())
        {
            timestamps.IncrGateCloseIndex();
            onGateClose.Invoke();
        }
        if (time > timestamps.GetNextGateOpenTimestamp())
        {
            timestamps.IncrGateOpenIndex();
            onGateOpen.Invoke();
        }

        if (SheetObject != null) {
            SheetObject.GetComponentInChildren<Sheet20>().UpdateUsingMusicTime(music.time);
        } else {
            Debug.LogWarning("SheetObject is not set in RhythmManager.");
        }
    }

    public bool CheckMove()
    {
        float time = music.time;
        float nextMoveTime = timestamps.GetNextMoveTimestamp();
        float prevMoveTime = timestamps.GetPrevMoveTimestamp();
        if (Mathf.Abs(time - prevMoveTime) <= tolerance)
        {
            timestamps.ResetNextMoveTimestamp(); // hitted
            SheetObject.GetComponentInChildren<Sheet20>().HittedUsingHitTime(prevMoveTime);
            return true;
        }
        else if (Mathf.Abs(time - nextMoveTime) <= tolerance)
        {
            timestamps.ResetNextMoveTimestamp(); // hitted
            SheetObject.GetComponentInChildren<Sheet20>().HittedUsingHitTime(nextMoveTime);
            return true;
        }
        return false;
    }

    public bool IsBellRinging()
    {
        return timestamps.GetNextBellRingTimestamp() > timestamps.GetNextBellStopTimestamp();
    }

    private void ParseMusicData()
    {
        music.clip = musicData.audioClip;
        timestamps.ParseFromJSON(musicData.timestamp.text);
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
