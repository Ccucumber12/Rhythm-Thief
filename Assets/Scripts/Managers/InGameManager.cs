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

    public OnPlayerDiedEvent onPlayerDied;
    public OnPlayerReachedGoalEvent onPlayerReachedGoal;
    public OnPlayerCollectStarEvent onPlayerCollectStar;
    public OnGamePausedEvent onGamePaused;
    public OnGameResumedEvent onGameResumed;
    public OnMusicEndedEvent onMusicEnded;

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
        playerInput = Player.Instance.gameObject.GetComponent<PlayerInput>();
    }

    public void PlayerDied()
    {
        //onPlayerDied.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerReachedGoal()
    {
        playerInput.SwitchCurrentActionMap("Ending");
        onPlayerReachedGoal.Invoke();
    }

    public void PlayerCollectStar(int uid)
    {
        onPlayerCollectStar.Invoke(uid);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        playerInput.SwitchCurrentActionMap("PauseGame");
        onGamePaused.Invoke();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        playerInput.SwitchCurrentActionMap("InGame");
        onGameResumed.Invoke();
    }

    public void MusicEnded()
    {
        playerInput.SwitchCurrentActionMap("Ending");
        onMusicEnded.Invoke();
    }

    [System.Serializable]
    public class OnPlayerCollectStarEvent : UnityEvent<int> { }

    [System.Serializable]
    public class OnPlayerDiedEvent : UnityEvent { }

    [System.Serializable]
    public class OnPlayerReachedGoalEvent : UnityEvent { }

    [System.Serializable]
    public class OnGamePausedEvent : UnityEvent { }

    [System.Serializable]
    public class OnGameResumedEvent : UnityEvent { }

    [System.Serializable]
    public class OnMusicEndedEvent : UnityEvent { }
}
