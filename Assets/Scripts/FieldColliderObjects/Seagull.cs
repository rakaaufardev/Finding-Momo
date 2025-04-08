using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : KnockablePlatformColliderObject
{
    [SerializeField] private SeagullParent seagullParent;
    [SerializeField] private Animator seagullAnimator;
    [SerializeField] private Collider seagullCollider;
    [SerializeField] private Transform seagullVisual;

    public SeagullParent SeagullParent
    {
        get
        {
            return seagullParent;
        }
    }

    public Animator SeagullAnimator
    {
        get
        {
            return seagullAnimator;
        }
    }

    public Collider SeagullCollider
    {
        get
        {
            return seagullCollider;
        }
    }
    
    public Transform SeagullVisual
    {
        get
        {
            return seagullVisual;
        }
    }

    public override void OnHitCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FMMainCharacter character = collision.gameObject.GetComponent<FMMainCharacter>();
            if (character != null)
            {
                character.OnHitSeagull(this, seagullParent, safeSpotConfig, gameObject.name);
            }
        }
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
                case WorldType.Surf:
                    character = other.gameObject.GetComponentInParent<FMSurfCharacter>();
                    ((FMSurfCharacter)character).OnHitObstacle();
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
}
