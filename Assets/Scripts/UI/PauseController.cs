using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    private Canvas canvas;
    private InGameManager inGameManager;
    private AudioManager audioManager;
    private PlayerInput input;

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
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        inGameManager = InGameManager.Instance;
        inGameManager.onGamePaused.AddListener(EnablePauseScreen);
        inGameManager.onGameResumed.AddListener(DisablePauseScreen);
        DisablePauseScreen();

        audioManager = AudioManager.Instance;

        // setup callback
        resumeButton.onClick.AddListener(() => inGameManager.ResumeGame());
        restartButton.onClick.AddListener(() => GameManager.Instance.UpdateGameState(GameState.InGame));
        returnButton.onClick.AddListener(() => GameManager.Instance.UpdateGameState(GameState.StageSelect));
    }

    private void OnDestroy()
    {
        inGameManager.onGamePaused.RemoveListener(EnablePauseScreen);
        inGameManager.onGameResumed.RemoveListener(DisablePauseScreen);
    }

    public void EnablePauseScreen()
    {
        input.enabled = true;
        canvas.enabled = true;
        focus = 0;
    }

    public void DisablePauseScreen()
    {
        input.enabled = false;
        canvas.enabled = false;
    }

    public void FocusIncr() {
        if (focus == 2) audioManager.Play("SelectOut");
        else audioManager.Play("Select");

        focus += 1;
    }

    public void FocusDecr() {
        if (focus == 0) audioManager.Play("SelectOut");
        else audioManager.Play("Select");

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
        Button[] buttons = {resumeButton, restartButton, returnButton};
        buttons[focus].onClick.Invoke();

        audioManager.Play("Click");
    }

    public void OnReturnToGame(InputValue value)
    {
        inGameManager.ResumeGame();
    }
}
