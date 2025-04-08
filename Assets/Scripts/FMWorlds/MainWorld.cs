using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewMode
{
    SideView,
    BackView,
    COUNT
}

public enum PlatformTheme
{
    Sand,
    Town,
    Busan,
    Surf,
    GyeongJu,
    Seoul,
    COUNT
}

public enum PlatformStatus
{
    None,
    StartSideView,
    EndSideView,
    StartBackView,
    EndBackView,
    StartPoint,
    StartBackViewWithPortal,
    COUNT
}

public class MainWorld : FMWorld
{
    [SerializeField] private Transform rootCharacter;
    private FMMainCharacterActionMachine characterActionMachine;
    private PlatformTheme currentMapTheme;
    private bool spawnPlatformTriggered;
    private float gameTimer;
    private MainCameraMode currentRunCameraMode;
    private List<Transform> trampolineHitboxCollide;

    [SerializeField] private Transform rootParallaxBackground;
    private FMParallaxBackground parallaxBridge;
    private FMParallaxBackground parallaxCruise;
    private FMLeaderboard leaderboard;

    private int continueCost;

    private bool loadWorldAssetComplete;
    private bool isTutorial;
    private KoreaCity selectedKoreaCity;

    public float GameTimer
    {
        get
        {
            return gameTimer;
        }
        set
        {
            gameTimer = value;
        }
    }

    public bool SpawnPlatformTriggered
    {
        get
        {
            return spawnPlatformTriggered;
        }
        set
        {
            spawnPlatformTriggered = value;
        }
    }

    public MainCameraMode CurrentCameraMode
    {
        get
        {
            return currentRunCameraMode;
        }
        set
        {
            currentRunCameraMode = value;
        }
    }

    public List<Transform> TrampolineHitboxCollide
    {
        get
        {
            return trampolineHitboxCollide;
        }
        set
        {
            trampolineHitboxCollide = value;
        }
    }

    public int ContinueCost
    {
        get => continueCost;
        set => continueCost = value;
    }

    public KoreaCity SelectedKoreaCity
    {
        get => selectedKoreaCity;
    }

    public void Init(FMLeaderboard inLeaderboard)
    {
        leaderboard = inLeaderboard;
    }

    public override void StartGame(params object[] transferVariables)
    {
#if ENABLE_CHEAT
        VDDebugTool.Get().AddDebugWorld_Main();
#endif

        bool fromMiniGame = (bool)transferVariables[0];
        int coinCollected = (int)transferVariables[1];

        if (!UITutorial.IsDone("Finish"))
        {
            isTutorial = true;
        }

        FMPlatformController platformController = FMPlatformController.Get();

        platformController.LoadRandomPlatform = fromMiniGame;
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        colliderObjectToReset = new List<PlatformColliderObject>();
        trampolineHitboxCollide = new List<Transform>();
        UIMain uiMain = mainScene.GetUI();
        CacheMainGameData cacheMainGameData = mainScene.CacheMainGameData;

        KoreaCity koreaCity = (transferVariables.Length > 2) ? (KoreaCity)transferVariables[2] : cacheMainGameData.CacheKoreaCity;
        selectedKoreaCity = koreaCity;
        // TODO: DELETE AFTER THERE IS MEDIUM, HARD, VERY HARD PLATFORM FOR OTHER CITIES
        platformController.SelectedKoreaCity = koreaCity;

        if (fromMiniGame)
        {
            int startIndex = mainScene.CacheMainGameData.CachePlatformIndex - 1;
            platformController.RegainMainPlatformData();
            platformController.SetMainPlatformIndex(startIndex);
            
            FMScoreController scoreController = mainScene.GetScoreController();
            //scoreController.UpdateGarbageScore();
        }
        else
        {
            FMSceneController.Get().Camera.transform.position = Vector3.zero;
            mainScene.InitGarbage();
            mainScene.InitScore();

            switch (koreaCity)
            {
                case KoreaCity.Busan:
                    currentMapTheme = isTutorial ? PlatformTheme.Town : PlatformTheme.Sand;
                    break;
                case KoreaCity.GyeongJu:
                    currentMapTheme = PlatformTheme.GyeongJu;
                    break;
                case KoreaCity.Seoul:
                    currentMapTheme = PlatformTheme.Seoul;
                    break;
            }
            platformController.ChangeViewMapCountdown = VDParameter.CHANGE_VIEW_MAP_COUNTDOWN;

            platformController.ResetIndexes();

            uiMain.SetHudGarbage();
        }

        mainScene.GameStatus = GameStatus.Idle;

        WorldType worldType = mainScene.GetCurrentWorldType();
        gameTimer = fromMiniGame ? cacheMainGameData.CacheTimerRemain : VDParameter.GAME_TIME_LIMIT_IN_SEC;
        continueCost = fromMiniGame ? cacheMainGameData.CacheContinueCost : VDParameter.DIAMOND_CONTINUE_COST;

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        uiMain.SetHUD(worldType);
        uiMain.UpdateGameTimer(gameTimer);
        uiMain.UpdateDiamondText(userInfo.inventoryData.gems);
        uiMain.SwitchCurrencyHUD(true);

        StartCoroutine(LoadWorldAssets(mainScene, fromMiniGame, coinCollected, koreaCity));        
        platformController.SetRandomStartSign();
    }

