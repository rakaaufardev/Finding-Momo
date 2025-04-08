using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWindowTransitionSurf : VDUIWindow
{

    [SerializeField] private Animator animTransition;
    [SerializeField] private GameObject enterSurfText;
    [SerializeField] private GameObject enterMainText;

    public override void Show(object[] dataContainer)
    {
        PlayTransitionIn();
    }

    public override void Hide()
    {

    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }

    public void PlayTransitionIn()
    {
        animTransition.SetTrigger("show");
    }

    public void PlayTransitionOut()
    {
        animTransition.SetTrigger("hide");
    }

    public bool IsTransitionInEnd()
    {
        AnimatorStateInfo stateInfo = animTransition.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName("Active");
        return result;
    }

    public bool IsTransitionOutEnd()
    {
        AnimatorStateInfo stateInfo = animTransition.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName("Inactive");
        return result;
    }



    public void ShowEnterSurfText()
    {
        enterSurfText.SetActive(true);   
    }

    public void ShowEnterMainText()
    {
        enterMainText.SetActive(true);
    }
}
