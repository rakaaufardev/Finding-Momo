using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMSurfCharacterActionMachine_Idle : FMCharacterActionState
{
    private FMMainScene mainScene;
    private SurfWorld surfWorld;
    private FMSurfCharacter surfCharacter;

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            surfCharacter = inCharacter as FMSurfCharacter;
            inputController = surfCharacter.GetInputController();
            actionMachine = surfCharacter.GetActionMachine();

            mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
        }
    }

    public override IEnumerator Enter(object prev, params object[] values)
    {
        surfCharacter.RootVisual.localEulerAngles = new Vector3(0, 180, 0);
        surfCharacter.TriggerAnimation("Idle");
        surfCharacter.RootFoamEffect.gameObject.SetActive(true);
        yield return null;
    }

    public override void DoUpdate()
    {
#if UNITY_STANDALONE
        if (inputController.IsStartRunningPressed())
        {
            UIWindowSurfMission surfMissionPopup = FMUIWindowController.Get.CurrentWindow as UIWindowSurfMission;

            if (surfMissionPopup != null)
            {
                SetStateToRun();
            }
        }
#endif
    }

    public void SetStateToRun()
    {
        actionMachine.SetState(SurfCharacterActionType.Run);
        mainScene.GameStatus = GameStatus.Play;
    }

    public override void DoFixedUpdate()
    {

    }

    public override IEnumerator Exit(object next)
    {
        //surfCharacter.transform.SetParent(surfWorld.RootCharacter);
        surfWorld.CameraController.ActivateCamera(SurfCameraMode.SideView);
        //surfWorld.SurfBackground.HideIdleBoat();
        yield return null;
    }
}
