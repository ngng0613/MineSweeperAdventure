using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
    bool isClicked = false;
    public Image thisObjectImage;
    Color defaultColor;
    Color pushedColor;
    [SerializeField] float darkness = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        defaultColor = thisObjectImage.color;
        pushedColor = defaultColor;
        pushedColor.r -= darkness;
        pushedColor.g -= darkness;
        pushedColor.b -= darkness;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeState(bool state)
    {
        if (state)
        {
            isClicked = true;
            thisObjectImage.color = pushedColor;

        }
        else
        {
            isClicked = false;
            thisObjectImage.color = defaultColor;
        }

    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (isClicked)
        {
            ChangeState(false);
        }
        else
        {
            ChangeState(true);
        }
    }

}
