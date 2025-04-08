using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMMainCharacterActionState_GameOver : FMCharacterActionState
{
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

        mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        uiMain = mainScene.GetUI();
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        bool runGameOverAnimation = (bool)values[0];
        HitObstacleType hitObstacleType = values[1] == null ? HitObstacleType.NONE : (HitObstacleType)values[1];
        
        bool shouldContinue = false;

        actionMachine.SetActionMachineStatus(ActionMachineStatus.Stop);
        FMSceneController.Get().PlayParticle("WorldParticle_HitBurst", character.transform.position);

        mainScene.EnableHitboxCollider(false);
        ((FMMainCharacter)character).HitForceFeedback(hitObstacleType);

        int healthDecrease = runGameOverAnimation ? ((FMMainCharacter)character).CurrentHealth : 1;
        ((FMMainCharacter)character).SpendHealth(healthDecrease);
        uiMain.UpdateHealthIcon();

        if (runGameOverAnimation)
        {
            ((FMMainCharacter)character).TriggerAnimation("GameOver");

            while (!shouldContinue)
            {
                shouldContinue = ((FMMainCharacter)character).IsAnimationEnd("GameOver");
                yield return new WaitForEndOfFrame();
            }
        }

        FMSceneController.Get().ShowContinueConfirmation();
    }

    public override void DoUpdate()
    {

    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        mainScene.EnableHitboxCollider(true);

        yield return null;
    }

}
