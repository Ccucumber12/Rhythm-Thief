using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Processors;

public class RhythmManager : MonoBehaviour
{
    private static RhythmManager _instance;
    public static RhythmManager Instance { get => _instance; }

    public StageMusicData musicData;
    public float tolerance;
    public float moveDeadZoneTime;
    // If input time is in [time-tolerance, time+tolerance], than succeeds.
    // If input time is in [time-tolerance-moveDeadZone, time-tolerance), than the next move fails.

    public GameObject SheetObject;

    [Header("Events")]
    public OnLightOnEvent onLightsOn;
    public OnLightOffEvent onLightsOff;
    public OnGateOpenEvent onGateOpen;
    public OnGateCloseEvent onGateClose;

    private AudioSource music;
    private TimestampManager timestamps = new TimestampManager();

    private InGameManager inGameManager;
    private Coroutine waitUntilMusicFinishedCoroutine;

    private float usedMoveTime = -1;

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
        inGameManager = InGameManager.Instance;
        inGameManager.onGamePaused.AddListener(PauseMusic);
        inGameManager.onGameResumed.AddListener(ResumeMusic);
        inGameManager.onGameEnded.AddListener(EndMusic);

        ParseMusicData();
        music.Play();
        waitUntilMusicFinishedCoroutine = StartCoroutine(WaitUntilMusicFinished());
    }

    private void OnDestroy()
    {
        inGameManager.onGamePaused.RemoveListener(PauseMusic);
        inGameManager.onGameResumed.RemoveListener(ResumeMusic);
        inGameManager.onGameEnded.RemoveListener(EndMusic);
        if (waitUntilMusicFinishedCoroutine != null)
            StopCoroutine(waitUntilMusicFinishedCoroutine);
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
            Debug.LogError("SheetObject is not set in RhythmManager.");
        }
    }

    public bool CheckMove()
    {
        float time = music.time;
        float nextMoveTime = timestamps.GetNextMoveTimestamp();
        float prevMoveTime = timestamps.GetPrevMoveTimestamp();
        if (usedMoveTime < prevMoveTime && time - prevMoveTime <= tolerance)
        {
            usedMoveTime = prevMoveTime;
            SheetObject.GetComponentInChildren<Sheet20>().HittedUsingHitTime(prevMoveTime);
            return true;
        }
        else if (usedMoveTime < nextMoveTime && nextMoveTime - time <= tolerance)
        {
            usedMoveTime = nextMoveTime;
            SheetObject.GetComponentInChildren<Sheet20>().HittedUsingHitTime(nextMoveTime);
            return true;
        }
        else if (usedMoveTime < nextMoveTime && nextMoveTime - time < tolerance + moveDeadZoneTime)
        {
            // next move failed (too early)
            usedMoveTime = nextMoveTime;
        }
        return false;
    }

    public bool IsBellRinging()
    {
        return timestamps.GetNextBellRingTimestamp() > timestamps.GetNextBellStopTimestamp();
    }

    public float GetMusicLength()
    {
        return musicData.audioClip.length;
    }

    public float GetMusicCurrentTime()
    {
        return music.time;
    }

    private void ParseMusicData()
    {
        music.clip = musicData.audioClip;
        timestamps.ParseFromJSON(musicData.timestamp.text);
    }

    private IEnumerator WaitUntilMusicFinished()
    {
        yield return new WaitUntil(() => music.time > 0);
        yield return new WaitUntil(() => music.time == 0);

        Player.Instance.OutOfTime();
    }

    public void PauseMusic()
    {
        music.Pause();
    }

    public void ResumeMusic()
    {
        music.UnPause();
    }

    public void EndMusic()
    {
        if (waitUntilMusicFinishedCoroutine != null)
            StopCoroutine(waitUntilMusicFinishedCoroutine);
        music.Stop();
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
