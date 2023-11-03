using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
    public const string Sprite_Name = "Sprite";

    private static Player _instance;
    public static Player Instance { get => _instance; }

    [Header("Basic")]
    [SerializeField] private float playerSpeed;
    [SerializeField] private float inputCoolDown;

    [Header("Fire")]
    [SerializeField] private float fireCoolDown;
    [SerializeField] private GameObject bulletPrefab;

    public OnPlayerAlertEvent onPlayerAlert;
    public Vector2 playerDirection { get; private set; }

    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private GameObject playerSprite;

    private Vector3 playerSpawnPosition;
    private float lastInputTime;
    private float lastFireTime;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);

        playerSprite = transform.Find(Sprite_Name).gameObject;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        rhythmManager = RhythmManager.Instance;
        gameManager.onPlayerDied.AddListener(RespawnPlayer);

        playerSpawnPosition = transform.position;
        playerDirection = transform.rotation.eulerAngles;

        lastInputTime = -inputCoolDown;
        lastFireTime = -fireCoolDown;
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {
        gameManager.onPlayerDied.RemoveListener(RespawnPlayer);
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

    private void TryMove(Vector2 direction)
    {
        if (Time.time < lastInputTime + inputCoolDown)
            return;
        lastInputTime = Time.time;
        if (rhythmManager.CheckMove() || true) // TODO
        {
            playerDirection = direction;
            transform.position += new Vector3(direction.x, direction.y, 0);
            UpdateSpriteDirection();
        }
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (Time.time < lastFireTime + fireCoolDown)
                return;

            lastFireTime = Time.time;
            if (!rhythmManager.CheckFire())
                onPlayerAlert.Invoke();

            Quaternion quaternion = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, playerDirection));
            GameObject bullet = Instantiate(bulletPrefab, transform.position, quaternion);
            bullet.GetComponent<Bullet>().bulletDirection = playerDirection;
        }
    }

    public void RespawnPlayer()
    {
        // Temporary method
        transform.position = playerSpawnPosition;
    }

    private void UpdateSpriteDirection()
    {
        Vector3 scale = playerSprite.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(playerDirection.x);
        playerSprite.transform.localScale = scale;
    }

    [System.Serializable]
    public class OnPlayerAlertEvent : UnityEvent { }
}
