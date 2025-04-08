using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using VD;

public enum WorldMissionType
{
    Distance,
    Garbage,
    Jump,
    Mission,
    COUNT
}

public enum SurfMissionType
{
    MainDistance,
    SurfDistance,
    CollectCoins,
    TimePlayed,
    AirTime,
    ArnoldHit,
    SafeAnimal_SeaTurtle,
    SafeAnimal_Dolphin,
    SafeAnimal_SeaLion,
    SafeAnimal_MantaRay,
    SafeAnimal_KillerWhale,
    SafeAnimal_All,
    COUNT
}

public enum WorldMissionCountry
{
    Australia,
    Chile,
    Egypt,
    Indonesia,
    Japan,
    Korea,
    Russia,
    UK,
    USA,
    COUNT
}

public enum SurfMissionCategory
{
    PermanentMission,
    SurfMission,
    COUNT
}

public class CacheSurfMissionComplete
{
    public string MissionID { get; set; }
    public int MultiplyMission { get; set; }
    public CacheSurfMissionComplete(string missionID, int multiplyMission = 1)
    {
        MissionID = missionID;
        MultiplyMission = multiplyMission;
    }
}

[Serializable]
public class WorldMissionSetupData
{
    public string missionID;
    public string missionDescription;
    public float goalValue;
    public WorldMissionType missionType;
    public RewardType rewardType;
    public int coin;
    public int gem;
    public Costume costume;
    public Surfboard surfboard;
}

[Serializable]
public class WorldMissionSetupWrapper
{
    public List<WorldMissionSetupData> missionList;
}


[Serializable]
public class WorldMissionProgressData
{
    public string missionID;
    public string missionDescription;
    public float goalValue;
    public float goalProgress;
    public WorldMissionType missionType;
    public RewardType rewardType;
    public int coin;
    public int gem;
    public Costume costume;
    public Surfboard surfboard;
    public bool isCompleted;
    public bool isClaimed;

    public WorldMissionProgressData(WorldMissionSetupData missionData)
    {
        missionID = missionData.missionID;
        missionDescription = missionData.missionDescription;
        goalValue = missionData.goalValue;
        missionType = missionData.missionType;
        rewardType = missionData.rewardType;
        coin = missionData.coin;
        gem = missionData.gem;
        costume = missionData.costume;
        surfboard = missionData.surfboard;
    }
}

[Serializable]
public class SurfMissionSetupData
{
    public string missionID;
    public string missionDescription;
    public float goalValue;
    public int rewardValue;
    public SurfMissionType missionType;
    public Surfboard surfboardReward;
}

[Serializable]
public class SurfMissionSetupWrapper
{
    public List<SurfMissionSetupData> missionList;
}

[Serializable]
public class SurfMissionProgressData
{
    public string missionID;
    public string missionDescription;
    public float goalValue;
    public float goalProgress;
    public SurfMissionType missionType;
    public Surfboard surfboardReward;
    public int rewardScoreValue;
    public bool isCompleted;
    public bool showUI;
    public bool hasClaimed;

    public SurfMissionProgressData(SurfMissionSetupData missionData)
    {
        missionID = missionData.missionID;
        missionDescription = missionData.missionDescription;
        goalValue = missionData.goalValue;
        goalProgress = 0;
        missionType = missionData.missionType;
        rewardScoreValue = missionData.rewardValue;
        surfboardReward = missionData.surfboardReward;
    }
}

public class FMMissionController : MonoBehaviour
{
    private static FMMissionController singleton;
    private FMScoreController scoreController;

    private List<WorldMissionProgressData> cacheBusanMissionProgress;
    private List<WorldMissionProgressData> cacheGyeongJuMissionProgress;
    private List<SurfMissionProgressData> cacheSurfMissionProgress;
    private List<SurfMissionProgressData> cachePermanentMissionProgressData;

