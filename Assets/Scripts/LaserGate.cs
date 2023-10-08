using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaserGate : MonoBehaviour
{
    public float tweenDuration;

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
        SetGateClosed();
        StartCoroutine(TestAnimation());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Temporary
            Player.Instance.RespawnPlayer();
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
        DOTween.To(() => mask.transform.localScale, x => mask.transform.localScale = x, maskLocalScale, tweenDuration);
        DOTween.To(() => boxCollider.size, x => boxCollider.size = x, boxColliderSize, tweenDuration);
        DOTween.To(() => boxCollider.offset, x => boxCollider.offset = x, boxColliderOffset, tweenDuration);
    }

    public void SetGateOpened()
    {
        Vector3 scale = maskLocalScale;
        scale.y = 0;
        DOTween.To(() => mask.transform.localScale, x => mask.transform.localScale = x, scale, tweenDuration);

        Vector2 size = boxColliderSize;
        size.y = 0;
        Vector2 offset = boxColliderOffset;
        offset.y += boxColliderSize.y / 2;
        DOTween.To(() => boxCollider.size, x => boxCollider.size = x, size, tweenDuration);
        DOTween.To(() => boxCollider.offset, x => boxCollider.offset = x, offset, tweenDuration);
    }
}
