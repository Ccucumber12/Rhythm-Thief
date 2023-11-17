using UnityEngine;
using UnityEngine.InputSystem;

public class SelectStageManager : MonoBehaviour
{
    public string stageSceneName;

    public void OnReturnToStart(InputValue value)
    {
        GameManager.Instance.UpdateGameState(GameState.Start);
    }

    public void OnSelectStage()
    {
        GameManager.Instance.SelectStage(stageSceneName);
    }
}