    private Dictionary<string, WorldMissionProgressData> busanMissionProgress;
    private Dictionary<string, WorldMissionProgressData> gyeongJuMissionProgress;
    private Dictionary<string, SurfMissionProgressData> surfMissionProgress;
    private Dictionary<string, SurfMissionProgressData> permanentMissionProgress;

    private List<CacheSurfMissionComplete> cacheSurfMissionComplete = new List<CacheSurfMissionComplete>();
    private List<string> surfMissionToRemove;
    private List<string> surfMissionToAdd;
    private int cacheMissionScore;

    private KoreaCity currentKoreaCity;

    public KoreaCity CurrentKoreaCity
    {
        get => currentKoreaCity;
        set => currentKoreaCity = value;
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMMissionController Get()
    {
        return singleton;
    }

    public Dictionary<string, SurfMissionProgressData> SurfMissionProgressDictionary
    {
        get => surfMissionProgress;
        set => surfMissionProgress = value;
    }
    
    public Dictionary<string, SurfMissionProgressData> PermanentProgressDictionary
    {
        get => permanentMissionProgress;
        set => permanentMissionProgress = value;
    }

    public void Init()
    {
        if (scoreController != null)
        {
            VDScene currentScene = FMSceneController.Get().GetCurrentScene();
            FMMainScene mainScene = currentScene as FMMainScene;
            scoreController = mainScene.GetScoreController();
        }

        // TODO: Ganti based on Map
        /*string pathKoreaMissionDB = "FMDatabase/KoreaMapMissionDatabase";*/
        string pathBusanMissionDB = "FMDatabase/BusanMissionDatabase";
        string pathGyeongJuMissionDB = "FMDatabase/GyeongJuMissionDatabase";

        string pathSurfMissionDB = "FMDatabase/SurfMissionDatabase";
        string pathPermanentMissionDB = "FMDatabase/PermanentMissionDatabase";
       
        TextAsset jsonBusanMissionFile = Resources.Load<TextAsset>(pathBusanMissionDB);
        TextAsset jsonGyeongJuMissionFile = Resources.Load<TextAsset>(pathGyeongJuMissionDB);
        TextAsset jsonSurfMissionFile = Resources.Load<TextAsset>(pathSurfMissionDB);
        TextAsset jsonPermanentMissionFile = Resources.Load<TextAsset>(pathPermanentMissionDB);

        List<WorldMissionSetupData> busanMissionList = new List<WorldMissionSetupData>();
        List<WorldMissionSetupData> gyeongJuMissionList = new List<WorldMissionSetupData>();
        List<SurfMissionSetupData> surfMissionList = new List<SurfMissionSetupData>();
        List<SurfMissionSetupData> permanentMissionList = new List<SurfMissionSetupData>();

        if (jsonBusanMissionFile != null)
        {
            WorldMissionSetupWrapper wrapper = JsonUtility.FromJson<WorldMissionSetupWrapper>(jsonBusanMissionFile.text);
            busanMissionList = wrapper.missionList;
        }
        
        if (jsonGyeongJuMissionFile != null)
        {
            WorldMissionSetupWrapper wrapper = JsonUtility.FromJson<WorldMissionSetupWrapper>(jsonGyeongJuMissionFile.text);
            gyeongJuMissionList = wrapper.missionList;
        }

        if (jsonSurfMissionFile != null)
        {
            SurfMissionSetupWrapper wrapper = JsonUtility.FromJson<SurfMissionSetupWrapper>(jsonSurfMissionFile.text);
            surfMissionList = wrapper.missionList;
            SurfMissionSetupData mainMission = surfMissionList[0];
            surfMissionList.RemoveAt(0);
            surfMissionList.Shuffle();
            surfMissionList.Insert(0, mainMission);
        }

        if(jsonPermanentMissionFile != null)
        {
            SurfMissionSetupWrapper wrapper = JsonUtility.FromJson<SurfMissionSetupWrapper>(jsonPermanentMissionFile.text);
            permanentMissionList = wrapper.missionList;
        }

        cacheBusanMissionProgress = new List<WorldMissionProgressData>();
        if (busanMissionList != null && busanMissionList.Count > 0)
        {
            for (int i = 0; i < busanMissionList.Count; i++)
            {
                cacheBusanMissionProgress.Add(new WorldMissionProgressData(busanMissionList[i]));
            }
        }
        
        cacheGyeongJuMissionProgress = new List<WorldMissionProgressData>();
        if (gyeongJuMissionList != null && gyeongJuMissionList.Count > 0)
        {
            for (int i = 0; i < gyeongJuMissionList.Count; i++)
            {
                cacheGyeongJuMissionProgress.Add(new WorldMissionProgressData(gyeongJuMissionList[i]));
            }
        }

        cacheSurfMissionProgress = new List<SurfMissionProgressData>();
        if (surfMissionList != null && surfMissionList.Count > 0)
        {
            for (int i = 0; i < surfMissionList.Count; i++)
            {
                cacheSurfMissionProgress.Add(new SurfMissionProgressData(surfMissionList[i]));
            }
        }

        cachePermanentMissionProgressData = new List<SurfMissionProgressData>();
        if (permanentMissionList != null && permanentMissionList.Count > 0)
        {
            for (int i = 0; i < permanentMissionList.Count; i++)
            {
                cachePermanentMissionProgressData.Add(new SurfMissionProgressData(permanentMissionList[i]));
            }
        }

        busanMissionProgress = new Dictionary<string, WorldMissionProgressData>();
        busanMissionProgress = AssignWorldMission(cacheBusanMissionProgress);
        
        gyeongJuMissionProgress = new Dictionary<string, WorldMissionProgressData>();
        gyeongJuMissionProgress = AssignWorldMission(cacheGyeongJuMissionProgress);

        surfMissionProgress = new Dictionary<string, SurfMissionProgressData>();
        surfMissionProgress = AssignSurfMission(cacheSurfMissionProgress, VDParameter.MINIGAME_MISSION_TOTAL);

        permanentMissionProgress = new Dictionary<string, SurfMissionProgressData>();
        permanentMissionProgress = AssignSurfMission(cachePermanentMissionProgressData, null);

        if (surfMissionToRemove == null)
        {
            surfMissionToRemove = new List<string>();
        }
        
        if (surfMissionToAdd == null)
        {
            surfMissionToAdd = new List<string>();
        }

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        bool isUserBusanMissionCreated = inventoryData.koreaMission.userBusanMissionProgress.Count > 0;
        if (isUserBusanMissionCreated)
        {
            List<WorldMissionProgressData> missions = inventoryData.koreaMission.userBusanMissionProgress;
            int count = missions.Count;
            for (int i = 0; i < count; i++)
            {
                WorldMissionProgressData mission = missions[i];
                string missionId = mission.missionID;
                float progress = mission.goalProgress;
                bool isCompleted = mission.goalProgress >= mission.goalValue;
                bool isClaimed = mission.isClaimed;
                busanMissionProgress[missionId].goalProgress = progress;
                busanMissionProgress[missionId].isCompleted = isCompleted;
                busanMissionProgress[missionId].isClaimed = isClaimed;
            }
        }
        else
        {
            int count = busanMissionProgress.Count;
            for (int i = 0; i < count; i++)
            {
                WorldMissionProgressData data = busanMissionProgress.GetValue(i);
                inventoryData.koreaMission.userBusanMissionProgress.Add(data);
            }

            FMUserDataService.Get().SaveInventoryData(inventoryData);
        }

        bool isUserGyeongJuMissionCreated = inventoryData.koreaMission.userGyeongJuMissionProgress.Count > 0;
        if (isUserGyeongJuMissionCreated)
        {
            List<WorldMissionProgressData> missions = inventoryData.koreaMission.userGyeongJuMissionProgress;
            int count = missions.Count;
            for (int i = 0; i < count; i++)
            {
                WorldMissionProgressData mission = missions[i];
                string missionId = mission.missionID;
                float progress = mission.goalProgress;
                bool isCompleted = mission.goalProgress >= mission.goalValue;
                bool isClaimed = mission.isClaimed;
                gyeongJuMissionProgress[missionId].goalProgress = progress;
                gyeongJuMissionProgress[missionId].isCompleted = isCompleted;
                gyeongJuMissionProgress[missionId].isClaimed = isClaimed;
            }
        }
        else
        {
            int count = gyeongJuMissionProgress.Count;
            for (int i = 0; i < count; i++)
            {
                WorldMissionProgressData data = gyeongJuMissionProgress.GetValue(i);
                inventoryData.koreaMission.userGyeongJuMissionProgress.Add(data);
            }

            FMUserDataService.Get().SaveInventoryData(inventoryData);
        }

        bool isUserPermanentMissionCreated = inventoryData.userPermanentMissionProgress.Count > 0;
        if (isUserPermanentMissionCreated)
        {
            List<SurfMissionProgressData> missions = inventoryData.userPermanentMissionProgress;
            int count = missions.Count;
            for (int i = 0; i < count; i++)
            {
                SurfMissionProgressData mission = missions[i];
                string missionId = mission.missionID;
                float progress = mission.goalProgress;
                bool showUI = mission.showUI;
                permanentMissionProgress[missionId].goalProgress = progress;
                permanentMissionProgress[missionId].showUI = showUI;
            }
        }
        else
        {
            int count = permanentMissionProgress.Count;
            for (int i = 0; i < count; i++)
            {
                SurfMissionProgressData data = permanentMissionProgress.GetValue(i);
                inventoryData.userPermanentMissionProgress.Add(data);
            }

            FMUserDataService.Get().SaveInventoryData(inventoryData);
        }
    }

    private Dictionary<string, WorldMissionProgressData> AssignWorldMission(List<WorldMissionProgressData> missions)
    {
        Dictionary<string, WorldMissionProgressData> result = new Dictionary<string, WorldMissionProgressData>();

        int count = missions.Count;
        for (int i = 0; i < count; i++)
        {
            WorldMissionProgressData data = missions[i];
            result.Add(data.missionID, data);
        }
        return result;
    }
    
    private Dictionary<string, SurfMissionProgressData> AssignSurfMission(List<SurfMissionProgressData> missions, int? limit)
    {
        Dictionary<string, SurfMissionProgressData> result = new Dictionary<string, SurfMissionProgressData>();
        HashSet<SurfMissionType> addedMissionTypes = new HashSet<SurfMissionType>();

        int count = missions.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData data = missions[i];
            if (limit != null)
            {
                if (result.Count < limit)
                {
                    bool missionSimilar = addedMissionTypes.Contains(data.missionType);
                    if (!missionSimilar)
                    {
                        result.Add(data.missionID, data);
                        addedMissionTypes.Add(data.missionType);
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                result.Add(data.missionID, data);
            }

        }
        return result;
    }

    public void UpdateWorldMissionProgress(KoreaCity koreaCity, WorldMissionType missionType, float value)
    {
        // TODO: SWITCHCASE MISSION BASED ON COUNTRY/NATION/MAP
        int missionCount = 0;
        WorldMissionProgressData[] collectMissionArray = null;

        switch (koreaCity)
        {
            case KoreaCity.Busan:
                missionCount = busanMissionProgress.Count;
                collectMissionArray = new WorldMissionProgressData[missionCount];
                busanMissionProgress.Values.CopyTo(collectMissionArray, 0);
                break;
            case KoreaCity.GyeongJu:
                missionCount = gyeongJuMissionProgress.Count;
                collectMissionArray = new WorldMissionProgressData[missionCount];
                gyeongJuMissionProgress.Values.CopyTo(collectMissionArray, 0);
                break;
        }

        for (int i = 0; i < missionCount; i++)
        {
            int index = i;
            var mission = collectMissionArray[index];

            InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
            float progress = value;

            if (mission.missionType == missionType)
            {
                CheckWorldMissionProgress(mission, progress);
            }
            SaveWorldMissionProgress(inventoryData.koreaMission.currentKoreaCity, mission, inventoryData);
        }
    }
    
    public void UpdateSurfMissionProgress(SurfMissionCategory missionCategory, SurfMissionType missionType, float value)
    {
        switch (missionCategory) {

            case SurfMissionCategory.SurfMission:
                int count = surfMissionProgress.Count;
                SurfMissionProgressData[] collectMissionArray = new SurfMissionProgressData[count];
                surfMissionProgress.Values.CopyTo(collectMissionArray, 0);
                for (int i = 0; i < count; i++)
                {
                    int index = i;
                    var mission = collectMissionArray[index];
                    float progress = value;

                    if (mission.missionType == missionType)
                    {
                        SurfMissionProgress(mission, progress);
                        break;
                    }
                }
                break;

            case SurfMissionCategory.PermanentMission:
                int countPermanentMission = permanentMissionProgress.Count;
                SurfMissionProgressData[] collectPermanentMissionArray = new SurfMissionProgressData[countPermanentMission];
                permanentMissionProgress.Values.CopyTo(collectPermanentMissionArray, 0);

                for (int j = 0; j < countPermanentMission; j++)
                {
                    int index = j;
                    var mission = collectPermanentMissionArray[index];
                    InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
                    UpdatePermanentMissionProgress(mission, inventoryData);

                    float progress = value;

                    if (mission.missionType == missionType)
                    {
                        SurfMissionProgress(mission, progress);
                    }
                }
                break;        
        }  
    }

    public void SaveWorldMissionProgress(KoreaCity koreaCity, WorldMissionProgressData missionProgressData, InventoryData inventory)
    {
        KoreaMission koreaMission = inventory.koreaMission;

        if (koreaMission.userBusanMissionProgress == null)
        {
            koreaMission.userBusanMissionProgress = new List<WorldMissionProgressData>();
        }
        if (koreaMission.userGyeongJuMissionProgress == null)
        {
            koreaMission.userGyeongJuMissionProgress = new List<WorldMissionProgressData>();
        }

        WorldMissionProgressData userWorldMission = null;

        switch (koreaCity)
        {
            case KoreaCity.Busan:
                foreach (var mission in koreaMission.userBusanMissionProgress)
                {
                    if (mission != null && mission.missionID == missionProgressData.missionID)
                    {
                        userWorldMission = mission;
                        break;
                    }
                }
                break;
            case KoreaCity.GyeongJu:
                foreach (var mission in koreaMission.userGyeongJuMissionProgress)
                {
                    if (mission != null && mission.missionID == missionProgressData.missionID)
                    {
                        userWorldMission = mission;
                        break;
                    }
                }
                break;
        }

        if (userWorldMission != null)
        {
            if (userWorldMission.goalProgress != missionProgressData.goalProgress || userWorldMission.goalValue != missionProgressData.goalValue)
            {
                userWorldMission.goalProgress = missionProgressData.goalProgress;
                userWorldMission.goalValue = missionProgressData.goalValue;
            }

            userWorldMission.isCompleted = userWorldMission.goalProgress >= userWorldMission.goalValue;
            userWorldMission.missionType = missionProgressData.missionType;
        }
        else
        {
            switch (koreaCity)
            {
                case KoreaCity.Busan:
                    koreaMission.userBusanMissionProgress.Add(missionProgressData);
                    break;
                case KoreaCity.GyeongJu:
                    koreaMission.userGyeongJuMissionProgress.Add(missionProgressData);
                    break;
            }
        }
        FMUserDataService.Get().SaveInventoryData(inventory);
    }
    
    public void UpdatePermanentMissionProgress(SurfMissionProgressData missionProgressData, InventoryData inventory)
    {
        if (inventory.userPermanentMissionProgress == null)
        {
            inventory.userPermanentMissionProgress = new List<SurfMissionProgressData>();
        }

        SurfMissionProgressData userPermanentMission = null;

        foreach (var mission in inventory.userPermanentMissionProgress)
        {
            if (mission != null && mission.missionID == missionProgressData.missionID)
            {
                userPermanentMission = mission;
                break;
            }
        }

        if (userPermanentMission != null)
        {
            if (missionProgressData.goalProgress == 0)
            {
                missionProgressData.goalProgress = userPermanentMission.goalProgress;
                missionProgressData.goalValue = userPermanentMission.goalValue;
            }

            if (userPermanentMission.goalProgress != missionProgressData.goalProgress || userPermanentMission.goalValue != missionProgressData.goalValue)
            {
                userPermanentMission.goalProgress = missionProgressData.goalProgress;
                userPermanentMission.goalValue = missionProgressData.goalValue;
            }
            userPermanentMission.missionType = missionProgressData.missionType;
            userPermanentMission.showUI = missionProgressData.showUI;
        }
        else
        {
            inventory.userPermanentMissionProgress.Add(missionProgressData);
        }
        FMUserDataService.Get().SaveInventoryData(inventory);
    }

    public void ResetMainMissionProgress()
    {
        int count = surfMissionProgress.Count;
        SurfMissionProgressData[] collectMissionArray = new SurfMissionProgressData[count];
        surfMissionProgress.Values.CopyTo(collectMissionArray, 0);
        collectMissionArray[0].goalProgress = 0;
    }

    public int UpdateMissionScore()
    {
        List<string> keys = new List<string>(surfMissionProgress.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            SurfMissionProgressData mission = surfMissionProgress[key];
            if (mission.isCompleted)
            {
                CacheSurfMissionComplete checkMissionID = null;
                for (int j = 0; j < cacheSurfMissionComplete.Count; j++)
                {
                    if (cacheSurfMissionComplete[j].MissionID == mission.missionID)
                    {
                        checkMissionID = cacheSurfMissionComplete[j];
                        break;
                    }
                }

                if (checkMissionID != null)
                {
                    checkMissionID.MultiplyMission++;
                }
                else
                {
                    cacheSurfMissionComplete.Add(new CacheSurfMissionComplete(mission.missionID));
                }

                cacheMissionScore += mission.rewardScoreValue;
                surfMissionToRemove.Add(mission.missionID);
            }
        }

        return cacheMissionScore;
    }

    public void ReplaceSurfMission()
    {
        if (surfMissionToRemove != null)
        {
            int count = surfMissionToRemove.Count;

            for (int i = 0; i < count; i++)
            {
                int index = i;
                SurfMissionProgressData missionToMove = cacheSurfMissionProgress.Find(mission => mission.missionID == surfMissionToRemove[index]);
                if (missionToMove.missionID != "m1")
                {
                    cacheSurfMissionProgress.Remove(missionToMove);
                    cacheSurfMissionProgress.Add(missionToMove);
                }
                missionToMove.goalProgress = 0;
                missionToMove.isCompleted = false;
                missionToMove.showUI = false;
            }
            surfMissionProgress = AssignSurfMission(cacheSurfMissionProgress, VDParameter.MINIGAME_MISSION_TOTAL);
        }
        surfMissionToRemove.Clear();
    }


    public void CheckSurfMainMissionProgress(FMSurfAnimalSpawn surfAnimalSpawner)
    {
        
        int count = cacheSurfMissionProgress.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData mission = cacheSurfMissionProgress[i];
            if (mission.missionID == "m1")
            {
                float value = mission.goalProgress;
                float maxGoal = mission.goalValue;
                surfAnimalSpawner.SetAnimalActive(value,maxGoal);
            }
        }
    }
    public SurfMissionProgressData GetSurfMainMission()
    {
        SurfMissionProgressData result = null;

        int count = cacheSurfMissionProgress.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData mission = cacheSurfMissionProgress[i];
            if (mission.missionID == "m1")
            {
                result = mission;
                break;
            }
        }

        return result;
    }

