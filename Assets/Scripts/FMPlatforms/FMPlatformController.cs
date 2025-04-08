using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;

[Serializable]
public class PlatformSetupWrapper
{
    public List<PlatformSetupData> setupList;
}

[Serializable]
public class RandomCollectiblePoolData
{
    public string itemName;
    public int probability;
}



[Serializable]
public class RandomCollectiblePoolWrapper
{
    public List<RandomCollectiblePoolData> itemPoolDatas;
}

[Serializable]
public class PlatformPoolData
{
    public string folderPath;
    public string platformName;
    public PlatformStatus platformStatus;
    public PlatformTheme platformTheme;
    public ViewMode viewMode;
    public int probability;
}

[Serializable]
public class PlatformPoolWrapper
{
    public List<PlatformPoolData> platformPools;
}

[Serializable]
public class PlatformSetupData
{
    public string platformName;
    public int inactiveCounter;
}

[Serializable]
public class PlatformData
{
    public string platformName;
    public string folderPath;
    public PlatformStatus platformStatus;
    public PlatformTheme platformTheme;
    public ViewMode viewMode;
    public int trackId;
    public int platformId;
    public int garbageModelIndex;
    public string garbageName;
    public bool isLastData;

    public PlatformData(string inPlatformName, string inFolderPath, string inGarbageName, 
        PlatformStatus inPlatformStatus, PlatformTheme inPlatformTheme, ViewMode inViewMode,
        int inTrackId, int inPlatformId, int inGarbageIndex)
    {
        platformName = inPlatformName;
        folderPath = inFolderPath;
        platformStatus = inPlatformStatus;
        platformTheme = inPlatformTheme;
        viewMode = inViewMode;
        trackId = inTrackId;
        platformId = inPlatformId;
        garbageModelIndex = inGarbageIndex;
        garbageName = inGarbageName;
    }
}

public class EndTrackPointData
{
    public Vector3 endTrackPosition;
    public Vector3[] arrowTransitionPositions;

    public EndTrackPointData(Vector3 endTrackPos, Vector3[] arrows)
    {
        endTrackPosition = endTrackPos;
        arrowTransitionPositions = arrows;
    }
}

public class FMPlatformController : MonoBehaviour
{
    [SerializeField] private Transform rootPlatform;
    [SerializeField] private List<PlatformData> platformDatas;
    [SerializeField] private List<FMPlatform> platforms;
    private List<FMPlatform> stashPlatforms;
    private List<PlatformColliderObject> stashRandomCollectibles;
    [SerializeField] private List<GameObject> stashGarbageVisual;
    private List<GameObject> stashCoinVisual;
    private List<PlatformData> mapFixedPool;
    private List<PlatformData> mapSurfPool;
    private Dictionary<ViewMode, List<PlatformData>> mapSandPool;
    private Dictionary<ViewMode, List<PlatformData>> mapTownPool;
    private Dictionary<ViewMode, List<PlatformData>> mapGyeongJuPool;
    private Dictionary<ViewMode, List<PlatformData>> mapSeoulPool;
    private Dictionary<ViewMode, List<PlatformData>> currentPlatformPool;
    private Dictionary<string, int> platformInactiveCounterRecords;
    private Dictionary<int,EndTrackPointData> endTrackPoints;
    private List<PlatformSetupData> platformSetupDatas;
    private List<string> randomCollectiblePools;
    private List<string> randomGarbageModelPools;
    private FMPoolCounter randomCollectiblePoolCounter;
    private FMPoolCounter platform2DPoolCounter;
    private FMPoolCounter platform3DPoolCounter;
    private FMPoolCounter platformFixedPoolCounter;
    private FMPoolCounter platformSurfPoolCounter;
    private int mainPlatformObjectIndex;
    private int surfPlatformObjectIndex;
    private int currentPlatformTrackId;
    private int spawnPortalCountdown;
    private int changeViewMapCountdown;
    private int garbageModelIndex;
    private int garbageTypeCount;
    private string garbageModelName;
    private string currentDifficultyLevel="Easy";
    private bool loadRandomPlatform;
    private Vector3 nextSpawnPos;
    private ViewMode platformViewMode;
    private FMWorld world;
    private FMRandomStartSign randomStartSign;
    private static FMPlatformController singleton;
    private KoreaCity selectedKoreaCity;

    public int ChangeViewMapCountdown
    {
        get
        {
            return changeViewMapCountdown;
        }
        set
        {
            changeViewMapCountdown = value;
        }
    }

    public int MainPlatformObjectIndex
    {
        get
        {
            return mainPlatformObjectIndex;
        }
        set
        {
            mainPlatformObjectIndex = value;
        }
    }

    public int SurfPlatformObjectIndex
    {
        get
        {
            return surfPlatformObjectIndex;
        }
        set
        {
            surfPlatformObjectIndex = value;
        }
    }

    public List<PlatformData> PlatformDatas
    {
        get
        {
            return platformDatas;
        }
        set
        {
            platformDatas = value;
        }
    }

    public List<FMPlatform> Platforms
    {
        get
        {
            return platforms;
        }
        set
        {
            platforms = value;
        }
    }

    public List<FMPlatform> StashPlatforms
    {
        get
        {
            return stashPlatforms;
        }
        set
        {
            stashPlatforms = value;
        }
    }

    public Transform RootPlatform
    {
        get
        {
            return rootPlatform;
        }
        set
        {
            rootPlatform = value;
        }
    }

    public bool LoadRandomPlatform
    {
        get
        {
            return loadRandomPlatform;
        }
        set
        {
            loadRandomPlatform = value;
        }
    }

