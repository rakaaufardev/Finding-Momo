using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMMainCharacterActionState_SwitchRight : FMCharacterActionState
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
        float changeLineSpeedAnim = 2 - VDParameter.CHARACTER_CHANGE_LINE_SPEED;
        ((FMMainCharacter)character).SetAnimationSpeed(changeLineSpeedAnim);
        ((FMMainCharacter)character).TriggerAnimation("SwitchRight");
        yield return null;
    }

    public override void DoUpdate()
    {
        ViewMode viewMode = ((FMMainCharacter)character).CharacterViewMode;

        if (viewMode == ViewMode.BackView)
        {
            if (inputController.IsChangeLeftLanePressed())
            {
                if (((FMMainCharacter)character).LineId > 0)
                {
                    //actionMachine.SetState(CharacterActionType.SwitchLeft);
                    ((FMMainCharacter)character).ChangeLaneAnimation(false);
                }

                ((FMMainCharacter)character).ChangeBackViewLane(false);
            }
            if (inputController.IsChangeRightLanePressed())
            {
                if (((FMMainCharacter)character).LineId < 2)
                {
                    //actionMachine.SetState(CharacterActionType.SwitchRight);
                    ((FMMainCharacter)character).ChangeLaneAnimation(true);
                }

                ((FMMainCharacter)character).ChangeBackViewLane(true);
            }
        }

        ((FMMainCharacter)character).Move();
    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        float animationSpeed = ((FMMainCharacter)character).AnimationSpeed;
        ((FMMainCharacter)character).SetAnimationSpeed(animationSpeed);
        yield return null;
    }
}
