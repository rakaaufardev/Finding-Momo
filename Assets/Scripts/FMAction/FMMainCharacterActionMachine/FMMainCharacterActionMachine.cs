using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VD;

public class FMMainCharacterActionMachine : VDActionMachine
{
    private VDInputController inputController;
    private MainCharacterActionType currentState;
    private Dictionary<MainCharacterActionType, FMCharacterActionState> stateData;
    private bool isSliding;
    private bool isJump;
    private float jumpBufferTimer;
    private UITutorial uiTutorial;
    private UIMain uiMain;

    public bool IsSliding
    {
        get
        {
            return isSliding;
        }
        set
        {
            isSliding = value;
        }
    }

    public bool IsJump
    {
        get
        {
            return isJump;
        }
        set
        {
            isJump = value;
        }
    }

    public override void Init()
    {
        nextWaitState = MainCharacterActionType.NONE;
        stateExceptions = new List<object>();

        stateQueueDatas = new List<StateQueueData>();
        stateData = new Dictionary<MainCharacterActionType, FMCharacterActionState>()
        {
            { MainCharacterActionType.Idle , new FMMainCharacterActionState_Idle() },
            { MainCharacterActionType.Run , new FMMainCharacterActionState_Run() },
            { MainCharacterActionType.Jump , new FMMainCharacterActionState_Jump() },
            { MainCharacterActionType.DoubleJump , new FMMainCharacterActionState_DoubleJump() },
            { MainCharacterActionType.Hit , new FMMainCharacterActionState_Hit() },
            { MainCharacterActionType.Slide , new FMMainCharacterActionState_Slide() },
            { MainCharacterActionType.TransitionSideToBack , new FMMainCharacterActionState_TransitionSideToBack() },
            { MainCharacterActionType.TransitionBackToSide , new FMMainCharacterActionState_TransitionBackToSide() },
            { MainCharacterActionType.SwitchLeft , new FMMainCharacterActionState_SwitchLeft() },
            { MainCharacterActionType.SwitchRight , new FMMainCharacterActionState_SwitchRight() },
            { MainCharacterActionType.Trampoline , new FMMainCharacterActionState_Trampoline() },
            { MainCharacterActionType.InstantDeathFall , new FMMainCharacterActionState_InstantDeathFall() },
            { MainCharacterActionType.GameOver , new FMMainCharacterActionState_GameOver() },
        };
    }

    public override void DoUpdate()
    {
        if (inputController == null)
        {
            inputController = character.GetInputController();
        }

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        bool isPause = mainScene.GameStatus == GameStatus.Pause;
        bool isTutorialActive = UITutorial.IsTutorialActive();
        bool actionMachineActive = machineStatus == ActionMachineStatus.Active;

        if (actionMachineActive && !isTutorialActive && !isPause)
        {
            if (inputController != null)
            {
                if (currentState != MainCharacterActionType.Idle)
                {
                    if (inputController.IsSlideRelease())
                    {
                        isSliding = false;
                    }

                    Vector3 characterPoint = ((FMMainCharacter)character).GetCharacterNormalizePosition();
                    float endTrackPointDistance = Vector3.Distance(((FMMainCharacter)character).CurrentEndTrackPoint, characterPoint);
                    bool readyToChangeTrack = endTrackPointDistance <= VDParameter.TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE;
                    
                    if (!readyToChangeTrack)
                    {
                        if (inputController.IsJumpPressed())
                        {
                            if (!isJump)
                            {
                                isJump = true;
                                jumpBufferTimer = VDParameter.JUMP_BUFFER_DURATION_IN_SEC;
                            }
                        }
                    }
                        

                    if (jumpBufferTimer > 0)
                    {
                        jumpBufferTimer -= Time.deltaTime;
                    }
                    else
                    {
                        if (isJump)
                        {
                            isJump = false;
                        }
                    }
                }
            }

            if (currentAction != null)
            {
                currentAction.DoUpdate();
            }
        }        
    }

    public override void DoFixedUpdate()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        bool isPause = mainScene.GameStatus == GameStatus.Pause;
        bool isTutorialActive = UITutorial.IsTutorialActive();

