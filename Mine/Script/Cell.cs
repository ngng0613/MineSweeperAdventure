using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public CellsManager manager;

    //自身の配列番号
    public int numberA;
    public int numberB;

    public bool flagMode = false;

    /*
    public enum CellState
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,

        Mine = -1,
    }
    */


    [SerializeField]
    private CellState _cellState = CellState.None;
    public CellState CellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }


    [SerializeField]
    private Text _view = null;

    public bool viewMode = false;


    private void OnValidate()
    {
        OnCellStateChanged();
    }

    private void Update()
    {

    }
    private void OnCellStateChanged()
    {
        if (_view == null || !viewMode) { return; }


        Color tempColor = gameObject.GetComponent<Image>().color;
        tempColor = Color.gray;
        if (_cellState == CellState.Mine)
        {
            gameObject.GetComponent<Image>().color = new Color(1, 0.3f, 0.3f);
        }
        else
        {
            gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
        }



        if (_cellState == CellState.None)
        {
            _view.text = "";
        }
        else if (_cellState == CellState.Mine)
        {
            _view.text = "X";
            _view.color = Color.red;
            manager.touchBomb = true;
            manager.pause = true;
        }
        else
        {
            _view.text = ((int)_cellState).ToString();
            _view.color = Color.blue;
        }
    }

    public void LeftClickAction()
    {
        Debug.Log(111);
        viewMode = true;

        OnCellStateChanged();
        if (_cellState == CellState.Mine)
        {
            return;
        }
        manager.checkedCell++;


        //自身の周囲の番号が0なら、周囲のLeftClickActionも呼び出す。

        if (_cellState == CellState.None)
        {
            for (int i = numberA - 1; i <= numberA + 1; i++)
            {
                if (i < 0 || i >= manager.gameScale)
                {
                    continue;
                }

                for (int k = numberB - 1; k <= numberB + 1; k++)
                {
                    if (k < 0 || k >= manager.gameScale)
                    {
                        continue;
                    }

                    if (manager.cellArray[i, k].GetComponent<Cell>().viewMode == false)
                    {
                        manager.cellArray[i, k].GetComponent<Cell>().LeftClickAction();
                    }
                }
            }

        }


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (manager.pause)
        {
            return;
        }


        switch (eventData.pointerId)
        {
            case -1: //左クリック
                Debug.Log("Left Click");

                if (!manager.touchedFirstCell)
                {
                    manager.touchedFirstCell = true;
                    if (_cellState == CellState.Mine)
                    {
                        manager.mineCount -= 1;
                        int gameScale = manager.gameScale;
                        for (int i = 0; i < gameScale; i++)
                        {
                            for (int k = 0; k < gameScale; k++)
                            {
                                if (i < 0 || i > gameScale || k < 0 || k > gameScale || (i == 0 && k == 0))
                                {
                                    continue;
                                }
                                manager.SearchBomb(numberA, numberB);
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
                break;
        }
    }
}


