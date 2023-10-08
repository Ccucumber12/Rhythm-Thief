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

    public float playerSpeed;

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
    }

    private void Update()
    {
        moveInputValue = moveAction.ReadValue<Vector2>();
        if (Mathf.Abs(moveInputValue.x) > 0.01)
            UpdateSpriteDirection();
    }   

    private void FixedUpdate()
    {
        playerRigidbody.velocity = moveInputValue * playerSpeed;
    }

    public int GetDirection()
    {
        return moveInputValue.x >= 0 ? 1 : -1;
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {

    }

    public void RespawnPlayer()
    {
        // Temporary method
        transform.position = playerSpawnPosition;
    }

    private void UpdateSpriteDirection()
    {
        Vector3 scale = playerSprite.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * GetDirection();
        playerSprite.transform.localScale = scale;
    }
}
