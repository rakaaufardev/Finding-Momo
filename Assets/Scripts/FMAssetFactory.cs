using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using IngameDebugConsole;
using Cinemachine;

public enum FontMaterial
{
    Exo2_Medium_SDF_Material,
    Exo2_Medium_SDF_Material_Yellow_Outline,
    Exo2_ExtraBold_SDF_Light_Blue_Outline,
    Exo2_ExtraBold_SDF_Yellow_Outline,
    Exo2_ExtraBold_SDF_Grass_Outline
}

public class FMAssetFactory : MonoBehaviour
{
    private static UITutorialMessage prefabUITutorialMessage;
    private static DebugLogManager prefabLogTool;
    private static VDDebugTool prefabDebugTool;
    private static UIVersion prefabUIVersion;
    private static FMMainCharacter prefabMainCharacter;
    private static FMSurfCharacter prefabSurfCharacter;
    private static HUDGarbageItem prefabHudGarbageItem;
    private static HUDHealthIcon prefabHudhealthIcon;
    private static HUDPowerUp prefabHudPowerUp;
    private static FMEndingController prefabEndingController;
    private static UILeaderboardItem prefabUILeaderboardItem;
    private static UIMissionItem prefabUIMissionItem;
    private static UIMissionItem prefabUISurfMission;
    private static UIScoreCategoryItem prefabUIScoreCategoryItem;
    private static UIFlagItem prefabUIFlagItem;
    private static UIDailyLoginItem prefabUIDailyLoginItem;
    private static ShopTabButton prefabShopTabButton;
    private static CostInfoButton prefabCostInfoButton;
    private static UIRewardAlertItem prefabRewardAlertItem;
    private static UIInventoryItem prefabInventoryItem;
    private static LevelMapButton prefabLevelMapButton;
    private static SpriteAtlas spriteAtlasInventoryItem;
    private static SpriteAtlas spriteAtlasShopThumbnail;
    private static SpriteAtlas spriteAtlasMissionThumbnail;
    private static SpriteAtlas spriteAtlasIcons;
    private static SpriteAtlas spriteAtlasGarbageIcon;
    private static SpriteAtlas spriteAtlasFlag;
    private static SpriteAtlas spriteAtlasPhotoThumbnail;
    private static SpriteAtlas spriteAtlasPanel;
    private static SpriteAtlas spriteAtlasButton;
    private static SpriteAtlas spriteAtlasPowerUpIcon;
    private static GameObject prefabMissionGroup;
    private static Dictionary<ShopLayout, ShopItem> prefabShopItems;
    private static Dictionary<string, GameObject> prefabShopModelViewers;
    private static Dictionary<string, FMParallaxBackground> prefabParallaxBackgrounds;
    private static Dictionary<string, FMWorld> prefabWorldContainer;
    private static Dictionary<string, FMPlatform> prefabPlatformContainer;
    private static Dictionary<string, PlatformColliderObject> prefabRandomCollectibleContainer;
    private static Dictionary<string, GameObject> prefabGarbage;
    private static Dictionary<string, GameObject> prefabCoin;
    private static Dictionary<string, FMParticle> prefabParticleContainer;
    private static Dictionary<string, FMFlyParticle> prefabFlyParticleContainer;
    private static Dictionary<string, FMDisplayObject> prefabDisplayObjectContainer;
    private static Dictionary<string, FMCostume> prefabCostume;
    private static Dictionary<string, FMSurfboard> prefabSurfboard;
    private static Dictionary<string, TextAsset> cacheDatabases;
    private static Dictionary<string, Material> cacheSkyboxMaterials;
    private static Dictionary<WorldType, CinemachineBlenderSettings> cameraBlenderSettingContainer;
    private static Dictionary<ShopGroupType, UIGroupContent> prefabShopGroups;
    private static Dictionary<FontMaterial, Material> cacheFontMaterials;
    private static bool initAssetComplete;

    const string PREFIX_WORLD = "{0}World";
    const string PREFIX_BUTTON = "GUI_Button_{0}";
    const string PREFIX_ICON = "GUI_Icon_{0}";
    const string PREFIX_POWERUP_ICON = "GUI_Power_{0}";
    const string PREFIX_FLAG = "GUI_Flag_{0}";
    const string PREFIX_SHOP_FRAME = "GUI_Box_Shop_{0}";
    const string PREFIX_DAILY_LOGIN_ITEM_DEFAULT = "GUI_DailyLogin_Default_{0}";
    const string PREFIX_DAILY_LOGIN_ITEM_BLUE = "GUI_DailyLogin_Blue_{0}";
    const string PREFIX_THUMBNAIL = "Thumbnail_Jon_{0}";
    const string PREFIX_BACKGROUND = "Background_{0}";
    const string PREFIX_COSTUME = "Costume_{0}_Jon_{1}";
    const string PREFIX_CAMERA_BLEND = "{0}CameraBlends";
    const string PREFIX_SURFBOARD = "Surfboard_Skin_{0}";
    const string PREFIX_DISPLAY_OBJECT = "Display_{0}";
    const string PREFIX_SHOP_ITEM = "ShopItem_{0}";
    const string PREFIX_SHOP_GROUP_TYPE = "GroupContent_{0}";