    public List<(string missionName, int missionReward, int missionRepeat)> GetCompletedMissionsData()
    {
        var completedMissions = new List<(string missionName, int missionReward, int missionRepeat)>();

        if (cacheSurfMissionComplete != null && cacheSurfMissionComplete.Count > 0)
        {
                for (int i = 0; i < cacheSurfMissionComplete.Count; i++)
                {
                    CacheSurfMissionComplete savedMission = cacheSurfMissionComplete[i];
                    SurfMissionProgressData missionProgressData = cacheSurfMissionProgress.Find(m => m.missionID == savedMission.MissionID);

                    if (missionProgressData != null)
                    {
                        string missionName = missionProgressData.missionDescription;
                        int missionRepeat = savedMission.MultiplyMission;
                        int missionReward = missionRepeat > 0
                            ? missionProgressData.rewardScoreValue * missionRepeat
                            : missionProgressData.rewardScoreValue;

                        completedMissions.Add((missionName, missionReward, missionRepeat));
                    }
                }
        }

        return completedMissions;
    }

    public SurfMissionProgressData GetPermanentMissionNearComplete()
    {
        SurfMissionProgressData permanentMission = null;
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        bool isUserPermanentMissionCreated = inventoryData.userPermanentMissionProgress.Count > 0;

        if (isUserPermanentMissionCreated)
        {
            List<SurfMissionProgressData> missions = inventoryData.userPermanentMissionProgress;
            List<SurfMissionProgressData> sortedProgressMissions = new List<SurfMissionProgressData>();
            int count = missions.Count;

            for (int i = 0; i < count; i++)
            {
                SurfMissionProgressData mission = missions[i];
                bool isComplete = mission.goalProgress == mission.goalValue;
                if (!isComplete)
                {
                    sortedProgressMissions.Add(missions[i]);
                }
            }

            if (sortedProgressMissions.Count > 0)
            {
                sortedProgressMissions.Sort((missionA, missionB) => (missionB.goalProgress / missionB.goalValue).CompareTo(missionA.goalProgress / missionA.goalValue));

                SurfMissionProgressData topMission = sortedProgressMissions[0];
                float percentage = topMission.goalProgress / topMission.goalValue;
                bool isNearCompletion = percentage < 1 && percentage >= 0.9f;

                if (isNearCompletion)
                {
                    permanentMission = topMission;
                }
            }
        }
        return permanentMission;
    }

