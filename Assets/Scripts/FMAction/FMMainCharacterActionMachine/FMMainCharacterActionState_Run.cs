using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMMainCharacterActionState_Run : FMCharacterActionState
{
    UIMain uiMain;
    public override void Init(VDCharacter inCharacter)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        uiMain = mainScene.GetUI();
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
        Vector3 angle = new Vector3(0, 90, 0);
        ((FMMainCharacter)character).RotateCharacterVisual(angle, false);

        if (((FMMainCharacter)character).CharacterViewMode == ViewMode.BackView)
        {
            ((FMMainCharacter)character).BackToCenterLane();
        }

        float animationSpeed = ((FMMainCharacter)character).AnimationSpeed;
        ((FMMainCharacter)character).CharacterRigidBody.WakeUp();
        ((FMMainCharacter)character).SetAnimationSpeed(animationSpeed);

        yield return null;
    }

    public override void DoUpdate()
    {
        ViewMode viewMode = ((FMMainCharacter)character).CharacterViewMode;

        if (viewMode == ViewMode.BackView)
        {
            if (inputController.IsChangeLeftLanePressed())
            {
                ChangeLeftLane();
            }
            if (inputController.IsChangeRightLanePressed())
            {
                ChangeRightLane();
            }
        }

        Vector3 characterPoint = ((FMMainCharacter)character).GetCharacterNormalizePosition();
        float endTrackPointDistance = Vector3.Distance(((FMMainCharacter)character).CurrentEndTrackPoint, characterPoint);
        bool readyToChangeTrack = endTrackPointDistance <= VDParameter.TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE;
        
        if (!readyToChangeTrack)
        {
            if (inputController.IsJumpPressed() || ((FMMainCharacterActionMachine)actionMachine).IsJump)
            {
                Jump();
            }
        }

        if (inputController.IsSlidePressed())
        {
            Slide();
        }

        ((FMMainCharacter)character).Move();
    }

    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        //character.ChangeLaneTween.Kill();
        yield return null;
    }

    public void Jump()
    {
        if (((FMMainCharacter)character).isWheelOver) // 13 March 2025 disable jump momentarily
        {
            return;
        }

        if (((FMMainCharacter)character).IsTutorial)
        {
            uiMain.HideHandTap();
            uiMain.HideHandSwipe();
        }
        uiMain.HideArrowDirection();
        uiMain.RotateArrowDirection(SwipeDirection.Up);
        ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Jump);
        ((FMMainCharacter)character).Jump();

    }

    public void ChangeLeftLane()
    {
        if (((FMMainCharacter)character).LineId > 0)
        {
            //actionMachine.SetState(CharacterActionType.SwitchLeft);
            ((FMMainCharacter)character).ChangeLaneAnimation(false);
        }
        uiMain.HideArrowDirection();
        uiMain.RotateArrowDirection(SwipeDirection.left);
        ((FMMainCharacter)character).ChangeBackViewLane(false);
    }
    public void ChangeRightLane()
    {
        if (((FMMainCharacter)character).LineId < 2)
        {
            //actionMachine.SetState(CharacterActionType.SwitchRight);
            ((FMMainCharacter)character).ChangeLaneAnimation(true);
        }
        uiMain.HideArrowDirection();
        uiMain.RotateArrowDirection(SwipeDirection.Right);
        ((FMMainCharacter)character).ChangeBackViewLane(true);
    }
    public void Slide()
    {
        if (((FMMainCharacter)character).IsTutorial)
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            UIMain uiMain = mainScene.GetUI();
            uiMain.HideHandTap();
            uiMain.HideHandSwipe();
        }

        if (!((FMMainCharacterActionMachine)actionMachine).IsSliding)
        {
            ((FMMainCharacterActionMachine)actionMachine).IsSliding = true;
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Slide);
            ((FMMainCharacter)character).Slide(true);
        }
        uiMain.HideArrowDirection();
        uiMain.RotateArrowDirection(SwipeDirection.Down);
    }
}
