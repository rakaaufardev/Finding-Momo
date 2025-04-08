using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMParallaxBackground : MonoBehaviour
{
    private Transform target;
    private float offset;
    private float prevTargetPosX;
    private float multiplier;

    public void SetTarget(Transform inTarget, float inMultiplier)
    {
        target = inTarget;
        prevTargetPosX = target.transform.position.x;
        multiplier = inMultiplier;
    }

    public void DoParallax()
    {
        float distance = Vector3.Distance(new Vector3(target.transform.position.x, 0, 0), new Vector3(transform.position.x, 0, 0));

        if(distance > VDParameter.PARALLAX_MAIN_GAME_MAX_DISTANCE)
        {
            multiplier = 0;
        }

        if (target != null)
        {
            float speed = target.transform.position.x - prevTargetPosX;
            prevTargetPosX = target.transform.position.x;
            offset += speed * multiplier * Time.deltaTime;

            float xPos = target.position.x - offset;
            Vector3 targetPos = new Vector3(xPos, 0, target.position.z);
            transform.position = Vector3.Slerp(transform.position, targetPos, 1f);
        } 
    }
}
