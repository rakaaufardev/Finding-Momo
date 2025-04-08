using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMSurfCharacterActionMachine_Finish : FMCharacterActionState
{
    private FMSurfCharacter surfCharacter;

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            surfCharacter = inCharacter as FMSurfCharacter;
            inputController = surfCharacter.GetInputController();
            actionMachine = surfCharacter.GetActionMachine();
        }
    }

    public override IEnumerator Enter(object prev, params object[] values)
    {
        surfCharacter.Move();
        surfCharacter.ResetAnimation("Up");
        surfCharacter.ResetAnimation("Jump");
        surfCharacter.TriggerAnimation("Down");
        yield return null;
    }

    public override void DoUpdate()
    {
        surfCharacter.Move();
    }

    public override void DoFixedUpdate()
    {

    }

    public override IEnumerator Exit(object next)
    {
        yield return null;
    }
}
