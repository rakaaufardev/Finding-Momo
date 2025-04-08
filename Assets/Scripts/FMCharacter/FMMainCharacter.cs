using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using VD;
using NUnit.Framework.Interfaces;


public enum HitObstacleType
{
    NONE,
    Front,
    Back,
    Above,
    Left,
    Right,
    Below
}

public enum Costume
{
    Casual,
    Christmas,
    Seollal,
    Ski,
    COUNT
}

public class FMMainCharacter : VDCharacter
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform rootVisual;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform hitBox;
    [SerializeField] private Transform magnetHitBox;
    [SerializeField] private Transform invulnerableVisual;
    [SerializeField] private Rigidbody characterRigidbody;

    private bool isTransition;
    private bool isInvulnerable;
    private bool isSpeedUP;
    private bool isUsingMagnet;
    private bool isMultiplerScore;
    private Coroutine invulnerableCoroutine;
    private Sequence invulnerableSequence;
    private bool fromMiniGame;
    private Animator animCostume;
    private Transform transformCostume;

    private ViewMode characterViewMode;
    private Costume currentCostume;
    private float hitObstacleForce;
    private float jumpForce;
    private float trampolineForce;
    private float animationSpeed;
    [SerializeField] private float speed;
    private float defaultSpeed;
    private float invulnerableTimer;
    private float speedUpTimer;
    private float scoreMultiplyTimer;
    private float magnetTimer;
    private float qtaSpeed;
    private bool isStartQTA;

    private bool speedMultiplierIsCountdown;
    private float speedMultiplierTimer;
    private float speedMultiplierDefaultDuration;
    private float speedMultiplierDuration;
    private int speedMultiplierLevel;
    private List<string> speedMultipliePowerUps;

    private int trackId;
    private int lastTrackId;
    [SerializeField] private int lineId;
    private int coinCollected;
    private Vector3 currentEndTrackPoint;

    private bool qtaIsMissed;
    private bool qtaIsComplete;
    private float startQTEDistance;
    private float qtaTimePercentage;
    private float qtaPerfectResult;
    private float qtaGoodResult;
    private float qtaBadResult;
    private float qtaMissResult;

    private int currentHealth;
    private int maxHealth;
    private float lastPosX;
    private float lastPosZ;

    private int adContinueChances;

    //Tutorial
    private float tutorialStopDistanceSideView;
    private float tutorialStopDistanceBackView;
    [SerializeField] private TutorialState tutorialCurrentState;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private Transform tutorialPoint;
    [SerializeField] private bool isTutorialSideView;
    [SerializeField] private int tutorialLineID;
    private bool isTutorial;
    private bool isInkParticlePlayedTutorial;
    private bool isInputDisabled;
    public bool isWheelOver;
    private bool isProceedPressed;
    private bool isResetSpeed;
    private Vector3 lastVelocity;
    private Coroutine shortTutorialUI;
    private FMPlatformController platformController;
    private FMScoreController scoreController;
    private FMMainCameraController cameraController;
    private FMUnityMainCamera camera;
    private Tweener changeLaneTween;
    private FMParticle inkParticleTutorial;
    private FMLeaderboard leaderboard;

    public float AnimationSpeed
    {
        get
        {
            return animationSpeed;
        }
        set
        {
            animationSpeed = value;
        }
    }

    public bool IsStartQTA
    {
        get
        {
            return isStartQTA;
        }
        set
        {
            isStartQTA = value;
        }
    }

    public float QtaSpeed
    {
        get
        {
            return qtaSpeed;
        }
        set
        {
            qtaSpeed = value;
        }
    }

    public Vector3 CurrentEndTrackPoint
    {
        get
        {
            return currentEndTrackPoint;
        }
        set
        {
            currentEndTrackPoint = value;
        }
    }

    public bool IsSpeedUp
    {
        get
        {
            return isSpeedUP;
        }
        set
        {
            isSpeedUP = value;
        }
    }

    public bool IsTutorial
    {
        get
        {
            return isTutorial;
        }
        set
        {
            isTutorial = value;
        }
    }

    public int SpeedMultiplierLevel
    {
        get
        {
            return speedMultiplierLevel;
        }
        set
        {
            speedMultiplierLevel = value;
        }

    }

    public float InvulnerableTimer
    {
        get
        {
            return invulnerableTimer;
        }
        set
        {
            invulnerableTimer = value;
        }
    }

    public float SpeedUpTimer
    {
        get
        {
            return speedUpTimer;
        }
        set
        {
            speedUpTimer = value;
        }
    }

    public bool IsInvulnerable
    {
        get
        {
            return isInvulnerable;
        }
        set
        {
            isInvulnerable = value;
        }
    }

    public Costume CurrentCostume
    {
        get
        {
            return currentCostume;
        }
        set
        {
            currentCostume = value;
        }
    }

    public bool IsTransition
    {
        get
        {
            return isTransition;
        }
        set
        {
            isTransition = value;
        }
    }

    public Tweener ChangeLaneTween
    {
        get
        {
            return changeLaneTween;
        }
        set
        {
            changeLaneTween = value;
        }
    }

    public int TrackId
    {
        get
        {
            return trackId;
        }
        set
        {
            trackId = value;
        }
    }

    public int LineId
    {
        get
        {
            return lineId;
        }
        set
        {
            lineId = value;
        }
    }

    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    public bool QtaIsMissed
    {
        get
        {
            return qtaIsMissed;
        }
        set
        {
            qtaIsMissed = value;
        }
    }

    public bool QtaIsComplete
    {
        get
        {
            return qtaIsComplete;
        }
        set
        {
            qtaIsComplete = value;
        }
    }

    public Transform TransformCostume
    {
        get
        {
            return transformCostume;
        }
    }

    public Rigidbody CharacterRigidBody
    {
        get
        {
            return characterRigidbody;
        }
    }

    public Transform CameraTarget
    {
        get
        {
            return cameraTarget;
        }
    }

    public ViewMode CharacterViewMode
    {
        get
        {
            return characterViewMode;
        }
        set
        {
            characterViewMode = value;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
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

    public TutorialState CurrentTutorialState
    {
        get => tutorialCurrentState;
        set => tutorialCurrentState = value;
    }

    public int AdContinueChances
    {
        get => adContinueChances;
        set => adContinueChances = value;
    }

    public void Init(FMLeaderboard inLeaderboard, bool isFromMiniGame)
    {
        leaderboard = inLeaderboard;
        fromMiniGame = isFromMiniGame;
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        InitController();

        speedMultiplierIsCountdown = true;
        invulnerableTimer = VDParameter.INVULNERABLE_DURATION_IN_SEC;
        CacheMainGameData cacheMainGameData = mainScene.CacheMainGameData;
        speedUpTimer = VDParameter.SPEEDUP_DURATION_IN_SEC;

        jumpForce = VDParameter.CHARACTER_JUMP_FORCE;
        trampolineForce = VDParameter.CHARACTER_TRAMPOLINE_FORCE;
        defaultSpeed = VDParameter.CHARACTER_SPEED;

        speedMultiplierLevel = fromMiniGame ? cacheMainGameData.CacheSpeedMultiplierLevel : VDParameter.SPEED_MULTIPLIER_START_LEVEL;
        speedMultiplierDefaultDuration = VDParameter.SPEED_MULTIPLIER_NEXT_LEVEL_DURATION;
        speedMultiplierDuration = fromMiniGame ? GetSpeedMultiplierDuration() : speedMultiplierDefaultDuration;
        speedMultiplierTimer = speedMultiplierDuration;

        List<FMPlatform> platforms = platformController.GetPlatforms();
        if (fromMiniGame)
        {
            FMPlatform platform = platforms[1];
            trackId = cacheMainGameData.CacheTrackId;
            lineId = cacheMainGameData.CacheLineId;
            characterRigidbody.position = platform.GetPortalPosition();

            qtaSpeed = cacheMainGameData.CacheQtaSpeed;
            SetNormalSpeed();
        }
        else
        {
            speed = defaultSpeed;
            int platformCount = platforms.Count;
            for (int i = 0; i < platformCount; i++)
            {
                FMPlatform platform = platforms[i];
                Transform spawnPoint = platform.GetCharacterSpawnPoint();
                bool spawnPointExist = spawnPoint.gameObject.activeSelf;
                if (spawnPointExist)
                {
                    characterRigidbody.position = spawnPoint.transform.position + VDParameter.CHARACTER_SPAWN_OFFSET_POSITION;
                    break;
                }
            }
            FMPlatformController.Get().SetCurrentLevel("Easy");
        }

        currentCostume = inventoryData.inUsedCostume;
        FMCostume costume = FMAssetFactory.GetCostume(currentCostume, WorldType.Main);
        transformCostume = costume.transform;
        transformCostume.SetParent(rootVisual);
        transformCostume.localPosition = Vector3.zero;
        animCostume = costume.Anim;

        UpdateAnimationSpeedValue();
        SetAnimationSpeed(animationSpeed);

        /*SetRotation(characterViewMode);*/

        FMMainCharacterActionMachine characterActionMachine = new FMMainCharacterActionMachine();
#if UNITY_STANDALONE
        inputController = new FMKeyboardController();
#elif UNITY_ANDROID
        inputController = new FMTouchScreenController();
#endif

        speedMultipliePowerUps = new List<string>();

        characterActionMachine.Init();
        characterActionMachine.SetCharacter(this);

        camera = FMSceneController.Get().Camera;
        actionMachine = characterActionMachine;
        characterViewMode = fromMiniGame ? ViewMode.BackView : ViewMode.SideView;

        actionMachine.SetActionMachineStatus(ActionMachineStatus.Active);
        characterActionMachine.SetState(MainCharacterActionType.Idle, characterViewMode);

        if (!isTutorial)
        {
            hitObstacleForce = VDParameter.CHARACTER_HIT_OBSTACLE_FORCE;
        }
        else
        {
            hitObstacleForce = 0;
        }

        startQTEDistance = VDParameter.TRANSITION_BACK_TO_SIDE_START_QTE_DISTANCE;
        qtaPerfectResult = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_PERFECT;
        qtaGoodResult = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_GOOD;
        qtaBadResult = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_BAD;
        qtaMissResult = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_MISS;

        //speed multiplier power up default effect
        string powerUpName = PowerUpType.SpeedUp.ToString();
        speedMultipliePowerUps.Add(powerUpName);

        //todo : add effect speed multiplier from costume here
        //---

        uiMain.UpdateSpeedMultiplierLevel(speedMultiplierLevel);
    }

    public override void DoUpdate()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        FMGarbageController garbageController = mainScene.GetGarbageController();

        if (mainScene.GameStatus == GameStatus.Transition)
        {
            return;
        }

#if UNITY_ANDROID
        //touch screen controller
        ((FMTouchScreenController)inputController).DoUpdateInGame();
#endif
        actionMachine.DoUpdate();

        FMMainCharacterActionMachine characterActionMachine = actionMachine as FMMainCharacterActionMachine;
        MainCharacterActionType currentState = characterActionMachine.GetCurrentState();

        //speed multiplier
        bool speedMultiplierActive = currentState == MainCharacterActionType.Run ||
            currentState == MainCharacterActionType.Jump ||
            currentState == MainCharacterActionType.DoubleJump ||
            currentState == MainCharacterActionType.Slide ||
            currentState == MainCharacterActionType.Trampoline;

        if (speedMultiplierActive && !isTutorial)
        {
            if (speedMultiplierLevel < VDParameter.SPEED_MULTIPLIER_MAX_LEVEL)
            {
                if (speedMultiplierTimer > 0)
                {
                    speedMultiplierTimer -= speedMultiplierIsCountdown ? Time.deltaTime : 0;
                }
                else
                {
                    speedMultiplierLevel++;
                    speedMultiplierDuration = GetSpeedMultiplierDuration();
                    speedMultiplierTimer = speedMultiplierDuration;

                    int powerUpCount = speedMultipliePowerUps.Count;
                    for (int i = 0; i < powerUpCount; i++)
                    {
                        string powerUpName = speedMultipliePowerUps[i];
                        string value = FMPowerUpCollection.powerUpList[powerUpName].value;
                        Action<string> powerUpEffect = FMPowerUpCollection.powerUpList[powerUpName].callback;
                        powerUpEffect?.Invoke(value);
                    }

                    uiMain.UpdateSpeedMultiplierLevel(speedMultiplierLevel);
                    if (speedMultiplierLevel >= VDParameter.SPEED_MULTIPLIER_MAX_LEVEL)
                    {
                        //Tracking
                        //FMTrackingController.Get().MaxSpeedDuration += Time.deltaTime;

                        uiMain.ShowMaxSpeedMultiplier();
                    }
                    else
                    {
                        uiMain.ShowLevelUpSpeedMultiplier();
                    }

                    UpdateAnimationSpeedValue();
                    SetAnimationSpeed(animationSpeed);
                }
            }
            else
            {
                speedMultiplierTimer = speedMultiplierDuration;
            }

            //Tracking
            //if (speedMultiplierLevel >= VDParameter.SPEED_MULTIPLIER_MAX_LEVEL)
            //{
            //    FMTrackingController.Get().MaxSpeedDuration += Time.deltaTime;
            //}
        }

        //transition
        Vector3 characterPoint = GetCharacterNormalizePosition();
        float endTrackPointDistance = Vector3.Distance(currentEndTrackPoint, characterPoint);

        if (characterViewMode == ViewMode.SideView)
        {
            bool isStartCountdown = IsCountdownTransition(endTrackPointDistance);
            bool isChangeTrackArea = endTrackPointDistance < VDParameter.TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE;

            if (isStartCountdown)
            {
                if (isSpeedUP && !isTransition)
                {
                    isTransition = true;
                    characterActionMachine.SetStateException(MainCharacterActionType.Run);
                    characterActionMachine.SetStateException(MainCharacterActionType.Slide);
                    characterActionMachine.SetStateException(MainCharacterActionType.Jump);
                    characterActionMachine.SetStateException(MainCharacterActionType.DoubleJump);
                    characterActionMachine.SetState(MainCharacterActionType.TransitionSideToBack);

                }
                else
                {
                    MainCharacterActionType characterState = characterActionMachine.GetCurrentState();
                    bool stateValid = characterState == MainCharacterActionType.Run;
                    if (stateValid && !isTransition)
                    {
                        isTransition = true;
                        characterActionMachine.SetStateException(MainCharacterActionType.Run);
                        characterActionMachine.SetStateException(MainCharacterActionType.Slide);
                        characterActionMachine.SetStateException(MainCharacterActionType.Jump);
                        characterActionMachine.SetStateException(MainCharacterActionType.DoubleJump);
                        characterActionMachine.SetState(MainCharacterActionType.TransitionSideToBack);
                    }
                }

                string messageGetReady = VDCopy.GetCopy(CopyTag.SIDE_VIEW_TRANSITION_GET_READY);
                if (endTrackPointDistance <= VDParameter.TRANSITION_SIDE_TO_BACK_START_COUNTDOWN_DISTANCE / 2)
                {
                    uiMain.ShowStaticMessage(true, string.Format(messageGetReady, "1"));
                }
                else if (endTrackPointDistance <= (VDParameter.TRANSITION_SIDE_TO_BACK_START_COUNTDOWN_DISTANCE / 4) * 3)
                {
                    uiMain.ShowStaticMessage(true, string.Format(messageGetReady, "2"));
                }
                else
                {
                    uiMain.ShowStaticMessage(true, string.Format(messageGetReady, "3"));
                }
            }
            else if (isChangeTrackArea)
            {
                string messagePress = VDCopy.GetCopy(CopyTag.SIDE_VIEW_TRANSITION_PRESS);
                if (characterActionMachine.IsStateExceptionExist())
                {
                    characterActionMachine.ClearAllStateException();
                }

                uiMain.ShowStaticMessage(true, messagePress);
                uiMain.ShowHandSwipe(HandSwipeDirection.Up, new Vector3(720, -250));
            }
        }
        else if (characterViewMode == ViewMode.BackView)
        {
            currentEndTrackPoint = new Vector3(characterPoint.x, currentEndTrackPoint.y, currentEndTrackPoint.z);

            if (!qtaIsMissed && !qtaIsComplete)
            {
                isStartQTA = endTrackPointDistance <= startQTEDistance;
                if (isStartQTA)
                {
                        MainCharacterActionType characterState = characterActionMachine.GetCurrentState();
                        bool stateValid = characterState == MainCharacterActionType.Run
                            || characterState == MainCharacterActionType.Jump
                            || characterState == MainCharacterActionType.DoubleJump
                            || characterState == MainCharacterActionType.Trampoline
                            || characterState == MainCharacterActionType.Slide;
                    if (stateValid && isSpeedUP && !isTransition)
                    {
                        characterActionMachine.SetStateException(MainCharacterActionType.Run);
                        characterActionMachine.SetStateException(MainCharacterActionType.Slide);
                        characterActionMachine.SetStateException(MainCharacterActionType.Jump);
                        characterActionMachine.SetStateException(MainCharacterActionType.DoubleJump);
                        ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.TransitionBackToSide);
                        uiMain.SkipQtaResult();
                        isTransition = true;
                    }
                    else
                    {

                        if (stateValid && !isTransition && !isSpeedUP)
                        {
                            uiMain.ShowQTA(true, isTutorial);
                            isTransition = true;

                            if (qtaSpeed <= 1)
                            {
                                qtaSpeed += VDParameter.TRANSITION_BACK_TO_SIDE_QTE_SPEED_INCREMENT;
                            }

                            characterActionMachine.SetStateException(MainCharacterActionType.Run);
                            characterActionMachine.SetStateException(MainCharacterActionType.Slide);
                            characterActionMachine.SetStateException(MainCharacterActionType.Jump);
                            characterActionMachine.SetStateException(MainCharacterActionType.DoubleJump);
                            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.TransitionBackToSide);

                            uiMain.SetQTAExplanation(isTutorial);
                            speed = defaultSpeed * VDParameter.TRANSITION_BACK_TO_SIDE_QTE_SPEED_MULTIPLIER * (1 + qtaSpeed);
                        }

                        qtaTimePercentage = endTrackPointDistance / startQTEDistance;
                        if (qtaTimePercentage <= qtaMissResult)
                        {
                            isStartQTA = false;
                            qtaIsMissed = true;
                            SetNormalSpeed();
                            SetAnimationSpeed(animationSpeed);
                            uiMain.SetResultsText(QTAResult.Miss);
                        }

                        uiMain.UpdateTimeQTEIndicator(qtaTimePercentage);

                        bool isInPerfect = qtaTimePercentage > qtaMissResult && qtaTimePercentage <= qtaPerfectResult && isTutorial;
                        if (isInPerfect)
                        {
                            speed = 0;
                        }
                    }
                }
            }
        }

        uiMain.UpdateSpeedMultiplierTimer(speedMultiplierTimer, speedMultiplierDuration);

        //score
        if (characterViewMode == ViewMode.SideView)
        {
            float currentPosX = Mathf.Abs(transform.position.x);
            if (currentPosX > lastPosX)
            {
                float deltaX = currentPosX - lastPosX;

                if (deltaX > VDParameter.ONE_UNIT_VALUE)
                {
                    if (!isMultiplerScore)
                    {
                        scoreController.MainNormalScore += 1;
                    }
                    else
                    {
                        scoreController.MainNormalScore += 8;
                    }
                        lastPosX = currentPosX; 
                }
            }
        }
        else if (characterViewMode == ViewMode.BackView)
        {
            float currentPosZ = Mathf.Abs(transform.position.z);
            if (currentPosZ > lastPosZ)
            {
                float deltaZ = currentPosZ - lastPosZ;

                if (deltaZ > VDParameter.ONE_UNIT_VALUE)
                {
                    if (isMultiplerScore)
                    {
                        scoreController.MainNormalScore += 8;
                    }
                    else
                    {
                        scoreController.MainNormalScore += 1;
                    }
                    lastPosZ = currentPosZ;
                }
            }
        }

        //Invulnerable
        if (isInvulnerable && currentState != MainCharacterActionType.Idle)
        {
            if (mainScene.GameStatus != GameStatus.Pause)
            {
                if (invulnerableTimer > 0)
                {
                    invulnerableTimer -= Time.deltaTime;
                    if (invulnerableSequence == null)
                    {
                        if (invulnerableTimer <= VDParameter.INVULNERABLE_START_BLINK_TIME)
                        {
                            invulnerableSequence = DOTween.Sequence();
                            Renderer invulnerableRenderer = invulnerableVisual.gameObject.GetComponent<Renderer>();
                            Material invulnerableMaterial = invulnerableRenderer.material;
                            invulnerableSequence.Append(DOTween.To(() => invulnerableMaterial.GetFloat("_Alpha"),
                                        x => invulnerableMaterial.SetFloat("_Alpha", x),
                                        0f, VDParameter.INVULNERABLE_BLINK_INTERVAL))
                                .Append(DOTween.To(() => invulnerableMaterial.GetFloat("_Alpha"),
                                        x => invulnerableMaterial.SetFloat("_Alpha", x),
                                        0.3f, VDParameter.INVULNERABLE_BLINK_INTERVAL))
                                .SetLoops(Mathf.FloorToInt(VDParameter.INVULNERABLE_START_BLINK_TIME / VDParameter.INVULNERABLE_BLINK_INTERVAL), LoopType.Yoyo)
                                .OnComplete(() => invulnerableMaterial.SetFloat("_Alpha", 0.3f));
                        }
                    }
                }
                else
                {
                    isInvulnerable = false;
                    ShowInvulnerable(false);
                    invulnerableSequence.Kill();
                    invulnerableSequence = null;
                    mainScene.EnableHitboxCollider(true);
                }
            }
        }

        //SpeedUp
        if (isSpeedUP)
        {
            if (mainScene.GameStatus != GameStatus.Pause)
            {
                if (speedUpTimer > 0)
                {
                    speedUpTimer -= Time.deltaTime;
                    speed = 50f;
                    mainScene.EnableHitboxCollider(false);
                }
                else
                {
                    isSpeedUP = false;
                    SetNormalSpeed();
                    mainScene.EnableHitboxCollider(true);
                }
            }
        }
        cameraController.SetDampingSpeedCamera(isSpeedUP);


        //Score Multipler

        if (isMultiplerScore)
        {
            if (mainScene.GameStatus != GameStatus.Pause)
            {
                if (scoreMultiplyTimer > 0)
                {
                    scoreMultiplyTimer -= Time.deltaTime;
                }
                else
                {
                   
                    isMultiplerScore = false;
                }
            }
        }

        scoreController.CacheMainDistanceScore = Mathf.RoundToInt(scoreController.MainNormalScore * VDParameter.SCORE_DISTANCE_MULTIPLIER);
        uiMain.UpdateTextScore(scoreController.CacheMainDistanceScore, isMultiplerScore);

        if (isUsingMagnet)
        {
            if (mainScene.GameStatus != GameStatus.Pause)
            {
                if (magnetTimer > 0)
                {
                    magnetTimer -= Time.deltaTime;
                }
                else
                {

                    isUsingMagnet = false;
                }
            }
        }
        magnetHitBox.gameObject.SetActive(isUsingMagnet);



        //Tutorial       
        if (isTutorial)
        {
            if (tutorialUI == null)
            {
                tutorialUI = GameObject.FindGameObjectWithTag("UITutorial_ContentTutorial");
            }

            if (tutorialCurrentState == TutorialState.Intro && inputController.IsProceedPressed())
            {
                UITutorial.OnNext();
                tutorialCurrentState = TutorialState.Jump;
                tutorialStopDistanceSideView = 5f;
                FMMainCharacterActionState_Idle stateStart = ((FMMainCharacterActionMachine)actionMachine).GetCurrentAction() as FMMainCharacterActionState_Idle;
                stateStart?.StartRunning();
            }

            if (tutorialPoint != null)
            {
                float stopDistance = 0;
                float stopDistanceTarget = 0;
                Vector3 playerPositionX = new Vector3(transform.position.x, 0, 0);
                Vector3 playerPositionZ = new Vector3(0, 0, transform.position.z);
                Vector3 tutorialPositionX = new Vector3(tutorialPoint.transform.position.x, 0, 0);
                Vector3 tutorialPositionZ = new Vector3(0, 0, tutorialPoint.transform.position.z);

                float tolerance = 0.0001f;
                if (isTutorialSideView)
                {
                    bool isSamePosition = (tutorialPositionX.x - playerPositionX.x) < tolerance;
                    stopDistance = isSamePosition ? 0 : (playerPositionX - tutorialPositionX).magnitude;
                }
                else
                {
                    bool isSamePosition = /*Mathf.Abs*/(tutorialPositionZ.z - playerPositionZ.z) < tolerance;
                    stopDistance = isSamePosition ? 0 : (playerPositionZ - tutorialPositionZ).magnitude;
                }

                stopDistanceTarget = isTutorialSideView ? tutorialStopDistanceSideView : tutorialStopDistanceBackView;
                if (stopDistance <= stopDistanceTarget)
                {
                    CheckTutorialStep(stopDistance, stopDistanceTarget);
                }
            }
            else
            {
                if (isResetSpeed)
                {
                    isResetSpeed = false;
                    characterRigidbody.velocity = lastVelocity;
                    characterRigidbody.isKinematic = false;
                    SetNormalSpeed();
                    UpdateAnimationSpeedValue();
                    SetAnimationSpeed(animationSpeed);
                }

                if (VDParameter.TUTORIAL_POINT_NAME.ContainsKey(tutorialCurrentState))
                {
                    VDLog.Log("tutorialCurrentState : " + tutorialCurrentState);
                    GameObject tutorialPointObject = GameObject.Find(VDParameter.TUTORIAL_POINT_NAME[tutorialCurrentState]);
                    if (tutorialPointObject != null)
                    {
                        tutorialPoint = tutorialPointObject.transform;
                    }
                    else
                    {
                        tutorialPoint = null;
                    }
                }
                else
                {
                    tutorialPoint = null;
                }
            }
            
            if (tutorialCurrentState == TutorialState.NONE)
            {
                OnEndTutorial();
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

    public void OnRespawn()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        actionMachine.SetActionMachineStatus(ActionMachineStatus.Active);

        AddHealth(maxHealth,false);

        //mainScene.EnableHitboxCollider(false);
        IsInvulnerable = false;
        EnableInvulnerable();

        Vector3 characterPoint = GetCharacterNormalizePosition();
        float endTrackPointDistance = Vector3.Distance(currentEndTrackPoint, characterPoint);
        bool inSideViewTransition = characterViewMode == ViewMode.SideView && IsCountdownTransition(endTrackPointDistance);
        bool isStartQTE = endTrackPointDistance <= startQTEDistance;

        if (inSideViewTransition)
        {
            TriggerAnimation("Run");
            actionMachine.SetState(MainCharacterActionType.TransitionSideToBack, characterViewMode);
        }
        else if (isStartQTE)
        {
            //TriggerAnimation("Run");
            //isTransition = false;
            //SetNormalSpeed();
            //SetAnimationSpeed(1);
            //SetNextEndTrack();
            //SetCurrentEndTrack();
            //SetSideViewLane();
            //mainWorld.ChangeViewMode();
            //mainWorld.SpawnPlatformTriggered = false;
        }
        else
        {
            actionMachine.SetState(MainCharacterActionType.Idle, characterViewMode);
        }

        transformCostume.localPosition = Vector3.zero;
        transformCostume.localEulerAngles = new Vector3(0, -90, 0);

        uiMain.UpdateHealthIcon();
        uiMain.EnablePauseButton(true);

        FMUIWindowController.Get.CloseAllWindow();
    }

    public void SetCharacter()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        InitController();

        CacheMainGameData cacheMainGameData = mainScene.CacheMainGameData;
        CacheSurfGameData cacheSurfGameData = mainScene.CacheSurfGameData;
        trackId = fromMiniGame ? cacheMainGameData.CacheTrackId : -1;
        lineId = fromMiniGame ? cacheMainGameData.CacheLineId : -1;
        lastTrackId = fromMiniGame ? trackId - 1 : -1;
        scoreController.MainNormalScore = fromMiniGame ? cacheMainGameData.CacheNormalScore : 0;
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        maxHealth = VDParameter.CHARACTER_MAX_HEALTH;
        currentHealth = fromMiniGame ? cacheMainGameData.CacheHealth : maxHealth;
        adContinueChances = fromMiniGame ? cacheMainGameData.CacheAdContinueChances : VDParameter.CONTINUE_AMOUNT;

        AddHealth(cacheSurfGameData.CacheAddHealth, true);
        cacheSurfGameData.CacheAddHealth = 0;

        if (!fromMiniGame)
        {
           SetNextEndTrackId();
        }

        if (!UITutorial.IsDone("Finish"))
        {
            isTutorial = true;
            isInkParticlePlayedTutorial = false;
            isResetSpeed = false;

            uiMain.HidePowerUpUI();

            if (!fromMiniGame)
            {
                tutorialStopDistanceSideView = 1f;
                tutorialStopDistanceBackView = 1f;
                isTutorialSideView = true;
                tutorialCurrentState = TutorialState.Intro;
                FMUserDataService.Get().ClearTutorialData();
                UITutorial.Create("Intro");
            }
            else
            {
                isTutorialSideView = false;
                tutorialStopDistanceBackView = 5f;
                tutorialCurrentState = TutorialState.MiniWheel;
            }
        }
        else
        {
            isTutorial = false;
        }

        SetCurrentEndTrack();
        uiMain.AddHealthIcon(currentHealth);
        uiMain.UpdateHealthIcon();
        uiMain.UpdateTextScore(scoreController.CacheMainDistanceScore,isMultiplerScore);
    }

    public void ShowInvulnerable(bool isShow)
    {
        invulnerableVisual.gameObject.SetActive(isShow);
    }

    public void HitForceFeedback(HitObstacleType hitObstacleType)
    {
        Vector3 direction = Vector3.zero;
        float shrinkForceMultiplier = 1;
        switch (CharacterViewMode)
        {
            case ViewMode.SideView:
                direction = hitObstacleType == HitObstacleType.Front ? Vector3.left : Vector3.right;
                break;
            case ViewMode.BackView:
                switch (hitObstacleType)
                {
                    case HitObstacleType.Front:
                        direction = Vector3.back;
                        break;
                    case HitObstacleType.Back:
                        direction = Vector3.forward;
                        break;
                    case HitObstacleType.Left:
                        direction = Vector3.left + Vector3.back;
                        lineId--;
                        shrinkForceMultiplier = 0.4f;
                        break;
                    case HitObstacleType.Right:
                        direction = Vector3.right + Vector3.back;
                        lineId++;
                        shrinkForceMultiplier = 0.4f;
                        break;
                }
                break;
        }

        Vector3 hitForce = direction * hitObstacleForce * shrinkForceMultiplier;
        characterRigidbody.velocity = hitForce;
    }

    public void ResetToSafeSpot(SafeSpotConfig safeSpotConfig)
    {
        if (safeSpotConfig.safeSpot != null)
        {
            lineId = safeSpotConfig.lineId;
            /*Vector3 position = new Vector3(safeSpotConfig.safeSpot.transform.position.x, 0.1f, safeSpotConfig.safeSpot.transform.position.z);*/
            Vector3 position = safeSpotConfig.safeSpot.transform.position;
            characterRigidbody.DOMove(position, 0.5f);
        }
    }

    private float ModifiedPlayerStats(InventoryData inventoryData, ItemType itemType, float defaultValue)
    {
        ItemData itemData = inventoryData.GetItemData(itemType);
        return defaultValue + itemData.amount;
    }

    public void ShowVisual(bool isShow)
    {
        rootVisual.gameObject.SetActive(isShow);
    }

    public void AddHealth(int amount, bool isHeal)
    {
        int healAmount = isHeal ? amount - (maxHealth - currentHealth) : 0;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            if (isHeal)
            {
                maxHealth += healAmount;
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = maxHealth;
            }
        }
    }

    public void SpendHealth(int amount)
    {
        if (!isTutorial)
        {
            currentHealth -= amount;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
        }
    }

    public bool IsCharacterNearDeath()
    {
        bool result = false;
        result = currentHealth <= 1;
        return result;
    }

    public void SetNextEndTrackId()
    {
        lastTrackId = trackId;
        trackId++;
    }

    public void SetCurrentEndTrack()
    {
        currentEndTrackPoint = platformController.GetEndTrackPoint(trackId);
    }

    public void SetNormalSpeed()
    {
        string powerUpName = PowerUpType.SpeedUp.ToString();
        string value = FMPowerUpCollection.powerUpList[powerUpName].value;
        int valueInt = Convert.ToInt32(value);

        speed = defaultSpeed + (valueInt * (speedMultiplierLevel - VDParameter.SPEED_MULTIPLIER_START_LEVEL));
    }

    public void SetBackViewLane()
    {
        float nearestDistance = 9999;
        Vector3 line = Vector3.zero;

        Vector3 characterPoint = GetCharacterNormalizePosition();
        Vector3[] arrows = platformController.GetArrowPositions(lastTrackId);
        int count = arrows.Length;
        for (int i = 0; i < count; i++)
        {
            Vector3 arrow = arrows[i];
            float distance = Vector3.Distance(arrow, characterPoint);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                line = arrow;
                lineId = i;
            }
        }

        characterRigidbody.DOMove(line, VDParameter.CHARACTER_CHANGE_TRANSITION_DURATION_IN_SEC);
    }

    public void SetSideViewLane()
    {
        Vector3 endTrackPoint = platformController.GetEndTrackPoint(trackId);
        Vector3 posTarget = new Vector3(characterRigidbody.position.x, 0, endTrackPoint.z);
        characterRigidbody.DOMove(posTarget, VDParameter.CHARACTER_CHANGE_TRANSITION_DURATION_IN_SEC);
    }

    public void ChangeBackViewLane(bool isTurnRight)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        uiMain.HideHandTap();
        uiMain.HideHandSwipe();

        lineId += isTurnRight ? 1 : -1;
        NormalizeLineId();
        BackToCenterLane();
    }

    public void NormalizeLineId()
    {
        if (lineId < 0)
        {
            lineId = 0;
        }
        else if (lineId > 2)
        {
            lineId = 2;
        }
    }

    public void BackToCenterLane()
    {
        bool lastTrackExist = lastTrackId > -1;
        if (lastTrackExist)
        {
            Vector3[] arrows = platformController.GetArrowPositions(lastTrackId);
            Vector3 linePos = arrows[lineId];

            changeLaneTween = characterRigidbody.DOMoveX(linePos.x, VDParameter.CHARACTER_CHANGE_LINE_SPEED).OnUpdate(() =>
            {
                characterRigidbody.velocity = new Vector3(characterRigidbody.velocity.x, characterRigidbody.velocity.y, characterRigidbody.velocity.z);
            });

            changeLaneTween.OnKill(() =>
            {
                changeLaneTween = null;
            });
        }
    }

    public void UpdateAnimationSpeedValue()
    {
        animationSpeed = 1 + 1 * ((VDParameter.MAX_ANIMATION_SPEED_INCREMENT / VDParameter.SPEED_MULTIPLIER_MAX_LEVEL) * (speedMultiplierLevel-1));
    }

    public void SetAnimationSpeed(float speed)
    {
        animCostume.speed = speed;
    }

    public void TriggerAnimation(string trigger)
    {
        animCostume.SetTrigger(trigger);
    }

    public void ResetAnimation(string trigger)
    {
        animCostume.ResetTrigger(trigger);
    }

    public bool IsAnimationEnd(string animStateName)
    {
        AnimatorStateInfo stateInfo = animCostume.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName(animStateName) && stateInfo.normalizedTime >= 1;
        return result;
    }

    public bool IsAnimationState(string animStateName)
    {
        AnimatorStateInfo stateInfo = animCostume.GetCurrentAnimatorStateInfo(0);
        bool result = stateInfo.IsName(animStateName);
        return result;
    }

    public float GetAnimationLength()
    {
        float result = 0;

        AnimatorStateInfo stateInfo = animCostume.GetCurrentAnimatorStateInfo(0);
        result = stateInfo.length;

        return result;
    }

    public void RotateCharacterVisual(Vector3 angle, bool isInstant)
    {
        if (isInstant)
        {
            rootVisual.localEulerAngles = angle;
        }
        else
        {
            rootVisual.DOLocalRotate(angle, 0.5f);
        }
    }

    public void SetRotation(ViewMode inViewMode)
    {
        switch (inViewMode)
        {
            case ViewMode.SideView:
                characterRigidbody.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case ViewMode.BackView:
                characterRigidbody.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    public Vector3 GetRotation(ViewMode inViewMode)
    {
        Vector3 result = Vector3.zero;

        switch (inViewMode)
        {
            case ViewMode.SideView:
                result = new Vector3(0, 90, 0);
                break;
            case ViewMode.BackView:
                result = Vector3.zero;
                break;
        }

        return result;
    }

    public Vector3 GetMoveDirection(ViewMode inViewMode)
    {
        Vector3 result = Vector3.zero;

        switch (inViewMode)
        {
            case ViewMode.SideView:
                //result = Vector3.right;
                result = new Vector3(speed, characterRigidbody.velocity.y, 0);
                break;
            case ViewMode.BackView:
                //result = Vector3.forward;
                result = new Vector3(0, characterRigidbody.velocity.y, speed);
                break;
        }

        return result;
    }

    public void ChangeLaneAnimation(bool isRight)
    {
        //float moveSpeed = isLeft ? -1 : 1;
        //characterRigidbody.velocity = new Vector3(moveSpeed * speed, characterRigidbody.velocity.y, characterRigidbody.velocity.z);

        string trigger = isRight ? "SwitchRight" : "SwitchLeft";
        float speedAnimation = 2 - VDParameter.CHARACTER_CHANGE_LINE_SPEED;
        //SetAnimationSpeed(speedAnimation);
        TriggerAnimation(trigger);
    }

    public void Move()
    {
        SetRotation(CharacterViewMode);
        Vector3 moveDirection = GetMoveDirection(CharacterViewMode);
        characterRigidbody.velocity = moveDirection;
        //characterRigidbody.transform.Translate(moveDirection * speed * Time.deltaTime,Space.World);
    }

    public void SupportForce()
    {
        //characterRigidbody.velocity += Vector3.down * VDParameter.CHARACTER_FALL_MULTIPLIER;
        if (characterRigidbody.velocity.y > 0)
        {
            characterRigidbody.AddForce(Vector3.down * VDParameter.CHARACTER_JUMP_GRAVITY, ForceMode.Acceleration);
        }
        else if (characterRigidbody.velocity.y < 0)
        {
            characterRigidbody.AddForce(Vector3.down * VDParameter.CHARACTER_FORCE_FALL_GRAVITY, ForceMode.Acceleration);
        }
    }

    public void QuickLand()
    {
        characterRigidbody.AddForce(Vector3.down * VDParameter.CHARACTER_QUICK_LAND_GRAVITY, ForceMode.Acceleration);
        float tempSpeed = 2f;
        SetAnimationSpeed(tempSpeed);
    }

    public void Jump()
    {
            
        //Vector3 direction = Vector3.up;
        //characterRigidbody.velocity = direction * jumpForce;
        ((FMMainCharacterActionMachine)actionMachine).SetStateException(MainCharacterActionType.Slide);
        ((FMMainCharacterActionMachine)actionMachine).JumpBufferReset();
        characterRigidbody.velocity = new Vector3(characterRigidbody.velocity.x, jumpForce, characterRigidbody.velocity.z);
        FMSoundController.Get().PlaySFX(SFX.SFX_Jump);
    }

    public void Slide(bool isSlide)
    {
        float targetScaleY = isSlide ? VDParameter.CHARACTER_SLIDE_HITBOX_SCALE_Y : VDParameter.CHARACTER_DEFAULT_HITBOX_SCALE_Y;
        float duration = 0.15f;
        hitBox.transform.DOScaleY(targetScaleY, duration);

        /*VDLog.Log("isSlide: " + isSlide);*/
    }

    public void Trampoline(string obstacleName)
    {
        VDLog.Log("Trampoline obstacleName : " + obstacleName);
        characterRigidbody.velocity = new Vector3(characterRigidbody.velocity.x, trampolineForce, characterRigidbody.velocity.z);
        FMSoundController.Get().PlaySFX(SFX.SFX_Jump);
    }

    private void ResetSpeedMultiplier()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        //tracking
        //if (speedMultiplierLevel == VDParameter.SPEED_MULTIPLIER_MAX_LEVEL)
        //{
        //    FMTrackingController.Get().TrackMaxSpeedObtained();
        //    FMTrackingController.Get().MaxSpeedDuration = 0;
        //}

        speed = defaultSpeed;
        speedMultiplierDuration = speedMultiplierDefaultDuration;
        speedMultiplierTimer = speedMultiplierDuration;
        speedMultiplierLevel = VDParameter.SPEED_MULTIPLIER_START_LEVEL;
        UpdateAnimationSpeedValue();
        SetAnimationSpeed(animationSpeed);

        uiMain.UpdateSpeedMultiplierTimer(speedMultiplierTimer, speedMultiplierDuration);
        uiMain.UpdateSpeedMultiplierLevel(speedMultiplierLevel);
        uiMain.TriggerSpeedMultiplierAnim("idle");
    }

    public void OnHitPortal(int platformId)
    {
        if (isTutorial) isInputDisabled = false;

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        mainScene.EnableHitboxCollider(false);

        speed = 0;
        SetAnimationSpeed(0);

        //MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        //mainWorld.GenerateNewPlatformData();

        VDLog.Log("ChangeViewMapCountdown " + FMPlatformController.Get().ChangeViewMapCountdown);
        mainScene.SaveCacheMainGameData(platformId);
        mainScene.SwitchToMiniGame(coinCollected);
        
        FMSoundController.Get().PlaySFX(SFX.SFX_TransitionSurf);
    }

    public void OnHitInk(InkObstacle inkObstacle)
    {
        if (IsInvulnerable) return;

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        
        currentWorld.ColliderObjectToReset.Add(inkObstacle);
        inkObstacle.gameObject.SetActive(false);

        Transform originalObject = inkObstacle.transform;
        Transform cameraCenterTransform = camera.CenterCamera;

        FMSceneController.Get().GetDisplayObject(inkObstacle, "Arnold", cameraCenterTransform, originalObject, characterViewMode);
    }

    public void OnHitCoin(Coin coin)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMGarbageController garbageController = mainScene.GetGarbageController();

        mainWorld.ColliderObjectToReset.Add(coin);
        
        coinCollected += VDParameter.COIN_VALUE;
        int lastCoinCollected = coinCollected;

        string coinName = coin.CoinName;
        garbageController.CollectGarbage(coinName,1);
        garbageController.CheckGarbageCollectionStatus();
        UpdateHUDGarbage(coinName);

        scoreController.UpdateCoinScore(coinCollected);

        FMSceneController.Get().PlayParticle("WorldParticle_Coin", coin.transform.position);

        RectTransform flyParticleDestination = uiMain.CoinIcon;
        FMUnityMainCamera mainCamera = FMSceneController.Get().Camera;
        Canvas canvas = FMSceneController.Get().Canvas;
        RectTransform canvasRect = FMSceneController.Get().RootCanvas;
        Vector2 canvasPosition = VDUtility.WorldToCanvasPosition(mainCamera.Camera, canvas, canvasRect, root.position);
        uiMain.PlayFlyParticle("FlyParticle_Coin", canvasPosition, flyParticleDestination, 2f, 1, false, Ease.Linear,() =>
        {
            uiMain.UpdateCoinText(lastCoinCollected);
            uiMain.PlayCoinIconBouncing();
        });

        FMSoundController.Get().PlaySFX(SFX.SFX_Coin);
    }

    public void OnHitGarbage(Garbage garbage)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        FMGarbageController garbageController = mainScene.GetGarbageController();

        coinCollected += VDParameter.COIN_BIG_VALUE;
        int lastCoinCollected = coinCollected;

        scoreController.UpdateCoinScore(coinCollected);
        
        HitGarbage(garbage, garbageController, platformController, scoreController);

        RectTransform flyParticleDestination = uiMain.CoinIcon;
        FMUnityMainCamera mainCamera = FMSceneController.Get().Camera;
        Canvas canvas = FMSceneController.Get().Canvas;
        RectTransform canvasRect = FMSceneController.Get().RootCanvas;
        Vector2 canvasPosition = VDUtility.WorldToCanvasPosition(mainCamera.Camera, canvas, canvasRect, root.position);
        uiMain.PlayFlyParticle("FlyParticle_Coin", canvasPosition, flyParticleDestination, 3, 1, false, Ease.Linear,() =>
        {
            uiMain.UpdateCoinText(lastCoinCollected);
            uiMain.PlayCoinIconBouncing();
        });
    }

    public void OnHitInstantDeathObstacle(SafeSpotConfig safeSpotConfig)
    {
        if (IsInvulnerable) return;

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UpdateLeaderboardData();

        ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.InstantDeathFall, safeSpotConfig);
        if(isTutorial)
        {
            actionMachine.SetStateException(MainCharacterActionType.InstantDeathFall);
        }
        else
        {
            mainScene.GameStatus = GameStatus.Finish;
        }

        UIMain uiMain = mainScene.GetUI();
        uiMain.UpdateHealthIcon();

        if (shortTutorialUI == null && isTutorial)
        {
            shortTutorialUI = StartCoroutine(ShowShortTutorialUI());
        }
    }

    public void OnHitBomb(Bomb bomb, SafeSpotConfig safeSpotConfig, string obstacleName)
    {
        if (IsInvulnerable) return;

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();

        currentWorld.ColliderObjectToReset.Add(bomb);
        OnHit(HitObstacleType.Front, safeSpotConfig, obstacleName);
        platformController.StoreStashRandomCollectible(bomb);
        FMSceneController.Get().PlayParticle("WorldParticle_Bomb", bomb.transform.position);

        ResetSpeedMultiplier();
    }

    public void OnHitSeagull(Seagull hitSeagull, SeagullParent seagullParent, SafeSpotConfig safeSpotConfig, string obstacleName)
    {
        if (IsInvulnerable) return;

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        mainWorld.ColliderObjectToReset.Add(hitSeagull);

        OnHit(HitObstacleType.Front, safeSpotConfig, obstacleName);
        FMSceneController.Get().PlayParticle("WorldParticle_Seagull", hitSeagull.transform.position);

        if (seagullParent != null)
        {
            Seagull[] seagullGroup = seagullParent.SeagullGroup;

            for (int i = 0; i < seagullGroup.Length; i++)
            {
                int index = i;

                Seagull seagull = seagullGroup[index];
                seagull.SeagullCollider.enabled = false;

                bool isOtherSeagull = seagull != hitSeagull;
                if (isOtherSeagull)
                {
                    seagull.SeagullVisual.transform.DOLocalMoveZ(20f, 2f).OnComplete(() =>
                    {
                        seagull.SeagullVisual.gameObject.SetActive(false);
                        //Destroy(seagullGroup[index].gameObject);
                    });
                }
                else
                {
                    StartCoroutine(SeagullFall(hitSeagull));
                }
            }
        }

        ResetSpeedMultiplier();
    }

    public void OnHitSpinGarbage(SpinWheelGarbage spinGarbage, string garbageName)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        UIMain uiMain = mainScene.GetUI();

        if (isSpeedUP)
        {
            coinCollected += 250;
            scoreController.UpdateCoinScore(coinCollected);

            RectTransform flyParticleDestination = uiMain.CoinIcon;
            uiMain.PlayFlyParticle("FlyParticle_Coin", Vector2.zero, flyParticleDestination, 3f, 50, true, Ease.InCubic, () =>
            {
                uiMain.PlayCoinIconBouncing();
                uiMain.UpdateCoinText(coinCollected);
            });

            return;
        }

        DisableOrEnableJump(true);
        uiMain.EnablePauseButton(false);
        ((MainWorld)currentWorld).PauseGame();

        currentWorld.ColliderObjectToReset.Add(spinGarbage);

        Transform originalObject = spinGarbage.transform;
        Transform cameraCenterTransform = camera.CenterCamera;

        FMSceneController.Get().GetDisplayObject(spinGarbage, garbageName, cameraCenterTransform, originalObject, characterViewMode);
    }

    IEnumerator SeagullFall(Seagull seagull)
    {
        bool shouldContinue = false;

        seagull.SeagullAnimator.SetTrigger("Dead");
        seagull.SeagullCollider.enabled = false;

        while (!shouldContinue)
        {
            shouldContinue = false;

            AnimatorStateInfo stateInfo = seagull.SeagullAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Fall"))
            {
                shouldContinue = true;
                seagull.SeagullVisual.DOLocalMoveY(0f, 1f).OnComplete(() =>
                {
                    seagull.SeagullVisual.gameObject.SetActive(false);
                    //Destroy(seagull.gameObject);
                }); ;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnHitWallTransitionObstacle(Collision collision, SafeSpotConfig safeSpotConfig, string obstacleName)
    {
        bool isHitFront = false;
        bool isHitDown = false;
        //HitObstacleType hitObstacleType = HitObstacleType.Front;
        HitObstacleType hitObstacleType = HitObstacleType.NONE;

        int count = collision.contacts.Length;
        for (int i = 0; i < count; i++)
        {
            ContactPoint contact = collision.contacts[i];
            //Vector3 contactPoint = contact.point;
            Vector3 normal = contact.normal;
            Vector3 rhsSide = characterViewMode == ViewMode.SideView ? Vector3.left : Vector3.back;
            Vector3 rhsBelow = Vector3.down;
            float contactFacingSide = Vector3.Dot(normal, rhsSide);
            float contactFacingBelow = Vector3.Dot(normal, rhsBelow);
            isHitFront = contactFacingSide < 0f;
            isHitDown = contactFacingBelow > 0f;
            hitObstacleType = isHitFront || isHitDown ? HitObstacleType.Front : HitObstacleType.Back;
        }

        ResetSpeedMultiplier();
        OnHit(hitObstacleType, safeSpotConfig, obstacleName);
    }

    public void OnHitObstacle(Collision collision, SafeSpotConfig safeSpotConfig, string obstacleName)
    {
        if (IsInvulnerable) return;
        bool isHitFront = false;
        bool isHitDown = false;
        //HitObstacleType hitObstacleType = HitObstacleType.Front;
        HitObstacleType hitObstacleType = HitObstacleType.NONE;

        int count = collision.contacts.Length;
        for (int i = 0; i < count; i++)
        {
            ContactPoint contact = collision.contacts[i];
            //Vector3 contactPoint = contact.point;
            Vector3 normal = contact.normal;
            Vector3 rhsSide = characterViewMode == ViewMode.SideView ? Vector3.left : Vector3.back;
            Vector3 rhsBelow = Vector3.down;
            float contactFacingSide = Vector3.Dot(normal, rhsSide);
            float contactFacingBelow = Vector3.Dot(normal, rhsBelow);
            isHitFront = contactFacingSide < 0f;
            isHitDown = contactFacingBelow > 0f;
            hitObstacleType = isHitFront || isHitDown ? HitObstacleType.Front : HitObstacleType.Back;
        }

        ResetSpeedMultiplier();
        OnHit(hitObstacleType,safeSpotConfig,obstacleName);
    }

    public void OnHitTopPassObstacle(Collision collision, SafeSpotConfig safeSpotConfig, string obstacleName)
    {
        if (isInvulnerable) return;
        bool isHitFront = false;
        bool isHitUp = false;
        bool isHitLeft = false;
        bool isHitRight = false;
        HitObstacleType hitObstacleType = HitObstacleType.NONE;

        int count = collision.contacts.Length;
        for (int i = 0; i < count; i++)
        {
            ContactPoint contact = collision.contacts[i];
            //Vector3 contactPoint = contact.point;
            Vector3 normal = contact.normal.normalized;
            Vector3 rhsSide = characterViewMode == ViewMode.SideView ? Vector3.left : Vector3.back;
            Vector3 rhsAbove = Vector3.up;
            Vector3 rhsLeft = Vector3.left;
            Vector3 rhsRight = Vector3.right;

            float epsilon = 0.0001f;
            if (Mathf.Abs(normal.x) < epsilon)
            {
                normal.x = 0;
            }

            if (Mathf.Abs(normal.y) < epsilon)
            {
                normal.y = 0;
            }

            if (Mathf.Abs(normal.z) < epsilon)
            {
                normal.z = 0;
            }

            isHitFront = characterViewMode == ViewMode.SideView ? normal.x > 0 : normal.z > 0;
            isHitUp = normal.y < 0;
            isHitLeft = normal.x > 0;
            isHitRight = normal.x < 0;

            if (isHitLeft)
            {
                hitObstacleType = HitObstacleType.Left;
            }
            else if (isHitRight)
            {
                hitObstacleType = HitObstacleType.Right;
            }
            else if (isHitFront)
            {
                hitObstacleType = HitObstacleType.Front;
            }
            else if (isHitUp)
            {
                hitObstacleType = HitObstacleType.Above;
            }
            else
            {
                hitObstacleType = HitObstacleType.Back;
            }
        }

        switch (hitObstacleType)
        {
            case HitObstacleType.Front:
            case HitObstacleType.Back:
            case HitObstacleType.Left:
            case HitObstacleType.Right:
                ResetSpeedMultiplier();
                OnHit(hitObstacleType, safeSpotConfig, obstacleName);
                break;
            case HitObstacleType.Above:
                ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
                break;
        }
    }

    public void OnHitTrampolineObstacle(bool isHitTrampoline, Transform trampolineHitbox, SafeSpotConfig safeSpotConfig , string obstacleName)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;

        if (isHitTrampoline)
        {
            mainWorld.TrampolineHitboxCollide.Add(trampolineHitbox);

            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Trampoline);
            Trampoline(obstacleName);
            if (characterViewMode != ViewMode.BackView)
            {
                cameraController.ActivateCamera(MainCameraMode.TrampolineView);
            }
            FMSoundController.Get().PlaySFX(SFX.SFX_Trampoline);
        }
        else
        {
            ResetSpeedMultiplier();
            OnHit(HitObstacleType.Front,safeSpotConfig,obstacleName);
        }
    }

    public void OnEndTutorial()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        FMGarbageController garbageController = mainScene.GetGarbageController();

        FMMainCharacterActionMachine characterActionMachine = actionMachine as FMMainCharacterActionMachine;
        MainCharacterActionType currentState = characterActionMachine.GetCurrentState();

        coinCollected = 0;
        
        scoreController.UpdateCoinScore(coinCollected);
        scoreController.MainNormalScore = 0;

        garbageController.ClearGarbagePool();
        garbageController.ClearAllGarbage();

        garbageController.RestoreGarbagePool();
        garbageController.ResetGarbagePoolCounter();
        
        uiMain.ClearAllGarbageHUD();
        uiMain.UpdateCoinText(coinCollected);
        
        hitObstacleForce = VDParameter.CHARACTER_HIT_OBSTACLE_FORCE;
        
        isTutorial = false;

        if (currentState != MainCharacterActionType.Idle)
        {
            SetAnimationSpeed(1);
            actionMachine.SetState(MainCharacterActionType.Idle, characterViewMode);
            actionMachine.SetStateException(MainCharacterActionType.Run);
            actionMachine.SetStateException(MainCharacterActionType.Jump);
            actionMachine.SetStateException(MainCharacterActionType.Slide);
        }

        StartCoroutine(FinishTutorial());
    }

    public bool IsCountdownTransition(float distance)
    {
        bool result  =
            distance >= VDParameter.TRANSITION_SIDE_TO_BACK_END_COUNTDOWN_DISTANCE && distance <= VDParameter.TRANSITION_SIDE_TO_BACK_START_COUNTDOWN_DISTANCE;

        return result;
    }

    public void EnableInvulnerable()
    {
        if (!isInvulnerable)
        {
            isInvulnerable = true;
            ShowInvulnerable(true);
            invulnerableTimer = VDParameter.INVULNERABLE_DURATION_IN_SEC;
        }
    }

    public void EnableSpeedUp()
    {
        if (!isSpeedUP)
        {
            isSpeedUP = true;
            speedUpTimer = VDParameter.SPEEDUP_DURATION_IN_SEC;
        }
    }

    public void EnableMultiplyScore()
    {
        if (!isMultiplerScore)
        {
            isMultiplerScore = true;
            scoreMultiplyTimer = VDParameter.MULTIPLY_SCORE_DURATION_IN_SEC;
        }
    }

    public void EnableMagnet()
    {
        if (!isUsingMagnet)
        {
            isUsingMagnet = true;
            magnetTimer = VDParameter.MAGNET_DURATION_IN_SEC;
        }
    }

    public void CheckQTAResult()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        FMGarbageController garbageController = mainScene.GetGarbageController();

        RectTransform flyParticleDestination = uiMain.CoinIcon;
        Vector2 flyParticleStartPosition = Vector3.zero;

        isStartQTA = false;
        qtaIsComplete = true;
        bool isPerfect = qtaTimePercentage > qtaMissResult && qtaTimePercentage <= qtaPerfectResult;
        bool isGood = qtaTimePercentage > qtaPerfectResult && qtaTimePercentage <= qtaGoodResult;
        bool isBad = qtaTimePercentage > qtaGoodResult;

        if (isPerfect)
        {
            string garbageReward = garbageController.GetQTAGarbageReward();
            VDLog.Log("QTA Perfect garbage reward : " + garbageReward);

            if (!string.IsNullOrEmpty(garbageReward))
            {
                platformController.HideDuplicateGarbage(garbageReward);
                garbageController.CollectGarbage(garbageReward,VDParameter.TRANSITION_BACK_TO_SIDE_GARBAGE_REWARD_COUNT);
                //garbageController.RemoveGarbagePool(garbageReward);
                garbageController.ResetGarbagePoolCounter();

                //scoreController.UpdateGarbageScore();

                bool isCollected = garbageController.GetGarbageStatus(garbageReward);
                UpdateHUDGarbage(garbageReward);

                garbageController.CheckGarbageCollectionStatus();
                
                FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);
            }
            else
            {
                coinCollected += VDParameter.COIN_REWARD_PERFECT_VALUE;
                //scoreController.UpdateCoinScore(coinCollected);


                uiMain.PlayFlyParticle("FlyParticle_Coin", flyParticleStartPosition, flyParticleDestination, 2, VDParameter.COIN_REWARD_PERFECT_VALUE, true, Ease.InCubic, () =>
                {
                    uiMain.UpdateCoinText(coinCollected);
                    uiMain.PlayCoinIconBouncing();
                });
                FMSoundController.Get().PlaySFX(SFX.SFX_Coin);
            }

            uiMain.SetResultsText(QTAResult.Perfect);
        }
        else if (isGood)
        {
            coinCollected += VDParameter.COIN_REWARD_GOOD_VALUE;
            scoreController.UpdateCoinScore(coinCollected);

            uiMain.PlayFlyParticle("FlyParticle_Coin", flyParticleStartPosition, flyParticleDestination, 2, VDParameter.COIN_REWARD_GOOD_VALUE, true, Ease.InCubic,()=>
            {
                uiMain.UpdateCoinText(coinCollected);
                uiMain.PlayCoinIconBouncing();
            });
            FMSoundController.Get().PlaySFX(SFX.SFX_Coin);

            uiMain.SetResultsText(QTAResult.Good);
        }
        else if (isBad)
        {
            if (!isSpeedUP)
            {
                /*FMSceneController.Get().PlayParticle("UIParticle_Ink", Vector3.zero, false);*/
                StartCoroutine(uiMain.InkParticleSequence(VDParameter.INK_MAIN_DURATION));
                uiMain.SetResultsText(QTAResult.Bad);
            }
        }

        ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
        qtaIsComplete = true;
    }

    public void OnGamePause(bool isPause)
    {
        if (isPause)
        {
            speed = 0;
            speedMultiplierIsCountdown = false;
            SetAnimationSpeed(0);
            characterRigidbody.isKinematic = true;
            invulnerableSequence.Pause();
            ((FMMainCharacterActionMachine)actionMachine).SetStateException(MainCharacterActionType.Jump);
            ((FMMainCharacterActionMachine)actionMachine).SetStateException(MainCharacterActionType.DoubleJump);

        }
        else
        {
            SetNormalSpeed();
            speedMultiplierIsCountdown = true;
            SetAnimationSpeed(animationSpeed);
            characterRigidbody.isKinematic = false;
            invulnerableSequence.Play();
            ((FMMainCharacterActionMachine)actionMachine).ClearAllStateException();
        }
    }

    public Vector3 GetCharacterNormalizePosition()
    {
        Vector3 result = Vector3.zero;
        Vector3 characterPosition = characterRigidbody.position;
        result = new Vector3(characterPosition.x, 0, characterPosition.z);
        return result;
    }

    public void UpdateLeaderboardData()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        GameStatus gameStatus = mainScene.GameStatus;
        if (gameStatus == GameStatus.Finish)
        {
            float playerScore = mainScene.GetScoreController().GetTotalScore();
            UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
            FMSceneController.Get().Leaderboard.AddLeaderboardData(playerScore, userInfo);
            //leaderboard.AddLeaderboardData(playerScore, userInfo);
        }
    }

    private void OnHit(HitObstacleType hitObstacleType, SafeSpotConfig safeSpotConfig, string obstacleName)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        VDLog.Log("obstacleName : " + obstacleName);

        bool isGameOver = IsCharacterNearDeath();
        if (isGameOver)
        {
            //Tracking
            //if (speedMultiplierLevel == VDParameter.SPEED_MULTIPLIER_MAX_LEVEL)
            //{
            //    FMTrackingController.Get().TrackMaxSpeedObtained();
            //    FMTrackingController.Get().MaxSpeedDuration = 0;
            //}

            uiMain.EnablePauseButton(false);
            FMScoreController scoreController = mainScene.GetScoreController();
            scoreController.UpdateGarbageScore();
            //FMTrackingController.Get().TrackCoinCollected(coinCollected);
            //FMTrackingController.Get().UpdateTrackingInfo();
            //FMTrackingController.Get().Save();
            //FMTrackingController.Get().Write();

            mainScene.GameStatus = GameStatus.Finish;
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.GameOver, true, hitObstacleType);
        }
        else
        {
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Hit, hitObstacleType, safeSpotConfig);
            ((FMMainCharacterActionMachine)actionMachine).SetStateException(MainCharacterActionType.Hit);
            //actionMachine.BreakUntil(CharacterActionType.Run);
        }

       
        if (shortTutorialUI == null && isTutorial)
        {
            shortTutorialUI = StartCoroutine(ShowShortTutorialUI());
        }
        
    }

    private void InitController()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;

        platformController = FMPlatformController.Get();
        scoreController = mainScene.GetScoreController();
        cameraController = mainWorld.CameraController as FMMainCameraController;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(VDParameter.TAG_GROUND))
        {
            OnAfterJump();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TopPassGround"))
        {
            OnAfterJump();
        }
    }

    private float GetSpeedMultiplierDuration()
    {
        float result = speedMultiplierDefaultDuration - (speedMultiplierDefaultDuration * VDParameter.SPEED_MULTIPLIER_NEXT_LEVEL_DURATION_SHRINK_MULTIPLIER * (speedMultiplierLevel - 1));
        return result;
    }

    private void OnAfterJump()
    {

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;

        FMMainCharacterActionMachine characterActionMachine = actionMachine as FMMainCharacterActionMachine;
        MainCharacterActionType currentState = characterActionMachine.GetCurrentState();

        bool stateValid = currentState == MainCharacterActionType.Jump || currentState == MainCharacterActionType.DoubleJump || currentState == MainCharacterActionType.Trampoline;
        VDLog.Log("OnAfterjump currentState : " + currentState);

        if (stateValid)
        {
            FMSceneController.Get().PlayParticle("WorldParticle_Land", new Vector3(transform.position.x, 0.5f, transform.position.z), false);
            cameraController.ActivateCamera(mainWorld.CurrentCameraMode);
        }
        
        if (stateValid && !isStartQTA)
        {
            VDLog.Log("OnAfterjump proceed to run");
            ((FMMainCharacterActionMachine)actionMachine).ClearAllStateException();
            ((FMMainCharacterActionMachine)actionMachine).SetState(MainCharacterActionType.Run);
        }
    }

    void ProcessNextTutorialStep(string createTutorialSection, TutorialState nextState, bool inputTrigger, Action additionalCallback)
    {
        UITutorial.Create(createTutorialSection);
        if (inputTrigger)
        {
            UITutorial.OnNext();
            tutorialCurrentState = nextState;
            tutorialPoint = null;
            isResetSpeed = true;
            isInputDisabled = false;

            additionalCallback?.Invoke();
        }
    }

    private bool isDialogueCreated;
    void CheckTutorialStep(float tutorialStopDistance, float stopDistance)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;

        bool isPressed = false;

        SetAnimationSpeed(0);
        speed = 0;
        lastVelocity = characterRigidbody.velocity;
        characterRigidbody.velocity = Vector3.zero;
        characterRigidbody.isKinematic = true;

        MainCharacterActionType currentState = ((FMMainCharacterActionMachine)actionMachine).GetCurrentState();
        FMMainCharacterActionState_Run stateRun = ((FMMainCharacterActionMachine)actionMachine).GetCurrentAction() as FMMainCharacterActionState_Run;
        FMMainCharacterActionState_TransitionSideToBack stateTransition = ((FMMainCharacterActionMachine)actionMachine).GetCurrentAction() as FMMainCharacterActionState_TransitionSideToBack;

        if (!isProceedPressed)
        {
            isPressed = inputController.IsProceedPressed();
        }

        switch (tutorialCurrentState)
        {
            case TutorialState.Jump:
                uiMain.ShowHandTap(new Vector3(600, -120));
                uiMain.ShowHandSwipe(HandSwipeDirection.Up, new Vector3(900, -120));
                
                if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                ProcessNextTutorialStep("Jump", TutorialState.DoubleJump, isPressed, () =>
                {
                    uiMain.HideHandTap();
                    uiMain.HideHandSwipe();
                    tutorialUI.SetActive(false);
                });
                break;
            case TutorialState.DoubleJump:
                if (speed == 0)
                {
                    uiMain.ShowHandTap(new Vector3(600, -120));
                    uiMain.ShowHandSwipe(HandSwipeDirection.Up, new Vector3(900, -120));

                    if (!isInputDisabled)
                    {
                        isInputDisabled = true;
                        StartCoroutine(DisableKey());
                    }

                    ProcessNextTutorialStep("DoubleJump", TutorialState.Slide, isPressed, () =>
                    {
                        uiMain.HideHandTap();
                        uiMain.HideHandSwipe();
                        tutorialUI.SetActive(false);
                    });
                }
                break;
            case TutorialState.Slide:
                if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                if (speed == 0)
                {
                    uiMain.ShowHandSwipe(HandSwipeDirection.Down, new Vector3(600, 0));
                    
                    ProcessNextTutorialStep("Slide", TutorialState.Coin, isPressed, () =>
                    {
                        uiMain.HideHandSwipe();
                        tutorialUI.SetActive(false);
                        SetNormalSpeed();
                        cameraController.FindTransitionCameraTutorial();
                    });
                }
                break;
            case TutorialState.Coin:
                if (!isDialogueCreated)
                {
                    UITutorial.Create("Coin");
                    isDialogueCreated = true;
                }

                if (isPressed)
                {
                    UITutorial.OnNext();

                    int currentStep = UITutorial.GetCurrentTutorialStep();

                    if (currentStep == 1)
                    {
                        if (!isInputDisabled)
                        {
                            isInputDisabled = true;
                            actionMachine.SetStateException(MainCharacterActionType.Jump);
                            actionMachine.SetStateException(MainCharacterActionType.Slide);

                            StartCoroutine(DisableKey());
                        }

                        mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
                        FMGarbageController garbageController = mainScene.GetGarbageController();
                        garbageController.ShowCompleteGarbage();
                    }
                    else if (currentStep == 2)
                    {
                        tutorialCurrentState = TutorialState.TransitionBackView;
                        tutorialPoint = null;
                        isResetSpeed = true;
                        isInputDisabled = false;

                        tutorialUI.SetActive(false);
                    }
                }
                /*ProcessNextTutorialStep("Coin", TutorialState.TransitionBackView, isPressed, () =>
                {
                    tutorialUI.SetActive(false);
                });*/
                break;
            case TutorialState.TransitionBackView:
                if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                if (speed == 0)
                {
                    cameraController.ActivateCamera(MainCameraMode.TransitionCamera);

                    ProcessNextTutorialStep("TransitionBackView", TutorialState.Portal, isPressed, () =>
                    {
                        cameraController.ActivateCamera(mainWorld.CurrentCameraMode);
                        tutorialStopDistanceBackView = 10f;
                    });
                }
                break;
            case TutorialState.Portal:
                if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                if (speed == 0)
                {
                    ProcessNextTutorialStep("Portal", TutorialState.MiniWheel, isPressed, () =>
                    {
                        isTutorialSideView = false;
                        tutorialStopDistanceBackView = 1f;

                        isDialogueCreated = false;
                    });
                }
                break;
            case TutorialState.MiniWheel:
                if (!isDialogueCreated)
                {
                    UITutorial.Create("MiniWheel");
                    isDialogueCreated = true;
                }

                if (isPressed)
                {
                    UITutorial.OnNext();

                    int currentStep = UITutorial.GetCurrentTutorialStep();

                    if (currentStep == 1)
                    {
                        if (!isInputDisabled)
                        {
                            isInputDisabled = true;
                            actionMachine.SetStateException(MainCharacterActionType.Jump);
                            actionMachine.SetStateException(MainCharacterActionType.Slide);

                            StartCoroutine(DisableKey());
                        }
                    }
                    else if (currentStep == 2)
                    {
                        tutorialCurrentState = TutorialState.ManHole;
                        tutorialPoint = null;
                        isResetSpeed = true;
                        isInputDisabled = false;

                        tutorialUI.SetActive(false);
                        tutorialStopDistanceBackView = 1f;
                    }
                }
                /*if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                if (speed == 0)
                {
                    ProcessNextTutorialStep("MiniWheel", TutorialState.ManHole, isPressed, () =>
                    {
                        SetNormalSpeed();
                    });
                }*/
                break;
            case TutorialState.ManHole:
                if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                if (speed == 0)
                {
                    ProcessNextTutorialStep("ManHole", TutorialState.Rhythm, isPressed, () =>
                    {
                        SetNormalSpeed();
                    });
                }
                break;
            case TutorialState.Rhythm:
                if (!isInputDisabled)
                {
                    isInputDisabled = true;
                    StartCoroutine(DisableKey());
                }

                if (speed == 0)
                {
                    ProcessNextTutorialStep("Rhythm", TutorialState.Finish, isPressed, () =>
                    {
                        tutorialStopDistanceSideView = 1f;
                        isTutorialSideView = true;
                    });
                }
                break;
            case TutorialState.Finish:
                if (speed == 0)
                {
                    ProcessNextTutorialStep("Finish", TutorialState.NONE, isPressed, null);
                }
                /*tutorialCurrentState = TutorialState.NONE;*/
                break;
            default:
                break;
        }
    }

    private IEnumerator FinishTutorial()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        yield return new WaitForSeconds(0.5f);

        uiMain.ShowTutorialEndTitle();

        yield return new WaitForSeconds(6f);

        FMUserDataService.Get().SetTutorialFinished();
        FMSceneController.Get().OnBackToLobby();
        /*FMUserDataService.Get().SetTutorialFinish();*/
    }

    IEnumerator DisableKey()
    {
        isProceedPressed = true;

        FMMainCharacterActionMachine characterActionMachine = actionMachine as FMMainCharacterActionMachine;

        yield return new WaitForSeconds(VDParameter.TUTORIAL_DISABLE_INPUT_DURATION);

        if (characterActionMachine.IsStateExceptionExist())
        {
            characterActionMachine.ClearAllStateException();
        }

        isProceedPressed = false;
    }
    
    IEnumerator ShowShortTutorialUI()
    {
        UITutorial.ShowSpaceImage(false);
        tutorialUI.SetActive(true);
        yield return new WaitForSeconds(3);
        UITutorial.ShowSpaceImage(true);
        tutorialUI.SetActive(false);
        shortTutorialUI = null;
    }

    public void DisableOrEnableJump(bool control) //minigame wheel pseudo pause
    {
        if (control) //rough way to disable controls
            isWheelOver = true;
        else
            isWheelOver = false;
    }

    public void AddPowerUpStats(ItemType itemType, ItemData itemData,HUDPowerUp powerUpItem,UIMain uiMain)
    {
        switch (itemType)
        {
            case ItemType.AddHealth:
                if (itemData.amount > 0)
                {
                    itemData.amount -= 1;
                    itemData.nextLevelIndex = 0;
                    itemData.costIncrement = 0;
                    AddHealth(1, true);
                    uiMain.AddHealthIcon(maxHealth);
                    uiMain.UpdateHealthIcon();
                    powerUpItem.SetItemAmount(itemData.amount);
                }
                break;
            case ItemType.AddDurationSpeedUp:

                if (itemData.amount > 0)
                {
                    itemData.amount -= 1;
                    itemData.nextLevelIndex = 0;
                    itemData.costIncrement = 0;
                    EnableSpeedUp();
                    SpeedUpTimer = VDParameter.SPEEDUP_DURATION_IN_SEC;
                    IsInvulnerable = false;
                    EnableInvulnerable();
                    InvulnerableTimer = VDParameter.SPEEDUP_DURATION_IN_SEC + 2;
                    //SpeedUp

                    powerUpItem.SetItemAmount(itemData.amount);
                }
                break;
            case ItemType.AddInvulnerableDuration:
                if (itemData.amount > 0)
                {
                    itemData.amount -= 1;
                    itemData.nextLevelIndex = 0;
                    itemData.costIncrement = 0;
                    IsInvulnerable = false;
                    EnableInvulnerable();
                    InvulnerableTimer = VDParameter.INVULNERABLE_DURATION_ITEM_IN_SEC;
                    powerUpItem.SetItemAmount(itemData.amount);
                }
                break;
            case ItemType.Magnet:
                if (itemData.amount > 0)
                {
                    itemData.amount -= 1;
                    itemData.nextLevelIndex = 0;
                    itemData.costIncrement = 0;
                    EnableMagnet();
                    powerUpItem.SetItemAmount(itemData.amount);
                }
                break;
            case ItemType.RemoveInk:
                if (itemData.amount > 0)
                {
                    itemData.amount -= 1;
                    itemData.nextLevelIndex = 0;
                    itemData.costIncrement = 0;
                    FMSceneController.Get().StopDisplayObjectInkAnimation();
                    powerUpItem.SetItemAmount(itemData.amount);
                }
                break;
            case ItemType.MultiplyScore:
                if (itemData.amount > 0)
                {
                    itemData.amount -= 1;
                    itemData.nextLevelIndex = 0;
                    itemData.costIncrement = 0;
                    EnableMultiplyScore();
                    powerUpItem.SetItemAmount(itemData.amount);
                }
                break;
        }
    }
}
