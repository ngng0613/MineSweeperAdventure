using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using DG.Tweening;

public class Attack : MonoBehaviour
{
    Sequence sequence;
    RectTransform rectTransform;

    public float monsterPosX;
    public float monsterPosY;
    public Vector3 monsterPosVector3;
    public float moveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        sequence = DOTween.Sequence();
        rectTransform = GetComponent<RectTransform>();
        monsterPosVector3 = new Vector3(monsterPosX, monsterPosY, 0);
        sequence.Append(transform.DOLocalMove(monsterPosVector3, 5f / moveSpeed))
        .OnComplete(() => { Destroy(this.gameObject); Destroy(this); });
    }
}
