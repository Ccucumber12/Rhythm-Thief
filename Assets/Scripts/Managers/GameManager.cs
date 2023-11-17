using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    [Header("Scene Names")]
    [SerializeField] private string startSceneName = "Start";
    [SerializeField] private string MenuSceneName = "Menu";


    [Header("Events")]
    public OnGameStateChangedEvent onGameStateChanged;

    public GameState state { get; private set; }
    private string stageSceneName;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        state = GameState.Start;
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (state)
        {
            case GameState.Start:
                HandleStart();
                break;
            case GameState.Menu:
                HandleMenu();
                break;
            case GameState.InGame: 
                HandleInGame();
                break;
        }
        onGameStateChanged.Invoke(state);
    }

    private void HandleStart()
    {
        SceneManager.LoadScene(startSceneName);
    }

    private void HandleMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    private void HandleInGame()
    {
        SceneManager.LoadScene(stageSceneName);
    }

    /// <summary>
    /// Select the stage scene, no need to call UpdateGameState again.
    /// </summary>
    /// <param name="sceneName"></param>
    public void SelectStage(string sceneName)
    {
        stageSceneName = sceneName;
        UpdateGameState(GameState.InGame);
    }

    [System.Serializable]
    public class OnGameStateChangedEvent : UnityEvent<GameState> { }
}

public enum GameState
{
    Start,
    Menu,
    InGame,
    Victory,
    Lose,
}