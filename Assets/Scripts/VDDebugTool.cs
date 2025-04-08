using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DUCK.DebugMenu;
using IngameDebugConsole;
using DG.Tweening;

public class DebugAction
{
    public string name;
    public KeyCode keyCode;
    public Action action;
}

public class VDDebugTool : MonoBehaviour
{
    [SerializeField] private DebugMenu debugMenu;
    private List<DebugAction> debugActions;

    private static VDDebugTool singleton;

    private FMLeaderboard leaderboard;

    private const string DEBUG_NAME_SHOW_LOG = "Show Log";
    private const string DEBUG_NAME_PAUSE = "Pause";
    private const string DEBUG_NAME_TIMELAPSE = "Timelapse";
    private const string DEBUG_NAME_ADD_COIN = "Add Coin";
    private const string DEBUG_NAME_ADD_DIAMOND = "Add Diamond";
    private const string DEBUG_NAME_ADD_DUMMY_LEADERBOARD = "Add Dummy Leaderboard";
    private const string DEBUG_NAME_ADD_SELF_DUMMY_LEADERBOARD = "Add Self Dummy Leaderboard";
    private const string DEBUG_NAME_COMPLETE_ALL_PERMANENT_MISSION = "Complete All Permanent Mission";

    private bool isInitialized;

    public bool IsInitialized
    {
        get
        {
            return isInitialized;
        }
        set
        {
            isInitialized = value;
        }
    }

    public static VDDebugTool Get()
    {
        return singleton;
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }

