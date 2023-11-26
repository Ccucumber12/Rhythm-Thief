using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public SceneAsset scene;

    public void OnReturnInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(scene.name);
        }
    }
}
