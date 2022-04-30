using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DamageManager : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;

    public int attackPower = 1;
    public TextMeshProUGUI damageDisplayText;

    public int DamageCalculation(GameObject[,] cellsList)
    {
        int totalDamage = 0;


        for (int i = 0; i < cellsList.GetLength(0); i++)
        {
            for (int k = 0; k < cellsList.GetLength(1); k++)
            {
                Cell_Battle cell = cellsList[i, k].GetComponent<Cell_Battle>();
                if (cell.viewMode && cell.cellState != CellState.Mine)
                {
                    totalDamage += (int)cell.cellState;
                }
            }
        }
        DamageDisplay(totalDamage);
        return totalDamage;
    }

    public void DamageDisplay(int damage)
    {
        Sequence sequence = DOTween.Sequence();

        damageDisplayText.text = damage + "ダメージ！";
        damageDisplayText.gameObject.SetActive(true);
        Vector3 movePos = damageDisplayText.transform.position + new Vector3(0f, 0f, 0f);
        sequence.Append(damageDisplayText.transform.DOJump(movePos, 10, 1, 0.5f).SetEase(Ease.OutBounce)).AppendInterval(0.8f)
            .AppendCallback(() =>
            {
                damageDisplayText.gameObject.SetActive(false);
                battleManager.NextProgress();
                
            });

    }
}
