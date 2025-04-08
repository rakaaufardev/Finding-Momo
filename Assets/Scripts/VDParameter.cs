using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using static System.Net.WebRequestMethods;

public static class VDParameter
{
    //
    // Easing website : https://easings.net
    //

    public const string EMPTY_STRING_VALUE = "Empty";
    public const float ONE_UNIT_VALUE = 1f;
    public const float SKYBOX_ROTATE_SPEED = 0.25f;
    public const float CINEMATIC_OFFSET_POSITION = 50f;
    public const float JUMP_BUFFER_DURATION_IN_SEC = 0.1f;
    public const float INVULNERABLE_DURATION_IN_SEC = 4f;
    public const float INVULNERABLE_DURATION_ITEM_IN_SEC = 4f;
    public const float SPEEDUP_DURATION_IN_SEC = 8f;
    public const float MULTIPLY_SCORE_DURATION_IN_SEC = 8f;
    public const float MAGNET_DURATION_IN_SEC = 8f;
    public const float INVULNERABLE_START_BLINK_TIME = 1.5f;
    public const float INVULNERABLE_BLINK_INTERVAL = 0.25f;
    public const float SURF_MAX_DISTANCE = 2200;
    public const float MAX_ANIMATION_SPEED_INCREMENT = 0.5f;
    public const float INK_MAIN_DURATION = 5f;
    public const float INK_TUTORIAL_DURATION = 2f;
    public const int MAXIMUM_FLY_PARTICLE = 50;
    public const int SPAWN_NEXT_PORTAL_COUNT = 1;
    public const int MINIGAME_MISSION_TOTAL = 4;
    public const int CONTINUE_AMOUNT = 2;
    public const int DIAMOND_CONTINUE_COST = 5;

    //Ads config
    public const string ADS_ID_REWARDED_VIDEO_TEST = "ca-app-pub-3940256099942544/5224354917";

    //Time config
    public const string TIME_ZONE = "Asia/Jakarta";
    public const string TIME_ZONE_API = "https://timeapi.io/api/Time/current/zone?timeZone={0}";

    //Bundle and shop online config
    /*
        Original: https://drive.google.com/file/d/FILE_ID/view?usp=sharing
       Change: https://drive.google.com/uc?id=FILE_ID
    */
    public static Dictionary<BundleName, BundleData> BUNDLE_CONTAINER = new Dictionary<BundleName, BundleData>()
    {
        { 
            BundleName.ShopThumbnail,
            new BundleData()
            {
                bundleUrl = "https://drive.google.com/uc?id=1GMu9tI4QZiZ6b4RzcQlWoQt80et7BaKP",
                manifestUrl = "https://drive.google.com/uc?id=1v3aeflsRYVE9s7-fAtNr4jrU9QK1-RAo",
                localBundlePath = Path.Combine(Application.persistentDataPath, "shop"),
                localVersionPath = Path.Combine(Application.persistentDataPath, "ShopThumbnail.txt")
            } 
        },
        { 
            BundleName.ShopCostume, 
            new BundleData()
            {
                bundleUrl = "https://drive.google.com/uc?id=1SrK_SZP5fjH9tZe20Wu3RXNXCMBlMAM7",
                manifestUrl = "https://drive.google.com/uc?id=1Nwh7gPQRqMSq8BASQb9YJuxKh81DgJOl",
                localBundlePath = Path.Combine(Application.persistentDataPath, "shopcostume"),
                localVersionPath = Path.Combine(Application.persistentDataPath, "ShopCostume.txt")
            }  
        }
    };

    //animal health
    public static Dictionary<RewardObstacleType, int> ANIMAL_HEALTH = new Dictionary<RewardObstacleType, int>
    {
        { RewardObstacleType.SeaTurtle, 1 },
        { RewardObstacleType.Dolphin, 1 },
        { RewardObstacleType.SeaLion, 1 },
        { RewardObstacleType.MantaRay, 2 },
        { RewardObstacleType.KillerWhale, 3 }
    };

    public static readonly Dictionary<RewardObstacleType, int> DEFAULT_ANIMAL_HEALTH = new Dictionary<RewardObstacleType, int>(ANIMAL_HEALTH);

    public const float RANDOM_ANIMAL_THRESHOLD = 0.4f;
    public const float MINIBOSS_THRESHOLD = 0.5f;
    public const float BOSS_THRESHOLD = 0.7f;

