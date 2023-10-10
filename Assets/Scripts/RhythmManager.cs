using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    private static RhythmManager _instance;
    public static RhythmManager Instance { get => _instance; }

    public OnGateCloseEvent onGateClose;
    public OnGateOpenEvent onGateOpen;
    public OnLightOffEvent onLightsOff;
    public OnLightOnEvent onLightsOn;
    public OnBellRingEvent onBellRing;
    public OnBellStopEvent onBellStop;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
        StartCoroutine(DemoStage());
    }

    private IEnumerator DemoStage()
    {
        var waitInterval = 2;
        while(true)
        {
            onGateOpen.Invoke();
            yield return new WaitForSeconds(waitInterval);
            onGateClose.Invoke();
            yield return new WaitForSeconds(waitInterval);

            onLightsOff.Invoke();
            yield return new WaitForSeconds(waitInterval);
            onLightsOn.Invoke();
            yield return new WaitForSeconds(waitInterval);

            onBellRing.Invoke();
            Debug.Log("Bell Ring!");
            yield return new WaitForSeconds(waitInterval);
            Debug.Log("Bell Stop!");
            onBellStop.Invoke();
            yield return new WaitForSeconds(waitInterval);
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

    [System.Serializable]
    public class OnBellRingEvent : UnityEvent { }

    [System.Serializable]
    public class OnBellStopEvent : UnityEvent { }
}
