using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public const string Sprite_Name = "Sprite";

    private static Player _instance;
    public static Player Instance { get => _instance; }

    [Header("Basic")]
    [SerializeField] private Vector2 facingDirection;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float inputFailedCoolDown;
    [SerializeField] private LayerMask wallLayer;

    [Header("Fire")]
    [SerializeField] private float fireCoolDown;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;

    [Header("Events")]
    public OnPlayerAlertEvent onPlayerAlert;

    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private GameObject playerSprite;

    private Vector3 playerSpawnPosition;
    private float lastInputFailedTime;
    private float lastFireTime;
    private Tween moveTween;
    private Animator animator;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);

        playerSprite = transform.Find(Sprite_Name).gameObject;
        animator = playerSprite.GetComponent<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        rhythmManager = RhythmManager.Instance;
        gameManager.onPlayerDied.AddListener(RespawnPlayer);
        gameManager.onPlayerSucceeded.AddListener(RespawnPlayer);

        playerSpawnPosition = transform.position;
        lastInputFailedTime = -inputFailedCoolDown;
        lastFireTime = -fireCoolDown;
    }

    private void OnDestroy()
    {
        gameManager.onPlayerDied.RemoveListener(RespawnPlayer);
        gameManager.onPlayerSucceeded.RemoveListener(RespawnPlayer);
    }

    public void OnUpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryMove(Vector2.up);
    }

    public void OnDownInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryMove(Vector2.down);
    }

    public void OnLeftInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryMove(Vector2.left);
    }

    public void OnRightInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryMove(Vector2.right);
    }

    private void AnimationStop()
    {
        animator.SetBool("Walking", false);
    }

    private void TryMove(Vector2 direction)
    {
        if (Time.time < lastInputFailedTime + inputFailedCoolDown)
            return;
        animator.SetFloat("X", direction[0]);
        animator.SetFloat("Y", direction[1]);
        if (rhythmManager.CheckMove() || true) // TODO
        {
            facingDirection = direction;
            Vector3 newPosition = transform.position + MathUtils.GetVector3FromVector2(direction);
            Collider2D cols = Physics2D.OverlapCircle(newPosition, 0.1f, wallLayer);
            if (cols == null)
            {
                // move success
                animator.SetBool("Walking", true);
                moveTween?.Kill(complete: true);
                moveTween = transform.DOMove(newPosition, 0.1f).OnComplete(AnimationStop);
                UpdateSpriteDirection();
            }
            else
            {
                // Bumped into wall
            }
        }
        else
        {
            lastInputFailedTime = Time.time;
        }
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (Time.time < lastFireTime + fireCoolDown)
                return;

            lastFireTime = Time.time;
            if (!rhythmManager.IsBellRinging())
                onPlayerAlert.Invoke();

            Quaternion quaternion = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, facingDirection));
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, quaternion);
            bullet.GetComponent<Bullet>().bulletDirection = facingDirection;
        }
    }

    public void RespawnPlayer()
    {
        // Temporary method
        moveTween?.Kill(complete: true);
        transform.position = playerSpawnPosition;
    }

    private void UpdateSpriteDirection()
    {
        // Vector3 scale = playerSprite.transform.localScale;
        // scale.x = Mathf.Abs(scale.x) * Mathf.Sign(facingDirection.x);
        // playerSprite.transform.localScale = scale;
    }

    [System.Serializable]
    public class OnPlayerAlertEvent : UnityEvent { }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bulletSpawnPosition.position, 0.15f);
    }
#endif
}
