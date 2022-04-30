using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> bgmClipList = new List<AudioClip>();
    [SerializeField] List<AudioClip> seClipList = new List<AudioClip>();
    [SerializeField] AudioSource myAudioSource;
    [SerializeField] AudioSource seAudioSource;
    Sequence sequence;
    [SerializeField] GameObject sePlayer;

    private void Start()
    {
    }

    public enum BgmType
    {
        Title,
        SelectStage,
        Battle
    }

    public enum SeType
    {
        TitleClick,


    }

    public void ChangeBGM(BgmType type)
    {
        sequence = DOTween.Sequence();
        sequence.Append(myAudioSource.DOFade(0.0f, 0.5f))
                .AppendCallback(() =>
                {
                    if (type == BgmType.Title)
                    {
                        myAudioSource.clip = bgmClipList[0];
                        myAudioSource.volume = 0.4f;
                    }
                    else if (type == BgmType.SelectStage)
                    {
                        myAudioSource.clip = bgmClipList[1];
                        myAudioSource.volume = 1.0f;
                    }
                    else if (type == BgmType.Battle)
                    {
                        myAudioSource.clip = bgmClipList[1];
                        myAudioSource.volume = 1.0f;
                    }

                    myAudioSource.Play();
                });
    }

    public void ChangeBGMStageSelect()
    {
        sequence = DOTween.Sequence();
        sequence.Append(myAudioSource.DOFade(0.0f, 0.5f))
                .AppendCallback(() =>
                {
                    myAudioSource.clip = bgmClipList[0];
                    myAudioSource.volume = 0.4f;
                    myAudioSource.Play();
                });
    }

    public void PlaySE(SeType type)
    {

        if (type == SeType.TitleClick)
        {
            seAudioSource.PlayOneShot(seClipList[0]);
        }

    }

    public void PlaySeTitle()
    {
        seAudioSource.PlayOneShot(seClipList[0]);

    }

    public void PlaySeSelectStage()
    {
        seAudioSource.PlayOneShot(seClipList[0]);
    }

    public void PlaySeBlockClick()
    {
        seAudioSource.volume = 0.7f;
        seAudioSource.PlayOneShot(seClipList[1]);
    }

    public void PlaySeAttack()
    {
        seAudioSource.volume = 0.7f;
        seAudioSource.PlayOneShot(seClipList[2]);
    }

    public void PlaySeDamage()
    {
        seAudioSource.volume = 0.7f;
        seAudioSource.PlayOneShot(seClipList[3]);
    }

    public void PlaySeSkill()
    {
        seAudioSource.volume = 0.7f;
        seAudioSource.PlayOneShot(seClipList[4]);
    }
}
