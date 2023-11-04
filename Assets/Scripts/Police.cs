using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;


public class Police : MonoBehaviour
{
    [Required] public GameConfigData gameConfigData;
    public Vector2 facingDirection;

    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private Player player;

    private NavMeshAgent agent;
    private FieldOfView vision;
    private Vector3 initialPosition;
    private Vector2 initialFacingDirection;
    private bool isAlert;
    private float awareDistance;

    private Vector3 previousVelocity;
    private bool isTurningLeft;

    private Tween lookAroundTween;
    private float lookAroundDuration;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponentInChildren<FieldOfView>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.onPlayerDied.AddListener(ResetState);

        rhythmManager = RhythmManager.Instance;
        rhythmManager.onLightsOff.AddListener(SetBlind);
        rhythmManager.onLightsOn.AddListener(SetSighted);

        player = Player.Instance;
        player.onPlayerAlert.AddListener(HeardPlayer);

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        initialPosition = transform.position;
        initialFacingDirection = facingDirection;
        vision.SetAngle(facingDirection);

        awareDistance = gameConfigData.policeAwareDistance;
        lookAroundDuration = gameConfigData.policeLookAroundDuration;
    }

    private void Update()
    {
        if (isAlert)
        {
            if (IsAgentReachedDestination())
            {
                SetNormal();
                LookAround();
            }
        }
        if (agent.velocity.magnitude > 0.01f)
        {
            facingDirection = agent.velocity.normalized;
            Vector3 crossProduct = Vector3.Cross(previousVelocity.normalized, agent.velocity.normalized);
            if (Mathf.Abs(crossProduct.z) > 0.01f)
                isTurningLeft = crossProduct.z > 0;
            previousVelocity = agent.velocity;
        }
        vision.SetAngle(facingDirection);
        Debug.DrawLine(transform.position, agent.steeringTarget);
    }

    private void OnDestroy()
    {
        gameManager.onPlayerDied.RemoveListener(ResetState);
        rhythmManager.onLightsOff.RemoveListener(SetBlind);
        rhythmManager.onLightsOn.RemoveListener(SetSighted);
        player.onPlayerAlert.RemoveListener(HeardPlayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gameManager.onPlayerDied.Invoke();
        }
    }

    public void SetBlind()
    {
        vision.SetBlind();
    }

    public void SetSighted()
    {
        vision.SetSighted();
    }

    public void HeardPlayer()
    {
        if ((player.transform.position - transform.position).magnitude <= awareDistance)
            SetAlert();
    }

    public void SetAlert()
    {
        ClearAgentState();
        agent.SetDestination(player.transform.position);
        isAlert = true;
        vision.SetAlert();
    }

    public void SetNormal()
    {
        ClearAgentState();
        isAlert = false;
        vision.SetNormal();
    }

    public void ClearAgentState()
    {
        lookAroundTween?.Kill(complete: true);
        agent.isStopped = true;
        agent.ResetPath();
    }

    private bool IsAgentReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void LookAround()
    {
        float angle = MathUtils.GetAngleFromVector(facingDirection);
        float delta = isTurningLeft ? 360 : -360;
        lookAroundTween = DOTween.To(UpdateFacingDirectionFromFloat, angle, angle + delta, lookAroundDuration)
            .SetEase(Ease.InOutCubic);
    }

    private void UpdateFacingDirectionFromFloat(float degree)
    {
        facingDirection = MathUtils.GetVectorFromAngle(degree);
    }

    public void Killed()
    {
        ResetState();
        transform.position = new Vector3(0, 0, -100);
    }

    public void ResetState()
    {
        SetNormal();
        lookAroundTween?.Kill(complete: true);
        agent.Warp(initialPosition);
        transform.up = Vector3.up;

        transform.position = initialPosition;
        facingDirection = initialFacingDirection;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float radius = 5f;
        FieldOfView v = GetComponentInChildren<FieldOfView>();
        GizmosExtensions.DrawWireArc(v.transform.position, facingDirection, v.viewAngle, radius);
    }
#endif
}
