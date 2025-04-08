using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockablePlatformColliderObject : PlatformColliderObject, IKnockable
{
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Sequence knockSequence;

    public override void OnHitCollision(Collision collision)
    {
    }

    public override void OnHitTrigger(Collider other)
    {
    }

    public void ResetTransform()
    {
        if (knockSequence != null)
        {
            knockSequence.Kill();
            knockSequence = null;
        }

        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
        GetComponentInChildren<Collider>().enabled = true;
    }

    protected void OnEnable()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    public void Knock()
    {
        float randomMoveX = Random.Range(-10f, 10f);
        float randomMoveY = Random.Range(20f, 30f);
        float randomMoveZ = Random.Range(-10f, 10f);
        float randomRotateX = Random.Range(20f, 180f);
        float randomRotateY = Random.Range(20f, 180f);
        float randomRotateZ = Random.Range(20f, 180f);
        float randomMoveSpeed = Random.Range(1f, 1.5f);
        float randomRotateSpeed = randomMoveSpeed;

        if (knockSequence != null && knockSequence.IsActive())
        {
            knockSequence.Kill();
        }

        knockSequence = DOTween.Sequence();
        knockSequence.Append(transform.DOLocalMove(new Vector3(randomMoveX, randomMoveY, randomMoveZ), randomMoveSpeed));
        knockSequence.Join(transform.DORotate(new Vector3(randomRotateX, randomRotateY, randomRotateZ), randomRotateSpeed));
        
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        mainWorld.ColliderObjectToReset.Add(this);

        GetComponentInChildren<Collider>().enabled = false;
    }
}
