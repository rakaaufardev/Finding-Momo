using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransitionObstacle : PlatformColliderObject
{
    public override void OnHitCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FMMainCharacter character = collision.gameObject.GetComponent<FMMainCharacter>();
            if (character != null)
            {
                character.OnHitWallTransitionObstacle(collision, safeSpotConfig, gameObject.name);
            }
        }
    }

    public override void OnHitTrigger(Collider other)
    {

    }
}
