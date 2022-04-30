using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blink : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public float blinkTime = 1.0f;

    private void Start()
    {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0.5f, blinkTime).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
