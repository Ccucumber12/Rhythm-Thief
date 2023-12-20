using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class LaserGate : MonoBehaviour
{
    [Required]
    public GameConfigData gameConfigData;
    public GameObject gate_minimap;

    private float openTweenDuration;
    private float closeTweenDuration;

    private RhythmManager rhythmManager;

    private SpriteMask mask;
    private BoxCollider2D boxCollider;
    private NavMeshObstacle navMeshObstacle;

    private Vector3 maskLocalScale;
    private Vector2 boxColliderSize;
    private Vector2 boxColliderOffset;

    private void Awake()
    {
        mask = GetComponentInChildren<SpriteMask>();
        maskLocalScale = mask.transform.localScale;

        boxCollider = GetComponent<BoxCollider2D>();
        boxColliderSize = boxCollider.size;
        boxColliderOffset = boxCollider.offset;

        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.size = MathUtils.GetVector3FromVector2(boxColliderSize) + new Vector3(0, 0, 1);
        navMeshObstacle.center = boxColliderOffset;
    }

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onGateClose.AddListener(SetGateClosed);
        rhythmManager.onGateOpen.AddListener(SetGateOpened);

        openTweenDuration = gameConfigData.laserGateOpenTweenDuration;
        closeTweenDuration = gameConfigData.laserGateCloseTweenDuration;

        SetGateClosed();
    }

    private void OnDestroy()
    {
        rhythmManager.onGateClose.RemoveListener(SetGateClosed);
        rhythmManager.onGateOpen.RemoveListener(SetGateOpened);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.Instance.KilledByLaserGate();
        }
    }

    public void SetGateClosed()
    {
        gate_minimap.SetActive(true);
        DOTween.To(() => mask.transform.localScale, x => mask.transform.localScale = x, maskLocalScale, closeTweenDuration);
        DOTween.To(() => boxCollider.size, x => {
            boxCollider.size = x;
            navMeshObstacle.size = MathUtils.GetVector3FromVector2(x) + new Vector3(0, 0, 1);
        }, boxColliderSize, closeTweenDuration);
        DOTween.To(() => boxCollider.offset, x => {
            boxCollider.offset = x;
            navMeshObstacle.center = x;
        }, boxColliderOffset, closeTweenDuration);
    }

    public void SetGateOpened()
    {
        Vector3 scale = maskLocalScale;
        scale.y = 0;
        DOTween.To(() => mask.transform.localScale, x => mask.transform.localScale = x, scale, openTweenDuration);
        gate_minimap.SetActive(false);
        Vector2 size = boxColliderSize;
        size.y = 0;
        Vector2 offset = boxColliderOffset;
        offset.y += boxColliderSize.y / 2;
        DOTween.To(() => boxCollider.size, x => {
            boxCollider.size = x;
            navMeshObstacle.size = MathUtils.GetVector3FromVector2(x) + new Vector3(0, 0, 1);
        }, size, openTweenDuration);
        DOTween.To(() => boxCollider.offset, x => {
            boxCollider.offset = x;
            navMeshObstacle.center = x;
        }, offset, openTweenDuration);
    }
}
