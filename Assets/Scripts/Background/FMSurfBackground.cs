using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMSurfBackground : MonoBehaviour
{
    [SerializeField] private Transform transformIdleBoat;
    [SerializeField] private Transform rootCharacter;
    [SerializeField] private Transform rootAnimalSpawn;
    [SerializeField] private Transform rootPortal;
    [SerializeField] private GameObject rootForeground;
    private Transform transformTarget;
    private Vector3 offset;
    [SerializeField] private FMSurfAnimalSpawn surfAnimalSpawner;

    public FMSurfAnimalSpawn SurfAnimalSpawner => surfAnimalSpawner;

    public Transform RootPortal
    {
        get
        {
            return rootPortal;
        }
        set
        {
            rootPortal = value;
        }
    }

    public Transform RootAnimalSpawn
    {
        get
        {
            return rootAnimalSpawn;
        }
        set
        {
            rootAnimalSpawn = value;
        }
    }

    public void SetCharacter(Transform character)
    {
        character.SetParent(rootCharacter);
        character.localPosition = Vector3.zero;
        transform.localPosition = new Vector3(-20,0,0);
    }

    public void SetFollowTarget(Transform target)
    {
        transformTarget = target;
        offset = new Vector3(-5, 0, 0);
    }

    public void Move()
    {
        transform.position = new Vector3(transformTarget.position.x,0,0) + offset;
    }

    public void ShowIdleBoat()
    {
        transformIdleBoat.transform.localPosition = new Vector3(0, 9, 0);
    }

    public void HideIdleBoat()
    {
        transformIdleBoat.transform.DOLocalMoveX(-10f, 2f);
    }

    public void ShowLandForeground()
    {
        rootForeground.SetActive(true);
    }
}
