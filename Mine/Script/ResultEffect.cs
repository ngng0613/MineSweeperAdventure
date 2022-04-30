using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResultEffect : MonoBehaviour
{
    [SerializeField] GameObject textNice;
    [SerializeField] GameObject textBad;
    public bool effectEnd = false;
    public Sequence sequence;

    public void Effect_Nice()
    {
        ObjectMove(textNice.transform);
    }
    public void Effect_Bad()
    {
        ObjectMove(textBad.transform);
    }

    void ObjectMove(Transform t)
    {
        t.localPosition = new Vector3(-900, 0, 0);
        sequence = DOTween.Sequence();
        sequence.Append(t.DOLocalMoveX(0, 0.5f))
            .AppendInterval(0.5f)
            .Append(t.DOLocalMoveX(900, 0.5f))
            .AppendCallback(() => effectEnd = true);

    }

    public void ResetPos()
    {
        //sequence = DOTween.Sequence();
        textNice.transform.localPosition = new Vector3(-900, 0, 0);
        textBad.transform.localPosition = new Vector3(-900, 0, 0);

    }
}
