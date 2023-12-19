using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject homeCanvas;
    public GameObject stageCanvas;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject tutorialPanel;
    public GameObject creditPanel;

    [Header("Buttons")]
    public Button startGameButton;
    public Button tutorialButton;
    public Button creditButton;
    public Button creditFirstSelected;
    public Button stageFirstSelected;

    private PlayerInput playerInput;
    private GameManager gameManager;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager.state == GameState.StageSelect)
            OpenStage();
    }

    public void OpenTutorial()
    {
        menuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
        playerInput.enabled = true;
        playerInput.SwitchCurrentActionMap("Tutorial");
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        menuPanel.SetActive(true);
        playerInput.enabled = false;
        tutorialButton.Select();
    }

    public void OpenCredit()
    {
        menuPanel.SetActive(false);
        creditPanel.SetActive(true);
        playerInput.enabled = true;
        playerInput.SwitchCurrentActionMap("Credit");
        creditFirstSelected.Select();
    }

    public void CloseCredit()
    {
        creditPanel.SetActive(false);
        menuPanel.SetActive(true);
        playerInput.enabled = false;
        creditButton.Select();
    }

    public void OpenStage()
    {
        gameManager.UpdateGameState(GameState.StageSelect);
        homeCanvas.SetActive(false);
        stageCanvas.SetActive(true);
        playerInput.enabled = true;
        playerInput.SwitchCurrentActionMap("StageSelect");
        stageFirstSelected.Select();
    }

    public void CloseStage()
    {
        gameManager.UpdateGameState(GameState.Home);
        homeCanvas.SetActive(true);
        stageCanvas.SetActive(false);
        playerInput.enabled = false;
        startGameButton.Select();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit Game!");
#endif
        Application.Quit();
    }
}
