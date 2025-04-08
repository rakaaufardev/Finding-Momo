using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeathObstacle : KnockablePlatformColliderObject
{
    public override void OnHitCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FMMainCharacter character = collision.gameObject.GetComponent<FMMainCharacter>();
            if (character != null)
            {
                character.OnHitInstantDeathObstacle(safeSpotConfig);
            }
        }
    }

    public override void OnHitTrigger(Collider other)
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