    public KoreaCity SelectedKoreaCity
    {
        get => selectedKoreaCity;
        set => selectedKoreaCity = value;
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMPlatformController Get()
    {
        return singleton;
    }

    private static string[] PATH_MAP_SAND_POOL_DB = new string[8]
    {
        VDParameter.PATH_MAP_SAND_2D_EASY_DB,
        VDParameter.PATH_MAP_SAND_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_SAND_2D_HARD_DB,
        VDParameter.PATH_MAP_SAND_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_SAND_3D_EASY_DB,
        VDParameter.PATH_MAP_SAND_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_SAND_3D_HARD_DB,
        VDParameter.PATH_MAP_SAND_3D_VERYHARD_DB,

    };

    private static string[] PATH_MAP_TOWN_POOL_DB = new string[8]
    {
        VDParameter.PATH_MAP_TOWN_2D_EASY_DB,
        VDParameter.PATH_MAP_TOWN_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_TOWN_2D_HARD_DB,
        VDParameter.PATH_MAP_TOWN_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_TOWN_3D_EASY_DB,
        VDParameter.PATH_MAP_TOWN_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_TOWN_3D_HARD_DB,
        VDParameter.PATH_MAP_TOWN_3D_VERYHARD_DB,
    };
    
    private static string[] PATH_MAP_GYEONGJU_POOL_DB = new string[8]
    {
        VDParameter.PATH_MAP_GYEONGJU_2D_EASY_DB,
        VDParameter.PATH_MAP_GYEONGJU_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_GYEONGJU_2D_HARD_DB,
        VDParameter.PATH_MAP_GYEONGJU_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_GYEONGJU_3D_EASY_DB,
        VDParameter.PATH_MAP_GYEONGJU_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_GYEONGJU_3D_HARD_DB,
        VDParameter.PATH_MAP_GYEONGJU_3D_VERYHARD_DB,
    };

    private static string[] PATH_MAP_SEOUL_POOL_DB = new string[8]
    {
        VDParameter.PATH_MAP_SEOUL_2D_EASY_DB,
        VDParameter.PATH_MAP_SEOUL_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_SEOUL_2D_HARD_DB,
        VDParameter.PATH_MAP_SEOUL_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_SEOUL_3D_EASY_DB,
        VDParameter.PATH_MAP_SEOUL_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_SEOUL_3D_HARD_DB,
        VDParameter.PATH_MAP_SEOUL_3D_VERYHARD_DB,
    };

    public void InitSurfPlatforms()
    {
        InitGeneralPlatforms();
        platformSurfPoolCounter = new FMPoolCounter();
        mapSurfPool.Shuffle();
    }

    public void ResetIndexes()
    {
        currentPlatformTrackId = -1;
        mainPlatformObjectIndex = 0;
        surfPlatformObjectIndex = 0;
    }

    public void InitMainPlatforms(bool fromMiniGame)
    {
        InitGeneralPlatforms();
        randomCollectiblePoolCounter = new FMPoolCounter();
        platformFixedPoolCounter = new FMPoolCounter();
        platform2DPoolCounter = new FMPoolCounter();
        platform3DPoolCounter = new FMPoolCounter();

        changeViewMapCountdown = fromMiniGame ? changeViewMapCountdown : VDParameter.CHANGE_VIEW_MAP_COUNTDOWN;
        platformViewMode = fromMiniGame ? platformViewMode : ViewMode.SideView;
        spawnPortalCountdown = VDParameter.SPAWN_NEXT_PORTAL_COUNT;
        
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();

        List<string> garbageNames = garbageController.GarbageNames;
        garbageTypeCount = garbageNames.Count;
        randomGarbageModelPools = garbageNames;
        randomGarbageModelPools.Shuffle();

        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        world = currentWorld as MainWorld;

        bool spawnStarterPlatform = stashPlatforms.Count <= 0;
        if (spawnStarterPlatform)
        {
            SetMainPlatformIndex(0);
        }
    }

    private void InitGeneralPlatforms()
    {
        nextSpawnPos = Vector3.zero;

        if (platformDatas == null)
        {
            platformDatas = new List<PlatformData>();
        }

        if (platforms == null)
        {
            platforms = new List<FMPlatform>();
        }

        if (platformInactiveCounterRecords == null)
        {
            platformInactiveCounterRecords = new Dictionary<string, int>();
        }

        if (endTrackPoints == null)
        {
            endTrackPoints = new Dictionary<int, EndTrackPointData>();
        }

        if (stashGarbageVisual == null)
        {
            stashGarbageVisual = new List<GameObject>();
        }

        if (stashCoinVisual == null)
        {
            stashCoinVisual = new List<GameObject>();
        }

        if (stashRandomCollectibles == null)
        {
            stashRandomCollectibles = new List<PlatformColliderObject>();
        }
    }

    public void PrepareStarterPlatforms()
    {
        if (stashPlatforms == null)
        {
            stashPlatforms = new List<FMPlatform>();
        }

        if (stashRandomCollectibles == null)
        {
            stashRandomCollectibles = new List<PlatformColliderObject>();
        }

        if (stashCoinVisual == null)
        {
            stashCoinVisual = new List<GameObject>();
        }

        if (stashPlatforms.Count <= 0)
        {
            mapFixedPool = LoadPlatformDatas(VDParameter.PATH_FIXED_PLATFORMS_DB);
            mapSurfPool = LoadPlatformDatas(VDParameter.PATH_MAP_SURF_POOL_DB);
            mapSandPool = Load(PATH_MAP_SAND_POOL_DB);
            mapTownPool = Load(PATH_MAP_TOWN_POOL_DB);
            mapGyeongJuPool = Load(PATH_MAP_GYEONGJU_POOL_DB);
            mapSeoulPool = Load(PATH_MAP_SEOUL_POOL_DB);

            Dictionary<string, FMPlatform> prefabContainer = FMAssetFactory.PrefabPlatformContainer;
            int listCount = prefabContainer.Count;
            for (int i = 0; i < listCount; i++)
            {
                FMPlatform prefab = prefabContainer.GetValue(i);
                FMPlatform platform = Instantiate(prefab, rootPlatform);
                platform.gameObject.name = prefab.name;
                platform.gameObject.SetActive(false);
                stashPlatforms.Add(platform);
            }
        }

        if (stashRandomCollectibles.Count <= 0)
        {
            Dictionary<string, PlatformColliderObject> prefabRandomCollectibleContainer = FMAssetFactory.PrefabRandomCollectibleContainer;
            int prefabCount = prefabRandomCollectibleContainer.Count;
            for (int i = 0; i < prefabCount; i++)
            {
                int starterCount = 10;
                for (int j = 0; j < starterCount; j++)
                {
                    string key = prefabRandomCollectibleContainer.GetKey(i);
                    PlatformColliderObject prefab = prefabRandomCollectibleContainer.GetValue(i);
                    if (key != "Empty")
                    {
                        PlatformColliderObject collectible = Instantiate(prefab, rootPlatform);
                        collectible.gameObject.name = prefab.name;
                        collectible.gameObject.SetActive(false);
                        stashRandomCollectibles.Add(collectible);
                    }
                }
            }
        }

        if (stashCoinVisual.Count <= 0)
        {
            Dictionary<string, GameObject> prefabCoin = FMAssetFactory.PrefabCoin;
            int prefabCount = prefabCoin.Count;
            for (int i = 0; i < prefabCount; i++)
            {
                int starterCount = 100;
                for (int j = 0; j < starterCount; j++)
                {
                    GameObject prefab = prefabCoin.GetValue(i);
                    GameObject coin = Instantiate(prefab, rootPlatform);
                    coin.gameObject.name = prefab.name;
                    coin.gameObject.SetActive(false);
                    stashCoinVisual.Add(coin);
                }
            }
        }
       
    }

    public void LoadRandomCollectiblePool()
    {
        TextAsset jsonFile = FMAssetFactory.GetDatabaseAsset(VDParameter.PATH_RANDOM_COLLECTIBLE_DB);
        randomCollectiblePools = new List<string>();
        if (jsonFile != null)
        {
            RandomCollectiblePoolWrapper wrapper = JsonUtility.FromJson<RandomCollectiblePoolWrapper>(jsonFile.text);
            List<RandomCollectiblePoolData> poolData = wrapper.itemPoolDatas;
            int poolCount = poolData.Count;
            for (int i = 0; i < poolCount; i++)
            {
                RandomCollectiblePoolData data = poolData[i];
                string item = data.itemName;
                int probability = data.probability;
                for (int j = 0; j < probability; j++)
                {
                    randomCollectiblePools.Add(item);
                }
            }
        }

        randomCollectiblePools.Shuffle();
    }

    public void LoadPlatformSetup()
    {
        string pathDB = "FMDatabase/PlatformSetupDatabase";

        TextAsset jsonFile = Resources.Load<TextAsset>(pathDB);
        if (jsonFile != null)
        {
            PlatformSetupWrapper platformSetupWrapper = JsonUtility.FromJson<PlatformSetupWrapper>(jsonFile.text);
            platformSetupDatas = platformSetupWrapper.setupList;
        }
    }
    
    public Vector3 GetEndTrackPoint(int trackId)
    {
        //debug
        //int count = endTrackPoints.Count;
        //int[] trackIds = new int[count];
        //endTrackPoints.Keys.CopyTo(trackIds, 0);
        //Debug.Log("Track Id Check : " + trackId);
        //string debugText = string.Empty;
        //for (int i = 0; i < count; i++)
        //{
        //    debugText += string.Format("Track Id Check : EndTrackPoints {0}\n", trackIds[i]);
        //}
        //Debug.Log(debugText);

        EndTrackPointData data = endTrackPoints[trackId];
        return data.endTrackPosition;
    }

    public Vector3 GetLastEndTrackPoint()
    {
        int count = endTrackPoints.Count;
        int lastIndex = endTrackPoints.Count - 1;
        EndTrackPointData[] datas = new EndTrackPointData[count];
        endTrackPoints.Values.CopyTo(datas, 0);
        EndTrackPointData data = datas[lastIndex];
        return data.endTrackPosition;
    }

    public Vector3[] GetArrowPositions(int trackId)
    {
        EndTrackPointData data = endTrackPoints[trackId];
        return data.arrowTransitionPositions;
    }

    public List<FMPlatform> GetPlatforms()
    {
        return platforms;
    }

    public void GenerateFixedPlatform()
    {
        int platformIndex = platformFixedPoolCounter.GetIndex();
        ViewMode lastViewMode = ViewMode.COUNT;

        int dataCount = VDParameter.MAX_PLATFORM_DATA_GENERATED_IN_GAME;
        for (int i = 0; i < dataCount; i++)
        {
            int platformCount = mapFixedPool.Count;
            for (int j = 0; j < platformCount; j++)
            {
                PlatformData platformData = mapFixedPool[platformIndex];

                if (platformData.platformStatus == PlatformStatus.StartSideView 
                    || platformData.platformStatus == PlatformStatus.StartBackView
                    || platformData.platformStatus == PlatformStatus.StartBackViewWithPortal)
                {
                    currentPlatformTrackId++;
                }

                if (lastViewMode != platformData.viewMode)
                {
                    lastViewMode = platformData.viewMode;
                    GenerateCoinGarbage();
                }

                int platformId = platformDatas.Count;
                PlatformData newPlatformData = new PlatformData(platformData.folderPath, platformData.platformName, garbageModelName, platformData.platformStatus, platformData.platformTheme, platformData.viewMode, currentPlatformTrackId, platformId, garbageModelIndex);
                platformDatas.Add(newPlatformData);

                platformFixedPoolCounter.Next();
                if (platformFixedPoolCounter.IsEndPool(mapFixedPool.Count))
                {
                    loadRandomPlatform = true;
                    break;
                }

                platformIndex = platformFixedPoolCounter.GetIndex();
            }

            if (platformCount == 0)
            {
                loadRandomPlatform = true;
            }

            if (loadRandomPlatform)
            {
                break;
            }
        }
    }

    public void GenerateMainPlatformData(int changeViewMapCountdown, bool firstTimeLoad)
    {
        bool isSpawnPortal = false;
        PlatformTheme mapTheme = ((MainWorld)world).GetMapTheme();
        int dataCount = VDParameter.MAX_PLATFORM_DATA_GENERATED_IN_GAME;
        for (int i = 0; i < dataCount; i++)
        {
            changeViewMapCountdown--;
            bool isLastTrack = changeViewMapCountdown == 0;
            bool isFirstTrack = changeViewMapCountdown == VDParameter.CHANGE_VIEW_MAP_COUNTDOWN-1;
            bool isFirstPlatform = platformDatas.Count <= 0;

            //manage platform data status
            PlatformStatus platformStatus = PlatformStatus.None;

            if (isFirstPlatform)
            {
                platformStatus = PlatformStatus.StartPoint;
            }

            if (isFirstTrack && !isFirstPlatform)
            {
                currentPlatformTrackId++;

                spawnPortalCountdown--;
                if (spawnPortalCountdown <= 0)
                {
                    isSpawnPortal = true;
                    spawnPortalCountdown = VDParameter.SPAWN_NEXT_PORTAL_COUNT;
                }

                platformStatus = platformViewMode == ViewMode.BackView ? (isSpawnPortal ? PlatformStatus.StartBackViewWithPortal : PlatformStatus.StartBackView) : PlatformStatus.StartSideView;
                GenerateCoinGarbage();
            }
            else if (isLastTrack)
            {
                platformStatus = platformViewMode == ViewMode.SideView ? PlatformStatus.EndSideView : PlatformStatus.EndBackView;
            }
            else
            {
                if (isSpawnPortal)
                {
                    isSpawnPortal = false;
                }
            }

            //create new platform data
            PlatformData newPlatformData = null;
            FMPoolCounter poolCounter = platformViewMode == ViewMode.SideView ? platform2DPoolCounter : platform3DPoolCounter;

            int platformId = platformDatas.Count;
            List<PlatformData> platforms = currentPlatformPool[platformViewMode];
            Tuple<PlatformData, List<PlatformData>> resultProcess = GeneratePlatformDataProcess(platforms, poolCounter, platformId, false);
            
            newPlatformData = resultProcess.Item1;
            newPlatformData.platformStatus = platformStatus;
            newPlatformData.platformTheme = mapTheme;
            newPlatformData.viewMode = platformViewMode;
            newPlatformData.garbageModelIndex = garbageModelIndex;
            newPlatformData.garbageName = garbageModelName;

            currentPlatformPool[platformViewMode] = resultProcess.Item2;

            platformDatas.Add(newPlatformData);

            //change view
            bool isChangeView = changeViewMapCountdown < 0;
            if (isLastTrack)
            {
                changeViewMapCountdown = VDParameter.CHANGE_VIEW_MAP_COUNTDOWN;
                platformViewMode = platformViewMode == ViewMode.SideView ? ViewMode.BackView : ViewMode.SideView;

                ((MainWorld)world).GenerateMapTheme();
                mapTheme = ((MainWorld)world).GetMapTheme();
                SetPlatformPool(mapTheme);
            }

            //debug
            if (i == dataCount - 1)
            {
                newPlatformData.isLastData = true;
            }
            //====
        }

        // TODO: DELETE IFELSE AFTER THERE IS MEDIUM, HARD, VERY HARD PLATFORM FOR OTHER CITIES
        if (selectedKoreaCity == KoreaCity.Busan || selectedKoreaCity == KoreaCity.GyeongJu)
        {
            UpgradeDifficultyLevel();
        }
    }

    public void GenerateSurfPlatformData()
    {
        int dataCount = VDParameter.MAX_PLATFORM_DATA_GENERATED_IN_GAME;

        GenerateCoinGarbage();

        int nextSpawnCountdown = VDParameter.TOTAL_PLATFORM_SPAWN;
        for (int i = 0; i < dataCount; i++)
        {
            GenerateSurfPlatformDataProcess();

            nextSpawnCountdown--;
            if (nextSpawnCountdown <= 0)
            {
                currentPlatformTrackId++;
            }
        }
    }

    public void ClearPlatforms()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMWorld world = mainScene.GetCurrentWorldObject();
        world.ResetColliderObjects();

        WorldType worldType = mainScene.GetCurrentWorldType();
        if (worldType == WorldType.Main)
        {
            MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
            mainWorld.ResetTrampolineHitboxes();
        }

        int count = platforms.Count;
        for (int i = 0; i < count; i++)
        {
            FMPlatform platform = platforms[i];
            platform.Show(false);
            stashPlatforms.Add(platform);
            StashRandomCollectibleObject(platform);
        }

        platformDatas.Clear();
        platforms.Clear();
        platformInactiveCounterRecords.Clear();
        endTrackPoints.Clear();

        count = stashPlatforms.Count;
        for (int i = 0; i < count; i++)
        {
            FMPlatform platform = stashPlatforms[i];
            platform.transform.position = Vector3.zero;
            platform.gameObject.SetActive(false);
        }
    }

