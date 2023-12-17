using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    public CanvasGroup victory;
    public CanvasGroup failure;

    private InGameManager inGameManager;
    private AudioManager audioManager;

    private void Start()
    {
        inGameManager = InGameManager.Instance;
        inGameManager.onGameEnded.AddListener(ShowEndingScene);

        audioManager = AudioManager.Instance;
    }

    private void OnDestroy()
    {
        inGameManager.onGameEnded.RemoveListener(ShowEndingScene);
    }

    public void ShowEndingScene()
    {
        if (inGameManager.isVictory)
            ShowVictory();
        else
            ShowFaiulre();
    }

    public void ShowVictory()
    {
        victory.alpha = 1;
        audioManager.Play("Win");
    }

    public void ShowFaiulre()
    {
        failure.alpha = 1;
        audioManager.Play("Lose");
    }
}
