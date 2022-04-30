using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsManager : MonoBehaviour
{
    public int gameScale = 3;
    public bool gameClear = false;
    public GameObject clearText;
    public GameObject gameOverText;
    public GameObject restartButton;
    public bool pause = false;
    public bool touchBomb = false;

    public bool touchedFirstCell = false;

    [SerializeField] int cell_Size = 50;
    [SerializeField] Cell cell_Prefab;
    [SerializeField] GameObject panel;
    public int checkedCell;
    public int mineCount;

    [SerializeField] float zoom_Camera;

    int[,] numbersArray;
    public GameObject[,] cellArray;
    public bool[,] bombsArray;

    [SerializeField] GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkedCell + mineCount >= gameScale * gameScale)
        {
            pause = true;
            gameClear = true;
            clearText.SetActive(true);
            restartButton.SetActive(true);
        }
        if (touchBomb)
        {
            gameOverText.SetActive(true);
            restartButton.SetActive(true);
        }
    }

    public void SearchBomb(int i, int k)
    {
        //周囲の爆弾の数
        int bombs_Count = 0;

        //周囲の爆弾チェック
        for (int m = -1; m <= 1; m++)
        {
            for (int n = -1; n <= 1; n++)
            {
                if ((m == 0 && n == 0) || (i + m < 0) || (i + m >= gameScale) || (k + n < 0) || (k + n >= gameScale))
                {
                    continue;
                }

                if (numbersArray[i + m, k + n] == -1)
                {
                    bombs_Count++;
                }
            }
        }
        numbersArray[i, k] = bombs_Count;
        cellArray[i, k].GetComponent<Cell>().CellState = (CellState)bombs_Count;
    }

    public void Setup()
    {
        clearText.SetActive(false);
        gameOverText.SetActive(false);
        restartButton.SetActive(false);

        numbersArray = new int[gameScale, gameScale];
        cellArray = new GameObject[gameScale, gameScale];
        bombsArray = new bool[gameScale, gameScale];
        checkedCell = 0;

        Vector3 tempScale = new Vector3(zoom_Camera * 13 / gameScale, zoom_Camera * 13 / gameScale, 1);
        gameObject.transform.localScale = tempScale;

        /*
        for (int i = 0; i < gameScale; i++)
        {
            //列の爆弾配置箇所を決める
            int bombNumber = Random.Range(0, gameScale);
            numbers_Array[i, bombNumber] = -1;
        }
        */
        for (var i = 0; i < mineCount; i++)
        {
            var r = Random.Range(0, gameScale);
            var c = Random.Range(0, gameScale);
            if (numbersArray[r, c] == -1)
            {
                i--;
                continue;
            }
            numbersArray[r, c] = -1;
        }

        RectTransform panelRectTrans = panel.GetComponent<RectTransform>();
        panelRectTrans.sizeDelta = new Vector3(gameScale * cell_Size, gameScale * cell_Size, 1);
        for (int i = 0; i < numbersArray.GetLength(0); i++)
        {
            for (int k = 0; k < numbersArray.GetLength(1); k++)
            {
                cellArray[i, k] = Instantiate(cell_Prefab.gameObject);
                GameObject tempCell = cellArray[i, k];
                tempCell.transform.SetParent(panel.transform);

                tempScale = new Vector3(zoom_Camera, zoom_Camera, 1);
                cellArray[i, k].transform.localScale = tempScale;

                //自身が爆弾でないなら
                if (numbersArray[i, k] != -1)
                {
                    SearchBomb(i, k);
                }
                Cell choose_Cell = cellArray[i, k].GetComponent<Cell>();
                choose_Cell.CellState = (CellState)numbersArray[i, k];

                choose_Cell.manager = this;
                choose_Cell.numberA = i;
                choose_Cell.numberB = k;

            }
        }
    }

    public void Restart()
    {
        foreach (var item in cellArray)
        {
            Destroy(item.gameObject);
        }

        pause = false;
        touchBomb = false;
        touchedFirstCell = false;

        Setup();
    }


}

