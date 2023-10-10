using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Police : MonoBehaviour
{
    private const string AlertSector_Name = "AlertSector";

    public float policeSpeed;

    private GameManager gameManager;
    private RhythmManager rhythmManager;
    private Player player;

    private Rigidbody2D policeRigidbody;
    private GameObject alertSector;
    private Vector3 initialPosition;
    private bool isDeaf;
    private bool isChasingPlayer;

    private void Awake()
    {
        policeRigidbody = GetComponent<Rigidbody2D>();
        alertSector = transform.Find(AlertSector_Name).gameObject;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.onPlayerDied.AddListener(ResetState);

        rhythmManager = RhythmManager.Instance;
        rhythmManager.onLightsOff.AddListener(SetPoliceBlind);
        rhythmManager.onLightsOn.AddListener(SetPoliceSighted);
        rhythmManager.onBellRing.AddListener(SetPoliceDeaf);
        rhythmManager.onBellStop.AddListener(SetPoliceHearable);

        player = Player.Instance;
        player.onPlayerFired.AddListener(CheckCatchPlayer);

        initialPosition = transform.position;
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
        rhythmManager.onBellRing.RemoveListener(SetPoliceDeaf);
        rhythmManager.onBellStop.RemoveListener(SetPoliceHearable);

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
        alertSector.SetActive(false);
    }

    public void SetPoliceSighted()
    {
        alertSector.SetActive(true);
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
