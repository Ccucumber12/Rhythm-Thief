using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance { get => _instance; }

    public bool[] playerStarCollection { get; private set; } = new bool[3];
    public int playerDeathCount { get; private set; }
    public bool isPaused { get; private set; }
    public bool isVictory { get; private set; }

    public OnPlayerRespawnEvent onPlayerRespawn;
    public OnPlayerCollectStarEvent onPlayerCollectStar;
    public OnGamePausedEvent onGamePaused;
    public OnGameResumedEvent onGameResumed;
    public OnGameEndedEvent onGameEnded;

    private PlayerInput playerInput;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void PlayerCollectStar(int uid)
    {
        onPlayerCollectStar.Invoke(uid);
    }

    public void SetVictory()
    {
        isVictory = true;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        playerInput.enabled = false;
        onGamePaused.Invoke();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        playerInput.enabled = true;
        onGameResumed.Invoke();
    }

    public void EndGame()
    {
        Invoke("SwitchToEndingInput", 1.0f);
        onGameEnded.Invoke();
    }

    private void SwitchToEndingInput()
    {
        playerInput.SwitchCurrentActionMap("Ending");
    }

    [System.Serializable]
    public class OnPlayerCollectStarEvent : UnityEvent<int> { }

    [System.Serializable]
    public class OnPlayerRespawnEvent : UnityEvent { }

    [System.Serializable]
    public class OnGameEndedEvent : UnityEvent { }

    [System.Serializable]
    public class OnGamePausedEvent : UnityEvent { }

    [System.Serializable]
    public class OnGameResumedEvent : UnityEvent { }
}
