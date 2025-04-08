using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VD;

public class FMSurfCharacterActionMachine : VDActionMachine
{
    private VDInputController inputController;
    private SurfCharacterActionType currentState;
    private Dictionary<SurfCharacterActionType, FMCharacterActionState> stateData;

    public override void Init()
    {
        nextWaitState = SurfCharacterActionType.NONE;
        stateExceptions = new List<object>();

        stateQueueDatas = new List<StateQueueData>();
        stateData = new Dictionary<SurfCharacterActionType, FMCharacterActionState>()
        {
            { SurfCharacterActionType.Idle, new FMSurfCharacterActionMachine_Idle() },
            { SurfCharacterActionType.Run, new FMSurfCharacterActionMachine_Run() },
            { SurfCharacterActionType.GameOver, new FMSurfCharacterActionMachine_GameOver() },
            { SurfCharacterActionType.Finish, new FMSurfCharacterActionMachine_Finish() },
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
            if (currentAction != null)
            {
                currentAction.DoUpdate();
            }
        }

    }

    public override void DoFixedUpdate()
    {
        if (currentAction != null)
        {
            currentAction.DoFixedUpdate();
        }
    }

    public override void SetState(object state, params object[] values)
    {
        switch (machineStatus)
        {
            case ActionMachineStatus.BreakUntil:
                if ((SurfCharacterActionType)state != (SurfCharacterActionType)nextWaitState)
                {
                    VDLog.Log("State is Break until " + nextWaitState);
                    return;
                }
                else
                {
                    VDLog.Log("State is Continue");
                    nextWaitState = SurfCharacterActionType.NONE;
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
            coroutineState = character.StartCoroutine(StateChange((SurfCharacterActionType)state, values));
        }
        else
        {
            StateQueueData stateQueueData = new StateQueueData();
            stateQueueData.state = state;
            stateQueueData.values = values;
            stateQueueDatas.Add(stateQueueData);
        }
    }

    IEnumerator StateChange(SurfCharacterActionType state, params object[] values)
    {
        SurfCharacterActionType prevState = SurfCharacterActionType.NONE;
        if (currentAction != null)
        {
            prevState = GetCurrentState();
            yield return currentAction.Exit(state);
            ((FMCharacterActionState)currentAction).StateReady = false;
        }

        currentAction = stateData[state];
        ((FMCharacterActionState)currentAction).Init(character);
        yield return currentAction.Enter(prevState, values);
        ((FMCharacterActionState)currentAction).StateReady = true;

        currentState = state;
        VDLog.Log("currentState: " + currentState);

        if (stateQueueDatas.Count > 0)
        {
            StateQueueData stateQueueData = stateQueueDatas[0];
            SurfCharacterActionType nextState = (SurfCharacterActionType)stateQueueData.state;
            object[] nextValues = stateQueueData.values;
            stateQueueDatas.RemoveAt(0);

            bool breakState = prevState == SurfCharacterActionType.GameOver;

            VDLog.Log("Prev State : " + prevState);
            VDLog.Log("State Next Queue : " + nextState);
            if (!breakState)
            {
                VDLog.Log("State Change");

                switch (machineStatus)
                {
                    case ActionMachineStatus.Stop:
                        ClearStateQueue();
                        break;
                    case ActionMachineStatus.Active:
                        yield return StateChange(nextState, nextValues);
                        break;
                }
            }
            else
            {
                VDLog.Log("State break");
                ClearStateQueue();
            }
        }
        else
        {
            ClearStateQueue();
        }
    }

    public override void SetCharacter(VDCharacter inCharacter)
    {
        character = inCharacter;
    }

    public override void SetStateException(object state)
    {
        if (stateExceptions != null)
        {
            stateExceptions = new List<object>();
        }

        stateExceptions.Add((MainCharacterActionType)state);
    }

    public override void SetActionMachineStatus(ActionMachineStatus status)
    {
        machineStatus = status;
    }

    public override ActionMachineStatus GetActionMachineStatus()
    {
        return machineStatus;
    }

    public override VDActionState GetCurrentAction()
    {
        return currentAction;
    }

    public override void ClearAllStateException()
    {
        stateExceptions.Clear();
    }

    public override void ClearStateException(object state)
    {
        stateExceptions.Remove((MainCharacterActionType)state);
    }

    public override bool IsStateExceptionExist()
    {
        bool result = stateExceptions.Count > 0;
        return result;
    }

    public override void BreakUntil(object untilState)
    {
        if (machineStatus == ActionMachineStatus.BreakUntil)
        {
            return;
        }

        machineStatus = ActionMachineStatus.BreakUntil;
        nextWaitState = (SurfCharacterActionType)untilState;
        VDLog.Log("State break Until " + untilState);
    }

    public SurfCharacterActionType GetCurrentState()
    {
        return currentState;
    }

    private void ClearStateQueue()
    {
        stateQueueDatas.Clear();
        character.StopCoroutine(coroutineState);
        coroutineState = null;
    }
}
