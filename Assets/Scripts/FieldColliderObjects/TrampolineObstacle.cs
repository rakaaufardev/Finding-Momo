using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineObstacle : PlatformColliderObject
{
    [SerializeField] private Transform rootTrampolineHitbox;

    public override void OnHitCollision(Collision collision)
    {

    }

    public override void OnHitTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            rootTrampolineHitbox.gameObject.SetActive(false);
            FMMainCharacter character = other.gameObject.GetComponentInParent<FMMainCharacter>();
            if (character != null)
            {
                character.OnHitTrampolineObstacle(true, rootTrampolineHitbox, safeSpotConfig, gameObject.name);
            }
        }

    }



}
