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

    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private FieldOfView vision;
    private Vector3 initialPosition;
    private Vector2 initialFacingDirection;
    private bool isAlert;

    private Coroutine delaySetNormalCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
            agent.SetDestination(player.transform.position);
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
        isAlert = true;
        vision.SetAlert();
        delaySetNormalCoroutine = StartCoroutine(DelayedSetNormal(alertDuration));
    }

    public void SetNormal()
    {
        isAlert = false;
        vision.SetNormal();
    }

    private IEnumerator DelayedSetNormal(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetNormal();
    }

    public void Killed()
    {
        ResetState();
        transform.position = new Vector3(0, 0, -100);
    }

    public void ResetState()
    {
        agent.enabled = false;
        if (delaySetNormalCoroutine != null)
        {
            StopCoroutine(delaySetNormalCoroutine);
            SetNormal();
        }
        rb.velocity = Vector3.zero;
        transform.up = Vector3.up;
        transform.position = initialPosition;
        facingDirection = initialFacingDirection;
        agent.enabled = true;
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
