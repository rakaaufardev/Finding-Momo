using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMDisplayObject : MonoBehaviour
{
    [SerializeField] protected Transform rootVisual;
    [SerializeField] protected Animator anim;
    [SerializeField] protected List<GameObject> layerAnim;
    private bool isSequenceActive;
    private Sequence sequence;
    private Coroutine coroutine;

    public Transform RootVisual
    {
        get => rootVisual;
        set => rootVisual = value;
    }

    public Animator Anim
    {
        get => anim;
        set => anim = value;
    }

    public Sequence DisplaySequence
    {
        get => sequence;
        set => sequence = value;
    }

    public Coroutine DisplayCoroutine
    {
        get => coroutine;
        set => coroutine = value;
    }

    public bool IsSequenceActive
    {
        get => isSequenceActive;
        set => isSequenceActive = value;
    }

    public bool IsActive(bool isActive)
    {
        isSequenceActive = isActive;

        return isSequenceActive;
    }

    public  void SetAnimActive(bool isTrue)
    {
        for(int i=0;i<layerAnim.Count;i++)
        {
            layerAnim[i].SetActive(isTrue);
        }
    }

    public bool IsStateAnim(string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        bool result = stateInfo.IsName(animName);
        return result;
    }

    public bool IsAnimationEnd(string animStateName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        bool result = stateInfo.IsName(animStateName) && stateInfo.normalizedTime >= 1;
        return result;
    }

    public void ResetDisplayObject()
    {
        //trashVisual.gameObject.SetActive(true);
        rootVisual.transform.localPosition = Vector3.zero;
        rootVisual.transform.localScale = Vector3.one;
        rootVisual.transform.localEulerAngles = Vector3.zero;
        anim.SetTrigger("Idle");
    }

    /*public void SetPause(bool isPause)
    {
        if (isPause)
        {
            anim.speed = 0f;
            sequence.Pause();
        }
        else if (!isPause)
        {
            anim.speed = 1f; 
            sequence.Play();
        }
    }*/
}
