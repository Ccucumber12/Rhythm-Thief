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
        inGameManager.onPlayerReachedGoal.AddListener(ShowVictory);
        inGameManager.onMusicEnded.AddListener(ShowFaiulre);
    }

    private void OnDestroy()
    {
        inGameManager.onPlayerReachedGoal.RemoveListener(ShowVictory);
        inGameManager.onMusicEnded.RemoveListener(ShowFaiulre);
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
