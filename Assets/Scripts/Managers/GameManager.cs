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
    public onPlayerSucceededEvent onPlayerSucceeded;

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

    [System.Serializable]
    public class OnPlayerDiedEvent : UnityEvent { }

    [System.Serializable]
    public class onPlayerSucceededEvent : UnityEvent { }
}
