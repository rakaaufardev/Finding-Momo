using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using VD;


public enum RewardObstacleType
{
    SeaTurtle,
    Dolphin,
    SeaLion,
    MantaRay,
    KillerWhale
}

public class SafeRewardObstacle : PlatformColliderObject
{
    [SerializeField] protected Transform root;
    [SerializeField] protected Transform trashVisual;
    [SerializeField] protected Transform rootVisual;
    [SerializeField] protected Transform rootBubble;
    [SerializeField] protected Animator rewardAnim;
    [SerializeField] protected Animator firstLayerNet;
    [SerializeField] protected Animator secondLayerNet;
    [SerializeField] protected RewardObstacleType rewardObstacleType;
    private bool isSequenceRunning;
    private WorldType worldType;
    [SerializeField] private int animalHealth;
    FMSurfAnimalSpawn surfAnimalSpawner;


    public void Awake()
    {
        animalHealth = VDParameter.ANIMAL_HEALTH[rewardObstacleType];
    }
    public RewardObstacleType RewardObstacleType
    {
        get => rewardObstacleType;
        set => rewardObstacleType = value;
    }

    public override void OnHitCollision(Collision collision)
    {

    }

    public override void OnHitTrigger(Collider other)
    {
        surfAnimalSpawner = GetComponentInParent<FMSurfAnimalSpawn>();
        if (other.gameObject.CompareTag("Player"))
        {
            VDCharacter character = null;

            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            WorldType worldType = mainScene.GetCurrentWorldType();

            switch (worldType)
            {
                case WorldType.Main:
                    break;
                case WorldType.Surf:
                    character = other.gameObject.GetComponentInParent<FMSurfCharacter>();
                    if (character != null)
                    {                        
                        ((FMSurfCharacter)character).OnHitSafeReward(this, surfAnimalSpawner);
                    }
                    break;
            }
            
        }
        if (other.gameObject.CompareTag("AnimalCleaner"))
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            SurfWorld surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
            surfWorld.SetAnimalBackToParent();
            surfWorld.SpawnAnimal();
        }
    }

    public bool IsStateAnim(string animName)
    {
        AnimatorStateInfo stateInfo = rewardAnim.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName(animName);
        return result;
    }

    public void ResetSafeReward()
    {
        rootVisual.SetParent(root);
        rootVisual.transform.localPosition = Vector3.zero;
        rootVisual.transform.localScale = Vector3.one;
        rootVisual.transform.localEulerAngles = Vector3.zero;
        rootVisual.gameObject.SetActive(true);
        trashVisual.gameObject.SetActive(true);
        rootBubble.gameObject.SetActive(true);
        rewardAnim.SetTrigger("Idle");
    }

    public void SetInSequence(bool inSequence)
    {
        isSequenceRunning = inSequence;
    }

    public bool GetSequenceStatus()
    {
        return isSequenceRunning;
    }

    public void ApplyDamage(int damage)
    {
        //if (VDParameter.ANIMAL_HEALTH.ContainsKey(rewardObstacleType))
        //{
        //    VDParameter.ANIMAL_HEALTH[rewardObstacleType] -= damage;
        //    if (VDParameter.ANIMAL_HEALTH[rewardObstacleType] < 0)
        //        VDParameter.ANIMAL_HEALTH[rewardObstacleType] = 0; // Ensure health doesn't go negative
        //}
        animalHealth -= damage;
    }
    public int GetAnimalHealth()
    {
        //return VDParameter.ANIMAL_HEALTH.ContainsKey(rewardObstacleType) ? VDParameter.ANIMAL_HEALTH[rewardObstacleType] : 0;
        return animalHealth;
    }
    public void ResetAnimalHealth()
    {
        //if (VDParameter.ANIMAL_HEALTH.ContainsKey(rewardObstacleType))
        //{
        //    VDParameter.ANIMAL_HEALTH[rewardObstacleType] = VDParameter.DEFAULT_ANIMAL_HEALTH[rewardObstacleType];
        //}
        animalHealth = VDParameter.ANIMAL_HEALTH[rewardObstacleType];
    }
    public void PlayNetAnimation(int health, Action onComplete)
    {
        if(health == 2)
        {
            firstLayerNet.SetTrigger("Free");
            DOVirtual.DelayedCall(.3f, () =>
            {
                firstLayerNet.gameObject.SetActive(false);
                onComplete?.Invoke();
            });

        }
        else if(health == 1)
        {
            secondLayerNet.SetTrigger("Free");
            DOVirtual.DelayedCall(.3f, () =>
            {
                secondLayerNet.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
        else
        {
            return;
        }
        
    }

}
