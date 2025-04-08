using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMMainCharacterActionState_Trampoline : FMCharacterActionState
{
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
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        //mainScene.EnableHitboxCollider(false);
        ((FMMainCharacter)character).TriggerAnimation("Trampoline");
        yield return null;
    }

    public override void DoUpdate()
    {
        ViewMode viewMode = ((FMMainCharacter)character).CharacterViewMode;

        if (viewMode == ViewMode.BackView)
        {
            if (inputController.IsChangeLeftLanePressed())
            {
                ((FMMainCharacter)character).ChangeBackViewLane(false);
            }
            if (inputController.IsChangeRightLanePressed())
            {
                ((FMMainCharacter)character).ChangeBackViewLane(true);
            }
        }

        if (inputController.IsQuickLandPressed())
        {
            ((FMMainCharacter)character).QuickLand();
        }

        ((FMMainCharacter)character).Move();
    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        if (!((FMMainCharacter)character).IsInvulnerable)
        {
            mainScene.EnableHitboxCollider(true);
        }
        yield return null;
    }
}
