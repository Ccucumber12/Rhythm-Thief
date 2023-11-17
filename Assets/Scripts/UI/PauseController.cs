using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    private Canvas canvas;
    private InGameManager inGameManager;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
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
    }

    public void DisablePauseScreen()
    {
        canvas.enabled = false;
    }


}
