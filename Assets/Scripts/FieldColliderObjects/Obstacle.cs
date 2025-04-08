using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : KnockablePlatformColliderObject
{
    public override void OnHitCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            WorldType worldType = mainScene.GetCurrentWorldType();

            switch (worldType)
            {
                case WorldType.Main:
                    var character = collision.gameObject.GetComponent<FMMainCharacter>();
                    if (character != null)
                    {
                        ((FMMainCharacter)character).OnHitObstacle(collision, safeSpotConfig, gameObject.name);
                    }
                    break;
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
                    if (character != null)
                    {
                        ((FMSurfCharacter)character).OnHitObstacle();
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
}
