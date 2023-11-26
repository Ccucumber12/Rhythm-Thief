using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Stage> initStages;
    private List<Stage> stages;

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
        focusIndex -= 1;
    }

    public void OnChangeFocusPlus(InputValue value) {
        focusIndex += 1;
    }

    public void OnReturnToStart(InputValue value)
    {
        GameManager.Instance.UpdateGameState(GameState.Start);
    }

    public void OnSelectStage()
    {
        GameManager.Instance.SelectStage(stages[focusIndex].stageScene.name);
    }
}


[System.Serializable]
class Stage {
    public SceneAsset stageScene;
    public GameObject highlight;
    public bool enabled = true;
}
