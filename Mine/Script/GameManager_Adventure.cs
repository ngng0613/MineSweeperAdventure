using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager_Adventure : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    Sequence sequence;
    [SerializeField] FadeAnimation fadeAnimation;
    [SerializeField] Image fadeImage;
    [SerializeField] Canvas titleCanvas;
    [SerializeField] Canvas stageSelectCanvas;
    [SerializeField] StageSelectManager stageSelectManager;
    [SerializeField] GameObject pauseObject;
    [SerializeField] GameObject pauseObjectTitle;
    [SerializeField] ResultEffect resultEffect;
    [SerializeField] AudioManager audioManager;
    bool startButtonPressed = false;
    public bool pauseMode = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GotoStageSelect()
    {
        Fade(() =>
        {
            pauseObject.SetActive(false);
            pauseMode = false;
            titleCanvas.gameObject.SetActive(false);
            stageSelectCanvas.gameObject.SetActive(true);
            stageSelectManager.clickPermission = true;

        }, null);

    }

    public void StageStart()
    {
        Fade(() =>
        {
            stageSelectCanvas.gameObject.SetActive(false);
            battleManager.ResetStage();
            //battleManager.sequence.Pause();
            audioManager.ChangeBGM(AudioManager.BgmType.Battle);
        }, null);

    }

    public void SettingButton()
    {
        if (!pauseMode)
        {
            pauseObjectTitle.SetActive(true);
            pauseMode = true;

        }
        else
        {
            pauseObjectTitle.SetActive(false);
            pauseMode = false;
        }



    }

    public void PauseButton()
    {
        if (!pauseMode)
        {
            //ポーズ処理
            pauseObject.SetActive(true);
            battleManager.pauseMode = true;
            battleManager.sequence.Pause();
            resultEffect.sequence.Pause();
            pauseMode = true;
        }
        else
        {
            //ポーズ解除処理
            pauseObject.SetActive(false);
            battleManager.pauseMode = false;
            battleManager.sequence.Play();
            resultEffect.sequence.Play();
            pauseMode = false;
        }
    }

    public void RestartButton()
    {
        Fade(() =>
        {
            battleManager.sequence.Kill();
            pauseMode = false;
            pauseObject.SetActive(false);
            resultEffect.ResetPos();
            battleManager.ResetStage();
        }, () => battleManager.pauseMode = false);
    }

    public void BackToTitle()
    {
        Fade(() =>
        {
            audioManager.ChangeBGM(AudioManager.BgmType.Title);
            battleManager.sequence.Kill();
            titleCanvas.gameObject.SetActive(true);
            startButtonPressed = false;
            pauseMode = false;
            pauseObject.SetActive(false);
            resultEffect.ResetPos();
            battleManager.ResetStage();
    
        }, null);
    }

    public void Fade(TweenCallback duringFading, TweenCallback afterFading)
    {
        sequence = DOTween.Sequence();
        sequence
            .AppendCallback(() => fadeAnimation.FadeOut())
            .AppendInterval(1.5f)
            .AppendCallback(() => duringFading())
            .AppendCallback(() => fadeAnimation.FadeIn(afterFading));
    }
}