        debugMenu.DoAwake();
    }

    protected void Start()
    {
        debugMenu.Init();

        debugActions = new List<DebugAction>();
        AddDebugItem(DEBUG_NAME_SHOW_LOG, KeyCode.F1, Debug_ShowLog);
        AddDebugItem(DEBUG_NAME_PAUSE, KeyCode.F2, Debug_Pause);
        AddDebugItem(DEBUG_NAME_TIMELAPSE, KeyCode.F3, Debug_TimeLapse);
        AddDebugItem(DEBUG_NAME_ADD_COIN, KeyCode.F4, Debug_AddCoin);
        AddDebugItem(DEBUG_NAME_ADD_DIAMOND, KeyCode.F5, Debug_AddDiamond);
        AddDebugItem(DEBUG_NAME_ADD_DUMMY_LEADERBOARD, KeyCode.F6, Debug_AddDummyLeaderboard);
        AddDebugItem(DEBUG_NAME_ADD_SELF_DUMMY_LEADERBOARD, KeyCode.F8, Debug_AddDummySelfLeaderboard);
        AddDebugItem(DEBUG_NAME_COMPLETE_ALL_PERMANENT_MISSION, KeyCode.F9, Debug_CompleteAllPermanentMission);

        isInitialized = true;
    }

    public void Init(FMLeaderboard inLeaderboard)
    {
        leaderboard = inLeaderboard;
    }

    protected void Update()
    {
        foreach (DebugAction debug in debugActions)
        {
            KeyCode keyCode = debug.keyCode;
            if (Input.GetKeyDown(keyCode))
            {
                debug.action?.Invoke();
            }
        }
    }

    public void AddDebugScene_Lobby()
    {
        AddDebugItem("Show Offers", KeyCode.Keypad1, Debug_ShowOffers);
        AddDebugItem("Show Daily Login", KeyCode.Keypad2, Debug_ShowDailyLoginWindow);
    }

    public void AddDebugWorld_Main()
    {
        ClearDebugItem();
        AddDebugItem("Enable Invulnerable", KeyCode.Keypad1, Debug_EnableInvulnerable);
        AddDebugItem("Add Health", KeyCode.Keypad2, Debug_Addhealth);
        AddDebugItem("Add Health From Complete Garbage", KeyCode.Keypad3, Debug_AddHeartCompleteGarbage);
        AddDebugItem("Disable Invulnerable", KeyCode.Keypad4, Debug_DisableInvulnerable);
        AddDebugItem("Collect Coin", KeyCode.Keypad5, Debug_CollectCoin);
        AddDebugItem("Respawn", KeyCode.Keypad6, Debug_Respawn);
        AddDebugItem("Collect All Coin", KeyCode.Keypad7, Debug_CollectAllCoin);
        AddDebugItem("Idle", KeyCode.Keypad8, Debug_Idle);
        //AddDebugItem("Collect Garbage", KeyCode.Keypad7, Debug_CollectGarbage);

#if ENABLE_SURF
        AddDebugItem("Go To Surf", KeyCode.Keypad9, Debug_GoToSurf);
#endif
    }

    public void AddDebugWorld_Surf()
    {
        ClearDebugItem();
        AddDebugItem("Enable Hitbox", KeyCode.Keypad1, Debug_EnableHitbox);
        AddDebugItem("Disable Hitbox", KeyCode.Keypad2, Debug_DisableHitbox);
        AddDebugItem("Show Mission Notice", KeyCode.Keypad3, Debug_ShowMissionNotice);
        AddDebugItem("Display Sea Turtle", KeyCode.Keypad4, Debug_DisplaySeaTurtle);
        AddDebugItem("Display Dolphin", KeyCode.Keypad5, Debug_DisplayDolphin);
        AddDebugItem("Display Sea Lion", KeyCode.Keypad7, Debug_DisplaySeaLion);
        AddDebugItem("Display Manta Ray", KeyCode.Keypad8, Debug_DisplayMantaRay);
        AddDebugItem("Display Killer Whale", KeyCode.Keypad9, Debug_DisplayKillerWhale);
    }

    public void ClearDebugItem()
    {
        List<DebugAction> keysToRemove = new List<DebugAction>();
        foreach (DebugAction debug in debugActions)
        {
            if (debug.name != DEBUG_NAME_SHOW_LOG && debug.name != DEBUG_NAME_PAUSE && debug.name != DEBUG_NAME_TIMELAPSE)
            {
                keysToRemove.Add(debug);
            }
        }

        foreach (DebugAction debug in keysToRemove)
        {
            debugMenu.RemoveButton(debug.name);
            debugActions.Remove(debug);
        }
    }

    public DebugMenu GetDebugMenu()
    {
        return debugMenu;
    }

    void AddDebugItem(string name, KeyCode keyCode, Action debugAction)
    {
        DebugAction newDebugAction = new DebugAction();
        newDebugAction.name = name;
        newDebugAction.keyCode = keyCode;
        newDebugAction.action = debugAction;

        debugActions.Add(newDebugAction);
        debugMenu.AddButton(name, debugAction);
    }

    void Debug_ShowLog()
    {
        debugMenu.Hide();
        DebugLogManager.Instance.ShowLogWindow();
    }

    void Debug_CollectAllCoin()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        List<FMPlatform> platforms = FMPlatformController.Get().GetPlatforms();
        int platformCount = platforms.Count;
        for (int i = 0; i < platformCount; i++)
        {
            FMPlatform platform = platforms[i];
            Coin[] coins = platform.GetCoinObjects();
            int coinCount = coins.Length;
            for (int j = 0; j < coinCount; j++)
            {
                Coin coin = coins[j];
                character.OnHitCoin(coin);
                coin.Show(false);
            }
        }
    }

    void Debug_CollectCoin()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;
        FMGarbageController garbageController = mainScene.GetGarbageController();

        List<string> coinNames = garbageController.GarbageNames;
        int count = coinNames.Count-1;
        for (int i = 0; i < count; i++)
        {
            string name = coinNames[i];

            garbageController.CollectGarbage(name, 100);
            garbageController.CheckGarbageCollectionStatus();

            character.UpdateHUDGarbage(name);
        }

    }

    void Debug_CompleteAllPermanentMission()
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> missions = inventoryData.userPermanentMissionProgress;
        int count = missions.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData mission = missions[i];
            mission.goalProgress = mission.goalValue;
        }

        FMUserDataService.Get().Save();
    }

    void Debug_CollectGarbage()
    {
        List<FMPlatform> platforms = FMPlatformController.Get().GetPlatforms();
        int platformCount = platforms.Count;
        for (int i = 0; i < platformCount; i++)
        {
            FMPlatform platform = platforms[i];
            PlatformColliderObject[] randomCollectibles = platform.GetRandomCollectibleObjects();
            int collectibleCount = randomCollectibles.Length;
            for (int j = 0; j < collectibleCount; j++)
            {
                PlatformColliderObject collectible = randomCollectibles[j];
                if (collectible is Garbage garbage)
                {
                    if (garbage.gameObject.activeInHierarchy)
                    {
                        //todo : change this logic as function
                        string garbageName = garbage.GetGarbageName();
                        garbage.Show(false);

                        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
                        FMMainScene mainScene = currentScene as FMMainScene;
                        UIMain uiMain = mainScene.GetUI();
                        FMGarbageController garbageController = mainScene.GetGarbageController();

                        garbageController.CollectGarbage(garbageName,1);
                        //garbageController.RemoveGarbagePool(garbageName);
                        garbageController.ResetGarbagePoolCounter();
                        garbageController.CheckGarbageCollectionStatus();

                        FMPlatformController.Get().StoreStashGarbageVisual(garbage);
                        FMPlatformController.Get().StoreStashRandomCollectible(collectible);
                        FMPlatformController.Get().HideDuplicateGarbage(garbageName);
                        garbage.ClearGarbage();

                        platform.ClearRandomCollectible(collectible);

                        //uiMain.UpdateHudGarbage(garbageName, true, "Cheat");
                    }
                }
            }
        }
    }

    void Debug_ShowOffers()
    {
        FMShopController.Get.ShowOfferCTA();
    }

    void Debug_ShowDailyLoginWindow()
    {
        UIWindowDailyLogin dailyLogin = FMUIWindowController.Get.CurrentWindow as UIWindowDailyLogin;
        UIQuickAction quickAction = FMSceneController.Get().GetUIQuickAction;

        if (dailyLogin == null)
        {
            FMUIWindowController.Get.OpenWindow(UIWindowType.DailyLogin, quickAction);
        }
        else
        {
            FMUIWindowController.Get.CloseCurrentWindow();
        }
    }

    void Debug_AddCoin()
    {
        FMInventory.Get.AddInventory(RewardType.Coin, 0, 500000);
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Lobby)
        {
            InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

            LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
            UILobby uiLobby = lobbyScene.GetUI();
            UITopFrameHUD topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
            topFrameHUD.UpdateCoinText(inventoryData.coin);
        }
    }

    void Debug_AddDiamond()
    {
        FMInventory.Get.AddInventory(RewardType.Gems, 0, 1000);
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Lobby)
        {
            InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

            LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
            UILobby uiLobby = lobbyScene.GetUI();
            UITopFrameHUD topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
            topFrameHUD.UpdateDiamondText(inventoryData.gems);
        }
    }

    void Debug_Addhealth()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        UIMain uiMain = mainScene.GetUI();
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        character.AddHealth(1, true);
        uiMain.AddHealthIcon(character.MaxHealth);
        uiMain.UpdateHealthIcon();
    }

    void Debug_TimeLapse()
    {
        float timeScaleValue = Time.timeScale == 8 ? 1 : 8;
        Time.timeScale = timeScaleValue;
    }

    void Debug_Pause()
    {
        float timeScaleValue = Time.timeScale == 0 ? 1 : 0;
        Time.timeScale = timeScaleValue;
    }

    void Debug_EnableInvulnerable()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        character.IsInvulnerable = false;
        character.EnableInvulnerable();
        character.InvulnerableTimer = 99999999999999;

        Debug_DisableHitbox();
    }

    void Debug_DisableInvulnerable()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        character.IsInvulnerable = false;
        character.InvulnerableTimer = 0;
        character.ShowInvulnerable(false);

        Debug_EnableHitbox();
    }

    void Debug_DisableHitbox()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        mainScene.EnableHitboxCollider(false);
    }

    void Debug_EnableHitbox()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        mainScene.EnableHitboxCollider(true);
    }

    void Debug_Idle()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;
        FMMainCharacterActionMachine actionMachine = character.GetActionMachine() as FMMainCharacterActionMachine;
        
        actionMachine.SetState(MainCharacterActionType.Idle, character.CharacterViewMode);
    }

    void Debug_AddHeartCompleteGarbage()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();
        garbageController.ShowCompleteGarbage();
    }

    void Debug_GoToSurf()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        FMUIWindowController.Get.CloseAllWindow();

        FMPlatformController platformController = FMPlatformController.Get();

        int platformId = -1;
        int trackId = -1;
        bool portalFound = false;

        while (!portalFound)
        {
            List<FMPlatform> platforms = platformController.Platforms;
            int count = platforms.Count;
            for (int i = 0; i < count; i++)
            {
                FMPlatform platform = platforms[i];
                bool portalExist = platform.IsPortalExist();
                if (portalExist)
                {
                    Portal portal = platform.GetPortal();
                    platformId = portal.PlatformId;

                    int dataCount = platformController.PlatformDatas.Count;
                    for (int j = 0; j < dataCount; j++)
                    {
                        PlatformData platformData = platformController.PlatformDatas[j];
                        if (platformData.platformId == platformId)
                        {
                            trackId = platformData.trackId;
                            break;
                        }
                    }

                    portalFound = true;
                    break;
                }
            }

            if (portalFound)
            {
                character.TrackId = trackId + 1;
                character.LineId = 0;
                character.OnHitPortal(platformId);
            }
            else
            {
                StartCoroutine(mainWorld.GenerateNewMainPlatformAsync(false, false));
            }
        }
    }

    void Debug_ShowMissionNotice()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        uiMain.ShowMissionNotice("Test");
    }

    void Debug_AddDummyLeaderboard()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomPlayerName = string.Empty;
        int nameCharLength = UnityEngine.Random.Range(3, 6);
        int randomCharIndex = UnityEngine.Random.Range(0, chars.Length);
        for (int i = 0; i < nameCharLength; i++)
        {
            randomPlayerName += chars[randomCharIndex];
        }

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();

        string lastPlayerName = userInfo.currentPlayerName;
        float lastScore = userInfo.currentPlayerScore;
        userInfo.currentPlayerName = randomPlayerName;

        Debug_AddDummySelfLeaderboard();

        //userInfo.currentPlayerName = lastPlayerName;
        //userInfo.currentPlayerScore = lastScore;

        Debug.Log("New Player Name: " + randomPlayerName);

        FMUserDataService.Get().Save();
    }

    void Debug_AddDummySelfLeaderboard()
    {
        float score = UnityEngine.Random.Range(500, 50000);
        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        leaderboard.AddLeaderboardData(score, userInfo);
    }

    void Debug_Respawn()
    {
        VDAdController.Get.ShowRewardedAd(AdsRewardType.Continue);
    }

    void Debug_DisplaySeaTurtle()
    {
        FMUnityMainCamera camera = FMSceneController.Get().Camera;
        Transform cameraCenterTransform = camera.CenterCamera;
        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);

        FMDisplayObject displayObj = FMSceneController.Get().CreateDisplayObject("SeaTurtle", cameraCenterTransform);
        StartCoroutine(DisplayObjectSurfAnimation(displayObj));
    }
    
    void Debug_DisplayDolphin()
    {
        FMUnityMainCamera camera = FMSceneController.Get().Camera;
        Transform cameraCenterTransform = camera.CenterCamera;
        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);

        FMDisplayObject displayObj = FMSceneController.Get().CreateDisplayObject("Dolphin", cameraCenterTransform);
        StartCoroutine(DisplayObjectSurfAnimation(displayObj));
    }
    
    void Debug_DisplaySeaLion()
    {
        FMUnityMainCamera camera = FMSceneController.Get().Camera;
        Transform cameraCenterTransform = camera.CenterCamera;
        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);

        FMDisplayObject displayObj = FMSceneController.Get().CreateDisplayObject("SeaLion", cameraCenterTransform);
        StartCoroutine(DisplayObjectSurfAnimation(displayObj));
    }
    
    void Debug_DisplayMantaRay()
    {
        FMUnityMainCamera camera = FMSceneController.Get().Camera;
        Transform cameraCenterTransform = camera.CenterCamera;
        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);

        FMDisplayObject displayObj = FMSceneController.Get().CreateDisplayObject("MantaRay", cameraCenterTransform);
        StartCoroutine(DisplayObjectSurfAnimation(displayObj));
    }
    
    void Debug_DisplayKillerWhale()
    {
        FMUnityMainCamera camera = FMSceneController.Get().Camera;
        Transform cameraCenterTransform = camera.CenterCamera;
        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);

        FMDisplayObject displayObj = FMSceneController.Get().CreateDisplayObject("KillerWhale", cameraCenterTransform);
        StartCoroutine(DisplayObjectSurfAnimation(displayObj));
    }

    private IEnumerator DisplayObjectSurfAnimation(FMDisplayObject displayObject)
    {
        bool shouldContinue = false;

        displayObject.IsActive(true);
        displayObject.transform.localPosition = Vector3.zero;
        displayObject.RootVisual.gameObject.SetActive(true);
        displayObject.RootVisual.localScale = Vector3.one * 2f;
        displayObject.RootVisual.eulerAngles = new Vector3(0, 180, 0);

        displayObject.DisplaySequence = DOTween.Sequence();

        displayObject.RootVisual.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
        {
            displayObject.Anim.SetTrigger("Free");
        });

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsStateAnim("FreeInactive");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;

        displayObject.Anim.SetTrigger("Show");
        FMSceneController.Get().PlayParticle("WorldParticle_Heart", displayObject.RootVisual, Vector3.zero);

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsStateAnim("ShowInactive");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;

        displayObject.Anim.SetTrigger("Return");

        displayObject.DisplaySequence.Append(displayObject.RootVisual.DOLocalRotate(new Vector3(0, 0, 0), 1f));
        displayObject.DisplaySequence.Join(displayObject.RootVisual.DOScale(Vector3.one * 0.75f, 1f));
        displayObject.DisplaySequence.Join(displayObject.RootVisual.DOLocalMove(Vector3.back * 6f, 1f).OnComplete(() =>
        {
            FMSceneController.Get().PlayParticle("WorldParticle_Splash", displayObject.RootVisual.position);
            displayObject.IsActive(false);
            displayObject.RootVisual.gameObject.SetActive(false);
        }));

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsStateAnim("HideInactive");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;
    }
}
