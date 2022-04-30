using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UiEffect;
using TMPro;

public class BattleManager : MonoBehaviour
{
    //モンスターイベントのListを作る
    public List<BattleEvent> battleList;
    public int stageInProgress = 0;

    DamageManager damageManager;
    [SerializeField] GameManager_Adventure gameManager;
    [SerializeField] SkillButton skillButton;
    [SerializeField] AudioManager audioManager;

    public Sequence sequence;
    public Tweener tweener;


    public GameObject RemainingTurn;
    public Image stageBackgroundImage;
    public GameObject stageDisplay;
    public Text stageMessageText;
    public GameObject monsterDisplay;
    public TextMeshProUGUI monsterHpText;
    public Text turnDisplay;
    public Image loseEffect;
    [SerializeField] ResultEffect resultEffect;

    Color fadeColor;

    [SerializeField] CanvasGroup monsterCanvasGroup;
    Image monsterImage;
    string monsterName;
    int monsterHp = 1;
    int monsterPower = 10;
    int clearTurn;


    int damage;
    int numberOfCells;
    int numberOfMines = 2;

    [SerializeField] Image monsterImageObject;
    [SerializeField] TextMeshProUGUI monsterNameObject;

    [SerializeField] Battle_Minesweeper minesweeper;

    public float effectDuration = 1.0f;
    public bool pauseMode = false;

    public enum Phase
    {
        Null = -1,
        Setup = 0,
        NewStage = 1,
        MonsterAppears,
        Minesweeper,
        Damage,
        Win,
        Lose,
        Clear
    }

    [SerializeField] Phase phase = Phase.Setup;
    int phaseInProgress = 0;
    // Start is called before the first frame update
    void Start()
    {
        sequence = DOTween.Sequence();


    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMode) //pauseがオンなら何もしない
        {
            return;
        }

