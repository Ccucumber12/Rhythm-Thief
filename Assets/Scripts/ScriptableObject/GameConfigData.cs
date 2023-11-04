using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;


[CreateAssetMenu(fileName = "GameConfigData", menuName = "ScriptableObjects/GameConfigData", order = 10)]
public class GameConfigData : ScriptableObject
{
    [Header("Laser Gate")]
    public float laserGateOpenTweenDuration;
    public float laserGateCloseTweenDuration;

    [Header("Police")]
    public float policeAwareDistance;
    public float policeLookAroundDuration;
}
