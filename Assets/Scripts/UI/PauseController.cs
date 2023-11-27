using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    private Canvas canvas;
    private InGameManager inGameManager;

    private int _focus;
    public int focus {
        get {return _focus;}
        set {
            if (value < 0 || value >= 3) return;
            _focus = value;
            updateFocus();
        }
    }

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        DisablePauseScreen();
    }

    private void Start()
    {
        inGameManager = InGameManager.Instance;
        inGameManager.onGamePaused.AddListener(EnablePauseScreen);
        inGameManager.onGameResumed.AddListener(DisablePauseScreen);
    }

    private void OnDestroy()
    {
        inGameManager.onGamePaused.RemoveListener(EnablePauseScreen);
        inGameManager.onGameResumed.RemoveListener(DisablePauseScreen);
    }

    public void EnablePauseScreen()
    {
        canvas.enabled = true;
        focus = 0;
    }

    public void DisablePauseScreen()
    {
        canvas.enabled = false;
    }

    public void FocusIncr() {
        focus += 1;
    }

    public void FocusDecr() {
        focus -= 1;
    }

    void updateFocus() {
        Button focusedButton = (focus == 0) ? resumeButton
                             : (focus == 1) ? restartButton
                             : (focus == 2) ? returnButton
                             : null;
        Button[] buttons = {resumeButton, restartButton, returnButton};
        foreach (Button button in buttons) {
            button.GetComponent<Image>().color = (button == focusedButton) ? Color.gray : Color.white;
        }
    }

    public void OnChangeFocusMinus(InputValue value) {
        FocusDecr();
    }

    public void OnChangeFocusPlus(InputValue value) {
        FocusIncr();
    }

    public void OnSelectFocusedOption(InputValue value) {
        switch (focus) {
        case 0:
            inGameManager.ResumeGame();
            break;
        case 1:
            GameManager.Instance.UpdateGameState(GameState.InGame);
            break;
        case 2:
            GameManager.Instance.UpdateGameState(GameState.Menu);
            break;
        }
    }
}
