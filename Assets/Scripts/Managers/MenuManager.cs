using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Stage> initStages;
    private List<Stage> stages;
    private AudioManager audioManager;

    private int _focusIndex = 0;
    private int focusIndex {
        get => _focusIndex;
        set {
            if (value < 0 || value >= stages.Count) return;
            _focusIndex = value;
            updateFocus();
        }
    }

    void Start() {
        audioManager = AudioManager.Instance;

        foreach (Stage stage in initStages) {
            stage.highlight.SetActive(false);
        }
        stages = initStages.FindAll(stage => stage.enabled);
        focusIndex = 0;
    }

    private void updateFocus() {
        if (stages.Count == 0) return;
        for (int i = 0; i < stages.Count; ++i) {
            stages[i].highlight.SetActive(i == focusIndex);
        }
    }

    public void OnChangeFocusMinus(InputValue value) {
        if (focusIndex == 0) audioManager.Play("SelectOut");
        else audioManager.Play("Select");

        focusIndex -= 1;
    }

    public void OnChangeFocusPlus(InputValue value) {
        if (focusIndex == stages.Count-1) audioManager.Play("SelectOut");
        else audioManager.Play("Select");

        focusIndex += 1;
    }

    public void OnReturnToStart(InputValue value)
    {
        GameManager.Instance.UpdateGameState(GameState.Start);
    }

    public void OnSelectStage()
    {
        GameManager.Instance.SelectStage(stages[focusIndex].stageScene);
        audioManager.Play("Click");
    }
}


[System.Serializable]
class Stage {
    public string stageScene;
    public GameObject highlight;
    public bool enabled = true;
}
