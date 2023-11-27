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
    [SerializeField] private SceneAsset startSceneName;
    [SerializeField] private SceneAsset menuSceneName;


    [Header("Events")]
    public OnGameStateChangedEvent onGameStateChanged;

    public GameState state { get; private set; }
    private SceneAsset stageScene;

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
        SceneManager.LoadScene(startSceneName.name);
    }

    private void HandleMenu()
    {
        SceneManager.LoadScene(menuSceneName.name);
    }

    private void HandleInGame()
    {
        SceneManager.LoadScene(stageScene.name);
    }

    /// <summary>
    /// Select the stage scene, no need to call UpdateGameState again.
    /// </summary>
    /// <param name="scene"></param>
    public void SelectStage(SceneAsset scene)
    {
        stageScene = scene;
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
