using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
    public const string Action_Move = "Move";
    public const string Sprite_Name = "Sprite";

    public static Player Instance;
    private static Player instance { get => instance; set => instance = value; }

    [Header("Basic")]
    public float playerSpeed;

    [Header("Fire")]
    public GameObject bulletPrefab;

    public Vector2 playerDirection { get; private set; }

    private PlayerInput playerInput;
    private Rigidbody2D playerRigidbody;
    private InputAction moveAction;
    private GameObject playerSprite;

    private Vector2 moveInputValue;
    private Vector3 playerSpawnPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        moveAction = playerInput.actions[Action_Move];
        playerSprite = transform.Find(Sprite_Name).gameObject;
    }

    private void Start()
    {
        playerSpawnPosition = transform.position;
        playerDirection = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        moveInputValue = moveAction.ReadValue<Vector2>();
        if (moveInputValue.magnitude > 0.01)
        {
            playerDirection = moveInputValue.normalized;
            UpdateSpriteDirection();
        }

    }   

    private void FixedUpdate()
    {
        playerRigidbody.velocity = moveInputValue * playerSpeed;
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            Quaternion quaternion = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, playerDirection));
            Debug.Log(quaternion.eulerAngles);
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
}