    public static OnlineConfigData SHOP_ONLINE_CONFIG_DATA = new OnlineConfigData()
    {
        configUrl = "https://drive.google.com/uc?id=1vbbQR4rY5CZnts74ax_8tfSvlqTDcSxX",
        versionUrl = "https://drive.google.com/uc?id=1Fr5D9mu9_qSMZ5-AYe7SJAIIaRR8P3bt"
    };

    public static OnlineConfigData DAILY_LOGIN_ONLINE_CONFIG_DATA = new OnlineConfigData()
    {
        configUrl = "https://drive.google.com/uc?id=1R5zCwROyV0mlEzDRSAXSPW7TUFJVqLh1",
        versionUrl = "https://drive.google.com/uc?id=1Y9cMw0h552A3aCllJ-h7jeQF6SpyfYGo"
    };

    //Databases
    public const string PATH_FIXED_PLATFORMS_DB = "FMDatabase/FixedPlatformDatabase";

    public const string PATH_MAP_SAND_2D_POOL_DB = "FMDatabase/MainSand2DDatabase";
    public const string PATH_MAP_SAND_3D_POOL_DB = "FMDatabase/MainSand3DDatabase";

    public const string PATH_MAP_SAND_2D_EASY_DB = "FMDatabase/MainSand2D_Easy_Database";
    public const string PATH_MAP_SAND_2D_MEDIUM_DB = "FMDatabase/MainSand2D_Medium_Database";
    public const string PATH_MAP_SAND_2D_HARD_DB = "FMDatabase/MainSand2D_Hard_Database";
    public const string PATH_MAP_SAND_2D_VERYHARD_DB = "FMDatabase/MainSand2D_VeryHard_Database";

    public const string PATH_MAP_SAND_3D_EASY_DB = "FMDatabase/MainSand3D_Easy_Database";
    public const string PATH_MAP_SAND_3D_MEDIUM_DB = "FMDatabase/MainSand3D_Medium_Database";
    public const string PATH_MAP_SAND_3D_HARD_DB = "FMDatabase/MainSand3D_Hard_Database";
    public const string PATH_MAP_SAND_3D_VERYHARD_DB = "FMDatabase/MainSand3D_VeryHard_Database";

    public const string PATH_MAP_TOWN_2D_POOL_DB = "FMDatabase/MainTown2DDatabase";
    public const string PATH_MAP_TOWN_3D_POOL_DB = "FMDatabase/MainTown3DDatabase";

    public const string PATH_MAP_TOWN_2D_EASY_DB = "FMDatabase/MainTown2D_Easy_Database";
    public const string PATH_MAP_TOWN_2D_MEDIUM_DB = "FMDatabase/MainTown2D_Medium_Database";
    public const string PATH_MAP_TOWN_2D_HARD_DB = "FMDatabase/MainTown2D_Hard_Database";
    public const string PATH_MAP_TOWN_2D_VERYHARD_DB = "FMDatabase/MainTown2D_VeryHard_Database";

    public const string PATH_MAP_TOWN_3D_EASY_DB = "FMDatabase/MainTown3D_Easy_Database";
    public const string PATH_MAP_TOWN_3D_MEDIUM_DB = "FMDatabase/MainTown3D_Medium_Database";
    public const string PATH_MAP_TOWN_3D_HARD_DB = "FMDatabase/MainTown3D_Hard_Database";
    public const string PATH_MAP_TOWN_3D_VERYHARD_DB = "FMDatabase/MainTown3D_VeryHard_Database";

    public const string PATH_MAP_GYEONGJU_2D_EASY_DB = "FMDatabase/MainGyeongJu2D_Easy_Database";
    public const string PATH_MAP_GYEONGJU_2D_MEDIUM_DB = "FMDatabase/MainGyeongJu2D_Medium_Database";
    public const string PATH_MAP_GYEONGJU_2D_HARD_DB = "FMDatabase/MainGyeongJu2D_Hard_Database";
    public const string PATH_MAP_GYEONGJU_2D_VERYHARD_DB = "FMDatabase/MainGyeongJu2D_VeryHard_Database";

    public const string PATH_MAP_GYEONGJU_3D_EASY_DB = "FMDatabase/MainGyeongJu3D_Easy_Database";
    public const string PATH_MAP_GYEONGJU_3D_MEDIUM_DB = "FMDatabase/MainGyeongJu3D_Medium_Database";
    public const string PATH_MAP_GYEONGJU_3D_HARD_DB = "FMDatabase/MainGyeongJu3D_Hard_Database";
    public const string PATH_MAP_GYEONGJU_3D_VERYHARD_DB = "FMDatabase/MainGyeongJu3D_VeryHard_Database";

