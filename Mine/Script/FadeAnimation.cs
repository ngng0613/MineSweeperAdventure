using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeAnimation : MonoBehaviour
{
    [SerializeField] Image[] imagesArray;
    [SerializeField] Image BackgroundImage;
    public Sequence sequenceChildren;
    public Sequence sequenceParent;
    public float DurationSeconds;
    public Ease EaseType;
    bool isActive;
    [SerializeField] CanvasGroup parentCanvasgroup;
    [SerializeField] CanvasGroup childrenGroup;

    void Start()
    {
        parentCanvasgroup = this.gameObject.GetComponent<CanvasGroup>();
        sequenceChildren = DOTween.Sequence();
        sequenceParent = DOTween.Sequence();
        //StartAnimation();
    }

    public void FadeOut()
    {
        sequenceParent = DOTween.Sequence();
        sequenceChildren = DOTween.Sequence();
        sequenceChildren.Append(childrenGroup.DOFade(1.0f, DurationSeconds / 2))
            .AppendCallback(() => StartAnimation())
            .AppendInterval(0.36f)
            .Append(BackgroundImage.DOFade(1.0f, DurationSeconds));
    }

    public void FadeIn(TweenCallback tweenCallback)
    {
        sequenceParent = DOTween.Sequence();
        sequenceParent.Append(parentCanvasgroup.DOFade(0.0f, DurationSeconds))
            .AppendCallback(() =>
            {
                sequenceChildren.Kill();
                Color tempColor = BackgroundImage.color;
                tempColor.a = 0;
                BackgroundImage.color = tempColor;
                for (int i = 0; i < imagesArray.Length; i++)
                {
                    Color temp = imagesArray[i].color;
                    temp.a = 1.0f;
                    imagesArray[i].color = temp;
                }
                childrenGroup.alpha = 0.0f;
                parentCanvasgroup.alpha = 1.0f;

                tweenCallback();
            });

    }

    public void StartAnimation()
    {
        sequenceChildren = DOTween.Sequence();
        for (int i = 0; i < imagesArray.Length; i++)
        {
            sequenceChildren.Insert(0.5f + i * 0.064f, imagesArray[i].DOFade(0.0f, this.DurationSeconds))
                .Insert(1.0f + i * 0.064f, imagesArray[i].DOFade(1.0f, this.DurationSeconds))
                .SetLoops(-1);
        }
    }


}
