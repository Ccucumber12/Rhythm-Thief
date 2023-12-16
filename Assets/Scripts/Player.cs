using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public const string Sprite_Name = "Sprite";

    private static Player _instance;
    public static Player Instance { get => _instance; }

    [Header("Basic")]
    [SerializeField] private Vector2 facingDirection;
    [SerializeField] private float moveCoolDown;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private bool canFreeMove;

    [Header("Fire")]
    [SerializeField] private float fireCoolDown;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;

    [Header("Events")]
    public OnPlayerAlertEvent onPlayerAlert;

    public bool[] isStarCollected { get; private set; } = new bool[3];

    private InGameManager inGameManager;
    private RhythmManager rhythmManager;
    private AudioManager audioManager;
    private GameObject playerSprite;
    private GameObject upperPlayerSprite;

    private Vector3 playerSpawnPosition;
    private float lastMoveTime;
    private float lastFireTime;
    private Tween moveTween;
    private Animator animator;
    private Animator upperAnimator;

    private bool isFreezed;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);

        playerSprite = transform.Find(Sprite_Name).gameObject;
        animator = playerSprite.GetComponent<Animator>();

        upperPlayerSprite = transform.Find("UpperSprite").gameObject;
        upperAnimator = upperPlayerSprite.GetComponent<Animator>();
    }

    private void Start()
    {
        inGameManager = InGameManager.Instance;
        rhythmManager = RhythmManager.Instance;
        audioManager = AudioManager.Instance;
        inGameManager.onPlayerCollectStar.AddListener(StarCollected);

        playerSpawnPosition = transform.position;
        lastMoveTime = -moveCoolDown;
        lastFireTime = -fireCoolDown;
    }

    private void OnDestroy()
    {
        moveTween?.Kill(complete: true);
        inGameManager.onPlayerCollectStar.RemoveListener(StarCollected);
    }

    public void OnUp()
    {
        TryMove(Vector2.up);
    }

    public void OnDown()
    {
        TryMove(Vector2.down);
    }

    public void OnLeft()
    {
        TryMove(Vector2.left);
    }

    public void OnRight()
    {
        TryMove(Vector2.right);
    }

    public void OnFire()
    {
        if (isFreezed || inGameManager.isPaused || Time.time < lastFireTime + fireCoolDown)
            return;

        lastFireTime = Time.time;
        if (!rhythmManager.IsBellRinging())
            onPlayerAlert.Invoke();

        Quaternion quaternion = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, facingDirection));
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, quaternion);
        bullet.GetComponent<Bullet>().bulletDirection = facingDirection;

        audioManager.Play("GunShot");
    }

    public void OnPause()
    {
        inGameManager.PauseGame();

        audioManager.Play("Pause");
    }

    private void AnimationStop()
    {
        animator.SetBool("Walking", false);
        upperAnimator.SetBool("Walking", false);
    }

    private void TryMove(Vector2 direction)
    {
        if (isFreezed || inGameManager.isPaused || Time.time < lastMoveTime + moveCoolDown)
            return;
        lastMoveTime = Time.time;
        animator.SetFloat("X", direction[0]);
        animator.SetFloat("Y", direction[1]);
        upperAnimator.SetFloat("X", direction[0]);
        upperAnimator.SetFloat("Y", direction[1]);
        facingDirection = direction;
        if (canFreeMove || rhythmManager.CheckMove(direction))
        {
            Vector3 newPosition = transform.position + MathUtils.GetVector3FromVector2(direction);
            Collider2D cols = Physics2D.OverlapCircle(newPosition, 0.1f, wallLayer);
            if (cols == null)
            {
                // move success
                animator.SetBool("Walking", true);
                upperAnimator.SetBool("Walking", true);
                moveTween?.Kill(complete: true);
                moveTween = transform.DOMove(newPosition, 0.1f).OnComplete(AnimationStop);
            }
            else
            {
                // Bumped into wall
            }
        }
        else
        {
            // Move failed
        }
    }

    private void RespawnPlayer()
    {
        inGameManager.onPlayerRespawn.Invoke();
        moveTween?.Kill(complete: true);
        transform.position = playerSpawnPosition;
        isFreezed = false;
        animator.SetBool("CutInHalf", false);
        upperAnimator.SetBool("CutInHalf", false);
        upperAnimator.SetBool("Hitted", false);
        animator.SetBool("Hitted", false);
        UnsetInvincible();
    }

    public void SetRespawnPoint(Vector2 position)
    {
        playerSpawnPosition = position;
    }

    public void StarCollected(int uid)
    {
        isStarCollected[uid] = true;
        audioManager.Play("CollectStar");
    }

    public void KilledByPolice()
    {
        isFreezed = true;
        upperAnimator.SetBool("Hitted", true);
        animator.SetBool("Hitted", true);

        SetInvincible();
        // TODO: Killed by police animation
        float animationLength = 1f;
        Invoke("RespawnPlayer", animationLength);

        audioManager.Play("KilledByPolice");
    }

    public void KilledByLaserGate()
    {
        isFreezed = true;
        upperAnimator.SetBool("CutInHalf", true);
        animator.SetBool("CutInHalf", true);

        SetInvincible();
        // TODO: Killed by laser gate animation
        float animationLength = 1.1f;
        Invoke("RespawnPlayer", animationLength);

        audioManager.Play("KilledByLaser");
    }

    public void ReachedGoal()
    {
        inGameManager.SetVictory();
        isFreezed = true;
        SetInvincible();
        // TODO: Reached goal animation
        float animationLength = 1;
        Invoke("CallEndGame", animationLength);
    }

    public void OutOfTime()
    {
        isFreezed = true;
        SetInvincible();
        // TODO: Out of Time animation
        float animationLength = 1;
        Invoke("CallEndGame", animationLength);
    }

    public void CallEndGame()
    {
        inGameManager.EndGame();
    }

    public void SetInvincible()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void UnsetInvincible()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void OnReturnToMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.Menu);
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
