using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    public CanvasGroup victory;
    public CanvasGroup failure;

    private InGameManager inGameManager;

    private void Start()
    {
        inGameManager = InGameManager.Instance;
        inGameManager.onGameEnded.AddListener(ShowEndingScene);
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
    }

    public void ShowFaiulre()
    {
        failure.alpha = 1;
    }
}
