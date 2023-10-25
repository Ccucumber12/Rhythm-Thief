using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaserGate : MonoBehaviour
{
    public float openTweenDuration;
    public float closeTweenDuration;

    private GameManager gameManager;
    private RhythmManager rhythmManager;

    private SpriteMask mask;
    private BoxCollider2D boxCollider;

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
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        rhythmManager = RhythmManager.Instance;
        rhythmManager.onGateClose.AddListener(SetGateClosed);
        rhythmManager.onGateOpen.AddListener(SetGateOpened);

        SetGateClosed();
        //StartCoroutine(TestAnimation());
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
            gameManager.onPlayerDied.Invoke();
        }
    }

    private IEnumerator TestAnimation()
    {
        while(true)
        {
            yield return new WaitForSeconds(3);
            SetGateOpened();
            yield return new WaitForSeconds(3);
            SetGateClosed();
        }
    }

    public void SetGateClosed()
    {
        DOTween.To(() => mask.transform.localScale, x => mask.transform.localScale = x, maskLocalScale, openTweenDuration);
        DOTween.To(() => boxCollider.size, x => boxCollider.size = x, boxColliderSize, openTweenDuration);
        DOTween.To(() => boxCollider.offset, x => boxCollider.offset = x, boxColliderOffset, openTweenDuration);
    }

    public void SetGateOpened()
    {
        Vector3 scale = maskLocalScale;
        scale.y = 0;
        DOTween.To(() => mask.transform.localScale, x => mask.transform.localScale = x, scale, openTweenDuration);

        Vector2 size = boxColliderSize;
        size.y = 0;
        Vector2 offset = boxColliderOffset;
        offset.y += boxColliderSize.y / 2;
        DOTween.To(() => boxCollider.size, x => boxCollider.size = x, size, openTweenDuration);
        DOTween.To(() => boxCollider.offset, x => boxCollider.offset = x, offset, openTweenDuration);
    }
}