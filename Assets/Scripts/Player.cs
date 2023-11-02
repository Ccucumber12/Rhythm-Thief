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
    [SerializeField] private GameObject bulletPrefab;

    public OnPlayerFiredEvent onPlayerFired;
    public Vector2 playerDirection { get; private set; }

    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private PlayerInput playerInput;
    private Rigidbody2D playerRigidbody;
    private InputAction moveAction;
    private GameObject playerSprite;

    private Vector2 moveInputValue;
    private Vector3 playerSpawnPosition;
    private float lastInputTime;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);

        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSprite = transform.Find(Sprite_Name).gameObject;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        rhythmManager = RhythmManager.Instance;
        gameManager.onPlayerDied.AddListener(RespawnPlayer);

        playerSpawnPosition = transform.position;
        playerDirection = transform.rotation.eulerAngles;
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
        if (rhythmManager.CheckMove())
            PlayerMove(direction);
    }

    private void PlayerMove(Vector2 direction)
    {
        playerDirection = direction;
        transform.position += new Vector3(direction.x, direction.y, 0);
        UpdateSpriteDirection();
    }


    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            onPlayerFired.Invoke();

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
    public class OnPlayerFiredEvent : UnityEvent { }
}
