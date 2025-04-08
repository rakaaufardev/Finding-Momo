using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum WorldType
{
    Main,
    Surf,
    Building
}

public enum GameStatus
{
    Idle,
    Play,
    Pause,
    Finish,
    Transition,
}

public class CacheSurfGameData
{
    private int cacheAddHealth;

    public int CacheAddHealth
    {
        get
        {
            return cacheAddHealth;
        }
        set
        {
            cacheAddHealth = value;
        }
    }

    public void Save(int inAddHealth)
    {
        cacheAddHealth = inAddHealth;
    }
}

public class CacheMainGameData
{
    private List<PlatformData> cachePlatformDatas;
    private int cacheTrackId;
    private int cacheLineId;
    private int cacheHealth;
    private int cachePlatformIndex;
    private int cacheNormalScore;
    private int cacheSpeedMultiplierLevel;
    private int cacheAdContinueChances;
    private int cacheContinueCost;
    private float cacheTimerRemain;
    private float cacheQtaSpeed;
    private KoreaCity cacheKoreaCity;

    public int CacheSpeedMultiplierLevel
    {
        get
        {
            return cacheSpeedMultiplierLevel;
        }
        set
        {
            cacheSpeedMultiplierLevel = value;
        }
    }

    public int CacheTrackId
    {
        get
        {
            return cacheTrackId;
        }
        set
        {
            cacheTrackId = value;
        }
    }

    public int CacheLineId
    {
        get
        {
            return cacheLineId;
        }
        set
        {
            cacheLineId = value;
        }
    }

    public int CacheHealth
    {
        get
        {
            return cacheHealth;
        }
        set
        {
            cacheHealth = value;
        }
    }

    public int CachePlatformIndex
    {
        get
        {
            return cachePlatformIndex;
        }
        set
        {
            cachePlatformIndex = value;
        }
    }

    public int CacheNormalScore
    {
        get
        {
            return cacheNormalScore;
        }
        set
        {
            cacheNormalScore = value;
        }
    }

    public int CacheAdContinueChances
    {
        get => cacheAdContinueChances;
        set => cacheAdContinueChances = value;
    }
    
    public int CacheContinueCost
    {
        get => cacheContinueCost;
        set => cacheContinueCost = value;
    }

    public float CacheTimerRemain
    {
        get
        {
            return cacheTimerRemain;
        }
        set
        {
            cacheTimerRemain = value;
        }
    }

    public float CacheQtaSpeed
    {
        get
        {
            return cacheQtaSpeed;
        }
        set
        {
            cacheQtaSpeed = value;
        }
    }

    public KoreaCity CacheKoreaCity
    {
        get => cacheKoreaCity;
        set => cacheKoreaCity = value;
    }

    public void Save(List<PlatformData> inPlatformDatas, int inTrackId, int inLineId, 
        int inHealth, int inPlatformIndex, int inSpeedMultiplierLevel, int inNormalScore, 
        int inAdChances, int inDiamondsCost, float inTimerRemain, float inQtaSpeed, KoreaCity inKoreaCity)
    {
        cacheTrackId = inTrackId;
        cacheLineId = inLineId;
        cacheHealth = inHealth;
        cachePlatformIndex = inPlatformIndex;
        cacheNormalScore = inNormalScore;
        cacheTimerRemain = inTimerRemain;
        cacheSpeedMultiplierLevel = inSpeedMultiplierLevel;
        cacheQtaSpeed = inQtaSpeed;
        cacheAdContinueChances = inAdChances;
        cacheContinueCost = inDiamondsCost;
        cacheKoreaCity = inKoreaCity;

        if (cachePlatformDatas == null)
        {
            cachePlatformDatas = new List<PlatformData>();
        }

        int count = inPlatformDatas.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformData platformData = inPlatformDatas[i];
            cachePlatformDatas.Add(platformData);
        }
    }

    public List<PlatformData> GetCachePlatformDatas()
    {
        List<PlatformData> result = new List<PlatformData>();

        int count = cachePlatformDatas.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformData platformData = cachePlatformDatas[i];
            result.Add(platformData);
        }

        cachePlatformDatas.Clear();

        return result;
    }
}

public class FMMainScene : VDScene
{
    [SerializeField] private UIMain uiMainPrefab;
    [SerializeField] private Transform rootWorld;
    [SerializeField] private Transform rootWorldParticle;
    [SerializeField] private RectTransform rootUIParticle;
    [SerializeField] private FMDirectionalLight directionalLight;
    [SerializeField] private FMEndingController endingController;
    private GameStatus gameStatus;
    private FMGarbageController garbageController;
    private FMScoreController scoreController;
    private UIMain uiMainObject;
    private FMWorld currentWorldObject;
    private WorldType currentWorldType;
    private CacheMainGameData cacheMainGameData;
    private CacheSurfGameData cacheSurfGameData;
    private UIQuickAction uiQuickAction;
    private int costumeIndex;
    private Coroutine inkCoroutine;

