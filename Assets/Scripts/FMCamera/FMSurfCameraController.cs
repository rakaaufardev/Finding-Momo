using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum SurfCameraMode
{
    SideView,
    JumpView,
    IdleView
}

public class FMSurfCameraController : FMCameraController
{
    [SerializeField] private FMCameraObject sideViewCamera;
    [SerializeField] private FMCameraObject jumpViewCamera;
    [SerializeField] private FMCameraObject surfViewCamera;
    private FMSurfCharacter character;

    public override void Init()
    {
        sideViewCamera.Init();
        jumpViewCamera.Init();

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();

        character = currentWorld.GetCharacter() as FMSurfCharacter;

        WorldType worldType = mainScene.GetCurrentWorldType();
        CinemachineBlenderSettings cameraBlenderSettings = FMAssetFactory.GetCameraBlenderSetting(worldType);
        FMSceneController.Get().CameraCinemachineBrain.m_CustomBlends = cameraBlenderSettings;

        sideViewCamera.VirtualCamera.Follow = character.CameraTarget;
        jumpViewCamera.VirtualCamera.Follow = character.CameraTarget;
        surfViewCamera.VirtualCamera.Follow = character.CameraTarget;
    }

    public override void ActivateCamera(object camMode)
    {
        sideViewCamera.gameObject.SetActive((SurfCameraMode)camMode == SurfCameraMode.SideView);
        jumpViewCamera.gameObject.SetActive((SurfCameraMode)camMode == SurfCameraMode.JumpView);
        surfViewCamera.gameObject.SetActive((SurfCameraMode)camMode == SurfCameraMode.IdleView);
    }

    public override void DisableAllCamera()
    {
        sideViewCamera.gameObject.SetActive(false);
        jumpViewCamera.gameObject.SetActive(false);
        surfViewCamera.gameObject.SetActive(false);
    }
}
