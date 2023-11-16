using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance { get => _instance; }

    public bool[] playerStarCollection { get; private set; } = new bool[3];
    public int playerDeathCount { get; private set; }

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
        SceneManager.LoadScene("End");
    }

    public void PlayerCollectStar(int uid)
    {
        onPlayerCollectStar.Invoke(uid);
    }

    [System.Serializable]
    public class OnPlayerCollectStarEvent : UnityEvent<int> { }

    [System.Serializable]
    public class OnPlayerDiedEvent : UnityEvent { }

    [System.Serializable]
    public class OnPlayerSucceededEvent : UnityEvent { }
}