    const string PATH_UITUTORIAL = "FMUITutorials/{0}";
    const string PATH_WORLD = "FMWorlds/{0}";
    const string PATH_DEBUG = "FMDebugs/{0}";
    const string PATH_PLATFORM = "FMPlatforms/{0}/{1}";
    const string PATH_RANDOM_COLLECTIBLE = "FMPlatforms/RandomCollectible/{0}";
    const string PATH_CHARACTER = "FMCharacter/{0}";
    const string PATH_SURFBOARD = "FMSurfboard/{0}";
    const string PATH_ENDING = "FMEndings/{0}";
    const string PATH_COSTUME = "FMCharacter/Costume/{0}";
    const string PATH_GARBAGE = "FMGarbage/{0}";
    const string PATH_COIN = "FMCoin/{0}";
    const string PATH_HUD = "FMUIs/HUDs/{0}";
    const string PATH_PARTICLE = "FMParticle/{0}";
    const string PATH_SPRITE_ATLAS = "FMSpriteAtlas/{0}";
    const string PATH_BACKGROUND = "FMBackground/{0}";
    const string PATH_UI_ITEM = "FMUIItems/{0}";
    const string PATH_CAMERA = "FMCamera/{0}";
    const string PATH_SHOP = "FMShops/{0}";
    const string PATH_LEVEL_MAP = "FMLevelMap/{0}";
    const string PATH_MISSION = "FMMission/{0}";
    const string PATH_SKYBOX = "FMSkybox/MAT_Skybox_{0}";
    const string PATH_DISPLAY_OBJECT = "FMDisplayObject/{0}";
    const string PATH_SOUND = "Sounds/{0}";
    const string PATH_SHOP_MODEL_VIEWER = "FMShopCostume/{0}";
    const string PATH_FONT_MATERIALS = "FMFontMaterials/{0}";

    private static string[] PATH_PLATFORMS_DB = new string[]
    {
        VDParameter.PATH_FIXED_PLATFORMS_DB,

        VDParameter.PATH_MAP_SAND_2D_EASY_DB,
        VDParameter.PATH_MAP_SAND_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_SAND_2D_HARD_DB,
        VDParameter.PATH_MAP_SAND_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_SAND_3D_EASY_DB,
        VDParameter.PATH_MAP_SAND_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_SAND_3D_HARD_DB,
        VDParameter.PATH_MAP_SAND_3D_VERYHARD_DB,

        //VDParameter.PATH_MAP_SAND_2D_POOL_DB,
        //VDParameter.PATH_MAP_SAND_3D_POOL_DB,

        //VDParameter.PATH_MAP_TOWN_2D_POOL_DB,
        //VDParameter.PATH_MAP_TOWN_3D_POOL_DB,

        VDParameter.PATH_MAP_TOWN_2D_EASY_DB,
        VDParameter.PATH_MAP_TOWN_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_TOWN_2D_HARD_DB,
        VDParameter.PATH_MAP_TOWN_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_TOWN_3D_EASY_DB,
        VDParameter.PATH_MAP_TOWN_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_TOWN_3D_HARD_DB,
        VDParameter.PATH_MAP_TOWN_3D_VERYHARD_DB,

        VDParameter.PATH_MAP_GYEONGJU_2D_EASY_DB,
        VDParameter.PATH_MAP_GYEONGJU_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_GYEONGJU_2D_HARD_DB,
        VDParameter.PATH_MAP_GYEONGJU_2D_VERYHARD_DB,

        VDParameter.PATH_MAP_GYEONGJU_3D_EASY_DB,
        VDParameter.PATH_MAP_GYEONGJU_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_GYEONGJU_3D_HARD_DB,
        VDParameter.PATH_MAP_GYEONGJU_3D_VERYHARD_DB,

        VDParameter.PATH_MAP_SEOUL_2D_EASY_DB,
        VDParameter.PATH_MAP_SEOUL_2D_MEDIUM_DB,
        VDParameter.PATH_MAP_SEOUL_2D_HARD_DB,
        VDParameter.PATH_MAP_SEOUL_2D_VERYHARD_DB,
        
        VDParameter.PATH_MAP_SEOUL_3D_EASY_DB,
        VDParameter.PATH_MAP_SEOUL_3D_MEDIUM_DB,
        VDParameter.PATH_MAP_SEOUL_3D_HARD_DB,
        VDParameter.PATH_MAP_SEOUL_3D_VERYHARD_DB,

        VDParameter.PATH_MAP_SURF_POOL_DB,
    };

    private static Dictionary<FontMaterial, string> FONT_MATERIALS = new Dictionary<FontMaterial, string>
    {
        { FontMaterial.Exo2_Medium_SDF_Material,"Exo2-Medium_SDF_Material" },
        { FontMaterial.Exo2_Medium_SDF_Material_Yellow_Outline,"Exo2-Medium_SDF_Material_Yellow_Outline" },
        { FontMaterial.Exo2_ExtraBold_SDF_Light_Blue_Outline,"Exo2-ExtraBold_SDF_Light_Blue_Outline" },
        { FontMaterial.Exo2_ExtraBold_SDF_Yellow_Outline,"Exo2-ExtraBold_SDF_Yellow_Outline" },
        { FontMaterial.Exo2_ExtraBold_SDF_Grass_Outline,"Exo2-ExtraBold_SDF_Grass_Outline" },
    };

    public static Dictionary<string,FMPlatform> PrefabPlatformContainer
    {
        get
        {
            return prefabPlatformContainer;
        }
        set
        {
            prefabPlatformContainer = value;
        }
    }

    public static Dictionary<string, GameObject> PrefabCoin
    {
        get
        {
            return prefabCoin;
        }
        set
        {
            prefabCoin = value;
        }
    }

    public static Dictionary<string, PlatformColliderObject> PrefabRandomCollectibleContainer
    {
        get
        {
            return prefabRandomCollectibleContainer;
        }
        set
        {
            prefabRandomCollectibleContainer = value;
        }
    }

    public static TextAsset GetDatabaseAsset(string path)
    {
        TextAsset result = null;

        if (cacheDatabases == null)
        {
            cacheDatabases = new Dictionary<string, TextAsset>();
        }

        bool isExist = IsDatabaseExist(path);
        if (isExist)
        {
            result = cacheDatabases[path];
        }
        else
        {
            result = Resources.Load<TextAsset>(path);
            cacheDatabases.Add(path, result);
        }

        return result;
    }

    private static bool IsDatabaseExist(string path)
    {
        bool result = cacheDatabases.ContainsKey(path);
        return result;
    }

    public static bool IsInitAssetsComplete()
    {
        return initAssetComplete;
    }

