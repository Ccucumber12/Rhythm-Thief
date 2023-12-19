using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageItemButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public StageItem stageItem;

    public void OnSelect(BaseEventData eventData)
    {
        stageItem.OnSelected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        stageItem.OnDeselected();
    }
}
