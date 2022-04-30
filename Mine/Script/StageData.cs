using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageData : MonoBehaviour, IPointerClickHandler
{
    public StageSelectManager stageSelectManager;
    public Sprite stageBackgroundImage;
    public List<BattleEvent> battleEventList;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (stageSelectManager.clickPermission)
        {
            stageSelectManager.StageSelectOnClick(this);
        }
    }
}