        if (phase == Phase.Setup)
        {
            minesweeper.pause = true;
            phase = Phase.NewStage;

        }
        else if (phase == Phase.NewStage)
        {
            if (phaseInProgress == 0)
            {
                phaseInProgress++;

                StageMessage("stage" + (stageInProgress + 1));
            }
            else if (phaseInProgress == 2)
            {
                phaseInProgress = 0;
                phase = Phase.MonsterAppears;
            }
        }
        else if (phase == Phase.MonsterAppears)
        {
            if (phaseInProgress == 0)
            {
                phaseInProgress++;

                sequence = DOTween.Sequence();

                monsterImageObject.sprite = battleList[stageInProgress].monsterImage;
                monsterImage = monsterImageObject.GetComponent<Image>();
                monsterImage.SetNativeSize();
                monsterImageObject.transform.localScale = new Vector3(battleList[stageInProgress].imageScale, battleList[stageInProgress].imageScale, 1);
                monsterNameObject.text = battleList[stageInProgress].monsterName;
                monsterHp = battleList[stageInProgress].hp;
                clearTurn = battleList[stageInProgress].clearTurn;
                numberOfCells = battleList[stageInProgress].numberOfCells;
                numberOfMines = battleList[stageInProgress].numberOfMines;

                monsterHpText.text = "HP:" + monsterHp;
                turnDisplay.text = "残りターン:" + clearTurn;
                if (clearTurn > 3)
                {
                    turnDisplay.GetComponent<GradientColor>().colorBottom = new Color(0.33f, 0.91f, 0.32f);
                }
                else
                {
                    turnDisplay.GetComponent<GradientColor>().colorBottom = new Color(0.91f, 0.56f, 0.32f);
                }
                minesweeper.Restart(numberOfCells, numberOfMines);

                sequence.Append(tweener = monsterDisplay.transform.DOScale(new Vector3(1, 1, 1), duration: 1.0f))
                .OnComplete(() =>
                {
                    skillButton.ChangeState(false);
                    phase = Phase.Minesweeper;
                    phaseInProgress = 0;
                });
            }
        }
        else if (phase == Phase.Minesweeper)
        {
            if (phaseInProgress == 0)
            {
                sequence = DOTween.Sequence();
                sequence.Append(RemainingTurn.transform.DOLocalMoveY(180f, 0.3f));
                phaseInProgress++;
                minesweeper.pause = false;
            }
        }
        else if (phase == Phase.Damage)
        {
            if (phaseInProgress == 0)
            {
                phaseInProgress++;
                minesweeper.AttackEffect();
            }
            else if (phaseInProgress == 2)
            {
                damage = minesweeper.DamageCalculation();
                if (monsterHp < damage)
                {
                    damage = monsterHp;
                }
                monsterHp -= damage;
                phaseInProgress++;
            }
            else if (phaseInProgress == 4)
            {
                phaseInProgress = 0;
                phase = Phase.Minesweeper;
                clearTurn--;
                turnDisplay.text = "残りターン:" + clearTurn;
                if (clearTurn > 3)
                {
                    turnDisplay.GetComponent<GradientColor>().colorBottom = new Color(0.33f, 0.91f, 0.32f);
                }
                else
                {
                    turnDisplay.GetComponent<GradientColor>().colorBottom = new Color(0.91f, 0.56f, 0.32f);
                }
                monsterHpText.text = "HP:" + monsterHp;
                if (monsterHp <= 0)
                {
                    phaseInProgress = 0;
                    phase = Phase.Win;
                    fadeColor = new Color(255, 255, 255, 255);
                }
                else
                {
                    if (clearTurn <= 0)
                    {
                        phase = Phase.Lose;
                    }
                    else
                    {
                        minesweeper.Restart(numberOfCells, numberOfMines);
                    }


                }

            }
        }
        else if (phase == Phase.Win)
        {
            sequence.Append(RemainingTurn.transform.DOLocalMoveY(45f, 0.3f));
            if (phaseInProgress == 0)
            {
                phaseInProgress++;
                sequence = DOTween.Sequence();
                sequence.Append(tweener = DOTween.ToAlpha(() => monsterImageObject.color, color => monsterImageObject.color = color, 0f, 1.0f))
                    .Join(tweener = DOTween.ToAlpha(() => monsterNameObject.color, color => monsterNameObject.color = color, 0f, 1.0f))
                    .Join(tweener = DOTween.ToAlpha(() => monsterHpText.color, color => monsterHpText.color = color, 0f, 1.0f))
                    .AppendCallback(() =>
                    {
                        phaseInProgress++;
                    });

            }
            else if (phaseInProgress == 2)
            {
                //もしまだステージ数が残っているなら
                if (stageInProgress < battleList.Count - 1)
                {
                    stageInProgress++;
                    if (battleList[stageInProgress])
                    {
                        Debug.Log("読みこみ");
                        monsterDisplay.transform.localScale = new Vector3(0, 0, 0);
                        monsterImageObject.color = new Color(255, 255, 255, 1);
                        monsterNameObject.color = new Color(255, 255, 255, 1);
                        monsterHpText.color = new Color(255, 255, 255, 1);
                        numberOfCells = battleList[stageInProgress].numberOfCells;
                        numberOfMines = battleList[stageInProgress].numberOfMines;



                        //minesweeper.Restart(numberOfCells, numberOfMines);
                        phaseInProgress = 0;
                        phase = Phase.NewStage;
                        return;
                    }

                }
                phaseInProgress = 0;
                phase = Phase.Clear;
            }


        }
        else if (phase == Phase.Clear)
        {
            if (phaseInProgress == 0)
            {
                phaseInProgress++;

                stageMessageText.text = "ステージクリア！";
                sequence = DOTween.Sequence();
                sequence.Append(tweener = stageDisplay.transform.DOScale(new Vector3(1, 1, 1), duration: effectDuration))
                .AppendInterval(1.5f)
                .AppendCallback(() =>
                {
                    monsterDisplay.transform.localScale = new Vector3(0, 0, 0);
                    monsterImageObject.color = new Color(255, 255, 255, 1);
                    monsterNameObject.color = new Color(255, 255, 255, 1);
                    monsterHpText.color = new Color(255, 255, 255, 1);
                    numberOfCells = battleList[stageInProgress].numberOfCells;
                    numberOfMines = battleList[stageInProgress].numberOfMines;

                    audioManager.ChangeBGMStageSelect();
                    gameManager.GotoStageSelect();
                });
            }
        }
        else if (phase == Phase.Lose)
        {
            if (phaseInProgress == 0)
            {
                phaseInProgress++;
                stageMessageText.text = "ゲームオーバー...";

                sequence = DOTween.Sequence();
                sequence.Append(DOTween.ToAlpha(() => loseEffect.color, color => loseEffect.color = color, 0.58f, 1.0f))
                .AppendInterval(0.5f)
                .Append(tweener = stageDisplay.transform.DOScale(new Vector3(1, 1, 1), duration: effectDuration))
                .AppendInterval(1.5f)
                .AppendCallback(() =>
                {
                    audioManager.ChangeBGMStageSelect();
                    gameManager.GotoStageSelect();
                });
            }
        }
    }

    public void StageMessage(string text)
    {
        stageMessageText.text = text;

        sequence = DOTween.Sequence();

        sequence.Append(tweener = stageDisplay.transform.DOScale(new Vector3(1, 1, 1), duration: effectDuration))
        .AppendInterval(1.0f)
        .Append(tweener = stageDisplay.transform.DOScale(new Vector3(1, 0, 1), duration: effectDuration))
        .AppendCallback(() =>
        {
            phaseInProgress++;
        });
    }


    public void SetBattleData(List<BattleEvent> battleData, Sprite backgroundImage)
    {
        this.battleList = battleData;
        this.stageBackgroundImage.sprite = backgroundImage;
    }

    public void GotoDamagePhase()
    {
        phaseInProgress = 0;
        phase = Phase.Damage;
    }
    public void NextProgress()
    {
        phaseInProgress++;
    }

    public void PhaseChange(Phase goToPhase)
    {
        phase = goToPhase;
    }

    public void ResetStage()
    {
        sequence = DOTween.Sequence();
        stageInProgress = 0;
        minesweeper.Restart(4, 3);
        phaseInProgress = 0;
        monsterDisplay.transform.localScale = Vector3.zero;
        if (monsterImage != null)
        {
            monsterImage.color = new Color(1, 1, 1, 1);
        }
        stageDisplay.transform.localScale = new Vector3(1, 0, 1);
        phase = Phase.Setup;
    }


}

