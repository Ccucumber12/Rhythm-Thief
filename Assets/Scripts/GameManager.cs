using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    public OnPlayerDied onPlayerDied;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);
    }

    public void PlayerDied()
    {
        onPlayerDied.Invoke();
    }

    [System.Serializable]
    public class OnPlayerDied : UnityEvent { }
}
