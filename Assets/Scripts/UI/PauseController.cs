using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] Button firstSelectButton;

    private Canvas canvas;
    private InGameManager inGameManager;
    private PlayerInput input;

    private GameManager gameManager;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        inGameManager = InGameManager.Instance;
        inGameManager.onGamePaused.AddListener(EnablePauseScreen);
        inGameManager.onGameResumed.AddListener(DisablePauseScreen);
        DisablePauseScreen();

        gameManager = GameManager.Instance;
    }

    private void OnDestroy()
    {
        inGameManager.onGamePaused.RemoveListener(EnablePauseScreen);
        inGameManager.onGameResumed.RemoveListener(DisablePauseScreen);
    }

    public void GameResume()
    {
        inGameManager.ResumeGame();
    }

    public void GameRestart()
    {
        gameManager.UpdateGameState(GameState.InGame);
    }

    public void GameReturn()
    {
        gameManager.UpdateGameState(GameState.StageSelect);
    }

    public void EnablePauseScreen()
    {
        input.enabled = true;
        canvas.enabled = true;
        firstSelectButton.Select();
    }

    public void DisablePauseScreen()
    {
        input.enabled = false;
        canvas.enabled = false;
    }
}
