using UnityEngine;
using UnityEngine.InputSystem;

public class StartManager : MonoBehaviour
{
    public void OnEnterGame(InputValue value)
    {
        GameManager.Instance.UpdateGameState(GameState.Menu);
    }
}