    public int CostumeIndex
    {
        get
        {
            return costumeIndex;
        }
        set
        {
            costumeIndex = value;
        }
    }

    public GameStatus GameStatus
    {
        get
        {
            return gameStatus;
        }
        set
        {
            gameStatus = value;
        }
    }

    public CacheMainGameData CacheMainGameData
    {
        get
        {
            return cacheMainGameData;
        }
        set
        {
            cacheMainGameData = value;
        }
    }

    public CacheSurfGameData CacheSurfGameData
    {
        get
        {
            return cacheSurfGameData;
        }
        set
        {
            cacheSurfGameData = value;
        }
    }

    public Transform RootWorldParticle
    {
        get
        {
            return rootWorldParticle;
        }
    }

    public RectTransform RootUIParticle
    {
        get
        {
            return rootUIParticle;
        }
    }

    public FMDirectionalLight DirectionalLight
    {
        get
        {
            return directionalLight;
        }
    }

    public Coroutine InkCoroutine
    {
        get
        {
            return inkCoroutine;
        }
        set
        {
            inkCoroutine = value;
        }
    }

    public IEnumerator InkSequence(InkObstacle inkObstacle)
    {
        bool shouldContinue = false;

        if (inkCoroutine != null)
        {
            StopCoroutine(inkCoroutine);
            uiMainObject.RootInkParticle.gameObject.SetActive(false);
        }

        Animator inkModelAnim = inkObstacle.InkModelAnim;
        Transform rootVisual = inkObstacle.RootVisual;

        inkModelAnim.SetTrigger("Fly");
        rootVisual.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() =>
        {
            shouldContinue = true;
        });

        while (!shouldContinue)
        {
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;
        inkModelAnim.SetTrigger("Shoot");

        inkCoroutine = StartCoroutine(uiMainObject.InkParticleSequence(VDParameter.INK_MAIN_DURATION));
        
        while (!shouldContinue)
        {
            shouldContinue = inkObstacle.IsAnimationEnd("Shoot");
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = inkObstacle.IsAnimationEnd("Fly");
        rootVisual.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            FMPlatformController.Get().StoreStashRandomCollectible(inkObstacle);
            inkObstacle.ResetModel();
            inkObstacle.gameObject.SetActive(false);
        });

