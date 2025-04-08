using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SafeSpotConfig
{
    public Transform safeSpot;
    public int lineId;
}

public abstract class PlatformColliderObject : MonoBehaviour
{
    [SerializeField] protected SafeSpotConfig safeSpotConfig;
    protected FMPlatform platform;

    public FMPlatform Platform
    {
        get
        {
            return platform;
        }
        set
        {
            platform = value;
        }
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnHitCollision(collision);
    }

    protected void OnTriggerEnter(Collider other)
    {
        OnHitTrigger(other);
    }

    protected void OnTriggerStay(Collider other)
    {
        OnStayTrigger(other);
    }

    public abstract void OnHitCollision(Collision collision);
    public abstract void OnHitTrigger(Collider other);
    public virtual void OnStayTrigger(Collider other) { }
}
