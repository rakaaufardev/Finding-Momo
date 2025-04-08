using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMMainCharacterActionState_InstantDeathFall : FMCharacterActionState
{
    private UIMain uiMain;

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            character = inCharacter as FMMainCharacter;
            inputController = character.GetInputController();
            actionMachine = character.GetActionMachine();
        }

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        uiMain = mainScene.GetUI();
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        SafeSpotConfig hitSafeSpot = (SafeSpotConfig)values[0];

        bool shouldContinue = false;

        //Tracking
        //if (((FMMainCharacter)character).SpeedMultiplierLevel == VDParameter.SPEED_MULTIPLIER_MAX_LEVEL)
        //{
        //    FMTrackingController.Get().TrackMaxSpeedObtained();
        //    FMTrackingController.Get().MaxSpeedDuration = 0;
        //}

        int coinCollected = ((FMMainCharacter)character).CoinCollected;
        //FMTrackingController.Get().TrackCoinCollected(coinCollected);
        //FMTrackingController.Get().UpdateTrackingInfo();
        //FMTrackingController.Get().Save();
        //FMTrackingController.Get().Write();

        ((FMMainCharacter)character).SpendHealth(((FMMainCharacter)character).CurrentHealth);

        uiMain.UpdateHealthIcon();

        ((FMMainCharacter)character).TriggerAnimation("Fall");

        ((FMMainCharacter)character).TransformCostume.DOMoveY(1f, 1f);

        ((FMMainCharacter)character).TransformCostume.DOLocalRotate(Vector3.zero, 1f).OnComplete(()=>
        {
            shouldContinue = true;
        });

        while (!shouldContinue)
        {            
            yield return new WaitForEndOfFrame();
        }

        ((FMMainCharacter)character).TransformCostume.DOMoveY(-10, 1f).OnComplete(() =>
        {
            if (((FMMainCharacter)character).IsTutorial)
            {
                ((FMMainCharacter)character).ResetToSafeSpot(hitSafeSpot);
            }
            else
            {
                ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.GameOver, false, null);
            }
        });

        if (((FMMainCharacter)character).IsTutorial)
        {
            yield return new WaitForSeconds(3f);
            ((FMMainCharacterActionMachine)actionMachine).ClearStateException(MainCharacterActionType.InstantDeathFall);
            ((FMMainCharacter)character).TransformCostume.localPosition = Vector3.zero;
            ((FMMainCharacter)character).TransformCostume.localEulerAngles = new Vector3(0, -90, 0);
            ((FMMainCharacter)character).TriggerAnimation("Run");

            uiMain.UpdateHealthIcon();

            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
        }
    }

    public override void DoUpdate()
    {

    }

    public override void DoFixedUpdate()
    {

    }

    public override IEnumerator Exit(object next)
    {
        yield return null;
    }
}
