using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell_Battle : MonoBehaviour, IPointerClickHandler
{
    public Battle_Minesweeper battleManager;
    public CellState cellState = CellState.None;

    public AudioManager audioManager;

    //自身の配列番号
    public int numberA;
    public int numberB;
    public bool flagMode = false;

    [SerializeField]
    private Text _view = null;

    public bool viewMode = false;

    [SerializeField] GameObject attackEffect;
    public Canvas canvas;

    private void OnValidate()
    {
        OnCellStateChanged();
    }

    public void OnCellStateChanged()
    {
        if (_view == null || !viewMode) { return; }


        Color tempColor = gameObject.GetComponent<Image>().color;
        tempColor = Color.gray;
        if (cellState == CellState.Mine)
        {
            gameObject.GetComponent<Image>().color = new Color(1, 0.5f, 0.5f);
        }
        else
        {
            gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
        }

        if (cellState == CellState.None)
        {
            _view.text = "";
        }
        else if (cellState == CellState.Mine)
        {
            _view.text = "X";
            _view.color = new Color(1f, 0.8f, 0.8f);
            battleManager.touchMine = true;
        }
        else
        {
            _view.text = ((int)cellState).ToString();
            _view.color = new Color(1f, 1f, 1f);
        }
    }

    public void LeftClickAction()
    {
        viewMode = true;

        OnCellStateChanged();
        if (cellState == CellState.Mine)
        {
            return;
        }
        battleManager.checkedCell++;

        //自身の周囲の番号が0なら、周囲のLeftClickActionも呼び出す。

        if (cellState == CellState.None)
        {
            for (int i = numberA - 1; i <= numberA + 1; i++)
            {
                if (i < 0 || i >= battleManager.gameScale)
                {
                    continue;
                }

                for (int k = numberB - 1; k <= numberB + 1; k++)
                {
                    if (k < 0 || k >= battleManager.gameScale)
                    {
                        continue;
                    }

                    if (battleManager.cellArray[i, k].GetComponent<Cell_Battle>().viewMode == false)
                    {
                        battleManager.cellArray[i, k].GetComponent<Cell_Battle>().LeftClickAction();
                    }
                }
            }

        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (battleManager.pause)
        {
            return;
        }

        switch (eventData.pointerId)
        {
            case -1: //左クリック
                Debug.Log("Left Click");

                audioManager.PlaySeBlockClick();

                if (!battleManager.touchedFirstCell)
                {
                    battleManager.touchedFirstCell = true;
                    if (cellState == CellState.Mine)
                    {
                        battleManager.numbersArray[numberA, numberB] = 0;

                        battleManager.AddMine(numberA, numberB);

                        int gameScale = battleManager.gameScale;
                        for (int i = 0; i < gameScale; i++)
                        {
                            for (int k = 0; k < gameScale; k++)
                            {

                                if (/*(i == 0 && k == 0) || */battleManager.numbersArray[i, k] == -1)
                                {
                                    continue;
                                }


                                battleManager.SearchMine(i, k);

                            }
                        }
                    }
                }
                if (flagMode)
                {
                    Debug.Log("フラグが立っているのでクリックできません");
                    break;
                }
                LeftClickAction();
                break;

            case -2: //右クリック
                Debug.Log("Right Click");
                RightClick();
                break;
        }
    }

    public void RightClick()
    {
        if (flagMode)
        {
            flagMode = false;
            gameObject.GetComponent<Image>().color = new Color(1, 1f, 1f);
        }
        else if (viewMode == false)
        {
            flagMode = true;
            gameObject.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f);
        }
    }

    /// <summary>
    /// 攻撃エフェクトを生成する
    /// </summary>
    public void CreateAttackEffect()
    {
        if (viewMode)
        {
            if ((cellState != CellState.Mine) && (cellState != CellState.None))
            {
                GameObject effect = Instantiate(attackEffect);
                effect.gameObject.transform.parent = canvas.gameObject.transform;
                effect.transform.position = this.transform.position;
                //effect.transform.localPosition = new Vector3(0,0,0);
            }
        }
    }
}
