using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    [Header("Scene Names")]
    [SerializeField] private string homeSceneName;


    [Header("Events")]
    public OnGameStateChangedEvent onGameStateChanged;

    public GameState state { get; private set; }
    private string stageScene;

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
        stageScene = SceneManager.GetActiveScene().name;
        state = GameState.Home;
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (state)
        {
            case GameState.Home:
                HandleHome();
                break;
            case GameState.StageSelect:
                HandleSelectStage();
                break;
            case GameState.InGame:
                HandleInGame();
                break;
        }
        onGameStateChanged.Invoke(state);
    }

    private void HandleHome()
    {
        if (SceneManager.GetActiveScene().name != homeSceneName)
            SceneManager.LoadScene(homeSceneName);
    }

    private void HandleSelectStage()
    {
        if (SceneManager.GetActiveScene().name != homeSceneName)
            SceneManager.LoadScene(homeSceneName);
        // StartManager should handle select stage canvas
    }

    private void HandleInGame()
    {
           SceneManager.LoadScene(stageScene);
    }

    /// <summary>
    /// Select the stage scene, no need to call UpdateGameState again.
    /// </summary>
    /// <param name="scene"></param>
    public void SelectStage(string scene)
    {
        stageScene = scene;
        UpdateGameState(GameState.InGame);
    }

    [System.Serializable]
    public class OnGameStateChangedEvent : UnityEvent<GameState> { }
}

public enum GameState
{
    Home,
    StageSelect,
    InGame,
    Victory,
    Lose,
}