    private Dictionary<ViewMode, List<PlatformData>> Load(string[] pathDB)
    {
        Dictionary<ViewMode, List<PlatformData>> result = new Dictionary<ViewMode, List<PlatformData>>();

        ViewMode[] viewModes = (ViewMode[])Enum.GetValues(typeof(ViewMode));
        for (int i = 0; i < viewModes.Length; i++)
        {
            result[viewModes[i]] = new List<PlatformData>();
        }

        PlatformPoolWrapper platformPoolWrapper = null;
        int pathCount = pathDB.Length;

        for (int i = 0; i < pathCount; i++)
        {
            string path = pathDB[i];
            ViewMode viewMode = path.Contains("2D") ? ViewMode.SideView : ViewMode.BackView;

            List<PlatformData> pools = new List<PlatformData>();
            TextAsset jsonFile = FMAssetFactory.GetDatabaseAsset(path);
            if (jsonFile != null)
            {
                platformPoolWrapper = JsonUtility.FromJson<PlatformPoolWrapper>(jsonFile.text);
                int poolCount = platformPoolWrapper.platformPools.Count;

                for (int j = 0; j < poolCount; j++)
                {
                    PlatformPoolData platformPoolData = platformPoolWrapper.platformPools[j];
                    string folderPath = platformPoolData.folderPath;
                    string platformName = platformPoolData.platformName;
                    int probabilityCount = platformPoolData.probability;

                    for (int k = 0; k < probabilityCount; k++)
                    {
                        PlatformData platformData = new PlatformData(folderPath, platformName, string.Empty, PlatformStatus.None, PlatformTheme.COUNT, viewMode, -1, -1, 0);
                        pools.Add(platformData);
                    }
                }
            }

            pools.Shuffle();
            result[viewMode].AddRange(pools);
        }

        return result;
    }


