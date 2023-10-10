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
        while(true)
        {
            for (var i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(1.87f);
                onLightsOff.Invoke();
                yield return new WaitForSeconds(1.88f);
                onLightsOn.Invoke();
            }
            for (var i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(1.87f);
                onGateOpen.Invoke();
                yield return new WaitForSeconds(1.88f);
                onGateClose.Invoke();
            }
            yield return new WaitForSeconds(0.46f);
            for (var i = 0; i < 4; i++)
            {
                onBellRing.Invoke();
                yield return new WaitForSeconds(1.88f);
                onBellStop.Invoke();
                yield return new WaitForSeconds(1.87f);
            }
            yield return new WaitForSeconds(7.04f);
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