        /*FMSceneController.Get().PlayParticle("UIParticle_Ink", Vector3.zero, false);*/
    }

    public override IEnumerator Enter(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];
        KoreaCity selectedKoreaCity = (KoreaCity)dataContainer[1];

        bool shouldContinue = false;
        while (!shouldContinue)
        {
            shouldContinue = FMAssetFactory.IsInitAssetsComplete();
            yield return null;
        }

        cacheMainGameData = new CacheMainGameData();
        cacheSurfGameData = new CacheSurfGameData();

        int costumeCount = (int)Costume.COUNT;
        costumeIndex = Random.Range(0, costumeCount);

        ShowUI();
        rootUIParticle = uiMainObject.RootUIParticle;

        #if UNITY_EDITOR
            if (RenderSettings.skybox.HasFloat("_Rotation"))
            {
                RenderSettings.skybox.SetFloat("_Rotation", 0f);
            }
        #endif

        FMSoundController.Get().PlayBGM(BGM.BGM_Main);
        FMPlatformController.Get().PrepareStarterPlatforms();

        bool fromMiniGame = false;
        int coinCollected = 0;
        
        ChangeWorld(WorldType.Main, fromMiniGame, coinCollected, selectedKoreaCity);
    }

    public override IEnumerator Exit()
    {
        FMSceneController.Get().SaveParticleToStorage();
        FMSceneController.Get().SaveDisplayObjectToStorage();
        Destroy(uiMainObject.gameObject);
        yield return null;
    }
    
    public void SaveCacheMainGameData(int platformId)
    {
        MainWorld mainWorld = GetCurrentWorldObject() as MainWorld;

        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        List<PlatformData> platformDatas = FMPlatformController.Get().PlatformDatas;
        int trackId = character.TrackId;
        int lineId = character.LineId;
        int healthRemain = character.CurrentHealth;
        int normalScore = scoreController.MainNormalScore;
        int speedMultiplierLevel = character.SpeedMultiplierLevel;
        int adChances = character.AdContinueChances;
        int continueCost = mainWorld.ContinueCost;
        float timerRemain = mainWorld.GameTimer;
        float qtaSpeed = character.QtaSpeed;
        KoreaCity koreaCity = mainWorld.SelectedKoreaCity;

        cacheMainGameData.Save(platformDatas, trackId, lineId, healthRemain, platformId, speedMultiplierLevel, normalScore, adChances, continueCost, timerRemain, qtaSpeed, koreaCity);
    }

    public void SwitchToMiniGame(int coinCollected)
    {
        gameStatus = GameStatus.Transition;
        StartCoroutine(OnSwitchToMiniGame(coinCollected));
    }

    private IEnumerator OnSwitchToMiniGame(int coinCollected)
    {
        bool shouldContinue = false;
        FMUIWindowController.Get.OpenWindow(UIWindowType.TransitionSurf);
        UIWindowTransitionSurf popup = FMUIWindowController.Get.CurrentWindow as UIWindowTransitionSurf;

        popup.ShowEnterSurfText();

        while (!shouldContinue)
        {
            shouldContinue = popup.IsTransitionInEnd();
            yield return new WaitForEndOfFrame();
        }

        FMPlatformController.Get().ClearPlatforms();
        ChangeWorld(WorldType.Surf, coinCollected);
    }

    public void SwitchToMainGame()
    {
        StartCoroutine(OnSwitchToMainGame());
    }

    private IEnumerator OnSwitchToMainGame()
    {
        gameStatus = GameStatus.Transition;

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        uiMain.EnablePauseButton(true);

        SurfWorld surfWorld = currentWorldObject as SurfWorld;
        FMSurfCharacter surfCharacter = surfWorld.GetCharacter() as FMSurfCharacter;

        bool shouldContinue = false;
        FMUIWindowController.Get.OpenWindow(UIWindowType.TransitionSurf);
        UIWindowTransitionSurf popup = FMUIWindowController.Get.CurrentWindow as UIWindowTransitionSurf;

        popup.ShowEnterMainText();


        while (!shouldContinue)
        {
            shouldContinue = popup.IsTransitionInEnd();
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;
        FMPlatformController.Get().ClearPlatforms();
        FMPlatformController.Get().gameObject.SetActive(true);
        FMSceneController.Get().SaveParticleToStorage();
        FMSceneController.Get().SaveDisplayObjectToStorage();

        scoreController.UpdateMissionScore();
        FMMissionController.Get().ReplaceSurfMission();

        shouldContinue = false;

        ChangeWorld(WorldType.Main, true, surfCharacter.CoinCollected);

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator CloseMiniGameTransition(bool showSurfMission, bool isTutorial)
    {
        bool shouldContinue = false;
        UIWindowTransitionSurf popup = FMUIWindowController.Get.CurrentWindow as UIWindowTransitionSurf;

        popup.PlayTransitionOut();

        while (!shouldContinue)
        {
            shouldContinue = popup.IsTransitionOutEnd() || popup == null;
            if (popup == null)
            {
                showSurfMission = false;
            }
            yield return new WaitForEndOfFrame();
        }

        FMUIWindowController.Get.CloseSpecificWindow(UIWindowType.TransitionSurf);

        if (showSurfMission && !isTutorial)
        {
            FMUIWindowController.Get.OpenWindow(UIWindowType.SurfMission);
        }
    }

    public void InitGarbage()
    {
        if (garbageController == null)
        {
            garbageController = new FMGarbageController();
            garbageController.Init();
        }
    }

    public void InitScore()
    {
        scoreController = new FMScoreController();
        scoreController.Init();
    }

    public void ChangeWorld(WorldType inWorldType, params object[] transferVariables)
    {
        if (currentWorldObject != null)
        {
            //todo : clean data before destroy
            Destroy(currentWorldObject.gameObject);
            currentWorldObject = null;
        }

        string worldName = inWorldType.ToString();
        currentWorldType = inWorldType;
        currentWorldObject = FMAssetFactory.GetWorld(worldName,rootWorld);

        FMSceneController.Get().EnableSkybox(true);

        RenderSettings.skybox = FMAssetFactory.GetSkyboxMaterial(worldName);
        currentWorldObject.StartGame(transferVariables);
    }

    public FMGarbageController GetGarbageController()
    {
        return garbageController;        
    }

    public FMScoreController GetScoreController()
    {
        return scoreController;
    }

    public FMWorld GetCurrentWorldObject()
    {
        return currentWorldObject;
    }

    public WorldType GetCurrentWorldType()
    {
        return currentWorldType;
    }

    public UIMain GetUI()
    {
        return uiMainObject;
    }

    public void EnableHitboxCollider(bool isEnable)
    {
        Physics.IgnoreLayerCollision(VDParameter.LAYER_CHARACTER, VDParameter.LAYER_OBSTACLE, !isEnable);
    }

    private void ShowUI()
    {
        Transform rootUI = FMSceneController.Get().GetRootUI();
        uiMainObject = Instantiate(uiMainPrefab, rootUI);
        uiMainObject.Init(uiQuickAction);
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * VDParameter.SKYBOX_ROTATE_SPEED);
    }

    private void OnApplicationQuit()
    {
        #if UNITY_EDITOR
            if (RenderSettings.skybox.HasFloat("_Rotation"))
            {
                RenderSettings.skybox.SetFloat("_Rotation", 0f);
            }
        #endif
    }
}