    public static async void InitAssets()
    {
        ResourceRequest resourceRequest = null;
        string dbPath = string.Empty;
        TextAsset jsonFile = null;
        int dataCount = 0;

        prefabPlatformContainer = new Dictionary<string, FMPlatform>();
        prefabRandomCollectibleContainer = new Dictionary<string, PlatformColliderObject>();
        prefabCoin = new Dictionary<string, GameObject>();
        List<string> randomCollectibleNames = new List<string>();
        List<string> garbageNames = new List<string>();

        if (cacheDatabases == null)
        {
            cacheDatabases = new Dictionary<string, TextAsset>();
        }

        //init platforms
        int count = PATH_PLATFORMS_DB.Length;
        for (int i = 0; i < count; i++)
        {
            dbPath = PATH_PLATFORMS_DB[i];
            jsonFile = await GetAndStoreDatabase(dbPath);
            if (jsonFile != null)
            {
                PlatformPoolWrapper platformPoolWrapper = JsonUtility.FromJson<PlatformPoolWrapper>(jsonFile.text);
                int poolCount = platformPoolWrapper.platformPools.Count;
                for (int j = 0; j < poolCount; j++)
                {
                    PlatformPoolData platformPoolData = platformPoolWrapper.platformPools[j];
                    string folderPath = platformPoolData.folderPath;
                    string platformName = platformPoolData.platformName;
                    string fullPath = string.Format(PATH_PLATFORM, folderPath, platformName);
                    await StorePrefabAssets(fullPath, platformName, prefabPlatformContainer);
                }
            }
        }

        //init random collectible
        dbPath = VDParameter.PATH_RANDOM_COLLECTIBLE_DB;
        jsonFile = await GetAndStoreDatabase(dbPath);

        if (jsonFile != null)
        {
            RandomCollectiblePoolWrapper wrapper = JsonUtility.FromJson<RandomCollectiblePoolWrapper>(jsonFile.text);
            List<RandomCollectiblePoolData> poolData = wrapper.itemPoolDatas;
            int poolCount = poolData.Count;
            for (int i = 0; i < poolCount; i++)
            {
                RandomCollectiblePoolData data = poolData[i];
                string item = data.itemName;
                randomCollectibleNames.Add(item);
            }
        }

        dataCount = randomCollectibleNames.Count;
        for (int i = 0; i < dataCount; i++)
        {
            string collectibleName = randomCollectibleNames[i];
            string path = string.Format(PATH_RANDOM_COLLECTIBLE, collectibleName);
            await StorePrefabAssets(path, collectibleName, prefabRandomCollectibleContainer);            
        }

        //init coin visual
        dbPath = VDParameter.PATH_GARBAGE_DB;
        jsonFile = await GetAndStoreDatabase(dbPath);

        if (jsonFile != null)
        {
            GarbageWrapper wrapper = JsonUtility.FromJson<GarbageWrapper>(jsonFile.text);
            List<GarbageSetupData> garbageList = wrapper.garbageList;
            int listCount = garbageList.Count;
            for (int i = 0; i < listCount; i++)
            {
                GarbageSetupData garbageSetupData = garbageList[i];
                string garbageName = garbageSetupData.garbageName;
                garbageNames.Add(garbageName);
            }
        }

        dataCount = garbageNames.Count;
        for (int i = 0; i < dataCount; i++)
        {
            string garbageName = garbageNames[i];
            string path = string.Format(PATH_COIN, garbageName);
            await StorePrefabAssets(path, garbageName, prefabCoin);
        }

        initAssetComplete = true;
    }

    private static async Task StorePrefabAssets<TClass>(string path, string key, Dictionary<string, TClass> container) where TClass : Object
    {
        ResourceRequest resourceRequest = Resources.LoadAsync<TClass>(path);
        while (!resourceRequest.isDone)
        {
            await Task.Yield();
        }

        TClass prefab = resourceRequest.asset as TClass;
        container.Add(key, prefab);
    }

    private static async Task<TextAsset> GetAndStoreDatabase(string path)
    {
        TextAsset jsonFile = null;

        bool isExist = IsDatabaseExist(path);
        if (isExist)
        {
            jsonFile = cacheDatabases[path];
        }
        else
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<TextAsset>(path);

            while (!resourceRequest.isDone)
            {
                await Task.Yield();
            }

            jsonFile = resourceRequest.asset as TextAsset;
            cacheDatabases.Add(path, jsonFile);
        }

