using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private InGameManager inGameManager;

    private void Start()
    {
        inGameManager = InGameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerReachedGoal();
        }
    }

    private void PlayerReachedGoal()
    {
        inGameManager.PlayerReachedGoal();
    }
}
