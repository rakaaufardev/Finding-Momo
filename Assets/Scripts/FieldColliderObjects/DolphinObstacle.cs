using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DolphinObstacle : SafeRewardObstacle
{
    [SerializeField] private Transform rootSilhouette;
    private FMSurfCharacter character;
    private bool isJump;
    private bool isHit;

    private IEnumerator DoJump()
    {
        bool shouldContinue = false;

        root.gameObject.SetActive(true);
        rootSilhouette.gameObject.SetActive(true);

        Vector3 scaleTarget = new Vector3(0.25f, 0.25f, 0.25f);
        float duration = VDParameter.DOLPHIN_JUMP_TO_TARGET_DURATION_IN_SEC;
        Ease ease = VDParameter.DOLPHIN_JUMP_EASE;
        rootSilhouette.transform.DOScale(scaleTarget, duration).SetEase(ease);
        root.DOLocalMoveY(VDParameter.DOLPHIN_JUMP_TO_TARGET_POSITION_Y, duration).SetEase(ease).OnComplete(()=>
        {
            shouldContinue = true;
        });

        while (!shouldContinue)
        {
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        scaleTarget = Vector3.one;
        duration = VDParameter.DOLPHIN_FALL_TARGET_DURATION_IN_SEC;
        ease = VDParameter.DOLPHIN_FALL_EASE;
        rootSilhouette.transform.DOScale(scaleTarget, duration).SetEase(ease);
        root.DOLocalMoveY(VDParameter.DOLPHIN_FALL_TARGET_POSITION_Y, duration).SetEase(ease).OnComplete(() =>
        {
            shouldContinue = true;
        });

        while (!shouldContinue)
        {
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        root.gameObject.SetActive(false);
        rootSilhouette.gameObject.SetActive(true);
    }

    protected void FixedUpdate()
    {
        //if (character == null)
        //{
        //    MainScene mainScene = FMSceneController.Get().GetCurrentScene() as MainScene;
        //    SurfWorld surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
        //    character = surfWorld.GetCharacter() as FMSurfCharacter;

        //    root.gameObject.SetActive(false);
        //    rootSilhouette.gameObject.SetActive(true);
        //}

        //if (!isJump)
        //{
        //    Vector3 dolphinPosX = new Vector3(root.position.x, 0, 0);
        //    Vector3 playerPosX = new Vector3(character.transform.position.x, 0, 0);
        //    float distance = Vector3.Distance(dolphinPosX, playerPosX);

        //    if (distance <= VDParameter.DOLPHIN_TRIGGER_JUMP_DISTANCE)
        //    {
        //        isJump = true;
        //        StartCoroutine(DoJump());
        //    }
        //}
    }
} 