        if (!isTutorialActive && !isPause)
        {
            if (currentAction != null)
            {
                currentAction.DoFixedUpdate();
            }
        }
    }

    public override void SetStateException(object state)
    {
         if (stateExceptions == null)
        {
            stateExceptions = new List<object>();
        }

        for (int i = 0; i < stateExceptions.Count + 1; i++)
        {
            if (!stateExceptions.Contains(state))
            {
                stateExceptions.Add((MainCharacterActionType)state);
                VDLog.Log("Add State Exceptions: " + stateExceptions[i]);
                break;
            }
        }
    }

    public override void ClearAllStateException()
    {
        stateExceptions.Clear();
        VDLog.Log("Removed All State Exception");
    }

    public override void ClearStateException(object state)
    {
        stateExceptions.Remove((MainCharacterActionType)state);
        VDLog.Log("Removed State Exception: " + state);
    }

    public override bool IsStateExceptionExist()
    {
        bool result = stateExceptions.Count > 0;
        return result;
    }

    public void JumpBufferReset()
    {
        isJump = false;
        jumpBufferTimer = 0;
    }

    public override void SetCharacter(VDCharacter inCharacter)
    {
        character = inCharacter;
    }

    public override void SetState(object state, params object[] values)
    {
        switch (machineStatus)
        {
            case ActionMachineStatus.BreakUntil:
                if ((MainCharacterActionType)state != (MainCharacterActionType)nextWaitState)
                {
                    VDLog.Log("State is Break until " + nextWaitState);
                    return;
                }
                else
                {
                    VDLog.Log("State is Continue");
                    nextWaitState = MainCharacterActionType.NONE;
                    machineStatus = ActionMachineStatus.Active;
                }
                break;
            case ActionMachineStatus.Stop:
                VDLog.Log("State is Stop");
                return;
        }

        object currentState = (MainCharacterActionType)state;
        bool stateExceptionValid = stateExceptions.Contains(currentState);
        if (stateExceptionValid)
        {
            return;
        }

        if (coroutineState == null) 
        {
            coroutineState = character.StartCoroutine(StateChange((MainCharacterActionType)state, values));
        }
        else
        {
            VDLog.Log("State assign new queue : " + state);
            bool isStateException = stateExceptions.Contains(state);
            if (!isStateException)
            {
                VDLog.Log("State assign new queue valid");
                StateQueueData stateQueueData = new StateQueueData();
                stateQueueData.state = state;
                stateQueueData.values = values;
                stateQueueDatas.Add(stateQueueData);
            }
            else
            {
                VDLog.Log("State assign new queue cancel, because it's exception : " + state);
            }           
        }
    }

    IEnumerator StateChange(MainCharacterActionType state, params object[] values)
    {
        MainCharacterActionType prevState = MainCharacterActionType.NONE;
        if (currentAction != null)
        {
            prevState = GetCurrentState();
            VDLog.Log("Exit State : " + prevState);
            yield return currentAction.Exit(state);
            ((FMCharacterActionState)currentAction).StateReady = false;
        }

        VDLog.Log("Execute new state : " + state);

        currentAction = stateData[state];
        ((FMCharacterActionState)currentAction).Init(character);
        yield return currentAction.Enter(prevState, values);
        ((FMCharacterActionState)currentAction).StateReady = true;

        currentState = state;
        VDLog.Log("currentState: " + currentState);

        if (stateQueueDatas.Count > 0) 
        {
            StateQueueData stateQueueData = stateQueueDatas[0];
            MainCharacterActionType nextState = (MainCharacterActionType)stateQueueData.state;
            object[] nextValues = stateQueueData.values;
            stateQueueDatas.RemoveAt(0);

            bool breakState = prevState == MainCharacterActionType.GameOver ||
                prevState == MainCharacterActionType.TransitionSideToBack ||
                prevState == MainCharacterActionType.TransitionBackToSide;

            VDLog.Log("State Next Queue : " + nextState);
            if (!breakState)
            {
                switch (machineStatus)
                {
                    case ActionMachineStatus.Stop:
                        ClearStateQueue();
                        break;
                    case ActionMachineStatus.Active:
                        VDLog.Log("State Next Queue Execute");
                        yield return StateChange(nextState, nextValues);
                        break;
                }
            }
            else
            {
                VDLog.Log("State Queue Break");
                ClearStateQueue();
            }
        }
        else
        {
            VDLog.Log("State Queue Clear");
            ClearStateQueue();
        }
    }

    public override VDActionState GetCurrentAction()
    {
        return currentAction;
    }

    public override void SetActionMachineStatus(ActionMachineStatus status)
    {
        machineStatus = status;
    }

    public override ActionMachineStatus GetActionMachineStatus()
    {
        return machineStatus;
    }

    public override void BreakUntil(object untilState)
    {
        if (machineStatus == ActionMachineStatus.BreakUntil)
        {
            return;
        }

        machineStatus = ActionMachineStatus.BreakUntil;
        nextWaitState = (MainCharacterActionType)untilState;
        VDLog.Log("State break Until " + untilState);
    }

    public MainCharacterActionType GetCurrentState()
    {
        return currentState;
    }

    public bool IsChangeStateInProcessed()
    {
        bool result = coroutineState != null;
        return result;
    }

    private void ClearStateQueue()
    {
        stateQueueDatas.Clear();
        character.StopCoroutine(coroutineState);
        coroutineState = null;
    }

    private void RemoveStateQueue(object stateToRemove)
    {
        stateQueueDatas.RemoveAll(item => item.state == stateToRemove);
    }
}
