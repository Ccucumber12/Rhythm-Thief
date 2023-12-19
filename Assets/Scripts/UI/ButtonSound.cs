using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button button;
    private AudioManager audioManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    public void OnSelect(BaseEventData eventData)
    {
    }

    public void OnDeselect(BaseEventData eventData)
    {
        audioManager?.Play("Select");
    }

    public void OnClick()
    {
        audioManager?.Play("Click");
    }
}
