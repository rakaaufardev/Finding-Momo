using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VD;

public class SurfWorld : FMWorld
{
    [SerializeField] private Transform rootCharacter;
    [SerializeField] private Transform rootAnimal;
    [SerializeField] private FMSurfBackground surfBackground;
    [SerializeField] private FMParallaxBackground bridgeBackground;
    [SerializeField] private FMParallaxBackground cruiseBackground;
    private FMSurfCharacterActionMachine characterActionMachine;
    private UIMain uiMain;
    private FMMainScene mainScene;

    public override void StartGame(params object[] transferVariables)
    {
#if ENABLE_CHEAT
        VDDebugTool.Get().AddDebugWorld_Surf();
#endif
        
        int coinCollected = (int)transferVariables[0];

        FMSceneController.Get().Camera.transform.position = Vector3.zero;

        mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        mainScene.EnableHitboxCollider(true);

        WorldType worldType = mainScene.GetCurrentWorldType();
        uiMain = mainScene.GetUI();
        uiMain.SetHUD(worldType);
        uiMain.UpdateDistanceLine(0);

        mainScene.GameStatus = GameStatus.Idle;

        FMSurfCharacter newCharacter = FMAssetFactory.GetSurfCharacter(rootCharacter);
        character = newCharacter;
        ((FMSurfCharacter)character).CoinCollected = coinCollected;
        ((FMSurfCharacter)character).Init();
        uiMain.UpdateCoinText(coinCollected);

        FMPlatformController platformController = FMPlatformController.Get();
        platformController.ResetIndexes();
        platformController.InitSurfPlatforms();
        platformController.LoadPlatformSetup();
        platformController.GenerateSurfPlatformData();
        GenerateNewPlatform(newCharacter.IsInTutorial);

        colliderObjectToReset = new List<PlatformColliderObject>();

        cameraController.Init();
        cameraController.ActivateCamera(SurfCameraMode.SideView);

        bridgeBackground.SetTarget(((FMSurfCharacter)character).transform, VDParameter.PARALLAX_SURF_GAME_MULTIPLIER_X);
        cruiseBackground.SetTarget(((FMSurfCharacter)character).transform, VDParameter.PARALLAX_SURF_GAME_SHIP);
        surfBackground.SetFollowTarget(((FMSurfCharacter)character).transform);
        //surfBackground.SetCharacter(character.transform);

        StartCoroutine(mainScene.CloseMiniGameTransition(true, ((FMSurfCharacter)character).IsInTutorial));
    }

    public override void PlayGame()
    {
        mainScene.GameStatus = GameStatus.Play;
        ((FMSurfCharacter)character).OnGamePause(false);
    }

    public override void PauseGame()
    {
        mainScene.GameStatus = GameStatus.Pause;
        ((FMSurfCharacter)character).OnGamePause(true);
        /*FMUIPopupController.Get().OpenPopup(UIPopupType.SurfMinigame);*/
    }

    private void GenerateNewPlatform(bool isTutorial)
    {
        FMPlatformController platformController = FMPlatformController.Get();

        //generate data
        if (platformController.IsPlatformReachEnd(platformController.SurfPlatformObjectIndex))
        {
            platformController.GenerateSurfPlatformData();
        }

        //generate platform
        int totalSpawn = VDParameter.TOTAL_PLATFORM_SPAWN;
        for (int i = 0; i < totalSpawn; i++)
        {
            platformController.GenerateSurfPlatform(isTutorial);
        }

        if (colliderObjectToReset != null)
        {
            ResetColliderObjects();
        }
    }

    public void OnSurfEnd()
    {
        ResetColliderObjects();
        StartCoroutine(SurfEndSequence());
    }

