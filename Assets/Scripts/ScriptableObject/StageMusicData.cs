using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageMusicData", menuName = "ScriptableObjects/StageMusicData", order = 1)]
public class StageMusicData : ScriptableObject
{
    public AudioClip audioClip;
    public TextAsset timestamp;
}