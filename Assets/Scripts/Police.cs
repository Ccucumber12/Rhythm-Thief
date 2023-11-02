using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    private const string FieldOfView_Name = "Vision";

    public float policeSpeed;

    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private Player player;

    private Rigidbody2D policeRigidbody;
    private FieldOfView vision;
    private Vector3 initialPosition;
    private bool isDeaf;
    private bool isChasingPlayer;

    private void Awake()
    {
        policeRigidbody = GetComponent<Rigidbody2D>();
        vision = GetComponentInChildren<FieldOfView>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.onPlayerDied.AddListener(ResetState);

        rhythmManager = RhythmManager.Instance;
        rhythmManager.onLightsOff.AddListener(SetPoliceBlind);
        rhythmManager.onLightsOn.AddListener(SetPoliceSighted);

        player = Player.Instance;
        player.onPlayerFired.AddListener(CheckCatchPlayer);

        initialPosition = transform.position;
        vision.SetAngle(-90);
    }

    private void Update()
    {
        if (isChasingPlayer)
        {
            transform.up = (transform.position - player.transform.position);

            Vector3 velocity = (player.transform.position - transform.position).normalized;
            velocity *= policeSpeed;
            policeRigidbody.velocity = velocity;
        }
    }

    private void OnDestroy()
    {
        gameManager.onPlayerDied.RemoveListener(ResetState);

        rhythmManager.onLightsOff.RemoveListener(SetPoliceBlind);
        rhythmManager.onLightsOn.RemoveListener(SetPoliceSighted);

        player.onPlayerFired.RemoveListener(CheckCatchPlayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gameManager.onPlayerDied.Invoke();
        }
    }

    public void SetPoliceBlind()
    {
        vision.SetBlind();
    }

    public void SetPoliceSighted()
    {
        vision.SetSighted();
    }

    public void SetPoliceDeaf()
    {
        isDeaf = true;
    }

    public void SetPoliceHearable()
    {
        isDeaf = false;
    }

    public void CheckCatchPlayer()
    {
        if (!isDeaf)
        {
            isChasingPlayer = true;
        }
    }

    public void Killed()
    {
        ResetState();
        transform.position = new Vector3(0, 0, -100);
    }

    public void ResetState()
    {
        isChasingPlayer = false;
        policeRigidbody.velocity = Vector3.zero;
        transform.up = Vector3.up;
        transform.position = initialPosition;
    }
}