    private IEnumerator SurfEndSequence()
    {
        bool shouldContinue = false;
        
        FMPlatformController platformController = FMPlatformController.Get();
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        Transform rootPortal = surfBackground.RootPortal;

        FMUIWindowController.Get.OpenWindow(UIWindowType.FadeScreen);
        UIWindowFadeScreen popup = FMUIWindowController.Get.CurrentWindow as UIWindowFadeScreen;

        Physics.IgnoreLayerCollision(VDParameter.LAYER_CHARACTER, VDParameter.LAYER_OBSTACLE,true);
        Physics.IgnoreLayerCollision(VDParameter.LAYER_CHARACTER, VDParameter.LAYER_COIN,true);

        while (!shouldContinue)
        {
            shouldContinue = popup.IsFadeInEnd();
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;
        popup.PlayFadeOut();
        platformController.gameObject.SetActive(false);
        surfBackground.ShowLandForeground();

        Physics.IgnoreLayerCollision(VDParameter.LAYER_CHARACTER, VDParameter.LAYER_OBSTACLE, false);
        Physics.IgnoreLayerCollision(VDParameter.LAYER_CHARACTER, VDParameter.LAYER_COIN, false);

        while (!shouldContinue)
        {
            shouldContinue = popup.IsFadeOutEnd();
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        FMUIWindowController.Get.CloseWindow();

        Sequence seq = DOTween.Sequence();
        seq.Append(character.transform.DOLocalMoveY(3f, 1.5f).OnUpdate(()=>
        {
            ((FMSurfCharacter)character).CharacterRigidBody.velocity = new Vector3(((FMSurfCharacter)character).CharacterRigidBody.velocity.x, ((FMSurfCharacter)character).CharacterRigidBody.velocity.y, ((FMSurfCharacter)character).CharacterRigidBody.velocity.z);
        }));
        seq.Append(rootPortal.transform.DOLocalMoveX(4f, 1.5f));
        seq.AppendCallback(() =>
        {
            mainScene.SwitchToMainGame();
        });
    }

    protected void Update()
    {
        if (mainScene.GameStatus == GameStatus.Transition)
        {
            return;
        }

        if (characterActionMachine != null)
        {
            SurfCharacterActionType state = characterActionMachine.GetCurrentState();

            if (state == SurfCharacterActionType.Run)
            {
                int lastIndex = FMPlatformController.Get().Platforms.Count - 1;
                Vector3 endTrackPoint = FMPlatformController.Get().Platforms[lastIndex].GetEndTrackPoint();
                Vector3 direction = character.transform.position - endTrackPoint;
                float distance = direction.magnitude;

                if (distance <= VDParameter.DISTANCE_TRIGGER_SPAWN_MAIN_PLATFORM)
                {
                    GenerateNewPlatform(((FMSurfCharacter)character).IsInTutorial);
                }

            }
            else if (state == SurfCharacterActionType.Idle)
            {
                character.transform.position = new Vector3(-15, 7.5f, 0);
            }

            surfBackground.Move();
        }
        else
        {
            characterActionMachine = character.GetActionMachine() as FMSurfCharacterActionMachine;
        }

        bridgeBackground.DoParallax();
        cruiseBackground.DoParallax();
    }

    public void CheckAnimalActive()
    {
        if (surfBackground.SurfAnimalSpawner != null)
        {
            FMMissionController.Get().CheckSurfMainMissionProgress(surfBackground.SurfAnimalSpawner);
            SetAnimalBackToParent();
        }

    }

    public void DeactiveAllAnimal()
    {
        if (surfBackground.SurfAnimalSpawner != null)
        {
            surfBackground.SurfAnimalSpawner.DeactivateAllAnimals();
        }

    }

    public void SpawnAnimal()
    {
        DOVirtual.DelayedCall(2f, () =>
        {
            surfBackground.SurfAnimalSpawner.SetRootParent(rootAnimal,false);   
        });
    }

    public void SetAnimalBackToParent()
    {
        surfBackground.SurfAnimalSpawner.SetActiveObstacle();
        surfBackground.SurfAnimalSpawner.SetRootParent(surfBackground.RootAnimalSpawn, true);
    }

    public void SetAnimalShadow(bool isTrue)
    {
        surfBackground.SurfAnimalSpawner.SetAnimalShadow(isTrue);
    }

    public Transform GetAnimalTransform()
    {
        return surfBackground.SurfAnimalSpawner.RootAnimalSurf;
    }

    public Vector3 GetAnimalParentPosition()
    {
        return surfBackground.RootAnimalSpawn.position;
    }
}