    private void GenerateCoinGarbage()
    {
        garbageModelIndex++;
        if (garbageModelIndex >= garbageTypeCount)
        {
            garbageModelIndex = 0;
        }
        garbageModelName = randomGarbageModelPools[garbageModelIndex];
    }

    private List<PlatformData> LoadPlatformDatas(string pathDB)
    {
        List<PlatformData> result = new List<PlatformData>();
        PlatformPoolWrapper platformPoolWrapper = null;
        string path = pathDB;
        TextAsset jsonFile = FMAssetFactory.GetDatabaseAsset(path);
        if (jsonFile != null)
        {
            platformPoolWrapper = JsonUtility.FromJson<PlatformPoolWrapper>(jsonFile.text);
            int poolCount = platformPoolWrapper.platformPools.Count;
            for (int j = 0; j < poolCount; j++)
            {
                PlatformPoolData platformPoolData = platformPoolWrapper.platformPools[j];
                string folderPath = platformPoolData.folderPath;
                string platformName = platformPoolData.platformName;
                PlatformData platformData = new PlatformData(folderPath, platformName, string.Empty, platformPoolData.platformStatus, platformPoolData.platformTheme, platformPoolData.viewMode, -1,-1, 0);
                result.Add(platformData);
            }
        }

        return result;
    }

