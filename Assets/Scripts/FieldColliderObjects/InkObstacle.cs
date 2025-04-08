using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkObstacle : KnockablePlatformColliderObject
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform rootVisual;
    [SerializeField] private Transform rootHitbox;
    [SerializeField] private Transform inkModelTransform;
    [SerializeField] private Animator inkModelAnim;

    private Vector3 defaultScaleModel;

    static Vector3 MODEL_SCALE_DEFAULT = new Vector3(0.7f, 0.7f, 0.7f);

    public Transform Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }
    
    public Transform RootVisual
    {
        get
        {
            return rootVisual;
        }
        set
        {
            rootVisual = value;
        }
    }

    public Transform RootHitbox
    {
        get
        {
            return rootHitbox;
        }
        set
        {
            rootHitbox = value;
        }
    }

    public Transform InkModelTransform
    {
        get
        {
            return inkModelTransform;
        }
        set
        {
            inkModelTransform = value;
        }
    }

    public Animator InkModelAnim
    {
        get
        {
            return inkModelAnim;
        }
        set
        {
            inkModelAnim = value;
        }
    }

    public override void OnHitCollision(Collision collision)
    {

    }

    public override void OnHitTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            VDCharacter character = null;

            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            WorldType worldType = mainScene.GetCurrentWorldType();

            switch (worldType)
            {
                case WorldType.Main:
                    character = other.gameObject.GetComponentInParent<FMMainCharacter>();
                    if (character != null)
                    {
                        ((FMMainCharacter)character).OnHitInk(this);
                    }
                    break;
                case WorldType.Surf:
                    character = other.gameObject.GetComponentInParent<FMSurfCharacter>();
                    if (character != null)
                    {
                        ((FMSurfCharacter)character).OnHitInk(this);
                    }
                    break;
            }
        }
        else if (other.gameObject.CompareTag("Bubble"))
        {
            FMMainCharacter character = other.gameObject.GetComponentInParent<FMMainCharacter>();
            if (character != null && character.IsInvulnerable)
            {
                Knock();
            }
        }
    }

    public override void OnStayTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            FMMainCharacter character = other.gameObject.GetComponentInParent<FMMainCharacter>();
            if (character != null && character.IsInvulnerable)
            {
                Knock();
            }
        }
    }

    public bool IsAnimationEnd(string animStateName)
    {
        AnimatorStateInfo stateInfo = inkModelAnim.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName(animStateName) && stateInfo.normalizedTime >= 1;
        return result;
    }

    public void ResetModel()
    {
        rootVisual.gameObject.SetActive(true);
    }
}
