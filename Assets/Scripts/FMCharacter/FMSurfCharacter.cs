using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VD;

public enum Surfboard
{
    Cool,
    Flame,
    Wavy,
    Swirly,
    COUNT
}

public class FMSurfCharacter : VDCharacter
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform rootVisual;
    [SerializeField] private Transform rootFoamEffect;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Rigidbody characterRigidbody;

    private float speed;
    private float defaultSpeed;
    private float lastPosX;
    private int coinCollected;
    private Costume currentCostume;
    private Transform transformCostume;
    private Animator animCostume;
    private Transform transformSurfboard;
    private FMPlatformController platformController;
    private FMScoreController scoreController;
    private FMUnityMainCamera camera;
    private Coroutine safeAnimalCoroutine;

    //Tutorial
    private bool isSurfTutorial;
    [SerializeField] private GameObject tutorialUI;
    private const string TUTORIAL_SECTION_SURF = "Surf";
    private const string TUTORIAL_SECTION_SURF_INTRO = "SurfIntro";

    public Transform RootVisual
    {
        get
        {
            return rootVisual;
        }
        set
        {
            rootVisual = value;
        }
    }

    public int CoinCollected
    {
        get
        {
            return coinCollected;
        }
        set
        {
            coinCollected = value;
        }
    }

    public Rigidbody CharacterRigidBody
    {
        get
        {
            return characterRigidbody;
        }
        set
        {
            characterRigidbody = value;
        }
    }

    public Animator AnimCostume
    {
        get
        {
            return animCostume;
        }
    }

    public Transform TransformSurfboard
    {
        get
        {
            return transformSurfboard;
        }
    }

    public Transform CameraTarget
    {
        get
        {
            return cameraTarget;
        }
    }

    public Transform RootFoamEffect
    {
        get
        {
            return rootFoamEffect;
        }
    }

    public Coroutine SafeAnimalCoroutine
    {
        get
        {
            return safeAnimalCoroutine;
        }
        set
        {
            safeAnimalCoroutine = value;
        }
    }

    public FMUnityMainCamera Camera
    {
        get
        {
            return camera;
        }
    }

    public bool IsInTutorial
    {
        get => isSurfTutorial;
    }

    public void Init()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();

        defaultSpeed = VDParameter.CHARACTER_SURF_SPEED;
        speed = defaultSpeed;
     
        FMSurfCharacterActionMachine characterActionMachine = new FMSurfCharacterActionMachine();
#if UNITY_STANDALONE
        inputController = new FMKeyboardController();
#elif UNITY_ANDROID
        inputController = new FMTouchScreenController();
#endif

        characterActionMachine.Init();
        characterActionMachine.SetCharacter(this);

        actionMachine = characterActionMachine; 
        platformController = FMPlatformController.Get();
        scoreController = mainScene.GetScoreController();
        camera = FMSceneController.Get().Camera;
        scoreController.MiniGameNormalScore = 0;
        FMMissionController.Get().ResetMainMissionProgress();

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        currentCostume = inventoryData.inUsedCostume;

        FMCostume costume = FMAssetFactory.GetCostume(currentCostume, WorldType.Surf);

        transformCostume = costume.transform;
        transformCostume.SetParent(rootVisual);
        transformCostume.localPosition = Vector3.zero;
        transformCostume.localScale = Vector3.one * 1.15f;

        animCostume = costume.Anim;

        FMSurfboard surfboard = FMAssetFactory.GetSurfboard(inventoryData.inUsedSurfboard);
        transformSurfboard = costume.Surfboard;
        transformSurfboard = surfboard.transform;
        transformSurfboard.SetParent(costume.Surfboard);

        actionMachine.SetActionMachineStatus(ActionMachineStatus.Active);
        characterActionMachine.SetState(SurfCharacterActionType.Idle);

        if (!UITutorial.IsDone("Finish"))
        {
            isSurfTutorial = true;
        }
    }

    public override void DoUpdate()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        UIMain uiMain = mainScene.GetUI();

#if UNITY_ANDROID
        //touch screen controller
        ((FMTouchScreenController)inputController).DoUpdateInGame();
