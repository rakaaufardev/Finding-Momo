using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMMainCharacterActionState_Idle : FMCharacterActionState
{
    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            character = inCharacter as FMMainCharacter;
            inputController = character.GetInputController();
            actionMachine = character.GetActionMachine();
        }
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        ViewMode viewMode = (ViewMode)values[0];
        ((FMMainCharacter)character).TriggerAnimation("Idle");

        float rotY = viewMode == ViewMode.SideView ? 135 : 90;
        Vector3 angle = new Vector3(0, rotY, 0);

        ((FMMainCharacter)character).SetRotation(viewMode);
        ((FMMainCharacter)character).RotateCharacterVisual(angle, true);

        yield return null;
    }

    public override void DoUpdate()
    {
        UIWindowTransitionSurf currentWindow = FMUIWindowController.Get.CurrentWindow as UIWindowTransitionSurf;

        if (inputController.IsStartRunningPressed() && currentWindow == null)
        {
            StartRunning();
        }
    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        mainScene.GameStatus = GameStatus.Play;
        ((FMMainCharacter)character).TriggerAnimation("Run");
        yield return null;
    }

    public void StartRunning()
    {
        ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
    }
}