    public const string PATH_MAP_SEOUL_2D_EASY_DB = "FMDatabase/MainSeoul2D_Easy_Database";
    public const string PATH_MAP_SEOUL_2D_MEDIUM_DB = "FMDatabase/MainSeoul2D_Medium_Database";
    public const string PATH_MAP_SEOUL_2D_HARD_DB = "FMDatabase/MainSeoul2D_Hard_Database";
    public const string PATH_MAP_SEOUL_2D_VERYHARD_DB = "FMDatabase/MainSeoul2D_VeryHard_Database";
    
    public const string PATH_MAP_SEOUL_3D_EASY_DB = "FMDatabase/MainSeoul3D_Easy_Database";
    public const string PATH_MAP_SEOUL_3D_MEDIUM_DB = "FMDatabase/MainSeoul3D_Medium_Database";
    public const string PATH_MAP_SEOUL_3D_HARD_DB = "FMDatabase/MainSeoul3D_Hard_Database";
    public const string PATH_MAP_SEOUL_3D_VERYHARD_DB = "FMDatabase/MainSeoul3D_VeryHard_Database";

    public const string PATH_MAP_SURF_POOL_DB = "FMDatabase/SurfDatabase";
    public const string PATH_RANDOM_COLLECTIBLE_DB = "FMDatabase/RandomCollectibleDatabase";
    public const string PATH_GARBAGE_DB = "FMDatabase/GarbageDatabase";
    public const string PATH_COIN_DB = "FMDatabase/CoinDatabase";

    //Leaderboard
    public const int LEADERBOARD_MAX_RANK_SHOW_COUNT = 10;
    public const int LEADERBOARD_MAX_PLAYER_RANK_SHOW_COUNT = 5;

    //Tags
    public const string TAG_GROUND = "Ground";
    public const string TAG_HITBOX = "HitBox";
    public const string TAG_TRAMPOLINE = "Trampoline";

    //Game Timer
#if SHORT_GAME
    public const float GAME_TIME_LIMIT_IN_SEC = 180f;
#elif LONG_GAME
    public const float GAME_TIME_LIMIT_IN_SEC = 3600f;
#endif

#if ENABLE_ENDLESS
    public const bool GAME_TIME_ACTIVE = false;
#elif DISABLE_ENDLESS
    public const bool GAME_TIME_ACTIVE = true;
#endif

    //layer
    public const int LAYER_CHARACTER = 3;
    public const int LAYER_OBSTACLE = 6;
    public const int LAYER_COIN = 9;

    //platform
    public const int CHANGE_VIEW_MAP_COUNTDOWN = 3;
    public const int MAX_PLATFORM_IN_GAME = 17;
    public const int MAX_PLATFORM_DATA_GENERATED_IN_GAME = 6; //exclude start platform | make sure end of generate is last platform in side view or back view
    public const int TOTAL_PLATFORM_SPAWN = 9;
    public const int TOTAL_PLATFORM_SPAWN_IN_TUTORIAL = 9;
    public const float DISTANCE_TRIGGER_SPAWN_MAIN_PLATFORM = 250f;

    //character main world
    public static Vector3 CHARACTER_SPAWN_OFFSET_POSITION = new Vector3(0, 0, 0);
    public const float CHARACTER_HIT_OBSTACLE_FORCE = 45f;
    public const float CHARACTER_JUMP_GRAVITY = 70f;
    public const float CHARACTER_FORCE_FALL_GRAVITY = 90f;
    public const float CHARACTER_QUICK_LAND_GRAVITY = 1440f;
    public const float CHARACTER_JUMP_FORCE = 32f;
    public const float CHARACTER_TRAMPOLINE_FORCE = 35f;
    public const float CHARACTER_SPEED = 15f;
    public const float CHARACTER_CHANGE_LINE_SPEED = 0.5f;
    public const float CHARACTER_DEFAULT_HITBOX_SCALE_Y = 2f;
    public const float CHARACTER_DEFAULT_HITBOX_POS_Y = 0f;
    public const float CHARACTER_SLIDE_HITBOX_SCALE_Y = 1.4f;
    public const float CHARACTER_SLIDE_HITBOX_POS_Y = -0.15f;
    public const float CHARACTER_SLIDE_DURATION_IN_SEC = 1f;
    public const float CHARACTER_CHANGE_TRANSITION_DURATION_IN_SEC = 0.75f;
    public const int CHARACTER_MAX_HEALTH = 3;

