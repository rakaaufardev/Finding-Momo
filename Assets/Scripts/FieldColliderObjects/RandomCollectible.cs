using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCollectible : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private PlatformColliderObject collectible;
    [SerializeField] private bool isPermanent;
    [SerializeField] private string permanentCollectibleName;

    public void SetCollectibleObject(PlatformColliderObject inCollectible)
    {
        collectible = inCollectible;
        collectible.transform.SetParent(root);
        collectible.transform.localPosition = Vector3.zero;
    }

    public PlatformColliderObject GetCollectibleObject()
    {
        return collectible;
    }

    public string GetCollectibleName()
    {
        return permanentCollectibleName;
    }

    public void ClearCollectible()
    {
        collectible = null;
    }

    public bool IsCollectiblePermanent()
    {
        bool result = isPermanent;
        return result;
    }
}