        return jsonFile;
    }

    public static CinemachineBlenderSettings GetCameraBlenderSetting(WorldType worldType)
    {
        CinemachineBlenderSettings result = null;
        string name = string.Format(PREFIX_CAMERA_BLEND, worldType.ToString());

        if (cameraBlenderSettingContainer == null)
        {
            cameraBlenderSettingContainer = new Dictionary<WorldType, CinemachineBlenderSettings>();
        }

        if (!cameraBlenderSettingContainer.ContainsKey(worldType))
        {
            string path = string.Format(PATH_CAMERA, name);
            result = Resources.Load<CinemachineBlenderSettings>(path);
            cameraBlenderSettingContainer.Add(worldType, result);
        }
        else
        {
            result = cameraBlenderSettingContainer[worldType];
        }

        return result;
    }

    public static AudioClip GetSound(string name)
    {
        AudioClip result = Resources.Load<AudioClip>(string.Format(PATH_SOUND, name));
        return result;
    }

    public static FMEndingController GetEndingController(Transform root)
    {
        if (prefabEndingController == null)
        {
            prefabEndingController = Resources.Load<FMEndingController>(string.Format(PATH_ENDING, "CinematicEnding"));
        }

        FMEndingController ending = Instantiate(prefabEndingController, root);
        ending.gameObject.name = prefabEndingController.gameObject.name;

        return ending;
    }

    public static LevelMapButton GetLevelMapButton(RectTransform root)
    {
        if (prefabLevelMapButton == null)
        {
            prefabLevelMapButton = Resources.Load<LevelMapButton>(string.Format(PATH_LEVEL_MAP, "LevelMapButton"));
        }

        LevelMapButton button = Instantiate(prefabLevelMapButton, root);
        button.gameObject.name = prefabLevelMapButton.gameObject.name;

        return button;
    }

    public static CostInfoButton GetCostInfoButton(RectTransform root)
    {
        if (prefabCostInfoButton == null)
        {
            prefabCostInfoButton = Resources.Load<CostInfoButton>(string.Format(PATH_SHOP, "CostInfoButton"));
        }

        CostInfoButton button = Instantiate(prefabCostInfoButton, root);
        button.gameObject.name = prefabCostInfoButton.gameObject.name;

        return button;
    }

    public static ShopItem GetShopItem(ShopLayout layout, Transform root)
    {
        ShopItem prefab = null;
        ShopItem item = null;

        if (prefabShopItems == null)
        {
            prefabShopItems = new Dictionary<ShopLayout, ShopItem>();
        }

        if (!prefabShopItems.ContainsKey(layout))
        {
            string itemName = string.Format(PREFIX_SHOP_ITEM, layout.ToString());
            string path = string.Format(PATH_SHOP, itemName);
            prefab = Resources.Load<ShopItem>(path);
            item = Instantiate(prefab, root);
            prefabShopItems.Add(layout, prefab);
        }
        else
        {
            prefab = prefabShopItems[layout];
            item = Instantiate(prefab, root);
        }

        item.gameObject.name = prefab.gameObject.name;

        return item;
    }

    public static UIGroupContent GetShopGroupItem(RectTransform root, ShopGroupType shopGroupType)
    {
        UIGroupContent prefab = null;
        UIGroupContent group = null;

        if (prefabShopGroups == null)
        {
            prefabShopGroups = new Dictionary<ShopGroupType, UIGroupContent>();
        }

        if (!prefabShopGroups.ContainsKey(shopGroupType))
        {
            string groupName = string.Format(PREFIX_SHOP_GROUP_TYPE, shopGroupType);
            string path = string.Format(PATH_SHOP, groupName);
            prefab = Resources.Load<UIGroupContent>(path);
            group = Instantiate(prefab, root);
            prefabShopGroups.Add(shopGroupType, prefab);
        }
        else
        {
            prefab = prefabShopGroups[shopGroupType];
            group = Instantiate(prefab, root);
        }

        group.gameObject.name = prefab.gameObject.name;

        return group;
    }

    public static UIRewardAlertItem GetRewardAlertItem(RectTransform root)
    {
        if (prefabRewardAlertItem == null)
        {
            prefabRewardAlertItem = Resources.Load<UIRewardAlertItem>(string.Format(PATH_UI_ITEM, "RewardAlertItem"));
        }

        UIRewardAlertItem alertItem = Instantiate(prefabRewardAlertItem, root);
        alertItem.gameObject.name = prefabRewardAlertItem.gameObject.name;

        return alertItem;
    }

    public static UIInventoryItem GetInventoryItem(RectTransform root)
    {
        if (prefabInventoryItem == null)
        {
            prefabInventoryItem = Resources.Load<UIInventoryItem>(string.Format(PATH_UI_ITEM, "InventoryItem"));
        }

        UIInventoryItem inventoryItem = Instantiate(prefabInventoryItem, root);
        inventoryItem.gameObject.name = prefabInventoryItem.gameObject.name;

        return inventoryItem;
    }

    public static Sprite GetInventoryItemSprite(string assetName)
    {
        Sprite result = null;

        if (spriteAtlasInventoryItem == null)
        {
            spriteAtlasInventoryItem = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Inventory"));
        }

        if (spriteAtlasInventoryItem != null)
        {
            result = spriteAtlasInventoryItem.GetSprite(assetName);
        }

        if(result == null)
        {
            result = GetDefaultIcon();
        }

        return result;
    }

    public static FMParallaxBackground GetParallaxBackground(string backgroundName, Transform rootParallaxBackground)
    {
        FMParallaxBackground prefab = null;
        FMParallaxBackground background = null;

        if (prefabParallaxBackgrounds == null)
        {
            prefabParallaxBackgrounds = new Dictionary<string, FMParallaxBackground>();
        }

        if (!prefabParallaxBackgrounds.ContainsKey(backgroundName))
        {
            string path = string.Format(PATH_BACKGROUND, backgroundName);
            prefab = Resources.Load<FMParallaxBackground>(path);
            background = Instantiate(prefab, rootParallaxBackground);
            prefabParallaxBackgrounds.Add(backgroundName, prefab);
        }
        else
        {
            prefab = prefabParallaxBackgrounds[backgroundName];
            background = Instantiate(prefab, rootParallaxBackground);
        }

        background.gameObject.name = prefab.gameObject.name;

        return background;
    }

    public static Sprite GetLeaderboardIcon(string iconName)
    {
        Sprite result = null;

        if (spriteAtlasIcons == null)
        {
            spriteAtlasIcons = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Icons"));
        }

        if (spriteAtlasIcons != null)
        {
            string spriteName = string.Format(PREFIX_ICON, iconName);
            result = spriteAtlasIcons.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetCurrencyIcon(RewardType rewardType)
    {
        Sprite result = null;
        string iconName = string.Empty;

        switch (rewardType)
        {
            case RewardType.Coin:
                iconName = "CoinJohn";
                break;
            case RewardType.Gems:
                iconName = "Diamond";
                break;
        }

        if (spriteAtlasIcons == null)
        {
            spriteAtlasIcons = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Icons"));
        }

        if (spriteAtlasIcons != null)
        {
            string spriteName = string.Format(PREFIX_ICON, iconName);
            result = spriteAtlasIcons.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetDefaultIcon()
    {
        Sprite result = null;

        if (spriteAtlasIcons == null)
        {
            spriteAtlasIcons = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Icons"));
        }

        if (spriteAtlasIcons != null)
        {
            string spriteName = string.Format(PREFIX_ICON, "ArnoldHead");
            result = spriteAtlasIcons.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetIcon(string iconName)
    {
        Sprite result = null;

        if (spriteAtlasIcons == null)
        {
            spriteAtlasIcons = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Icons"));
        }

        if (spriteAtlasIcons != null)
        {
            string spriteName = string.Format(PREFIX_ICON, iconName);
            result = spriteAtlasIcons.GetSprite(spriteName);
        }

        if (result == null)
        {
            result = GetDefaultIcon();
        }

        return result;
    }

    public static Sprite GetRewardAlertItemThumbnailImage(string thumbnailName)
    {
        Sprite result = null;

        if (spriteAtlasShopThumbnail == null)
        {
            spriteAtlasShopThumbnail = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Shop"));
        }

        if (spriteAtlasShopThumbnail != null)
        {
            result = spriteAtlasShopThumbnail.GetSprite(thumbnailName);
        }

        if (result == null)
        {
            return GetDefaultIcon();
        }

        return result;
    }

    public static Sprite GetRewardAlertItemIconImage(string iconName)
    {
        Sprite result = null;

        if (spriteAtlasIcons == null)
        {
            spriteAtlasIcons = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Icons"));
        }

        if (spriteAtlasIcons != null)
        {
            string spriteName = string.Format(PREFIX_ICON, iconName);
            result = spriteAtlasIcons.GetSprite(spriteName);
        }

        if(result == null)
        {
            return GetRewardAlertItemIconImage("ArnoldHead");
        }

        return result;
    }


    public static Sprite GetShopFrame(ShopFrameType frameType)
    {
        Sprite result = null;
        string frameName = string.Format(PREFIX_SHOP_FRAME, frameType);

        if (spriteAtlasPanel == null)
        {
            spriteAtlasPanel = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Panel"));
        }

        if (spriteAtlasPanel != null)
        {
            result = spriteAtlasPanel.GetSprite(frameName);
        }

        return result;
    }

    public static Sprite GetBackgroundItem(RewardType rewardType, DailyLoginState state)
    {
        Sprite result = null;
        string prefix = rewardType == RewardType.Coin ? PREFIX_DAILY_LOGIN_ITEM_DEFAULT : PREFIX_DAILY_LOGIN_ITEM_BLUE;
        string backgroundName = string.Format(prefix,state);

        if (spriteAtlasPanel == null)
        {
            spriteAtlasPanel = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Panel"));
        }

        if (spriteAtlasPanel != null)
        {
            result = spriteAtlasPanel.GetSprite(backgroundName);
        }

        return result;
    }

    public static Sprite GetGarbageIcon(string iconName)
    {
        Sprite result = null;

        if (spriteAtlasGarbageIcon == null)
        {
            spriteAtlasGarbageIcon = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Main"));
        }

        if (spriteAtlasGarbageIcon != null)
        {
            string spriteName = string.Format(PREFIX_ICON, iconName);
            result = spriteAtlasGarbageIcon.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetPowerUpIcon(string iconName)
    {
        Sprite result = null;

        if (spriteAtlasPowerUpIcon == null)
        {
            spriteAtlasPowerUpIcon = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Main"));
        }

        if(spriteAtlasPowerUpIcon != null)
        {
            string spriteName = string.Format(PREFIX_POWERUP_ICON, iconName);
            result = spriteAtlasPowerUpIcon.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetFlag(string flagName)
    {
        Sprite result = null;

        if (spriteAtlasFlag == null)
        {
            spriteAtlasFlag = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Flag"));
        }

        if (spriteAtlasFlag != null)
        {
            string spriteName = string.Format(PREFIX_FLAG, flagName);
            result = spriteAtlasFlag.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetLeaderboardItemFrame(bool isCurrent)
    {
        Sprite result = null;

        if (spriteAtlasPanel == null)
        {
            spriteAtlasPanel = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Panel"));
        }

        if (spriteAtlasPanel != null)
        {
            string spriteName = isCurrent ? "GUI_LeaderboardItem_Current" : "GUI_LeaderboardItem_Default";
            result = spriteAtlasPanel.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetPhotoThumbnail(string name)
    {
        Sprite result = null;

        if (spriteAtlasPhotoThumbnail == null)
        {
            spriteAtlasPhotoThumbnail = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_JonThumbnail"));
        }

        if (spriteAtlasPhotoThumbnail != null)
        {
            string spriteName = string.Format(PREFIX_THUMBNAIL, name);
            result = spriteAtlasPhotoThumbnail.GetSprite(spriteName);
        }

        return result;
    }

    public static Material GetSkyboxMaterial(string worldName)
    {
        Material result = null;

        if (cacheSkyboxMaterials == null)
        {
            cacheSkyboxMaterials = new Dictionary<string, Material>();
        }

        if (cacheSkyboxMaterials != null)
        {
            bool isExist = cacheSkyboxMaterials.ContainsKey(worldName);
            if (isExist)
            {
                result = cacheSkyboxMaterials[worldName];
            }
            else
            {
                result = Resources.Load<Material>(string.Format(PATH_SKYBOX, worldName));
            }
        }         

        return result;
    }

    public static FMParticle GetParticle(string particleName)
    {
        FMParticle prefab = null;
        FMParticle particle = null;

        string path = string.Format(PATH_PARTICLE, particleName);

        if (prefabParticleContainer == null)
        {
            prefabParticleContainer = new Dictionary<string, FMParticle>();
        }

        if (!prefabParticleContainer.ContainsKey(particleName))
        {
            prefab = Resources.Load<FMParticle>(path);
            particle = Instantiate(prefab);
            prefabParticleContainer.Add(particleName, prefab);
        }
        else
        {
            prefab = prefabParticleContainer[particleName];
            particle = Instantiate(prefab);
        }

        particle.gameObject.name = prefab.gameObject.name;

        return particle;
    }
    
    public static FMFlyParticle GetFlyParticle(string particleName)
    {
        FMFlyParticle prefab = null;
        FMFlyParticle particle = null;

        string path = string.Format(PATH_PARTICLE, particleName);

        if (prefabFlyParticleContainer == null)
        {
            prefabFlyParticleContainer = new Dictionary<string, FMFlyParticle>();
        }

        if (!prefabFlyParticleContainer.ContainsKey(particleName))
        {
            prefab = Resources.Load<FMFlyParticle>(path);
            particle = Instantiate(prefab);
            prefabFlyParticleContainer.Add(particleName, prefab);
        }
        else
        {
            prefab = prefabFlyParticleContainer[particleName];
            particle = Instantiate(prefab);
        }

        particle.gameObject.name = prefab.gameObject.name;

        return particle;
    }

    public static FMDisplayObject GetDisplayObject(string displayObjectName, Transform rootParent)
    {
        FMDisplayObject prefab = null;
        FMDisplayObject displayObject = null;

        string prefixDisplayObject = string.Format(PREFIX_DISPLAY_OBJECT, displayObjectName);
        string path = string.Format(PATH_DISPLAY_OBJECT, prefixDisplayObject);

        if (prefabDisplayObjectContainer == null)
        {
            prefabDisplayObjectContainer = new Dictionary<string, FMDisplayObject>();
        }

        if (!prefabDisplayObjectContainer.ContainsKey(displayObjectName))
        {
            prefab = Resources.Load<FMDisplayObject>(path);
            displayObject = Instantiate(prefab, rootParent);
            prefabDisplayObjectContainer.Add(displayObjectName, prefab);
        }
        else
        {
            prefab = prefabDisplayObjectContainer[displayObjectName];
            displayObject = Instantiate(prefab, rootParent);
        }

        displayObject.gameObject.name = prefab.gameObject.name;

        return displayObject;
    }

    public static FMCostume GetCostume(Costume inCostume, WorldType inWorldType)
    {
        FMCostume prefab = null;
        FMCostume costume = null;
        string prefixCostume = string.Format(PREFIX_COSTUME, inWorldType, inCostume);
        string path = string.Format(PATH_COSTUME, prefixCostume);

        if (prefabCostume == null)
        {
            prefabCostume = new Dictionary<string, FMCostume>();
        }

        if (!prefabCostume.ContainsKey(prefixCostume))
        {
            prefab = Resources.Load<FMCostume>(path);
            costume = Instantiate(prefab);
            prefabCostume.Add(prefixCostume, prefab);
        }
        else
        {
            prefab = prefabCostume[prefixCostume];
            costume = Instantiate(prefab);
        }

        costume.gameObject.name = prefab.gameObject.name;

        return costume;
    }

    public static FMMainCharacter GetMainCharacter(Transform rootCharacter)
    {
        if (prefabMainCharacter == null)
        {
            prefabMainCharacter = Resources.Load<FMMainCharacter>(string.Format(PATH_CHARACTER, "MainCharacter"));
        }

        FMMainCharacter character = Instantiate(prefabMainCharacter, rootCharacter);

        return character;
    }

    public static FMSurfCharacter GetSurfCharacter(Transform rootCharacter)
    {
        if (prefabSurfCharacter == null)
        {
            prefabSurfCharacter = Resources.Load<FMSurfCharacter>(string.Format(PATH_CHARACTER, "SurfCharacter"));
        }

        FMSurfCharacter character = Instantiate(prefabSurfCharacter, rootCharacter);

        return character;
    }

    public static FMSurfboard GetSurfboard(Surfboard surfboardEquip)
    {
        FMSurfboard prefab = null;
        FMSurfboard surfboard = null;
        string prefixSurfboard = string.Format(PREFIX_SURFBOARD, surfboardEquip.ToString());
        string path = string.Format(PATH_SURFBOARD, prefixSurfboard);

        if (prefabSurfboard == null)
        {
            prefabSurfboard = new Dictionary<string, FMSurfboard>();
        }

        if (!prefabSurfboard.ContainsKey(prefixSurfboard))
        {
            prefab = Resources.Load<FMSurfboard>(path);
            surfboard = Instantiate(prefab);
            prefabSurfboard.Add(prefixSurfboard, prefab);
        }
        else
        {
            prefab = prefabSurfboard[prefixSurfboard];
            surfboard = Instantiate(prefab);
        }

        surfboard.gameObject.name = prefab.gameObject.name;

        return surfboard;
    }

    public static UIMissionItem GetUIMissionItemHorizontal(Transform root)
    {
        if (prefabUIMissionItem == null)
        {
            prefabUIMissionItem = Resources.Load<UIMissionItem>(string.Format(PATH_UI_ITEM, "MissionItemHorizontal"));
        }

        UIMissionItem item = Instantiate(prefabUIMissionItem, root);

        return item;
    }

    public static UIMissionItem GetUIMissionItemVertical(Transform root)
    {
        if (prefabUIMissionItem == null)
        {
            prefabUIMissionItem = Resources.Load<UIMissionItem>(string.Format(PATH_UI_ITEM, "MissionItemVertical"));
        }

        UIMissionItem item = Instantiate(prefabUIMissionItem, root);

        return item;
    }

    public static UIMissionItem GetUISurfMissionItemVertical(Transform root)
    {
        if (prefabUISurfMission == null)
        {
            prefabUISurfMission = Resources.Load<UIMissionItem>(string.Format(PATH_UI_ITEM, "SurfMissionItemVertical"));
        }

        UIMissionItem item = Instantiate(prefabUISurfMission, root);

        return item;
    }

    public static UIScoreCategoryItem GetUIScoreCategoryItem(Transform root)
    {
        if (prefabUIScoreCategoryItem == null)
        {
            prefabUIScoreCategoryItem = Resources.Load<UIScoreCategoryItem>(string.Format(PATH_UI_ITEM, "ScoreCategoryItem"));
        }

        UIScoreCategoryItem item = Instantiate(prefabUIScoreCategoryItem, root);

        return item;
    }

    public static UILeaderboardItem GetUILeaderboardItem()
    {
        if (prefabUILeaderboardItem == null)
        {
            prefabUILeaderboardItem = Resources.Load<UILeaderboardItem>(string.Format(PATH_UI_ITEM, "LeaderboardItem"));
        }

        UILeaderboardItem item = Instantiate(prefabUILeaderboardItem);

        return item;
    }

    public static UIFlagItem GetUIFlagItem(Transform root)
    {
        if (prefabUIFlagItem == null)
        {
            prefabUIFlagItem = Resources.Load<UIFlagItem>(string.Format(PATH_UI_ITEM, "FlagItem"));
        }

        UIFlagItem item = Instantiate(prefabUIFlagItem, root);

        return item;
    }

    public static UIDailyLoginItem GetUIDailyLoginItem()
    {
        if (prefabUIDailyLoginItem == null)
        {
            prefabUIDailyLoginItem = Resources.Load<UIDailyLoginItem>(string.Format(PATH_UI_ITEM, "DailyLoginItem"));
        }

        UIDailyLoginItem item = Instantiate(prefabUIDailyLoginItem);

        return item;
    }

    public static UITutorialMessage GetUiTutorialMessage(RectTransform rootTutorial)
    {
        if (prefabUITutorialMessage == null)
        {
            prefabUITutorialMessage = Resources.Load<UITutorialMessage>(string.Format(PATH_UITUTORIAL, "UITutorialMessage"));
        }

        UITutorialMessage uiTutorial = Instantiate(prefabUITutorialMessage, rootTutorial);

        return uiTutorial;
    }

    public static FMWorld GetWorld(string worldName, Transform rootWorld)
    {
        FMWorld prefab = null;
        FMWorld world = null; 
        string prefabName = string.Format(PREFIX_WORLD, worldName);

        if (prefabWorldContainer == null)
        {
            prefabWorldContainer = new Dictionary<string, FMWorld>();
        }

        if (!prefabWorldContainer.ContainsKey(prefabName))
        {
            string path = string.Format(PATH_WORLD, prefabName);
            prefab = Resources.Load<FMWorld>(path);
            world = Instantiate(prefab,rootWorld);
            prefabWorldContainer.Add(prefabName, prefab);
        }
        else
        {
            prefab = prefabWorldContainer[prefabName];
            world = Instantiate(prefab, rootWorld);
        }

        world.gameObject.name = prefab.gameObject.name;

        return world;
    }

    public static FMPlatform GetPlatform(string folderPath, string platformName, Transform rootField)
    {
        FMPlatform prefab = null;
        FMPlatform platform = null;

        if (prefabPlatformContainer == null)
        {
            prefabPlatformContainer = new Dictionary<string, FMPlatform>();
        }

        if (!prefabPlatformContainer.ContainsKey(platformName))
        {
            string path = string.Format(PATH_PLATFORM, folderPath, platformName);
            prefab = Resources.Load<FMPlatform>(path);
            platform = Instantiate(prefab, rootField);
            prefabPlatformContainer.Add(platformName, prefab);
        }
        else
        {
            prefab = prefabPlatformContainer[platformName];
            platform = Instantiate(prefab, rootField);
        }

        platform.gameObject.name = prefab.gameObject.name;

        return platform;
    }

    public static DebugLogManager GetDebugLog()
    {
        if (prefabLogTool == null)
        {
            prefabLogTool = Resources.Load<DebugLogManager>(string.Format(PATH_DEBUG, "IngameDebugConsole"));
        }

        DebugLogManager debugTool = Instantiate(prefabLogTool);

        return debugTool;
    }

    public static VDDebugTool GetDebugTool()
    {
        if (prefabDebugTool == null)
        {
            prefabDebugTool = Resources.Load<VDDebugTool>(string.Format(PATH_DEBUG, "DebugTool"));
        }

        VDDebugTool debugTool = Instantiate(prefabDebugTool);

        return debugTool;
    }

    public static UIVersion GetUIVersion()
    {
        if (prefabUIVersion == null)
        {
            prefabUIVersion = Resources.Load<UIVersion>(string.Format(PATH_DEBUG, "UIVersion"));
        }

        UIVersion uiVersion = Instantiate(prefabUIVersion);

        return uiVersion;
    }

    public static PlatformColliderObject GetRandomCollectible(string name)
    {
        PlatformColliderObject prefab = null;
        PlatformColliderObject collectible = null;

        if (prefabRandomCollectibleContainer == null)
        {
            prefabRandomCollectibleContainer = new Dictionary<string, PlatformColliderObject>();
        }

        if (!prefabRandomCollectibleContainer.ContainsKey(name))
        {
            string path = string.Format(PATH_RANDOM_COLLECTIBLE, name);
            prefab = Resources.Load<PlatformColliderObject>(path);
            collectible = Instantiate(prefab);
            prefabRandomCollectibleContainer.Add(name, prefab);
        }
        else
        {
            prefab = prefabRandomCollectibleContainer[name];
            collectible = Instantiate(prefab);
        }

        collectible.gameObject.name = prefab.gameObject.name;

        return collectible;
    }
    
    public static GameObject GetGarbageVisual(string garbageName)
    {
        GameObject prefab = null;
        GameObject garbage = null;
        string path = string.Format(PATH_GARBAGE, garbageName);

        if (prefabGarbage == null)
        {
            prefabGarbage = new Dictionary<string, GameObject>();
        }

        if (!prefabGarbage.ContainsKey(garbageName))
        {
            prefab = Resources.Load<GameObject>(path);
            garbage = Instantiate(prefab);
            prefabGarbage.Add(garbageName, prefab);
        }
        else
        {
            prefab = prefabGarbage[garbageName];
            garbage = Instantiate(prefab);
        }

        garbage.gameObject.name = prefab.gameObject.name;

        return garbage;
    }

    public static GameObject GetCoinVisual(string coinName)
    {
        GameObject prefab = null;
        GameObject coin = null;
        string path = string.Format(PATH_COIN, coinName);

        if (prefabCoin == null)
        {
            prefabCoin = new Dictionary<string, GameObject>();
        }

        if (!prefabCoin.ContainsKey(coinName))
        {
            prefab = Resources.Load<GameObject>(path);
            coin = Instantiate(prefab);
            prefabCoin.Add(coinName, prefab);
        }
        else
        {
            prefab = prefabCoin[coinName];
            coin = Instantiate(prefab);
        }

        coin.gameObject.name = prefab.gameObject.name;

        return coin;
    }

    public static HUDGarbageItem GetHUDGarbageItem()
    {
        if (prefabHudGarbageItem == null)
        {
            prefabHudGarbageItem = Resources.Load<HUDGarbageItem>(string.Format(PATH_HUD, "HUDGarbageItem"));
        }

        HUDGarbageItem hud = Instantiate(prefabHudGarbageItem);
        hud.gameObject.name = prefabHudGarbageItem.name;

        return hud;
    }

    public static HUDHealthIcon GetHUDHealthIcon()
    {
        if (prefabHudhealthIcon == null)
        {
            prefabHudhealthIcon = Resources.Load<HUDHealthIcon>(string.Format(PATH_HUD, "HUDHealthIcon"));
        }

        HUDHealthIcon hud = Instantiate(prefabHudhealthIcon);
        hud.gameObject.name = prefabHudhealthIcon.name;

        return hud;
    }

    public static HUDPowerUp GetHUDPowerUp()
    {
        if(prefabHudPowerUp == null)
        {
            prefabHudPowerUp = Resources.Load<HUDPowerUp>(string.Format(PATH_HUD, "HUDPowerUpItem"));
        }
        HUDPowerUp hud = Instantiate(prefabHudPowerUp);
        hud.gameObject.name = prefabHudPowerUp.name;

        return hud;
    }

    public static GameObject GetShopModelViewer(string inModelName)
    {
        GameObject prefab = null;

        if (prefabShopModelViewers == null)
        {
            prefabShopModelViewers = new Dictionary<string, GameObject>();
        }

        if (!prefabShopModelViewers.ContainsKey(inModelName))
        {
            string path = string.Format(PATH_SHOP_MODEL_VIEWER, inModelName);
            prefab = Resources.Load<GameObject>(path);
            prefabShopModelViewers.Add(inModelName, prefab);
        }
        else
        {
            prefab = prefabShopModelViewers[inModelName];
        }

        return prefab;
    }

    public static Sprite GetShopImageThumbnail(string thumbnailName)
    {
        Sprite result = null;

        if (spriteAtlasShopThumbnail == null)
        {
            spriteAtlasShopThumbnail = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Shop"));
        }

        if (spriteAtlasShopThumbnail != null)
        {
            result = spriteAtlasShopThumbnail.GetSprite(thumbnailName);
        }

        return result;
    }

    public static Sprite GetMissionImageThumbnail(string thumbnailName)
    {
        Sprite result = null;
        if (spriteAtlasMissionThumbnail == null)
        {
            spriteAtlasMissionThumbnail = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Mission"));
        }
        if(spriteAtlasMissionThumbnail != null)
        {
            result = spriteAtlasMissionThumbnail.GetSprite(thumbnailName);
        }
        return result;
    }

    public static ShopTabButton GetShopTabButton()
    {
        if (prefabShopTabButton == null)
        {
            prefabShopTabButton = Resources.Load<ShopTabButton>(string.Format(PATH_SHOP, "ShopTabButton"));
        }

        ShopTabButton button = Instantiate(prefabShopTabButton);
        button.gameObject.name = prefabShopTabButton.name;

        return button;
    }

    public static Material GetFontMaterial(FontMaterial fontMaterial)
    {
        Material result = null;

        if (cacheFontMaterials == null)
        {
            cacheFontMaterials = new Dictionary<FontMaterial, Material>();
        }

        bool isExist = cacheFontMaterials.ContainsKey(fontMaterial);
        if (isExist)
        {
            result = cacheFontMaterials[fontMaterial];
        }
        else
        {
            string materialName = FONT_MATERIALS[fontMaterial];
            string path = string.Format(PATH_FONT_MATERIALS, materialName);
            result = Resources.Load<Material>(path);
            cacheFontMaterials.Add(fontMaterial, result);
        }

        return result;
    }

    public static Material GetShopTabFontMaterial(bool buttonIsEnable)
    {
        Material result = null;

        FontMaterial fontMaterial = buttonIsEnable ? FontMaterial.Exo2_Medium_SDF_Material_Yellow_Outline : FontMaterial.Exo2_Medium_SDF_Material;
        result = GetFontMaterial(fontMaterial);

        return result;
    }

    public static Material GetShopPriceFontMaterial(bool useBlue)
    {
        Material result = null;

        FontMaterial fontMaterial = useBlue ? FontMaterial.Exo2_ExtraBold_SDF_Light_Blue_Outline : FontMaterial.Exo2_ExtraBold_SDF_Yellow_Outline;
        result = GetFontMaterial(fontMaterial);

        return result;
    }

    public static Material GetShopEquipFontMaterial(bool isOwned)
    {
        Material result = null;

        FontMaterial fontMaterial = isOwned ? FontMaterial.Exo2_ExtraBold_SDF_Grass_Outline : FontMaterial.Exo2_ExtraBold_SDF_Light_Blue_Outline;
        result = GetFontMaterial(fontMaterial);

        return result;
    }

    public static Sprite GetShopTabSprite(bool buttonIsEnable)
    {
        Sprite result = null;

        string spriteName = buttonIsEnable ? "GUI_Button_Shop_Tab_Active" : "GUI_Button_Shop_Tab_Inactive";

        if (spriteAtlasButton == null)
        {
            spriteAtlasButton = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Buttons"));
        }

        if (spriteAtlasButton != null)
        {
            result = spriteAtlasButton.GetSprite(spriteName);
        }

        return result;
    }

    public static Sprite GetShopEquipSprite(bool isCostumeBought)
    {
        Sprite result = null;

        string spriteName = isCostumeBought ? "GUI_Button_Start" : "GUI_Button_Tutorial";

        if (spriteAtlasButton == null)
        {
            spriteAtlasButton = Resources.Load<SpriteAtlas>(string.Format(PATH_SPRITE_ATLAS, "ATL_Buttons"));
        }

        if (spriteAtlasButton != null)
        {
            result = spriteAtlasButton.GetSprite(spriteName);
        }

        return result;
    }

    public static GameObject GetMissionGroup()
    {
        GameObject group = null;

        if (prefabMissionGroup == null)
        {
            prefabMissionGroup = Resources.Load<GameObject>((string.Format(PATH_MISSION, "MissionContentGroup")));
        }

        group = Instantiate(prefabMissionGroup);
        group.gameObject.name = prefabMissionGroup.gameObject.name;

        return group;
    }
}
