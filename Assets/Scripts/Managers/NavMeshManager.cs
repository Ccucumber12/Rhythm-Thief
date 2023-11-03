using NavMeshPlus.Components;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [Required] public GameConfigData gameConfigData;

    private NavMeshSurface navMeshSurface;
    private RhythmManager rhythmManager;

    private float gateOpenTweenDuration;
    private float gateCloseTweenDuration;

    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onGateClose.AddListener(GateCloseUpdate);
        rhythmManager.onGateOpen.AddListener(GateOpenUpdate);

        gateCloseTweenDuration = gameConfigData.laserGateCloseTweenDuration;
        gateOpenTweenDuration = gameConfigData.laserGateOpenTweenDuration;
    }

    private void OnDestroy()
    {
        rhythmManager.onGateClose.RemoveListener(GateCloseUpdate);
        rhythmManager.onGateOpen.RemoveListener(GateOpenUpdate);
    }

    public void GateCloseUpdate()
    {
        Invoke("UpdateNavMeshSurface", gateCloseTweenDuration + 0.05f);
    }

    public void GateOpenUpdate()
    {
        Invoke("UpdateNavMeshSurface", gateOpenTweenDuration + 0.05f);
    }


    private void UpdateNavMeshSurface()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