    private void GenerateSurfPlatformDataProcess()
    {
        PlatformData newPlatformData = null;
        int platformId = platformDatas.Count;
        Tuple<PlatformData, List<PlatformData>> resultProcess = GeneratePlatformDataProcess(mapSurfPool, platformSurfPoolCounter, platformId,true);
        newPlatformData = resultProcess.Item1;
        newPlatformData.platformTheme = PlatformTheme.Surf;
        newPlatformData.garbageModelIndex = garbageModelIndex;

        mapSurfPool = resultProcess.Item2;

        bool isFirstPlatform = platformDatas.Count <= 0;
        if (isFirstPlatform)
        {
            newPlatformData.platformStatus = PlatformStatus.StartPoint;
        }

        platformDatas.Add(newPlatformData);
    }


    static System.Random rnd = new System.Random();
    static List<int> availableIndices = new List<int> { 0, 1, 2, 3, 4,5 };
    public int GetNextPlatformIndex()
    {
        if (availableIndices.Count == 0)
        {
            availableIndices = new List<int> { 0, 1, 2, 3, 4 };
        }

        int randomIndex = rnd.Next(availableIndices.Count);
        int platformIndex = availableIndices[randomIndex];
        availableIndices.RemoveAt(randomIndex);

        return platformIndex;
    }
    private Tuple<PlatformData,List<PlatformData>> GeneratePlatformDataProcess(List<PlatformData> platformPools, FMPoolCounter poolCounter, int platformId, bool isSurfing)
    {
        PlatformData platformDataResult = null;
        bool allowToGenerate = false;

        List<PlatformData> filteredPlatforms = platformPools
       .Where(p => p.platformName.Contains(currentDifficultyLevel))
       .ToList();

        while (!allowToGenerate)
        {
            PlatformData platformData = null;
            if (!isSurfing)
            {
                System.Random rnd = new System.Random();
                if (currentDifficultyLevel == "All")
                {
                    int platformIndex = rnd.Next(0, VDParameter.MAX_PLATFORM_DATA_GENERATED_IN_GAME);
                    platformData = platformPools[platformIndex];
                }
                else
                {
                    int platformIndex = GetNextPlatformIndex();
                    platformData = filteredPlatforms[platformIndex];

                }
            }
            else
            {
                int platformIndex = GetNextPlatformIndex();
                platformData = platformPools[platformIndex];
            }
                platformDataResult = new PlatformData(platformData.folderPath, platformData.platformName, platformData.garbageName, platformData.platformStatus, platformData.platformTheme, platformData.viewMode, currentPlatformTrackId, platformId, platformData.garbageModelIndex);
            string platformName = platformDataResult.platformName;

            int inactiveCounter = -1;

            bool isRecorded = platformInactiveCounterRecords.ContainsKey(platformName);
            if (isRecorded)
            {
                inactiveCounter = platformInactiveCounterRecords[platformName];

                bool isActive = inactiveCounter <= 0;
                allowToGenerate = isActive;

                if (isActive)
                {
                    int resetCounter = GetPlatformInactiveCounter(platformName);
                    platformInactiveCounterRecords[platformName] = resetCounter;
                }
            }
            else
            {
                inactiveCounter = GetPlatformInactiveCounter(platformName);
                bool validCounter = inactiveCounter >= 0;
                if (validCounter)
                {
                    platformInactiveCounterRecords.Add(platformName, inactiveCounter);
                }

                allowToGenerate = true;
            }

            int count = platformInactiveCounterRecords.Count;
            string[] platformNamedatas = new string[count];
            int[] inactiveCounterDatas = new int[count];
            platformInactiveCounterRecords.Keys.CopyTo(platformNamedatas, 0);
            platformInactiveCounterRecords.Values.CopyTo(inactiveCounterDatas, 0);
            for (int i = 0; i < count; i++)
            {
                string name = platformNamedatas[i];
                int counter = inactiveCounterDatas[i];

                bool isGenerate = name == platformName;
                bool isInactive = counter > 0;
                bool validToCountdown = !isGenerate && isInactive;
                if (validToCountdown)
                {
                    counter--;
                    platformInactiveCounterRecords[name] = counter;
                }
            }

            poolCounter.Next();
            bool isEndPool = poolCounter.IsEndPool(platformPools.Count);
            if (isEndPool)
            {
                //platformPools.Shuffle();
                poolCounter.Reset();
            }
        }

        Tuple<PlatformData, List<PlatformData>> result = new Tuple<PlatformData, List<PlatformData>>(platformDataResult, platformPools);
        return result;
    }


    private void UpgradeDifficultyLevel()
    {
        if (currentDifficultyLevel == "Easy") currentDifficultyLevel = "Medium";
        else if (currentDifficultyLevel == "Medium") currentDifficultyLevel = "Hard";
        else if (currentDifficultyLevel == "Hard") currentDifficultyLevel = "VeryHard";
        else if (currentDifficultyLevel == "VeryHard") currentDifficultyLevel = "All";
    }

