using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMMainCharacterActionState_Slide : FMCharacterActionState
{
    private float slideTimer;
    private bool slideEnd;

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            character = inCharacter as FMMainCharacter;
            inputController = character.GetInputController();
            actionMachine = character.GetActionMachine();
        }
        slideTimer = VDParameter.CHARACTER_SLIDE_DURATION_IN_SEC;
        slideEnd = false;
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        ((FMMainCharacter)character).ResetAnimation("Run");
        ((FMMainCharacter)character).ResetAnimation("Jump");
        ((FMMainCharacter)character).ResetAnimation("DoubleJump");
        ((FMMainCharacter)character).TriggerAnimation("Slide");
        yield return null;
    }

    public override void DoUpdate()
    {
        if (stateReady)
        {
            ViewMode viewMode = ((FMMainCharacter)character).CharacterViewMode;

            if (slideTimer <= 0)
            {
                slideEnd = true;
            }
            else
            {
                slideTimer -= Time.deltaTime;
            }

            if (viewMode == ViewMode.BackView)
            {
                if (inputController.IsChangeLeftLanePressed())
                {
                    ((FMMainCharacter)character).ChangeBackViewLane(false);
                    slideEnd = true;
                }

                if (inputController.IsChangeRightLanePressed())
                {
                    ((FMMainCharacter)character).ChangeBackViewLane(true);
                    slideEnd = true;
                }
            }
        }

#if UNITY_STANDALONE
        bool buttonNeutral = !inputController.IsSlideRelease() && !inputController.IsSlidePressed();
        if (inputController.IsSlideRelease() || buttonNeutral || slideEnd)
        {
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
            ((FMMainCharacter)character).Slide(false);
        }
#elif UNITY_ANDROID
        if (inputController.IsSlideRelease() || slideEnd)
        {
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
            ((FMMainCharacter)character).Slide(false);
        }
#endif

        ((FMMainCharacter)character).Move();
    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        /*((FMMainCharacter)character).TriggerAnimation("Run");*/

        ((FMMainCharacter)character).ResetAnimation("Slide");
        ((FMMainCharacter)character).TriggerAnimation("Run");
        yield return new WaitForEndOfFrame();
    }
}
