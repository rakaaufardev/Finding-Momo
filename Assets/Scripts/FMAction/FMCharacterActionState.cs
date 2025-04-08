using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FMCharacterActionState : VDActionState
{
    protected VDCharacter character;
    protected VDInputController inputController;
    protected VDActionMachine actionMachine;

    protected bool isInitialized;

    public bool StateReady
    {
        get
        {
            return stateReady;
        }
        set
        {
            stateReady = value;
        }
    }

    public abstract void Init(VDCharacter inCharacter);
    public abstract override IEnumerator Enter(object prev, params object[] values);
    public abstract override IEnumerator Exit(object next);
    public abstract override void DoUpdate();
    public abstract override void DoFixedUpdate();
}