    private IEnumerator LoadWorldAssets(FMMainScene mainScene, bool fromMiniGame, int coinCollected, KoreaCity koreaCity)
    {
        UIMain uiMain = mainScene.GetUI();

        FMPlatformController platformController = FMPlatformController.Get();
        platformController.LoadPlatformSetup();
        platformController.LoadRandomCollectiblePool();
        platformController.SetPlatformPool(currentMapTheme);
        platformController.InitMainPlatforms(fromMiniGame);

        uiMain.UpdateCoinText(coinCollected);

        yield return GenerateNewMainPlatformAsync(fromMiniGame, true, isTutorial);

        FMMainCharacter newCharacter = FMAssetFactory.GetMainCharacter(rootCharacter);
        character = newCharacter;
        newCharacter.Init(leaderboard, fromMiniGame);
        newCharacter.SetCharacter();
        newCharacter.CoinCollected = coinCollected;

        yield return new WaitForFixedUpdate(); //wait for character position to be applied by rigidbody

        MainCameraMode mainCameraMode = fromMiniGame ? MainCameraMode.BackView : MainCameraMode.SideView;
        currentRunCameraMode = mainCameraMode;
        cameraController.Init();
        cameraController.ActivateCamera(mainCameraMode);

        switch (koreaCity)
        {
            case KoreaCity.Busan:
                parallaxCruise = FMAssetFactory.GetParallaxBackground("Background_Cruise", rootParallaxBackground);
                parallaxCruise.SetTarget(character.transform, VDParameter.PARALLAX_MAIN_GAME_MULTIPLIER_X);

                parallaxBridge = FMAssetFactory.GetParallaxBackground("Background_Bridge", rootParallaxBackground);
                //parallaxBridge = FMAssetFactory.GetParallaxBackground("Background_Bridge_Seoul", rootParallaxBackground);
                parallaxBridge.SetTarget(character.transform, VDParameter.PARALLAX_MAIN_GAME_MULTIPLIER_X);
                break;
            case KoreaCity.GyeongJu:
            case KoreaCity.Seoul:
                break;
        }

        mainScene.EnableHitboxCollider(true);

        if (fromMiniGame)
        {
            StartCoroutine(mainScene.CloseMiniGameTransition(false, isTutorial));
        }
        else
        {
            FMUIWindowController.Get.CloseWindow();
        }

        loadWorldAssetComplete = true;
    }

    public void ResetTrampolineHitboxes()
    {
        int count = trampolineHitboxCollide.Count;
        for (int i = 0; i < count; i++)
        {
            trampolineHitboxCollide[i].gameObject.SetActive(true);
        }

        trampolineHitboxCollide.Clear();
    }

