using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMMainCharacterActionState_TransitionBackToSide : FMCharacterActionState
{
    bool changeTrackTriggered;
    FMMainScene mainScene;
    FMDirectionalLight directionalLight;
    TutorialState currentTutorialState;
    
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
        actionMachine.ClearStateException(MainCharacterActionType.Run);
        float qtaSpeedAnim = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_SPEED_MULTIPLIER;
        ((FMMainCharacter)character).SetAnimationSpeed(qtaSpeedAnim);
        ((FMMainCharacter)character).Slide(false);
        //character.TriggerAnimation("Run");
        yield return null;
    }

    public override void DoUpdate()
    {
        if (inputController.IsChangeTransitionBackToSidePressed() || ((FMMainCharacter)character).IsSpeedUp )
        {
            if (!((FMMainCharacter)character).QtaIsMissed && !((FMMainCharacter)character).QtaIsComplete)
            {
                ((FMMainCharacter)character).CheckQTAResult();

                mainScene.DirectionalLight.RotateDirectionalLight(((FMMainCharacter)character).CharacterViewMode);

                changeTrackTriggered = true;
            }
        }

        if (!changeTrackTriggered)
        {
            ((FMMainCharacter)character).Move();
        }
    }

    public override void DoFixedUpdate()
    {

    }

    public override IEnumerator Exit(object next)
    {
        bool shouldContinue = false;

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        MainWorld mainWorld = currentWorld as MainWorld;
        UIMain uiMain = mainScene.GetUI();

        uiMain.HideHandTap();

        currentWorld.ResetColliderObjects();
        mainWorld.ResetTrampolineHitboxes();

        float animationSpeed = ((FMMainCharacter)character).AnimationSpeed;
        ((FMMainCharacter)character).IsTransition = false;
        ((FMMainCharacter)character).SetNormalSpeed();
        ((FMMainCharacter)character).SetAnimationSpeed(animationSpeed);

        ((FMMainCharacter)character).SetNextEndTrackId();
        ((FMMainCharacter)character).SetCurrentEndTrack();
        ((FMMainCharacter)character).SetSideViewLane();
        
        mainWorld.ChangeViewMode();
        mainWorld.SpawnPlatformTriggered = false;

        actionMachine.ClearAllStateException();

        if ((MainCharacterActionType)next != MainCharacterActionType.Hit && (MainCharacterActionType)next != MainCharacterActionType.GameOver)
        {
            ((FMMainCharacter)character).TriggerAnimation("TurnRight");

            while (!shouldContinue)
            {
                shouldContinue = ((FMMainCharacter)character).IsAnimationState("TurnRight");
                yield return new WaitForEndOfFrame();
            }

            shouldContinue = false;
            Vector3 rotateDir = ((FMMainCharacter)character).GetRotation(((FMMainCharacter)character).CharacterViewMode);
            float offsetDuration = 0.75f;
            float rotateDuration = ((FMMainCharacter)character).GetAnimationLength();
            rotateDuration *= offsetDuration;

            character.transform.DORotate(rotateDir, rotateDuration).OnComplete(() =>
            {
                shouldContinue = true;
            }).SetEase(Ease.Linear);

            while (!shouldContinue)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        if (((FMMainCharacter)character).IsStartQTA)
        {
            ((FMMainCharacter)character).IsStartQTA = false;
        }

        if (uiMain.QTAResultSequence != null)
        {
            uiMain.SkipQtaResult();
        }

        bool isQTAUIActive = uiMain.IsQTAUIActive();
        if (isQTAUIActive)
        {
            if (currentTutorialState == TutorialState.NONE && ((FMMainCharacter)character).IsTutorial)
            {
                ((FMMainCharacter)character).OnEndTutorial();
            }
            uiMain.ShowQTA(false, false);
        }

    }
}
