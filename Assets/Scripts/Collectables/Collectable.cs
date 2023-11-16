using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Collectable : MonoBehaviour
{
    [Header("Floating Animation")]
    public float floatingDistance;
    public float floatingPeriod;

    private Vector3 initialPosition;
    protected InGameManager inGameManager;

    protected virtual void Start()
    {
        inGameManager = InGameManager.Instance;
        initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        transform.position = initialPosition + new Vector3(0, floatingDistance / 2 * Mathf.Sin(Time.time / floatingPeriod * 2 * Mathf.PI), 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Collected();
        }
    }

    protected virtual void Collected()
    {
        Destroy(gameObject, 0.05f);
    }
}
