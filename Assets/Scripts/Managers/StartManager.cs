using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public string stageScene;

    public void OnStartInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(stageScene);
        }
    }
}
