using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FMEndingController : MonoBehaviour
{
    [SerializeField] private Transform root;

    [Header("Good Ending")]
    [SerializeField] private Transform rootGoodEnding;
    [SerializeField] private Transform rootCamera;
    [SerializeField] private Animator animGoodEnding;
    [SerializeField] private CinemachineVirtualCamera vCamGoodEnding;

    public void ShowGoodEnding()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();

        FMMainCharacter character = currentWorld.GetCharacter() as FMMainCharacter;
        FMMainCameraController cameraController = currentWorld.CameraController as FMMainCameraController;
        cameraController.DisableAllCamera();
        animGoodEnding.enabled = true;
        rootGoodEnding.gameObject.SetActive(true);
        rootCamera.gameObject.SetActive(true);
        character.ShowVisual(false);
        //vCamGoodEnding.Follow = character.CameraTarget;
    }

    public bool IsGoodEndingEnd()
    {
        AnimatorStateInfo stateInfo = animGoodEnding.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName("Show") && stateInfo.normalizedTime >= 1;
        return result;
    }
}
