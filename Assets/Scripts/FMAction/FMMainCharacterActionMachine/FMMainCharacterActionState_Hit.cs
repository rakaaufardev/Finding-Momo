using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMMainCharacterActionState_Hit : FMCharacterActionState
{
    private bool hitEnd;
    private UIMain uiMain;
    private FMMainScene mainScene;

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            character = inCharacter as FMMainCharacter;
            inputController = character.GetInputController();
            actionMachine = character.GetActionMachine();
        }

        //hitEnd = false;
        mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        uiMain = mainScene.GetUI();
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        HitObstacleType hitObstacleType = (HitObstacleType)values[0];
        SafeSpotConfig hitSafeSpot = (SafeSpotConfig)values[1];

        bool shouldContinue = false;
        FMSceneController.Get().PlayParticle("WorldParticle_HitBurst", character.transform.position);
        ((FMMainCharacter)character).Slide(false);
        //mainScene.EnableHitboxCollider(false);
        //((FMMainCharacter)character).EnableInvulnerable();

        if (((FMMainCharacter)character).IsTutorial)
        {
            ((FMMainCharacter)character).ResetToSafeSpot(hitSafeSpot);
        }
        else
        {
            //mainScene.EnableHitboxCollider(false);
            ((FMMainCharacter)character).EnableInvulnerable();
            ((FMMainCharacter)character).HitForceFeedback(hitObstacleType);
        }

        float animationSpeed = ((FMMainCharacter)character).AnimationSpeed;
        ((FMMainCharacter)character).ChangeLaneTween.Kill();
        ((FMMainCharacter)character).SetAnimationSpeed(animationSpeed);

        ((FMMainCharacter)character).SpendHealth(1);
        //((FMMainCharacter)character).HitForceFeedback(hitObstacleType);
        ((FMMainCharacter)character).NormalizeLineId();
        uiMain.UpdateHealthIcon();

        ((FMMainCharacter)character).TriggerAnimation("Hit");
        while (!shouldContinue)
        {
            shouldContinue = ((FMMainCharacter)character).IsAnimationEnd("Hit");
            yield return new WaitForEndOfFrame();
        }

        ((FMMainCharacter)character).TriggerAnimation("Run");
        //actionMachine.ClearStateException();

        hitEnd = true;
    }

    public override void DoUpdate()
    {
        if (hitEnd)
        {
            hitEnd = false;
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
        }
    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        ((FMMainCharacterActionMachine)actionMachine).ClearStateException(MainCharacterActionType.Hit);
        yield return null;
    }
}