    public override void PauseGame()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        mainScene.GameStatus = GameStatus.Pause;
        ((FMMainCharacter)character).OnGamePause(true);
    }

    public override void PlayGame()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        mainScene.GameStatus = GameStatus.Play;
        ((FMMainCharacter)character).OnGamePause(false);
    }

    public void GenerateMapTheme()
    {
        switch (selectedKoreaCity)
        {
            case KoreaCity.Busan:
                int themeCount = (int)PlatformTheme.Busan;
                int randomTheme = Random.Range(0, themeCount);
                currentMapTheme = (PlatformTheme)randomTheme;
                break;
            case KoreaCity.GyeongJu:
                currentMapTheme = PlatformTheme.GyeongJu;
                break;
            case KoreaCity.Seoul:
                currentMapTheme = PlatformTheme.Seoul;
                break;
        }
    }

    public PlatformTheme GetMapTheme()
    {
        return currentMapTheme;
    }

    public void ChangeViewMode()
    {
        ((FMMainCharacter)character).CharacterViewMode = ((FMMainCharacter)character).CharacterViewMode == ViewMode.SideView ? ViewMode.BackView : ViewMode.SideView;
        MainCameraMode cameraMode = ((FMMainCharacter)character).CharacterViewMode == ViewMode.SideView ? MainCameraMode.SideView : MainCameraMode.BackView;
        currentRunCameraMode = cameraMode;
        cameraController.ActivateCamera(cameraMode);
    }

    public IEnumerator GenerateNewMainPlatformAsync(bool fromMiniGame, bool firstTimeLoad, bool isTutorial = false)
    {
        FMPlatformController platformController = FMPlatformController.Get();

        //generate data
        if (/*!platformController.LoadRandomPlatform*/ isTutorial)
        {
            platformController.GenerateFixedPlatform();

            if (platformController.LoadRandomPlatform)
            {
                platformController.GenerateMainPlatformData(platformController.ChangeViewMapCountdown, firstTimeLoad);
            }
        }
        else
        {
            platformController.GenerateMainPlatformData(platformController.ChangeViewMapCountdown, firstTimeLoad);
        }

        //generate platform
        int totalSpawn = 0;
        if (isTutorial)
        {
            totalSpawn = VDParameter.TOTAL_PLATFORM_SPAWN_IN_TUTORIAL;
        }
        else
        {
            totalSpawn = VDParameter.TOTAL_PLATFORM_SPAWN;
        }

        int platformIndexHavePortal = 1;
        for (int i = 0; i < totalSpawn; i++)
        {
            if (fromMiniGame)
            {
                if (i == platformIndexHavePortal)
                {
                    platformController.GenerateMainPlatform(true);
                }
                else
                {
                    platformController.GenerateMainPlatform(false);
                }
            }
            else
            {
                platformController.GenerateMainPlatform(false);
            }

            yield return null;
        }

        if (colliderObjectToReset != null)
        {
            ResetColliderObjects();
        }
    }

    protected void Update()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        FMMainCharacter mainCharacter = character as FMMainCharacter;

        if (!loadWorldAssetComplete)
        {
            return;
        }

        //map
        if (characterActionMachine != null)
        {
            MainCharacterActionType characterState = characterActionMachine.GetCurrentState();
            ActionMachineStatus actionMachineStatus = characterActionMachine.GetActionMachineStatus();

            bool isStateRun = characterState == MainCharacterActionType.Run;
            bool isActionMachineActive = actionMachineStatus == ActionMachineStatus.Active;

            if (isActionMachineActive && isStateRun)
            {
                if (characterState == MainCharacterActionType.Run)
                {
                    if (!spawnPlatformTriggered)
                    {
                        Vector3 endTrackPoint = FMPlatformController.Get().GetLastEndTrackPoint();

                        float characterDistance = Vector3.Distance(mainCharacter.transform.position, endTrackPoint);
                        if (characterDistance <= VDParameter.DISTANCE_TRIGGER_SPAWN_MAIN_PLATFORM)
                        {
                            StartCoroutine(GenerateNewMainPlatformAsync(false, false));
                            spawnPlatformTriggered = true;
                        }
                    }
                }
            }
        }
        else
        {
            characterActionMachine = character.GetActionMachine() as FMMainCharacterActionMachine;
        }



        //game timer
        if (VDParameter.GAME_TIME_ACTIVE) 
        {
            if (mainScene.GameStatus == GameStatus.Play)
            {
                if (gameTimer <= 0)
                {
                    int coinCollected = mainCharacter.CoinCollected;
                    mainCharacter.UpdateLeaderboardData();

                    mainScene.GameStatus = GameStatus.Finish;
                    gameTimer = 0;
                    mainCharacter.OnGamePause(true);

                    FMSceneController.Get().ShowCinematicEnding(true);
                }
                else
                {
                    if (!mainCharacter.IsTutorial)
                    {
                        gameTimer -= Time.deltaTime;
                    }
                    uiMain.UpdateGameTimer(gameTimer);
                }
            }
        }

        parallaxBridge?.DoParallax();
        parallaxCruise?.DoParallax();
    }
}
