using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageMusicData", menuName = "ScriptableObjects/StageMusicData", order = 1)]
public class StageMusicData : ScriptableObject
{
    public AudioClip audioClip;
    public TextAsset timestamp;
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
    private Timestamp timestamp;

    private int moveIndex = 0;
    private int bellRingIndex = 0;
    private int bellStopIndex = 0;
    private int lightsOffIndex = 0;
    private int lightsOnIndex = 0;
    private int gateOpenIndex = 0;
    private int gateCloseIndex = 0;

    public void ParseFromJSON(string text) {
        timestamp = JsonUtility.FromJson<Timestamp>(text);
    }

    public void IncrMoveIndex() {
        moveIndex += 1;
    }

    public void IncrBellRingIndex() {
        bellRingIndex += 1;
    }

    public void IncrBellStopIndex() {
        bellStopIndex += 1;
    }

    public void IncrLightsOffIndex() {
        lightsOffIndex += 1;
    }

    public void IncrLightsOnIndex() {
        lightsOnIndex += 1;
    }

    public void IncrGateOpenIndex() {
        gateOpenIndex += 1;
    }

    public void IncrGateCloseIndex() {
        gateCloseIndex += 1;
    }

    public float GetNextMoveTimestamp() {
        return moveIndex < timestamp.move.Count ? timestamp.move[moveIndex] : Mathf.Infinity;
    }

    public float GetNextBellRingTimestamp() {
        return bellRingIndex < timestamp.bellRing.Count ? timestamp.bellRing[bellRingIndex] : Mathf.Infinity;
    }

    public float GetNextBellStopTimestamp() {
        return bellStopIndex < timestamp.bellStop.Count ? timestamp.bellStop[bellStopIndex] : Mathf.Infinity;
    }

    public float GetNextLightsOffTimestamp() {
        return lightsOffIndex < timestamp.lightsOff.Count ? timestamp.lightsOff[lightsOffIndex] : Mathf.Infinity;
    }

    public float GetNextLightsOnTimestamp() {
        return lightsOnIndex < timestamp.lightsOn.Count ? timestamp.lightsOn[lightsOnIndex] : Mathf.Infinity;
    }

    public float GetNextGateOpenTimestamp() {
        return gateOpenIndex < timestamp.gateOpen.Count ? timestamp.gateOpen[gateOpenIndex] : Mathf.Infinity;
    }

    public float GetNextGateCloseTimestamp() {
        return gateCloseIndex < timestamp.gateClose.Count ? timestamp.gateClose[gateCloseIndex] : Mathf.Infinity;
    }

    public float GetPrevMoveTimestamp() {
        return (moveIndex - 1) < 0 ? -Mathf.Infinity : timestamp.move[moveIndex - 1];
    }

    public float GetPrevBellRingTimestamp() {
        return (bellRingIndex - 1) < 0 ? -Mathf.Infinity : timestamp.bellRing[bellRingIndex - 1];
    }

    public float GetPrevBellStopTimestamp() {
        return (bellStopIndex - 1) < 0 ? -Mathf.Infinity : timestamp.bellStop[bellStopIndex - 1];
    }

    public float GetPrevLightsOffTimestamp() {
        return (lightsOffIndex - 1) < 0 ? -Mathf.Infinity : timestamp.lightsOff[lightsOffIndex - 1];
    }

    public float GetPrevLightsOnTimestamp() {
        return (lightsOnIndex - 1) < 0 ? -Mathf.Infinity : timestamp.lightsOn[lightsOnIndex - 1];
    }

    public float GetPrevGateOpenTimestamp() {
        return (gateOpenIndex - 1) < 0 ? -Mathf.Infinity : timestamp.gateOpen[gateOpenIndex - 1];
    }

    public float GetPrevGateCloseTimestamp() {
        return (gateCloseIndex - 1) < 0 ? -Mathf.Infinity : timestamp.gateClose[gateCloseIndex - 1];
    }
}
