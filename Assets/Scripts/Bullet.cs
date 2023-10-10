using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletLifespan;

    [HideInInspector] public Vector2 bulletDirection;

    private Rigidbody2D bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        bulletRigidbody.velocity = bulletDirection * bulletSpeed;
        Destroy(gameObject, bulletLifespan);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject, 0.01f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Police")
        {
            Destroy(collision.gameObject, 0.01f);
            Destroy(gameObject, 0.01f);
        }
    }
}