    //character surf world
    public const float CHARACTER_SURF_SPEED = 15f;
    public const float CHARACTER_MAX_SURF = 8.5f;
    public const float CHARACTER_MIN_SURF = 0f;
    public const float CHARACTER_SURFING_VALUE = 60f;
    public const float CHARACTER_GRAVITY_FALL_SURF = 110f;
    public const float CHARACTER_GRAVITY_HIGH_FALL_SURF = 55f;
    public const float CHARACTER_GRAVITY_QUICK_FALL = 2700f;
    public const float CHARACTER_MULTIPLIER_JUMP_FORCE_SURF = 1500f;
    public const float CHARACTER_THRESHOLD_JUMP_FORCE_SURF = 210f;
    public const float SWITCH_CAMERA_JUMP_POSITION = 12f;
    public const float SWITCH_CAMERA_SURF_POSITION = 7.5f;

    //dolphin
    public const float DOLPHIN_JUMP_TO_TARGET_POSITION_Y = 10f;
    public const float DOLPHIN_JUMP_TO_TARGET_DURATION_IN_SEC = 1f;
    public const Ease DOLPHIN_JUMP_EASE = Ease.OutCirc;
    public const float DOLPHIN_FALL_TARGET_POSITION_Y = 0f;
    public const float DOLPHIN_FALL_TARGET_DURATION_IN_SEC = 1f;
    public const Ease DOLPHIN_FALL_EASE = Ease.InQuad;
    public const float DOLPHIN_TRIGGER_JUMP_DISTANCE = 21f;

    //score
    public const int SCORE_DISTANCE_MULTIPLIER = 2;
    public const int SCORE_DISTANCE_MULTIPLER_POWERUP = 4;
    public const float SCORE_COIN_MULTIPLIER = 10f;
    public const int SCORE_WEIGHT_GARBAGE = 1000;

    //transition side to back
    public const float TRANSITION_SIDE_TO_BACK_START_COUNTDOWN_DISTANCE = 30f;
    public const float TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE = 11f;

    //transition back to side
    public const float TRANSITION_BACK_TO_SIDE_START_QTE_DISTANCE = 7.5f;
    public const float TRANSITION_BACK_TO_SIDE_QTE_SPEED_MULTIPLIER = 0.25f;
    public const float TRANSITION_BACK_TO_SIDE_QTE_SPEED_INCREMENT = 0.25f;
    public const float TRANSITION_BACK_TO_SIDE_QTE_PERFECT = 0.35f;
    public const float TRANSITION_BACK_TO_SIDE_QTE_GOOD = 0.6f;
    public const float TRANSITION_BACK_TO_SIDE_QTE_BAD = 1f;
    public const float TRANSITION_BACK_TO_SIDE_QTE_MISS = 0.1f;
    public const int TRANSITION_BACK_TO_SIDE_GARBAGE_REWARD_COUNT = 20;

    //Tutorial   
    public const string TUTORIAL_TAG_NAME = "CoinGroup";
    public const int TUTORIAL_TOTAL_PLATFORM_SPAWN = 6;
    public const float TUTORIAL_DISABLE_INPUT_DURATION = 0.5f;

