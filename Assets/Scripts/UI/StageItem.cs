using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageItem : MonoBehaviour
{
    public Image locateIcon;
    public Image target;
    public string stageScene;

    private Animator targetAnimator;

    private void Awake()
    {
        targetAnimator = target.GetComponent<Animator>();
        locateIcon.enabled = false;
        targetAnimator.enabled = false;
    }

    public void OnSelected()
    {
        locateIcon.enabled = true;
        targetAnimator.enabled = true;
    }

    public void OnDeselected()
    {
        locateIcon.enabled = false;
        targetAnimator.enabled = false;
    }

    public void SelectStage()
    {
        GameManager.Instance.SelectStage(stageScene);
    }
}