#endif

        actionMachine.DoUpdate();

        SurfCharacterActionType currentState = ((FMSurfCharacterActionMachine)actionMachine).GetCurrentState();
        VDActionState currentAction = ((FMSurfCharacterActionMachine)actionMachine).GetCurrentAction();

        //score
        if (mainScene.GameStatus == GameStatus.Play)
        {
            if (currentState != SurfCharacterActionType.Idle && currentState != SurfCharacterActionType.GameOver)
            {
                //normal score
                float currentPosX = Mathf.Abs(transform.position.x);
                if (currentPosX > lastPosX)
                {
                    float deltaX = currentPosX - lastPosX;

                    if (deltaX > VDParameter.ONE_UNIT_VALUE)
                    {
                        //scoreController.MiniGameNormalScore += 1; //Distance main mission
                        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission,SurfMissionType.SurfDistance, 1);
                        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, SurfMissionType.SurfDistance, 1);
                        lastPosX = currentPosX;

                        //air time score
                        if (currentState == SurfCharacterActionType.Run)
                        {
                            bool isHighJump = ((FMSurfCharacterActionMachine_Run)currentAction).IsHighJump;
                            if (isHighJump)
                            {
                                scoreController.SurfAirTime += Time.deltaTime;
                                FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission,SurfMissionType.AirTime, Time.deltaTime);
                                FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, SurfMissionType.AirTime, Time.deltaTime);
                                //debug
                                //string testAirTimeFormat = VDUtility.GetTimeStringFormat(scoreController.SurfAirTime);
                                //Debug.Log("testAirTimeFormat " + testAirTimeFormat);
                            }
                        }
                    }
                }

                //time accumulative score
                scoreController.UpdateSurfTimeAccumulative(Time.deltaTime);
                FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission,SurfMissionType.TimePlayed, Time.deltaTime);
                FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, SurfMissionType.TimePlayed, Time.deltaTime);
            }
        }

        //tutorial
        if (isSurfTutorial)
        {
            UIWindowTransitionSurf transitionSurf = FMUIWindowController.Get.CurrentWindow as UIWindowTransitionSurf;

            if (tutorialUI == null && transitionSurf == null)
            {
                UITutorial.Create(TUTORIAL_SECTION_SURF);
                tutorialUI = GameObject.FindGameObjectWithTag("UITutorial_ContentTutorial");
            }

            if (UITutorial.IsTutorialActive())
            {
                string currentTutorialSection = UITutorial.GetCurrentTutorialSection();

                switch (currentTutorialSection)
                {
                    case TUTORIAL_SECTION_SURF_INTRO:
                        if (inputController.IsProceedPressed())
                        {
                            UITutorial.OnNext();
                        }
                        break;
                    case TUTORIAL_SECTION_SURF:
                        OnGamePause(true);
                        SetAnimationSpeed(1);
                        if (inputController.IsProceedPressed())
                        {
                            UITutorial.OnNext();

                            int currentStep = UITutorial.GetCurrentTutorialStep();

                            TutorialCurrentStep(currentStep);
                        }
                        break;
                }
            }
        }
    }

    public override void DoFixedUpdate()
    {
        actionMachine.DoFixedUpdate();
    }

    public override void DoLateUpdate()
    {

    }

    private void TutorialCurrentStep(int currentStep)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        switch (currentStep)
        {
            case 2:
                uiMain.ShowHandTap(new Vector3(700, -200));
                break;
            case 3:
                uiMain.HideHandTap();
                break;
            case 4:
                OnGamePause(false);
                /*isSurfTutorial = false;*/

                SurfWorld surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
                surfWorld.SpawnAnimal();

                mainScene.GameStatus = GameStatus.Play;
                actionMachine.SetState(SurfCharacterActionType.Run);
                break;
        }
    }

    public void SetTutorialMessageUI(bool isShow)
    {
        tutorialUI.SetActive(isShow);
    }

    public void TriggerAnimation(string trigger)
    {
        animCostume.SetTrigger(trigger);
    }

    public void ResetAnimation(string trigger)
    {
        animCostume.ResetTrigger(trigger);
    }

    public void Move()
    {
        Vector3 moveDirection = new Vector3(speed, characterRigidbody.velocity.y, 0);
        characterRigidbody.velocity = moveDirection;
    }

    public void QuickFall()
    {
        characterRigidbody.AddForce(Vector3.down * VDParameter.CHARACTER_GRAVITY_QUICK_FALL, ForceMode.Acceleration);
    }

    public void OnGamePause(bool isPause)
    {
        if (isPause)
        {
            speed = 0;
            SetAnimationSpeed(0);
            characterRigidbody.isKinematic = true;
        }
        else
        {
            speed = defaultSpeed;
            SetAnimationSpeed(1);
            characterRigidbody.isKinematic = false;
        }
    }

    public void OnGameOver()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;

        mainScene.GameStatus = GameStatus.Finish;
        DOTween.To(() => speed, x => speed = x, 0f, 1f).OnComplete(() =>
        {
            mainScene.SwitchToMainGame();
        });
    }

    public void SetAnimationSpeed(float speed)
    {
        animCostume.speed = speed;
    }

    public void OnHitSafeReward(SafeRewardObstacle safeRewardObstacle, FMSurfAnimalSpawn surfAnimalSpawner)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        UIMain uiMain = mainScene.GetUI();
        SurfWorld surfWorld = currentWorld as SurfWorld;

        SurfMissionType missionType = SurfMissionType.COUNT;
        string safeRewardName = safeRewardObstacle.RewardObstacleType.ToString();
        switch (safeRewardObstacle.RewardObstacleType)
        {
            case RewardObstacleType.SeaTurtle:
                missionType = SurfMissionType.SafeAnimal_SeaTurtle;
                break;
            case RewardObstacleType.Dolphin:
                missionType = SurfMissionType.SafeAnimal_Dolphin;
                break;
            case RewardObstacleType.SeaLion:
                missionType = SurfMissionType.SafeAnimal_SeaLion;
                break;
            case RewardObstacleType.MantaRay:
                missionType = SurfMissionType.SafeAnimal_MantaRay;
                //isMiniBoss = true;
                //surfWorld.MiniBossTakeDamage();
                break;
            case RewardObstacleType.KillerWhale:
                missionType = SurfMissionType.SafeAnimal_KillerWhale;
                break;
        }

        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission, missionType, 1);
        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission, SurfMissionType.SafeAnimal_All, 1);
        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, missionType, 1);
        Transform originalParent = safeRewardObstacle.transform;
        Transform cameraCenterTransform = camera.CenterCamera;
        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);

        safeRewardObstacle.ApplyDamage(1);
        int healthAnimal = safeRewardObstacle.GetAnimalHealth();
        if (healthAnimal <= 0)
        {
            surfWorld.DeactiveAllAnimal();
            FMSceneController.Get().GetDisplayObject(safeRewardObstacle, safeRewardName, cameraCenterTransform, originalParent);
            safeRewardObstacle.ResetAnimalHealth();
        }
        else
        {
            safeRewardObstacle.PlayNetAnimation(healthAnimal, () =>
            {
                OnNetAnimationComplete(safeRewardObstacle, surfWorld);
            });
        }

        SurfMissionProgressData mainMission = FMMissionController.Get().GetSurfMainMission();
        float maxRescue = isSurfTutorial ? 1 : mainMission.goalValue;
        scoreController.MiniGameNormalScore += 1;
        bool surfFinishValid = scoreController.MiniGameNormalScore >= maxRescue && mainScene.GameStatus == GameStatus.Play;
        if (surfFinishValid)
        {
            FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, SurfMissionType.MainDistance, 1);
            mainScene.GameStatus = GameStatus.Finish;
            actionMachine.SetState(SurfCharacterActionType.Finish);
            ((SurfWorld)currentWorld).OnSurfEnd();
            //mainScene.OnSwitchToMainGame();
        }

        float sliderValue = scoreController.MiniGameNormalScore / maxRescue;
        uiMain.UpdateDistanceLine(sliderValue);
    }

    public void OnNetAnimationComplete(SafeRewardObstacle safeRewardObstacle, SurfWorld surfWorld)
    {
        safeRewardObstacle.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            surfWorld.SetAnimalShadow(true);
            safeRewardObstacle.gameObject.SetActive(false);
            surfWorld.GetAnimalTransform().DOMove(surfWorld.GetAnimalParentPosition(), 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                surfWorld.SetAnimalShadow(false);
                safeRewardObstacle.gameObject.SetActive(true);
                safeRewardObstacle.transform.DOScale(Vector3.one, 0).OnComplete(() =>
                {
                    surfWorld.SetAnimalBackToParent();
                    surfWorld.SpawnAnimal();
                });
            });
        });
    }

    public void OnHitCoin(Coin coin)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        UIMain uiMain = mainScene.GetUI();
        FMGarbageController garbageController = mainScene.GetGarbageController();

        currentWorld.ColliderObjectToReset.Add(coin);

        coinCollected += VDParameter.COIN_VALUE;

        string coinName = coin.CoinName;
        garbageController.CollectGarbage(coinName,1);
        garbageController.CheckGarbageCollectionStatus();
        UpdateHUDGarbage(coinName);

        scoreController.UpdateCoinScore(coinCollected);
        uiMain.UpdateCoinText(coinCollected);

        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission, SurfMissionType.CollectCoins, VDParameter.COIN_VALUE);
        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, SurfMissionType.CollectCoins, VDParameter.COIN_VALUE);
        FMSceneController.Get().PlayParticle("WorldParticle_Coin", coin.transform.position);

        FMSoundController.Get().PlaySFX(SFX.SFX_Coin);
    }

    public void OnHitObstacle()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        UIMain uiMain = mainScene.GetUI();

        StopAllCoroutines();

        uiMain.EnablePauseButton(false);
        mainScene.EnableHitboxCollider(false);

        currentWorld.ResetColliderObjects();

        actionMachine.SetState(SurfCharacterActionType.GameOver);
    }

    public void OnHitGarbage(Garbage garbage)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();

        coinCollected += VDParameter.COIN_VALUE;
        scoreController.UpdateCoinScore(coinCollected);

        HitGarbage(garbage, garbageController, platformController, scoreController);
    }

    public void OnHitInk(InkObstacle inkObstacle)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        currentWorld.ColliderObjectToReset.Add(inkObstacle);

        Transform cameraCenterTransform = camera.CenterCamera;
        Transform originalParent = inkObstacle.transform;
        inkObstacle.gameObject.SetActive(false);

        FMSceneController.Get().GetDisplayObject(inkObstacle, "Arnold", cameraCenterTransform, originalParent,0);
        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.SurfMission, SurfMissionType.ArnoldHit, 1);
        FMMissionController.Get().UpdateSurfMissionProgress(SurfMissionCategory.PermanentMission, SurfMissionType.ArnoldHit, 1);
    }
}
