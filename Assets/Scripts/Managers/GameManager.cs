using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    public OnPlayerDiedEvent onPlayerDied;
    public OnPlayerSucceededEvent onPlayerSucceeded;
    public OnPlayerCollectStarEvent onPlayerCollectStar;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);
    }

    public void PlayerDied()
    {
        //onPlayerDied.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerSucceeded()
    {
        //onPlayerSucceeded.Invoke();
        SceneManager.LoadScene("Win");
    }

    public void PlayerCollectStar()
    {
        onPlayerCollectStar.Invoke();
    }

    [System.Serializable]
    public class OnPlayerCollectStarEvent : UnityEvent { }

    [System.Serializable]
    public class OnPlayerDiedEvent : UnityEvent { }

    [System.Serializable]
    public class OnPlayerSucceededEvent : UnityEvent { }
}