    int GetPlatformInactiveCounter(string platformName)
    {
        int result = -1;

        int count = platformSetupDatas.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformSetupData platformSetupData = platformSetupDatas[i];
            if (platformSetupData.platformName == platformName)
            {
                result = platformSetupData.inactiveCounter;
                break;
            }
        }

        return result;
    }

    public bool IsPlatformReachEnd(int objectIndex)
    {
        bool result = false;
        int platformDataCount = platformDatas.Count;
        result = objectIndex >= platformDataCount;
        return result;
    }

    public int GetSurfPlatformID()
    {
        PlatformData platformData = platformDatas[surfPlatformObjectIndex];
        int platformId = platformData.platformId;
        return platformId;
    }   

    public void GenerateSurfPlatform(bool isTutorial)
    {
        //spawn platform
        if (IsPlatformReachEnd(surfPlatformObjectIndex))
        {
            return;
        }

        PlatformData platformData = platformDatas[surfPlatformObjectIndex];
        PlatformStatus edgeMapType = platformData.platformStatus;
        int platformId = platformData.platformId;
        bool isStartPoint = edgeMapType == PlatformStatus.StartPoint;

        string folderPath = platformData.folderPath;
        string nextPlatformName = platformData.platformName;

        string coinModelName = randomGarbageModelPools[platformData.garbageModelIndex];
        //string coinModelName = randomGarbageModelPools[6];
        FMPlatform nextPlatform = GetPlatform(folderPath, nextPlatformName, rootPlatform);
        nextPlatform.Show(true);
        nextPlatform.SetCharacterSpawnPoint(isStartPoint);
        nextPlatform.ShowObjectInSpawnPoint(!isStartPoint);
        nextPlatform.SetCoinModels(coinModelName, ViewMode.SideView);
        nextPlatform.ShowObstacles(!isTutorial);

        StashRandomCollectibleObject(nextPlatform);
        SetRandomCollectible(nextPlatform);
        
        nextPlatform.transform.position = nextSpawnPos;
        nextSpawnPos = nextPlatform.GetNextConnectorPosition();

        int trackId = platformData.trackId;
        if (!endTrackPoints.ContainsKey(trackId))
        {
            Vector3 endTrackPoint = nextPlatform.GetEndTrackPoint();
            EndTrackPointData endTrackPointData = new EndTrackPointData(endTrackPoint, null);
            endTrackPoints.Add(trackId, endTrackPointData);
        }

        platforms.Add(nextPlatform);

        surfPlatformObjectIndex++;

        //stash platform
        int platformCount = platforms.Count;
        if (platformCount > VDParameter.MAX_PLATFORM_IN_GAME)
        {
            int hidePlatformIndex = platformCount - VDParameter.MAX_PLATFORM_IN_GAME - 1;
            FMPlatform platformToHide = platforms[hidePlatformIndex];

            platformToHide.Show(false);
            stashPlatforms.Add(platformToHide);
            platforms.RemoveAt(hidePlatformIndex);
        }
    }

    public void SetMainPlatformIndex(int platformIndex)
    {
        mainPlatformObjectIndex = platformIndex;
    }

    public void RegainMainPlatformData()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        List<PlatformData> datas = mainScene.CacheMainGameData.GetCachePlatformDatas();
        int count = datas.Count;

        int startTrackId = datas[count-1].trackId;
        currentPlatformTrackId = startTrackId;

        for (int i = 0; i < count; i++)
        {
            PlatformData platformData = datas[i];
            platformDatas.Add(platformData);
        }
    }

    public void GenerateMainPlatform(bool fromMiniGame)
    {
        //spawn platform
        if (IsPlatformReachEnd(mainPlatformObjectIndex))
        {
            return;
        }

        PlatformData platformData = platformDatas[mainPlatformObjectIndex];
        PlatformStatus platformStatus = platformData.platformStatus;
        PlatformTheme platformTheme = platformData.platformTheme;
        ViewMode viewMode = platformData.viewMode;
        int platformId = platformData.platformId;
        bool isSideViewTransitionPlatform = platformStatus == PlatformStatus.EndSideView || platformStatus == PlatformStatus.StartBackView || platformStatus == PlatformStatus.StartBackViewWithPortal;
        bool isStartPoint = platformStatus == PlatformStatus.StartPoint;
        bool isStartBackView = platformStatus == PlatformStatus.StartBackView || platformStatus == PlatformStatus.StartBackViewWithPortal;
#if ENABLE_SURF
        bool isShowPortal = platformStatus == PlatformStatus.StartBackViewWithPortal && !fromMiniGame/* && Random.Range(0, 2) == 0*/;
#elif DISABLE_SURF
        bool isShowPortal = false;
#endif
        bool isStartSideView = platformStatus == PlatformStatus.StartSideView;

        string folderPath = platformData.folderPath;
        string nextPlatformName = platformData.platformName;

        //string coinModelName = platformTheme == PlatformTheme.Sand ? "Coin_PlasticBottle" : "Coin_NoodleCup";
        string coinModelName = randomGarbageModelPools[platformData.garbageModelIndex];
        FMPlatform nextPlatform = GetPlatform(folderPath, nextPlatformName, rootPlatform);
        nextPlatform.Show(true);
        nextPlatform.SetPlatformId(platformId);
        nextPlatform.SetCharacterSpawnPoint(isStartPoint);
        if (isStartPoint) nextPlatform.SetRandomStartLogo();
        nextPlatform.HideSideViewTransitionEnvironment(isSideViewTransitionPlatform);
        nextPlatform.ShowSideViewTransitionEnvironment(isSideViewTransitionPlatform);
        nextPlatform.HideSideViewTransitionObstacles(isSideViewTransitionPlatform && fromMiniGame);
        nextPlatform.ShowArrowTransition(isStartBackView);
        nextPlatform.ShowPortal(isShowPortal);
        nextPlatform.ShowObjectInSpawnPoint(!isStartSideView && !isStartBackView && !isStartPoint);
        nextPlatform.SetCoinModels(coinModelName, viewMode);

        StashRandomCollectibleObject(nextPlatform);
        SetRandomCollectible(nextPlatform);

        nextPlatform.transform.position = nextSpawnPos;
        nextSpawnPos = nextPlatform.GetNextConnectorPosition();

        bool isEndTrack =
            platformStatus == PlatformStatus.StartSideView ||
            platformStatus == PlatformStatus.StartBackView ||
            platformStatus == PlatformStatus.StartBackViewWithPortal &&
            platformStatus != PlatformStatus.StartPoint;

        if (isEndTrack)
        {
            int trackId = platformData.trackId;
            Vector3 endTrackPoint = nextPlatform.GetEndTrackPoint();
            Vector3[] arrowTransitionPositions = nextPlatform.GetArrowTransitionPositions();
            EndTrackPointData endTrackPointData = new EndTrackPointData(endTrackPoint,arrowTransitionPositions);
            endTrackPoints.Add(trackId, endTrackPointData);
        }
                    
        platforms.Add(nextPlatform);

        //stash platform
        int platformCount = platforms.Count;
        if (platformCount > VDParameter.MAX_PLATFORM_IN_GAME)
        {
            int hidePlatformIndex = platformCount - VDParameter.MAX_PLATFORM_IN_GAME - 1;
            FMPlatform platformToHide = platforms[hidePlatformIndex];
            StashRandomCollectibleObject(platformToHide);
            platformToHide.Show(false);
            stashPlatforms.Add(platformToHide);
            platforms.RemoveAt(hidePlatformIndex);
        }

        mainPlatformObjectIndex++;
    }

    private void SetRandomCollectible(FMPlatform nextPlatform)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();

        int randomCollectible = nextPlatform.GetRandomCollectibleCount();
        for (int i = 0; i < randomCollectible; i++)
        {
            RandomCollectible checkRandomCollectible = nextPlatform.GetRandomCollectibleObject(i);
            bool isPermanent = checkRandomCollectible.IsCollectiblePermanent(); 
            int poolIndex = -1;
            string randomCollectibleName = string.Empty;

            if (isPermanent)
            {
                randomCollectibleName = checkRandomCollectible.GetCollectibleName();
            }
            else
            {
                poolIndex = randomCollectiblePoolCounter.GetIndex();
                randomCollectibleName = randomCollectiblePools[poolIndex];
            }            

            if (randomCollectibleName == "Garbage")
            {
                bool isPoolAvailable = garbageController.IsGarbagePoolAvailable();
                bool isAllCollected = garbageController.AllCollected;
                while (isAllCollected || !isPoolAvailable)
                {
                    RandomCollectibleNextIndex();

                    poolIndex = randomCollectiblePoolCounter.GetIndex();
                    randomCollectibleName = randomCollectiblePools[poolIndex];

                    if (randomCollectibleName != "Garbage")
                    {
                        break;
                    }
                }
            }

            if (randomCollectibleName != VDParameter.EMPTY_STRING_VALUE)
            {
                PlatformColliderObject collectible = GetRandomCollectible(randomCollectibleName);
                collectible.Show(true);
                nextPlatform.SetRandomCollectibleObject(collectible, i);

                if (collectible is Garbage garbage)
                {
                    //error on android build when get random garbage name using json...
                    //string nextGarbageName = GarbageNameManager.GetRandomGarbageName();
                    string nextGarbageName = "TrashBag";
                    GameObject garbageVisual = GetGarbageVisual(nextGarbageName);
                    garbageVisual.gameObject.SetActive(true);
                    garbage.SetGarbage(garbageVisual);
                }

                if (collectible is InkObstacle inkObstacle)
                {
                    inkObstacle.ResetModel();
                }

                if (collectible is Bomb bomb)
                {
                    bomb.ResetModel();
                }
            }

            if (!isPermanent)
            {
                RandomCollectibleNextIndex();
            }
        }
    }

    public void StashRandomCollectibleObject(FMPlatform platformToHide)
    {
        PlatformColliderObject[] collectiblesToHide = platformToHide.GetRandomCollectibleObjects();
        int collectibleCount = collectiblesToHide.Length;
        for (int i = 0; i < collectibleCount; i++)
        {
            PlatformColliderObject collectible = collectiblesToHide[i];
            if (collectible != null)
            {
                StoreStashRandomCollectible(collectible);

                if (collectible is Garbage garbage)
                {
                    StoreStashGarbageVisual(garbage);
                    garbage.ClearGarbage();
                    garbage.Platform = null;
                }
            }
            platformToHide.ClearRandomCollectible(collectible);
        }
    }

    public void HideDuplicateGarbage(string CheckGarbageName)
    {
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
                    string garbageName = garbage.GetGarbageName();
                    if (garbageName == CheckGarbageName)
                    {
                        garbage.Show(false);
                        platform.ClearRandomCollectible(collectible);
                    }
                }
            }
        }
    }

    public void ReshowGarbages()
    {
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
                    garbage.Show(true);
                }
            }
        }
    }

    public void StoreStashRandomCollectible(PlatformColliderObject collectible)
    {
        collectible.Show(false);
        stashRandomCollectibles.Add(collectible);
    }

    public void StoreStashGarbageVisual(Garbage garbage)
    {
        GameObject garbageVisual = garbage.GetGarbageVisual();
        if (garbageVisual != null)
        {
            garbageVisual.gameObject.SetActive(false);
            stashGarbageVisual.Add(garbageVisual);
        }
    }

    public void StoreStashCoinVisual(GameObject coinVisual)
    {
        coinVisual.gameObject.SetActive(false);
        stashCoinVisual.Add(coinVisual);
    }

    public void SetPlatformPool(PlatformTheme mapTheme)
    {
        switch (mapTheme)
        {
            case PlatformTheme.Sand:
                currentPlatformPool = mapSandPool;
                break;
            case PlatformTheme.Town:
                currentPlatformPool = mapTownPool;
                break;
            case PlatformTheme.GyeongJu:
                currentPlatformPool = mapGyeongJuPool;
                break;
            case PlatformTheme.Seoul:
                currentPlatformPool = mapSeoulPool;
                break;
        }
    }

    private void RandomCollectibleNextIndex()
    {
        randomCollectiblePoolCounter.Next();
        bool isPoolEnd = randomCollectiblePoolCounter.IsEndPool(randomCollectiblePools.Count);
        if (isPoolEnd)
        {
            randomCollectiblePoolCounter.Reset();
        }
    }

    private FMPlatform GetPlatform(string folderPath, string platformName, Transform rootField)
    {
        FMPlatform result = null;
        bool isReuse = false;

        int stashCount = stashPlatforms.Count;
        for (int i = 0; i < stashCount; i++)
        {
            FMPlatform stashPlatform = stashPlatforms[i];
            string stashPlatformName = stashPlatform.gameObject.name;
            if (stashPlatformName == platformName)
            {
                result = stashPlatform;
                stashPlatforms.Remove(stashPlatform);
                isReuse = true;
                break;
            }
        }

        if (!isReuse)
        {
            result = FMAssetFactory.GetPlatform(folderPath, platformName, rootPlatform);
        }

        return result;
    }

    private PlatformColliderObject GetRandomCollectible(string collectibleName)
    {
        PlatformColliderObject result = null;
        bool isReuse = false;

        int stashCount = stashRandomCollectibles.Count;
        for (int i = 0; i < stashCount; i++)
        {
            PlatformColliderObject stash = stashRandomCollectibles[i];
            string stashName = stash.gameObject.name;
            bool isCorrect = stashName == collectibleName;
            if (isCorrect)
            {
                result = stash;
                stashRandomCollectibles.Remove(stash);
                isReuse = true;
                break;
            }
        }

        if (!isReuse)
        {
            result = FMAssetFactory.GetRandomCollectible(collectibleName);
        }

        return result;
    }

    private GameObject GetGarbageVisual(string garbageName)
    {
        GameObject result = null;
        bool isReuse = false;

        int stashCount = stashGarbageVisual.Count;
        for (int i = 0; i < stashCount; i++)
        {
            GameObject stash = stashGarbageVisual[i];
            string stashName = stash.gameObject.name;
            bool isCorrect = stashName == garbageName;
            if (isCorrect)
            {
                result = stash;
                stashGarbageVisual.Remove(stash);
                isReuse = true;
                break;
            }
        }

        if (!isReuse)
        {
            result = FMAssetFactory.GetGarbageVisual(garbageName);
        }

        return result;
    }

    public GameObject GetCoinVisual(string coinName)
    {
        GameObject result = null;
        bool isReuse = false;

        int stashCount = stashCoinVisual.Count;
        for (int i = 0; i < stashCount; i++)
        {
            GameObject stash = stashCoinVisual[i];
            string stashName = stash.gameObject.name;
            bool isCorrect = stashName == coinName;
            if (isCorrect)
            {
                result = stash;
                stashCoinVisual.Remove(stash);
                isReuse = true;
                break;
            }
        }

        if (!isReuse)
        {
            result = FMAssetFactory.GetCoinVisual(coinName);
        }

        return result;
    }

    public void SetRandomStartSign()
    {
        randomStartSign = null;
        randomStartSign = Object.FindFirstObjectByType<FMRandomStartSign>();
        if (randomStartSign != null)
        {
            randomStartSign.GenerateStartSign();
        }
    }

    public string SetCurrentLevel(string level)
    {
        currentDifficultyLevel = level;
        return currentDifficultyLevel;
    }

    protected void Update()
    {
        //if (platforms != null && platforms.Count > 0)
        //{
        //    int count = platforms.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        FMPlatform platform = platforms[i];
        //        PlatformColliderObject[] randomCollectibles = platform.GetRandomCollectibleObjects();
        //        int collectiblesCount = randomCollectibles.Length;
        //        for (int j = 0; j < collectiblesCount; j++)
        //        {
        //            PlatformColliderObject randomCollectible = randomCollectibles[j];
        //            if (randomCollectible is Garbage)
        //            {
        //                Dictionary<string, bool> garbageCollected = garbageController.GarbageCollected;
        //                string garbageName = ((Garbage)randomCollectible).GetGarbageName();
        //                string platformName = platform.name;
        //                string log = string.Format("Check Garbage : Garbage {0} in {1},", garbageName, platformName);

        //                bool duplicateGarbageExist = garbageCollected[garbageName];
        //                if (duplicateGarbageExist)
        //                {
        //                    log += "Garbage is duplicated!";
        //                    Debug.Break();
        //                }

        //                Debug.Log(log);
        //            }
        //        }
        //    }
        //}

        //if (platforms != null && platforms.Count > 0)
        //{
        //    VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        //    MainScene mainScene = currentScene as MainScene;
        //    FMGarbageController garbageController = mainScene.GetGarbageController();

        //    Dictionary<string, bool> garbageCollected = garbageController.GetGarbageCollected();
        //    foreach (KeyValuePair<string, bool> pairs in garbageCollected)
        //    {
        //        string name = pairs.Key;
        //        bool isExist = pairs.Value;
        //        string log = string.Format("Check Garbage : {0} isExist {1}", name, isExist);
        //        Debug.Log(log);
        //    }

        //    int count = platforms.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        FMPlatform platform = platforms[i];
        //        PlatformColliderObject[] randomCollectibles = platform.GetRandomCollectibleObjects();
        //        int collectiblesCount = randomCollectibles.Length;
        //        for (int j = 0; j < collectiblesCount; j++)
        //        {
        //            PlatformColliderObject randomCollectible = randomCollectibles[j];
        //            if (randomCollectible is Garbage)
        //            {
        //                string garbageName = ((Garbage)randomCollectible).GetGarbageName();
        //                bool dataExist = garbageCollected[garbageName];
        //                //string log = string.Format("Check Garbage : {0} isExist {1}", garbageName, dataExist);
        //                //Debug.Log(log);

        //                UIMain uiMain = mainScene.GetUI();

        //                bool uiActive = uiMain.HudGarbages[garbageName].IsActive;

        //                bool notSync = !dataExist && uiActive;
        //                if (notSync)
        //                {
        //                    Debug.Log("Check Garbage : NOT SYNC!!!");
        //                    //Debug.Break();
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