    public void CompleteAllPermanentMissions()
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> savedPermanentMissions = inventoryData.userPermanentMissionProgress;
        int count = savedPermanentMissions.Count;
        UIRewardAlertItemData[] alertitemDatas = new UIRewardAlertItemData[count];
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData savedMission = savedPermanentMissions[i];
            bool isComplete = savedMission.goalProgress == savedMission.goalValue && !savedMission.hasClaimed;
            if (isComplete)
            {
                savedMission.hasClaimed = true;
                FMInventory.Get.AddInventory(RewardType.Surfboard, (int)savedMission.surfboardReward, 0);
                FMSceneController.Get().CheckClaimableMissionReward();                
                alertitemDatas[i] = new UIRewardAlertItemData(RewardType.Surfboard, (int)savedMission.surfboardReward, 0);
            }
        }
        FMUIWindowController.Get.OpenWindow(UIWindowType.RewardAlert, alertitemDatas);

        count = permanentMissionProgress.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData missionProgressData = permanentMissionProgress.GetValue(i);
            bool isComplete = missionProgressData.goalProgress == missionProgressData.goalValue;
            if (isComplete)
            {
                missionProgressData.isCompleted = true;
            }
        }
    }

    private void CheckWorldMissionProgress(WorldMissionProgressData missionProgressData, float progress)
    {
        if (missionProgressData.isCompleted)
        {
            return;
        }

        if (missionProgressData.goalProgress < missionProgressData.goalValue)
        {
            missionProgressData.goalProgress += progress;

            bool isCompleted = missionProgressData.goalProgress >= missionProgressData.goalValue;

            if (isCompleted)
            {
                missionProgressData.isCompleted = true;
            }
        }
    }
    
    private void SurfMissionProgress(SurfMissionProgressData missionProgressData, float progress)
    {
        if (missionProgressData.isCompleted)
        {
            return;
        }

        if (missionProgressData.goalProgress < missionProgressData.goalValue)
        {
            missionProgressData.goalProgress += progress;

            bool isCompleted = missionProgressData.goalProgress >= missionProgressData.goalValue;

            if (isCompleted)
            {
                missionProgressData.isCompleted = true;

                if (!missionProgressData.showUI)
                {
                    VDScene currentScene = FMSceneController.Get().GetCurrentScene();
                    FMMainScene mainScene = currentScene as FMMainScene;
                    UIMain uiMain = mainScene.GetUI();

                    missionProgressData.showUI = true;
                    uiMain.ShowMissionNotice(missionProgressData.missionDescription);
                }
            }
        }
    }

    public bool IsClaimableMissionExist()
    {
        bool result = false;

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> savedPermanentMissions = inventoryData.userPermanentMissionProgress;
        int count = savedPermanentMissions.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData savedMission = savedPermanentMissions[i];
            result = savedMission.goalProgress >= savedMission.goalValue && !savedMission.hasClaimed;
            if (result)
            {
                break;
            }
        }

        return result;
    }

    public bool IsMapMissionAllCompleted(WorldMissionCountry map)
    {
        bool isCompleted = false;

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        List<WorldMissionProgressData> worldMissionList = new();
        switch (map)
        {
            case WorldMissionCountry.Korea:
                worldMissionList = userInfo.inventoryData.koreaMission.userGyeongJuMissionProgress;
                break;
            case WorldMissionCountry.Australia:
            case WorldMissionCountry.Chile:
            case WorldMissionCountry.Egypt:
            case WorldMissionCountry.Indonesia:
            case WorldMissionCountry.Japan:
            case WorldMissionCountry.Russia:
            case WorldMissionCountry.UK:
            case WorldMissionCountry.USA:
            default:
                break;
        }

        // FUTURE-PROOFING, DELETE THE IF WHEN EVERY CATEGORY HAS A MISSION LIST
        if (worldMissionList.Count > 0)
        {
            int count = worldMissionList.Count;
            int lastIndex = count - 1;

            isCompleted = worldMissionList[lastIndex].isCompleted;
        }

        return isCompleted;
    }

    public bool IsClaimable(InventoryData inventoryData, Surfboard surfboardReward, float goalProgress, float goalValue)
    {
        bool result = false;
        bool ownEquipment = IsOwnEquipment(inventoryData, surfboardReward);
        bool isComplete = goalProgress == goalValue;
        bool rewardClaimed = ownEquipment;
        result = isComplete && !rewardClaimed;

        return result;
    }

    public bool IsOwnEquipment(InventoryData inventoryData, Surfboard surfboardReward)
    {
        bool result = false;
        int surfboardCount = inventoryData.surfboards.Count;
        for (int j = 0; j < surfboardCount; j++)
        {
            Surfboard userInventorySurfboard = inventoryData.surfboards[j];
            if (userInventorySurfboard == surfboardReward)
            {
                result = true;
                break;
            }
        }

        return result;
    }
}
