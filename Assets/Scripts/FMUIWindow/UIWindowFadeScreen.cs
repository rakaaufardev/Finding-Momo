using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowFadeScreen : VDUIWindow
{
    [SerializeField] private Animator animFade;

    public override void Show(object[] dataContainer)
    {
        PlayFadeIn();
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

    public void PlayFadeIn()
    {
        animFade.SetTrigger("Show");
    }

    public void PlayFadeOut()
    {
        animFade.SetTrigger("Hide");
    }

    public bool IsFadeInEnd()
    {
        AnimatorStateInfo stateInfo = animFade.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName("ShowEnd");
        return result;
    }
    public bool IsFadeOutEnd()
    {
        AnimatorStateInfo stateInfo = animFade.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName("HideEnd");
        return result;
    }
}
