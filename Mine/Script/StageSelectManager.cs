using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] GameManager_Adventure gameManager;
    [SerializeField] BattleManager battleManager;
    public bool clickPermission = true;

    private void Start()
    {

    }

    public void StageSelectOnClick(StageData data)
    {
        clickPermission = false;
        battleManager.SetBattleData(data.battleEventList, data.stageBackgroundImage);
        gameManager.StageStart();
    }
}
