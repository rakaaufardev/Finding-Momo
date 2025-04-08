using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum MainCameraMode
{
    SideView,
    BackView,
    TrampolineView,
    TransitionCamera,
    PortalCamera
}

public class FMMainCameraController : FMCameraController
{
    [SerializeField] private FMCameraObject sideViewCamera;
    [SerializeField] private FMCameraObject backViewCamera;
    [SerializeField] private FMCameraObject trampolineViewCamera;
    [SerializeField] private FMCameraObject transitionCameraTutorial;
    [SerializeField] private FMCameraObject portalCameraTutorial;
    private FMMainCharacter character;

    public override void Init()
    {
        sideViewCamera.Init();
        backViewCamera.Init();
        trampolineViewCamera.Init();

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();

        character = currentWorld.GetCharacter() as FMMainCharacter;

        WorldType worldType = mainScene.GetCurrentWorldType();
        CinemachineBlenderSettings cameraBlenderSettings = FMAssetFactory.GetCameraBlenderSetting(worldType);
        FMSceneController.Get().CameraCinemachineBrain.m_CustomBlends = cameraBlenderSettings;

        sideViewCamera.VirtualCamera.Follow = character.CameraTarget;
        backViewCamera.VirtualCamera.Follow = character.CameraTarget;
        trampolineViewCamera.VirtualCamera.Follow = character.CameraTarget;
    }

    public override void ActivateCamera(object camMode)
    {
        sideViewCamera.gameObject.SetActive((MainCameraMode)camMode == MainCameraMode.SideView);
        backViewCamera.gameObject.SetActive((MainCameraMode)camMode == MainCameraMode.BackView);
        trampolineViewCamera.gameObject.SetActive((MainCameraMode)camMode == MainCameraMode.TrampolineView);

        if (transitionCameraTutorial != null)
        {
            CinemachineVirtualCamera transitionCamera = transitionCameraTutorial.GetComponent<CinemachineVirtualCamera>();
            transitionCamera.enabled = (MainCameraMode)camMode == MainCameraMode.TransitionCamera;
        }
        if (portalCameraTutorial != null)
        {
            CinemachineVirtualCamera portalCamera = portalCameraTutorial.GetComponent<CinemachineVirtualCamera>();
            portalCamera.enabled = (MainCameraMode)camMode == MainCameraMode.PortalCamera;
        }
    }

    public override void DisableAllCamera()
    {
        sideViewCamera.gameObject.SetActive(false);
        backViewCamera.gameObject.SetActive(false);
    }

    public void FindTransitionCameraTutorial()
    {
        if (transitionCameraTutorial == null)
        {
            GameObject cameraObject = GameObject.FindGameObjectWithTag("TransitionCameraTutorial");
            if (cameraObject != null)
            {
                transitionCameraTutorial = cameraObject.GetComponent<FMCameraObject>();
                transitionCameraTutorial.Init();
            }
        }
    }

    public void FindPortalCameraTutorial()
    {
        if (portalCameraTutorial == null)
        {
            GameObject cameraObject = GameObject.FindGameObjectWithTag("PortalCameraTutorial");
            if (cameraObject != null)
            {
                portalCameraTutorial = cameraObject.GetComponent<FMCameraObject>();
                portalCameraTutorial.Init();
            }
        }
    }

    public void DisableTutorialCamera()
    {
        transitionCameraTutorial.gameObject.SetActive(false);
    }

    public void SetDampingSpeedCamera(bool isSpeedUpActive)
    {
        if (isSpeedUpActive)
        {
            backViewCamera.SetDamping(0,0,0);
        }
        else
        {
            backViewCamera.SetDamping(0.5f,1,0);
        }
    }
}
