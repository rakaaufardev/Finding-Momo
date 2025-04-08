using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMMainCharacterActionState_Jump : FMCharacterActionState
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
        ((FMMainCharacter)character).Slide(false);
        ((FMMainCharacter)character).TriggerAnimation("Jump");
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

        Vector3 characterPoint = ((FMMainCharacter)character).GetCharacterNormalizePosition();
        float endTrackPointDistance = Vector3.Distance(((FMMainCharacter)character).CurrentEndTrackPoint, characterPoint);
        bool readyToChangeTrack = endTrackPointDistance <= VDParameter.TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE;
        if (!readyToChangeTrack)
        {
            if (inputController.IsJumpPressed() || ((FMMainCharacterActionMachine)actionMachine).IsJump)
            {
                if (((FMMainCharacter)character).isWheelOver) // 13 March 2025 disable jump momentarily
                {
                    return;
                }
                /*Debug.Log("bypassed is wheel over 1 ");
                if (((FMMainCharacter)character).isUIWheelOver)
                {
                    Debug.Log("isuiwheeloveryes");
                    ((FMMainCharacter)character).isUIWheelOver = false;
                    
                    return;
                }
                if (((FMMainCharacter)character).isUIWheelOver2)
                {
                    Debug.Log("isuiwheeloveryes2");
                    ((FMMainCharacter)character).isUIWheelOver2 = false;

                    return;
                }*/

                ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.DoubleJump);
                ((FMMainCharacter)character).Jump();

                if (viewMode == ViewMode.SideView)
                {
                    float xOffset = 1f;
                    float yOffset = 2f;
                    Vector3 particlePosition = new(character.transform.position.x + xOffset, character.transform.position.y + yOffset, character.transform.position.z);
                    FMSceneController.Get().PlayParticle("WorldParticle_Jump", particlePosition);
                }
                else
                {
                    float zOffset = 1f;
                    float yOffset = 2f;
                    Vector3 particlePosition = new(character.transform.position.x, character.transform.position.y + yOffset, character.transform.position.z + zOffset);
                    FMSceneController.Get().PlayParticle("WorldParticle_Jump", particlePosition);
                }
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
        yield return null;
    }
}