    public static Dictionary<TutorialState, string> TUTORIAL_POINT_NAME = new Dictionary<TutorialState, string>
    {
        { TutorialState.Coin, "TutorialPoint_Coin" },
        { TutorialState.Jump, "TutorialPoint_Jump" },
        { TutorialState.Trampoline, "TutorialPoint_Trampoline" },
        { TutorialState.DoubleJump, "TutorialPoint_DoubleJump" },
        { TutorialState.Slide, "TutorialPoint_Slide" },
        { TutorialState.TransitionBackView, "TutorialPoint_TransitionBackView" },
        { TutorialState.PressTransitionForce,"TutorialPoint_GoUp" },
        { TutorialState.MoveRight, "TutorialPoint_MoveRight" },
        { TutorialState.MoveLeft, "TutorialPoint_MoveLeft" },
        { TutorialState.JumpBackSand,"TutorialPoint_Jump_BackSand"},
        { TutorialState.DoubleJumpBackSand,"TutorialPoint_DoubleJump_BackSand"},
        { TutorialState.SlideBackSand,"TutorialPoint_Slide_BackSand"},
        { TutorialState.Rhythm, "TutorialPoint_Rhythm" },
        { TutorialState.MiniWheel, "TutorialPoint_MiniWheel" },
        { TutorialState.ManHole, "TutorialPoint_Manhole" },
        { TutorialState.Collectible, "TutorialPoint_CollectibleGarbage" },
        { TutorialState.Ink, "TutorialPoint_Ink" },
        { TutorialState.Bomb, "TutorialPoint_Bomb" },
        { TutorialState.SlideSideCity, "TutorialPoint_Slide_SideCity"},
        { TutorialState.TrashBinLeft, "TutorialPoint_TrashBinLeft" },
        { TutorialState.TrashBinMiddle, "TutorialPoint_TrashBinMiddle" },
        { TutorialState.TrashBinRight, "TutorialPoint_TrashBinRight" },
        { TutorialState.JumpBackCity,"TutorialPoint_Jump_BackCity" },
        { TutorialState.Truck, "TutorialPoint_Truck" },
        { TutorialState.SlideBackCity,"TutorialPoint_Slide_BackCity" },
        { TutorialState.Finish, "TutorialPoint_Finish" },
        { TutorialState.Portal, "TutorialPoint_Portal" },
        { TutorialState.NONE,"" }
    };

    //speed multiplier
    public const float SPEED_MULTIPLIER_NEXT_LEVEL_DURATION = 15f;
    public const float SPEED_MULTIPLIER_NEXT_LEVEL_DURATION_SHRINK_MULTIPLIER = 0.06f;
    public const int SPEED_MULTIPLIER_MAX_LEVEL = 5;
    public const int SPEED_MULTIPLIER_START_LEVEL = 1;

    //HUD Garbage
    public const float HEX_HUD_GARBAGE_ALPHA_ACTIVE = 1f;
    public const float HEX_HUD_GARBAGE_ALPHA_INACTIVE = 1f;

    //Coin
    public const int COIN_VALUE = 10;
    public const int COIN_BIG_VALUE = 100;
    public const int COIN_REWARD_PERFECT_VALUE = 50;
    public const int COIN_REWARD_GOOD_VALUE = 10;

    //Background Parallax Effect
    public const float PARALLAX_MAIN_GAME_MULTIPLIER_X = 10f;
    public const float PARALLAX_SURF_GAME_MULTIPLIER_X = 1.1f;
    public const float PARALLAX_SURF_GAME_SHIP = 5f;
    public const float PARALLAX_MAIN_GAME_MAX_DISTANCE = 500f;

    //Shop
    public const float SHOP_MODEL_VIEWER_ROTATE_SPEED = 0.2f;

    //World Map
    public const float WORLD_MAP_ZOOM_MULTIPLIER = 4f;
    public const float WORLD_MAP_ZOOM_SPEED = 0.4f;

    //Spin Wheel Minigame
    public const float SPIN_WHEEL_MAX_SPEED = 5f;
    public const float SPIN_WHEEL_ACCELERATION = 20f;

    //Camera
    public static CinemachineBlendDefinition GetBlendCamera_Cut()
    {
        CinemachineBlendDefinition blend = new CinemachineBlendDefinition();
        blend.m_Style = CinemachineBlendDefinition.Style.Cut;

        return blend;
    }

    //Flag
    public static string[] FLAG_NAMES = new string[]
    {
        "Algeria", "Arab", "Argentina", "Australia", "Brazil",
        "Canada", "China", "France", "Germany", "India",
        "Indonesia", "Israel", "Italy", "Jamaica", "Japan",
        "Korea", "Laos", "Libya", "Malaysia", "Mongolia", "Nepal",
        "Nigeria", "Pakistan", "Peru", "Philipphines", "Poland", "Portugal", "Russia", "Singapore",
        "SouthAfrica", "Spain", "Taiwan", "Thailand", "Turkey", "USA", "Vietnam"
    };

   

    public static CinemachineBlendDefinition GetBlendCamera_EaseInOut()
    {
        CinemachineBlendDefinition blend = new CinemachineBlendDefinition();
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        blend.m_Time = 1.5f;

        return blend;
    }

    //Title Text
    public static string TITLE_CHANGE_FLAG = "Choose Your Nation";
    public static string TITLE_CHANGE_NAME = "Enter Your Name";
}
