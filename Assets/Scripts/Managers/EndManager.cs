using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public string startScene;
    
    public void OnReturnInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(startScene);
        }
    }
}
