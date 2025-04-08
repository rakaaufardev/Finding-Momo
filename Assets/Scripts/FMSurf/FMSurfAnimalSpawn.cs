using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMSurfAnimalSpawn : MonoBehaviour
{
    [SerializeField] private List<SafeRewardObstacle> randomAnimalSpawn;
    [SerializeField] private SafeRewardObstacle miniBossAnimal;
    [SerializeField] private SafeRewardObstacle bossAnimal;
    [SerializeField] private Transform rootAnimalSurf;
    [SerializeField] private Transform topSpawn;
    [SerializeField] private Transform midSpawn;
    [SerializeField] private Transform bottomSpawn;
    [SerializeField] private Transform animalShadowObject;
    [SerializeField] Collider[] hitColliders;

    public Transform RootAnimalSurf
    {
        get
        {
            return rootAnimalSurf;
        }
        set
        {
            rootAnimalSurf = value;
        }
    }

    public void SetAnimalActive(float progress,float maxGoal)
    {
        if (maxGoal <= 0) return;

        float progressPercentage = progress / maxGoal;

        DeactivateAllAnimals();

        switch (progressPercentage)
        {
            case <= VDParameter.RANDOM_ANIMAL_THRESHOLD:
                ActivateRandomAnimal();
                break;
            case >= VDParameter.MINIBOSS_THRESHOLD and < VDParameter.BOSS_THRESHOLD:
                miniBossAnimal?.gameObject.SetActive(true);
                break;
            case >= VDParameter.BOSS_THRESHOLD:
                bossAnimal?.gameObject.SetActive(true);
                break;
        }
    }

    private void ActivateRandomAnimal()
    {
        if (randomAnimalSpawn == null || randomAnimalSpawn.Count == 0)
        {
            return;
        }

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        FMSurfCharacter surfCharacter = currentWorld.GetCharacter() as FMSurfCharacter;
        bool isInTutorial = surfCharacter.IsInTutorial;

        int randomIndex = isInTutorial ? 0 : Random.Range(0, randomAnimalSpawn.Count);
        randomAnimalSpawn[randomIndex].gameObject.SetActive(true);
    }

    public void DeactivateAllAnimals()
    {
        int count = randomAnimalSpawn.Count;
        for (int i = 0; i < count; i++)
        {
            randomAnimalSpawn[i].gameObject.SetActive(false);
        }

        if (miniBossAnimal != null) miniBossAnimal.gameObject.SetActive(false);
        if (bossAnimal != null) bossAnimal.gameObject.SetActive(false);
    }
    public void SetRootParent(Transform rootParent, bool isBackOriginalParent)
    {
        rootAnimalSurf.SetParent(rootParent);
        if (isBackOriginalParent)
        {
            float[] possibleYPositions = { topSpawn.localPosition.y, midSpawn.localPosition.y, bottomSpawn.localPosition.y };
            float randomY = possibleYPositions[Random.Range(0, possibleYPositions.Length)];

            rootAnimalSurf.localPosition = new Vector3(0, randomY, 0);
            rootAnimalSurf.localRotation = Quaternion.identity;
        }
        RemoveNearObstacle();
    }

    private void RemoveNearObstacle()
    {
        hitColliders = Physics.OverlapSphere(transform.position, 7f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] is BoxCollider)
            {
                Transform parent = hitColliders[i].transform.parent; 
                Transform grandparent = parent != null ? parent.parent : null; 

                if (grandparent != null)
                {
                    grandparent.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetActiveObstacle()
    {
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] is BoxCollider)
            {
                Transform parent = hitColliders[i].transform.parent;
                Transform grandparent = parent != null ? parent.parent : null;

                if (grandparent != null)
                {
                    grandparent.gameObject.SetActive(true);
                }
            }
        }
    }

    public void SetAnimalShadow(bool isTrue)
    {

        animalShadowObject.gameObject.SetActive(isTrue);
    }
}
