using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMMainCharacterActionState_TransitionSideToBack : FMCharacterActionState
{
    bool changeTrackTriggered;
    FMMainScene mainScene;
    FMDirectionalLight directionalLight;

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

        changeTrackTriggered = false;
    }

    public override IEnumerator Enter(object prevState, params object[] values)
    {
        //character.TriggerAnimation("Run");
        ((FMMainCharacter)character).Slide(false);
        yield return null;
    }

    public override void DoUpdate()
    {
        Vector3 characterPoint = ((FMMainCharacter)character).GetCharacterNormalizePosition();
        float endTrackPointDistance = Vector3.Distance(((FMMainCharacter)character).CurrentEndTrackPoint, characterPoint);
        bool readyToChangeTrack = endTrackPointDistance <= VDParameter.TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE;
        if (readyToChangeTrack)
        {
            if (inputController.IsChangeTransitionSideToBackPressed() || ((FMMainCharacter)character).IsSpeedUp)
            {
                TransitionBackView();
            }
        }

        if (!changeTrackTriggered)
        {
            ((FMMainCharacter)character).Move();
        }
    }
    public void TransitionBackView()
    {
        actionMachine.SetState(MainCharacterActionType.Run);

        mainScene.DirectionalLight.RotateDirectionalLight(((FMMainCharacter)character).CharacterViewMode);

        changeTrackTriggered = true;
    }
    public override void DoFixedUpdate()
    {
        ((FMMainCharacter)character).SupportForce();
    }

    public override IEnumerator Exit(object next)
    {
        bool shouldContinue = false;

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        MainWorld mainWorld = currentWorld as MainWorld;
        UIMain uiMain = mainScene.GetUI();

        uiMain.HideHandSwipe();

        currentWorld.ResetColliderObjects();
        mainWorld.ResetTrampolineHitboxes();

        ((FMMainCharacter)character).IsTransition = false;
        ((FMMainCharacter)character).QtaIsMissed = false;
        ((FMMainCharacter)character).QtaIsComplete = false;

        ((FMMainCharacter)character).SetNextEndTrackId();
        ((FMMainCharacter)character).SetCurrentEndTrack();
        ((FMMainCharacter)character).SetBackViewLane();
        
        mainWorld.ChangeViewMode();
        mainWorld.SpawnPlatformTriggered = false;

        uiMain.ShowStaticMessage(false);

        actionMachine.ClearAllStateException();

        if(((FMMainCharacter)character).IsTutorial && ((FMMainCharacter)character).CurrentTutorialState == TutorialState.TrashBinLeft)
        {
            int lineId = ((FMMainCharacter)character).LineId;
            switch (lineId)
            {
                case 0:
                    ((FMMainCharacter)character).CurrentTutorialState = TutorialState.TrashBinLeft;
                    break;
                case 1:
                    ((FMMainCharacter)character).CurrentTutorialState = TutorialState.TrashBinMiddle;
                    break;
                case 2:
                    ((FMMainCharacter)character).CurrentTutorialState = TutorialState.TrashBinRight;
                    break;
            }
        }

        if ((MainCharacterActionType)next != MainCharacterActionType.Hit && (MainCharacterActionType)next != MainCharacterActionType.GameOver)
        {
            ((FMMainCharacter)character).TriggerAnimation("TurnLeft");

            while (!shouldContinue)
            {
                shouldContinue = ((FMMainCharacter)character).IsAnimationState("TurnLeft");
                yield return new WaitForEndOfFrame();
            }

            shouldContinue = false;
            Vector3 rotateDir = ((FMMainCharacter)character).GetRotation(((FMMainCharacter)character).CharacterViewMode);
            float offsetDuration = 0.75f;
            float rotateDuration = ((FMMainCharacter)character).GetAnimationLength();
            rotateDuration *= offsetDuration;

            character.transform.DORotate(rotateDir, rotateDuration).OnComplete(()=>
            {
                shouldContinue = true;
            }).SetEase(Ease.Linear);

            while (!shouldContinue)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
