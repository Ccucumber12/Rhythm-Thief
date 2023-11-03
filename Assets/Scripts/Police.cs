using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Police : MonoBehaviour
{
    [Required] public GameConfigData gameConfigData;
    public Vector2 facingDirection;

    private float alertDuration;
    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private Player player;

    private NavMeshAgent agent;
    private FieldOfView vision;
    private Vector3 initialPosition;
    private Vector2 initialFacingDirection;
    private bool isAlert;

    private Coroutine delaySetNormalCoroutine;

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
        player.onPlayerAlert.AddListener(SetAlert);

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        alertDuration = gameConfigData.policAlertDuration;
        initialPosition = transform.position;
        initialFacingDirection = facingDirection;
        vision.SetAngle(facingDirection);
    }

    private void Update()
    {
        if (isAlert)
        {
            if (IsAgentReachedDestination())
            {
                isAlert = false;
                vision.SetNormal();
                LookAround();
            }
        }
        if (agent.velocity.magnitude > 0.01f)
            facingDirection = agent.velocity.normalized;
        vision.SetAngle(facingDirection);
        Debug.DrawLine(transform.position, agent.steeringTarget);
    }

    private void OnDestroy()
    {
        gameManager.onPlayerDied.RemoveListener(ResetState);
        rhythmManager.onLightsOff.RemoveListener(SetBlind);
        rhythmManager.onLightsOn.RemoveListener(SetSighted);
        player.onPlayerAlert.RemoveListener(SetAlert);
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

    public void SetAlert()
    {
        agent.isStopped = true;
        agent.ResetPath();
        agent.SetDestination(player.transform.position);
        isAlert = true;
        vision.SetAlert();
    }

    public void SetNormal()
    {
        isAlert = false;
        vision.SetNormal();
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
        // TODO
    }

    public void Killed()
    {
        ResetState();
        transform.position = new Vector3(0, 0, -100);
    }

    public void ResetState()
    {
        if (delaySetNormalCoroutine != null)
        {
            StopCoroutine(delaySetNormalCoroutine);
            SetNormal();
        }
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
