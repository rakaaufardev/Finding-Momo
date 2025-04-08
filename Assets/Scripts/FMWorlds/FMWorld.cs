using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum FieldType
{
    Sand2D
}

public abstract class FMWorld : MonoBehaviour
{
    [SerializeField] protected FMCameraController cameraController;
    protected List<PlatformColliderObject> colliderObjectToReset;
    protected VDCharacter character;

    public List<PlatformColliderObject> ColliderObjectToReset
    {
        get
        {
            return colliderObjectToReset;
        }
        set
        {
            colliderObjectToReset = value;
        }
    }

    public FMCameraController CameraController
    {
        get
        {
            return cameraController;
        }
        set
        {
            cameraController = value;
        }
    }

    public abstract void StartGame(params object[] transferVariables);
    public abstract void PauseGame();
    public abstract void PlayGame();

    private void SaveData()
    {

    }

    private void OnEndAd()
    {

    }
    //todo collectible reset
    public void ResetColliderObjects()
    {
        int count = colliderObjectToReset.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformColliderObject colliderObject = colliderObjectToReset[0];
            colliderObject.gameObject.SetActive(true);

            if (colliderObject is Seagull)
            {
                ((Seagull)colliderObject).SeagullParent.ResetSeagulls();
            }

            if (colliderObject is SafeRewardObstacle)
            {
                bool isSequenceActive = ((SafeRewardObstacle)colliderObject).GetSequenceStatus();
                if (!isSequenceActive)
                {
                    ((SafeRewardObstacle)colliderObject).ResetSafeReward();
                }
            }

            if (colliderObject is SpinWheelGarbage)
            {
                ((SpinWheelGarbage)colliderObject).ResetModel();
            }

            if (colliderObject is InkObstacle)
            {
                //todo reset setactive to true
                ((InkObstacle)colliderObject).ResetModel();
            }

            if (colliderObject is Bomb)
            {
                //todo reset setactive to true
                ((Bomb)colliderObject).ResetModel();
            }

            if (colliderObject is Obstacle || colliderObject is Seagull || colliderObject is TrampolineColliderObject 
                || colliderObject is Bomb || colliderObject is InstantDeathObstacle || colliderObject is InkObstacle 
                || colliderObject is TopPassObstacle)
            {
                ((IKnockable)colliderObject).ResetTransform();
            }

            if(colliderObject is Coin)
            {
                ((Coin)colliderObject).ResetCoinPosition();
            }
            colliderObjectToReset.Remove(colliderObject);
        }
    }

    public VDCharacter GetCharacter()
    {
        return character;
    }
}
