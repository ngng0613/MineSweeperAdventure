using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Battle_Minesweeper : MonoBehaviour
{
    public int gameScale = 3;
    public bool gameClear = false;

    public GameObject clearText;
    public GameObject gameOverText;
    public GameObject restartButton;
    public bool pause = false;
    public bool touchMine = false;
    public bool touchedFirstCell = false;
    public int checkedCell;
    public int mineCount;
    public int[,] numbersArray;
    public GameObject[,] cellArray;
    public SkillButton skillButton;
    public AudioManager audioManager;

    [SerializeField] GameObject camera;
    [SerializeField] BattleManager battleManager;
    [SerializeField] DamageManager damageManager;
    [SerializeField] float zoom_Camera;
    [SerializeField] int cell_Size = 50;
    [SerializeField] Cell_Battle cell_Prefab;
    [SerializeField] GameObject panel;
    [SerializeField] Canvas canvas;
    [SerializeField] ResultEffect resultEffect;

    bool skillUsed = false;

    bool oneTime = false;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        damageManager = GetComponent<DamageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause)
        {
            if (oneTime == false)
            {
                if (checkedCell + mineCount >= gameScale * gameScale)
                {
                    pause = true;
                    TurnEnd(true);
                    oneTime = true;
                }
                if (touchMine)
                {
                    pause = true;
                    TurnEnd(false);
                    oneTime = true;
                }
            }
        }
        if (resultEffect.effectEnd)
        {
            resultEffect.effectEnd = false;
            GotoDamagePhase();
        }

    }

    public void SearchMine(int i, int k)
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
        cellArray[i, k].GetComponent<Cell_Battle>().cellState = (CellState)bombs_Count;
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    public void Setup()
    {
        skillUsed = false;
        skillButton.ChangeState(false);
        oneTime = false;

        numbersArray = new int[gameScale, gameScale];
        cellArray = new GameObject[gameScale, gameScale];
        checkedCell = 0;

        Vector3 tempScale = new Vector3(zoom_Camera * 16 / gameScale, zoom_Camera * 16 / gameScale, 1);
        gameObject.transform.localScale = tempScale;


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
                tempCell.GetComponent<Cell_Battle>().canvas = this.canvas;

                tempScale = new Vector3(zoom_Camera, zoom_Camera, 1);
                cellArray[i, k].transform.localScale = tempScale;

                //自身が爆弾でないなら
                if (numbersArray[i, k] != -1)
                {
                    SearchMine(i, k);
                }
                Cell_Battle choose_Cell = cellArray[i, k].GetComponent<Cell_Battle>();
                choose_Cell.cellState = (CellState)numbersArray[i, k];

                choose_Cell.battleManager = this;
                choose_Cell.audioManager = this.audioManager;
                choose_Cell.numberA = i;
                choose_Cell.numberB = k;

            }
        }
    }

    public void Restart(int numberOfCells, int numberOfMine)
    {
        if (cellArray.GetLength(0) > 0)
        {
            foreach (var item in cellArray)
            {
                Destroy(item.gameObject);
            }
        }


        pause = false;
        touchMine = false;
        touchedFirstCell = false;
        gameScale = numberOfCells;
        mineCount = numberOfMine;
        skillUsed = false;

        Setup();
    }

    /// <summary>
    /// 引数no座標以外のCELLに爆弾を新たに配置する
    /// </summary>
    /// <param name="originR"></param>
    /// <param name="originC"></param>
    public void AddMine(int originR, int originC)
    {
        while (true)
        {
            int r = Random.Range(0, gameScale);
            int c = Random.Range(0, gameScale);
            if (!cellArray[r, c].GetComponent<Cell_Battle>().viewMode && r != originR && c != originC)
            {
                if (cellArray[r, c].GetComponent<Cell_Battle>().cellState != CellState.Mine)
                {
                    cellArray[r, c].GetComponent<Cell_Battle>().cellState = CellState.Mine;
                    numbersArray[r, c] = -1;

                    for (int i = r - 1; i <= r + 1; i++)
                    {
                        for (int k = c - 1; k <= c + 1; k++)
                        {
                            if (i < 0 || i >= gameScale || k < 0 || k >= gameScale || (i == r && k == c))
                            {
                                continue;
                            }
                            SearchMine(i, k);
                        }
                    }

                    Debug.Log("Add: " + r + "," + c);
                    break;
                }
            }
        }

    }

    public void TurnEnd(bool result)
    {
        if (result == true)
        {
            resultEffect.Effect_Nice();

        }
        else
        {
            resultEffect.Effect_Bad();

        }
    }

    public void GotoDamagePhase()
    {
        battleManager.GotoDamagePhase();
    }

    public int DamageCalculation()
    {
        return damageManager.DamageCalculation(cellArray);
    }

    //以下スキル関数

    /// <summary>
    /// 未クリックの地雷1つにフラグを立てる。
    /// </summary>
    public void Skill_MineRock()
    {
        if (skillUsed)
        {
            return;
        }
        skillUsed = true;

        audioManager.PlaySeSkill();

        List<Cell_Battle> mineList = new List<Cell_Battle>();

        for (int i = 0; i < cellArray.GetLength(0); i++)
        {
            for (int k = 0; k < cellArray.GetLength(1); k++)
            {
                if (cellArray[i, k].GetComponent<Cell_Battle>().cellState == CellState.Mine)
                {
                    if (!cellArray[i, k].GetComponent<Cell_Battle>().flagMode && !cellArray[i, k].GetComponent<Cell_Battle>().viewMode)
                    {
                        mineList.Add(cellArray[i, k].GetComponent<Cell_Battle>());
                    }

                }
            }
        }

        int randomPoint = Random.Range(0, mineList.Count);

        mineList[randomPoint].RightClick();
    }

    public void AttackEffect()
    {
        Sequence sequence = DOTween.Sequence();
        audioManager.PlaySeAttack();

        sequence.AppendCallback(() =>
        {
            for (int i = 0; i < gameScale; i++)
            {
                for (int k = 0; k < gameScale; k++)
                {
                    cellArray[i, k].GetComponent<Cell_Battle>().CreateAttackEffect();
                }
            }

        }).AppendInterval(0.9f)
        .AppendCallback(() =>
        {
            audioManager.PlaySeDamage();
            battleManager.NextProgress();
        });

    }

}
