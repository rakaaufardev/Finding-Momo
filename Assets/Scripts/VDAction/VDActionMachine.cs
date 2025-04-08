using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionMachineStatus
{
    Stop,
    Active,
    BreakUntil
}

public enum MainCharacterActionType
{
    NONE,
    Idle,
    Run,
    Jump,
    DoubleJump,
    Hit,
    Slide,
    TransitionSideToBack,
    TransitionBackToSide,
    SwitchLeft,
    SwitchRight,
    Trampoline,
    InstantDeathFall,
    GameOver
}

public enum SurfCharacterActionType
{
    NONE,
    Idle,
    Run,
    GameOver,
    Finish
}

public class StateQueueData
{
    public object state;
    public object[] values;
}

public abstract class VDActionMachine
{
    protected VDCharacter character;
    protected VDActionState currentAction;
    protected ActionMachineStatus machineStatus;
    protected object nextWaitState;
    protected List<object> stateExceptions;
    protected Coroutine coroutineState;
    protected List<StateQueueData> stateQueueDatas;

    public abstract void Init();
    public abstract void DoUpdate();
    public abstract void DoFixedUpdate();
    public abstract void SetCharacter(VDCharacter inCharacter);
    public abstract void SetState(object state, params object[] values);
    public abstract void SetActionMachineStatus(ActionMachineStatus status);
    public abstract ActionMachineStatus GetActionMachineStatus();
    public abstract void BreakUntil(object untilState);
    public abstract void SetStateException(object state);
    public abstract void ClearAllStateException();
    public abstract void ClearStateException(object state);
    public abstract VDActionState GetCurrentAction();
    public abstract bool IsStateExceptionExist();
}
