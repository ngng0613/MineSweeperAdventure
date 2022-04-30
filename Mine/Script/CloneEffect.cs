using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloneEffect : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Sequence sequence;
    public float effectTime = 1.0f;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        sequence = DOTween.Sequence();
        sequence.AppendCallback(() => canvasGroup.alpha = 0)
            .AppendInterval(0.33f)
            .AppendCallback(() => canvasGroup.alpha = 1)
            .Append(transform.DOScale(new Vector3(1.5f, 1.5f, 1), 1.0f))
            .Join(canvasGroup.DOFade(0.0f, effectTime))

            .SetLoops(-1);
    }
}
