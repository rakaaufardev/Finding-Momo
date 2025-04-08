using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using DG.Tweening;
using VD;

public class FMSceneController : VDSceneController
{
    private Dictionary<string,List<FMParticle>> particleContainers;
    private Dictionary<string, Action> animEventContainers;
    private Dictionary<string, List<FMDisplayObject>> displayObjectContainers;
    [SerializeField] private FMUnityMainCamera camera;
    [SerializeField] private RectTransform rootCanvas;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private UITopFrameHUD uiTopFrameHUD;
    [SerializeField] private UIQuickAction uiQuickAction;
    private bool initComplete;
    private FMDisplayObject activeInkDisplayObject;

    private FMLeaderboard leaderboard;

    private static FMSceneController singleton;
    
    public bool InitComplete
    {
        get
        {
            return initComplete;
        }
        set
        {
            initComplete = value;
        }
    }

    public FMUnityMainCamera Camera
    {
        get
        {
            return camera;
        }
    }

    public CinemachineBrain CameraCinemachineBrain
    {
        get
        {
            return cameraCinemachineBrain;
        }
    }

    public Canvas Canvas
    {
        get
        {
            return canvas;
        }
    }

    public RectTransform RootCanvas
    {
        get
        {
            return rootCanvas;
        }
    }
    
    public UITopFrameHUD UiTopFrameHUD
    {
        get
        {
            return uiTopFrameHUD;
        }
    }

    public UIQuickAction GetUIQuickAction
    {
        get => uiQuickAction;
    }

    public FMLeaderboard Leaderboard
    {
        get
        {
            return leaderboard;
        }
    }

    const string PATH_SCENE = "FMScenes/FM{0}Scene";
    
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMSceneController Get()
    {
        return singleton;
    }

    public void CheckClaimableMissionReward()
    {
        uiQuickAction.CheckClaimableMissionReward();
    }

    public void OnBackToLobby()
    {
        FMMainScene mainScene = GetCurrentScene() as FMMainScene;
        mainScene.GameStatus = GameStatus.Transition;

        FMWorld world = mainScene.GetCurrentWorldObject();
        world.GetCharacter();
        VDCharacter character = world.GetCharacter();
        VDActionMachine actionMachine = character.GetActionMachine();
        actionMachine.SetActionMachineStatus(ActionMachineStatus.Stop);

        FMGarbageController garbageController = mainScene.GetGarbageController();

        garbageController.ClearGarbagePool();
        garbageController.ClearAllGarbage();

        garbageController.RestoreGarbagePool();
        garbageController.ResetGarbagePoolCounter();

        FMUIWindowController.Get.CloseWindow();
        FMPlatformController.Get().ClearPlatforms();

        uiQuickAction.OnCloseWindow(true);
        uiQuickAction.Show();
        ChangeScene(SceneState.Lobby, uiQuickAction);
    }

    public void SaveParticleToStorage()
    {
        if (particleContainers == null)
        {
            return;
        }

        int count = particleContainers.Count;
        List<FMParticle>[] particles = new List<FMParticle>[count];
        particleContainers.Values.CopyTo(particles, 0);
        for (int i = 0; i < count; i++)
        {
            List<FMParticle> particleContainer = particles[i];
            int particleCount = particleContainer.Count;
            for (int j = 0; j < particleCount; j++)
            {
                FMParticle particle = particleContainer[j];
                FMParticleStorage.Get().Save(particle);
            }
        }
    }

    public FMParticle PlayParticle(string particleName, Vector3 position, bool isWorldParticle = true)
    {
        FMParticle particle = CreateParticle(particleName);

        VDScene currentScene = GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;

        Transform rootParticle = isWorldParticle ? mainScene.RootWorldParticle : mainScene.RootUIParticle;
        particle.transform.SetParent(rootParticle);
        particle.transform.localScale = Vector3.one;
        particle.SetPosition(position);
        particle.PlayParticle();
        return particle;
    }

    public FMParticle PlayParticle(string particleName, Transform rootParent, Vector3 positionOffset, bool isWorldParticle = true)
    {
        FMParticle particle = CreateParticle(particleName);

        particle.transform.SetParent(rootParent);
        particle.transform.localScale = Vector3.one;
        particle.transform.localPosition = Vector3.zero + positionOffset;
        particle.PlayParticle();

        return particle;
    }

    public void ShowCinematicEnding(bool isGoodEnding)
    {
        StartCoroutine(OnCinematicEnding(isGoodEnding));
    }

    private IEnumerator OnCinematicEnding(bool isGoodEnding)
    {
        bool shouldContinue = false;

        FMUIWindowController.Get.OpenWindow(UIWindowType.FadeScreen);
        UIWindowFadeScreen popup = FMUIWindowController.Get.CurrentWindow as UIWindowFadeScreen;

        while (!shouldContinue)
        {
            shouldContinue = popup.IsFadeInEnd();
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;
        
        FMMainScene mainScene = currentSceneObject as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        FMMainCharacter character = currentWorld.GetCharacter() as FMMainCharacter;
        
        cameraCinemachineBrain.m_DefaultBlend = VDParameter.GetBlendCamera_Cut();
        UIMain uiMain = mainScene.GetUI();
        uiMain.ShowContent(false);

        Vector3 endTrackPos = FMPlatformController.Get().GetEndTrackPoint(character.TrackId);

        FMEndingController endingController = FMAssetFactory.GetEndingController(currentWorld.transform);
        Vector3 cinematicPos = character.CharacterViewMode == ViewMode.SideView ? new Vector3(endTrackPos.x - VDParameter.CINEMATIC_OFFSET_POSITION, 0, character.transform.position.z) : new Vector3(character.transform.position.x, 0, endTrackPos.z - VDParameter.CINEMATIC_OFFSET_POSITION);
        endingController.transform.localPosition = new Vector3(0, 1000, 0);
        endingController.transform.localEulerAngles = Vector3.zero;

        //List<FMPlatform> platforms = FMPlatformController.Get().GetPlatforms();
        //int platformCount = platforms.Count;
        //for (int i = 0; i < platformCount; i++)
        //{
        //    FMPlatform platform = platforms[i];
        //    platform.ShowObstacles(false);
        //}

        if (isGoodEnding)
        {
            endingController.ShowGoodEnding();
        }

        popup.PlayFadeOut();

        while (!shouldContinue)
        {
            shouldContinue = endingController.IsGoodEndingEnd();
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        //TGS and BINUS Version
        //popup.PlayFadeIn();
        //FMUIPopupController.Get().OpenPopup(UIPopupType.GameOver);
        //====================

        //GStar version
        FMUIWindowController.Get.CloseSpecificWindow(UIWindowType.FadeScreen);

        FMKeyboardController keyboardController = new FMKeyboardController();
        //popup.PlayFadeIn();
        cinematicCallback = () =>
        {
            if (keyboardController.IsProceedPressed())
            {
                shouldContinue = true;
            }
        };
        uiMain.ShowPressSpaceText(true);

        while (!shouldContinue)
        {
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        uiMain.ShowPressSpaceText(false);
        FMUIWindowController.Get.OpenWindow(UIWindowType.GameOver, uiQuickAction);
        //===================

        cameraCinemachineBrain.m_DefaultBlend = VDParameter.GetBlendCamera_EaseInOut();
    }

    public void EnableSkybox(bool isEnable)
    {
        CameraClearFlags clearFlags = isEnable ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        camera.Camera.clearFlags = clearFlags;
    }

    private FMParticle CreateParticle(string particleName)
    {
        if (particleContainers == null)
        {
            particleContainers = new Dictionary<string, List<FMParticle>>();
        }

        FMParticle particle = null;

        if (particleContainers.Count <= 0 || !particleContainers.ContainsKey(particleName))
        {
            particle = DoCreateParticle(particleName);
        }
        else
        {
            List<FMParticle> particles = particleContainers[particleName];

            FMParticle newParticle = null;
            bool isFound = false;
            int indexCursor = 0;
            while (!isFound)
            {
                newParticle = particles[indexCursor];
                bool isPlaying = newParticle.IsParticlePlaying();
                if (!isPlaying)
                {
                    isFound = true;
                    break;
                }

                if (indexCursor == particles.Count - 1)
                {
                    break;
                }

                indexCursor++;
            }

            if (isFound)
            {
                particle = newParticle;
            }
            else
            {
                particle = FMParticleStorage.Get().Load(particleName);
                bool particleExistInStorage = particle != null;
                if (!particleExistInStorage)
                {
                    particle = DoCreateParticle(particleName);
                }
            }
        }

        return particle;
    }

    private FMParticle DoCreateParticle(string particleName)
    {
        FMParticle particle = FMAssetFactory.GetParticle(particleName);

        if (!particleContainers.ContainsKey(particleName))
        {
            List<FMParticle> particles = new List<FMParticle>();
            particles.Add(particle);

            particleContainers.Add(particleName, particles);
        }
        else
        {
            particleContainers[particleName].Add(particle);
        }

        return particle;
    }

    public void SaveDisplayObjectToStorage()
    {
        if (displayObjectContainers == null)
        {
            return;
        }

        int count = displayObjectContainers.Count;

        List<FMDisplayObject>[] displayObjects = new List<FMDisplayObject>[count];
        displayObjectContainers.Values.CopyTo(displayObjects, 0);

        for (int i = 0; i < count; i++)
        {
            List<FMDisplayObject> displayObjectContainer = displayObjects[i];
            int displayObjectCount = displayObjectContainer.Count;
            for (int j = 0; j < displayObjectCount; j++)
            {
                FMDisplayObject displayObject = displayObjectContainer[j];
                FMDisplayObjectStorage.Get().Save(displayObject);
                displayObject.RootVisual.gameObject.SetActive(false);
            }
        }
    }

    public FMDisplayObject GetDisplayObject(PlatformColliderObject colliderObject, string objectName, Transform cameraCenterTransform, Transform platformObject, ViewMode viewMode = ViewMode.SideView)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        SurfWorld surfWorld = currentWorld as SurfWorld;
        FMDisplayObject displayObj = CreateDisplayObject(objectName, cameraCenterTransform);

        Vector3 rootCameraParent = cameraCenterTransform.parent.transform.position;
        Vector3 platformObjectPos = platformObject.transform.position;

        switch (colliderObject)
        {
            case InkObstacle:
                Vector3 offset = new Vector3(1.38f, -0.93f, 9.46f);
                Vector3 targetInkSpawn = (platformObjectPos - rootCameraParent)/* - offset*/;
                StartCoroutine(DisplayObjectInkAnimation(displayObj, targetInkSpawn, viewMode));
                break;
            case SafeRewardObstacle:
                Vector3 targetSurfSpawn = platformObjectPos - rootCameraParent;
                StartCoroutine(DisplayObjectSurfAnimation(displayObj, objectName, platformObject, targetSurfSpawn, () =>
                {
                    surfWorld.CheckAnimalActive();
                    surfWorld.SpawnAnimal();
                }));
                break;
            case SpinWheelGarbage:
                Vector3 targetGarbageSpawn = platformObjectPos - rootCameraParent;
                StartCoroutine(DisplayObjectGarbageAnimation(displayObj, targetGarbageSpawn, viewMode, objectName));
                break;
        }

        return displayObj;
    }

    private IEnumerator DisplayObjectInkAnimation(FMDisplayObject displayObject, Vector3 targetSpawn, ViewMode viewMode)
    {
        FMMainScene mainScene = GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        bool shouldContinue = false;

        if (displayObject.DisplayCoroutine != null)
        {
            StopCoroutine(displayObject.DisplayCoroutine);
            uiMain.RootInkParticle.gameObject.SetActive(false);
        }

        Animator anim = displayObject.Anim;
        Transform rootVisual = displayObject.RootVisual;
        Vector3 offset = Vector3.zero;

        displayObject.IsActive(true);
        displayObject.transform.localPosition = Vector3.zero;
        displayObject.transform.eulerAngles = Vector3.zero;

        rootVisual.gameObject.SetActive(true);
        rootVisual.transform.localPosition = targetSpawn;

        if (viewMode == ViewMode.BackView)
        {
            offset = new Vector3(0f, 0f, -12f);
        }

        rootVisual.transform.eulerAngles = new Vector3(0, 180, 0);
        rootVisual.transform.localScale = Vector3.one * 0.7f;

        anim.SetTrigger("Fly");
        rootVisual.DOScale(Vector3.one, 0.5f);
        rootVisual.DOLocalMove(new Vector3(0, -1.5f, 15f) + offset, 0.5f).OnComplete(() =>
        {
            shouldContinue = true;
        });

        while (!shouldContinue)
        {
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;

        anim.SetTrigger("Shoot");

        displayObject.DisplayCoroutine = StartCoroutine(uiMain.InkParticleSequence(VDParameter.INK_MAIN_DURATION));

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsAnimationEnd("Shoot");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;

        rootVisual.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            rootVisual.gameObject.SetActive(false);
            displayObject.IsActive(false);
        });
    }

    private IEnumerator DisplayObjectSurfAnimation(FMDisplayObject displayObject, string objectName, Transform originalParent, Vector3 targetSpawn, Action onComplete)
    {
        bool shouldContinue = false;

        originalParent.gameObject.SetActive(false);

        displayObject.IsActive(true);
        displayObject.transform.localPosition = Vector3.zero;
        displayObject.RootVisual.gameObject.SetActive(true);
        displayObject.RootVisual.localPosition = targetSpawn;
        displayObject.RootVisual.localScale = Vector3.one * 2f;
        displayObject.RootVisual.eulerAngles = new Vector3(0, 180, 0);

        displayObject.DisplaySequence = DOTween.Sequence();

        displayObject.SetAnimActive(false);
        displayObject.RootVisual.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() =>
        {
            displayObject.Anim.SetTrigger("Free");
        });

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsStateAnim("Show");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;
        PlayParticle("WorldParticle_Heart", displayObject.RootVisual, Vector3.zero);

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsStateAnim("ShowInactive");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;

        displayObject.Anim.SetTrigger("Return");

        Vector3 rotationTarget = new Vector3(0, 0, 0);
        if (objectName == "MantaRay")
        {
            rotationTarget = new Vector3(0, 180, 0);
        }

        displayObject.DisplaySequence.Append(displayObject.RootVisual.DOLocalRotate(rotationTarget, 0.3f));
        displayObject.DisplaySequence.Join(displayObject.RootVisual.DOScale(Vector3.one * 0.75f, 0.6f));
        displayObject.DisplaySequence.Join(displayObject.RootVisual.DOLocalMove(Vector3.back * 6f, 0.6f).OnComplete(() =>
        {
            PlayParticle("WorldParticle_Splash", displayObject.RootVisual.position);
            FMSoundController.Get().PlaySFX(SFX.SFX_Splash);
            displayObject.IsActive(false);
            displayObject.RootVisual.gameObject.SetActive(false);
            displayObject.SetAnimActive(true);
            originalParent.gameObject.SetActive(true);
            onComplete?.Invoke();
        }));

        while (!shouldContinue)
        {
            shouldContinue = displayObject.IsStateAnim("ReturnInactive");
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;
    }
    public void StopDisplayObjectInkAnimation()
    {
        FMMainScene mainScene = GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        uiMain.RootInkParticle.gameObject.SetActive(false);
        uiMain.StopInkParticleSequence();
    }


    private IEnumerator DisplayObjectGarbageAnimation(FMDisplayObject displayObject, Vector3 targetSpawn, ViewMode viewMode, string garbageName)
    {
        FMMainScene mainScene = GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        UIMiniGameSpinWheel uiSpinWheel = uiMain.UISpinWheel;

        bool shouldContinue = false;

        Transform rootVisual = displayObject.RootVisual;
        Vector3 targetPosition = Vector3.zero;

        displayObject.IsActive(true);
        displayObject.transform.localPosition = Vector3.zero;
        displayObject.transform.eulerAngles = Vector3.zero;
        displayObject.transform.localScale = Vector3.one;

        rootVisual.gameObject.SetActive(true);
        rootVisual.transform.localPosition = targetSpawn;
        rootVisual.transform.eulerAngles = Vector3.zero;
        rootVisual.transform.localScale = Vector3.one * 3f;

        targetPosition = viewMode == ViewMode.BackView ? new Vector3(0f, -0.5f, 0f) : new Vector3(0f, -2f, 5f);

        rootVisual.DOScale(Vector3.one * 5f, 0.5f);
        rootVisual.DOLocalMove(targetPosition, 0.5f).OnComplete(() =>
        {
            shouldContinue = true;

            DOVirtual.DelayedCall(0.25f, () =>
            {
                rootVisual.gameObject.SetActive(false);
                displayObject.IsActive(false);

                int slotType = UnityEngine.Random.Range(3, 7);
                uiSpinWheel.SpawnWheel(garbageName, slotType);
                uiSpinWheel.Show(true);
            });
        });

        while (!shouldContinue)
        {
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;
    }

    public FMDisplayObject CreateDisplayObject(string displayObjectName, Transform rootParent)
    {
        if (displayObjectContainers == null)
        {
            displayObjectContainers = new Dictionary<string, List<FMDisplayObject>>();
        }

        FMDisplayObject displayObject = null;

        if (displayObjectContainers.Count <= 0 || !displayObjectContainers.ContainsKey(displayObjectName))
        {
            displayObject = DoCreateDisplayObject(displayObjectName, rootParent);
        }
        else
        {
            List<FMDisplayObject> displayObjects = displayObjectContainers[displayObjectName];

            FMDisplayObject newDisplayObject = null;
            bool isFound = false;
            int indexCursor = 0;

            while (!isFound)
            {
                newDisplayObject = displayObjects[indexCursor];
                bool isSequenceActive = newDisplayObject.IsSequenceActive;

                if (!isSequenceActive)
                {
                    isFound = true;
                    break;
                }

                if (indexCursor == displayObjects.Count - 1)
                {
                    break;
                }

                indexCursor++;
            }

            if (isFound)
            {
                displayObject = newDisplayObject;
                displayObject.transform.SetParent(rootParent);
            }
            else
            {
                displayObject = FMDisplayObjectStorage.Get().Load(displayObjectName);
                bool displayObjectExistInStorage = displayObject != null;

                if (!displayObjectExistInStorage)
                {
                    displayObject = DoCreateDisplayObject(displayObjectName, rootParent);
                }
            }
        }
        if (displayObjectName == "Arnold")
        {
            activeInkDisplayObject = displayObject;
        }
        return displayObject;
    }

    private FMDisplayObject DoCreateDisplayObject(string displayObjectName, Transform rootParent)
    {
        FMDisplayObject prefab = FMAssetFactory.GetDisplayObject(displayObjectName, rootParent);
        
        if (!displayObjectContainers.ContainsKey(displayObjectName))
        {
            List<FMDisplayObject> displayObjects = new List<FMDisplayObject>();
            displayObjects.Add(prefab);

            displayObjectContainers.Add(displayObjectName, displayObjects);
        }
        else
        {
            displayObjectContainers[displayObjectName].Add(prefab);
        }

        return prefab;
    }

    public void AnimEvent_RunParticle()
    {
        FMMainScene scene = currentSceneObject as FMMainScene;
        FMWorld currentWorld = scene.GetCurrentWorldObject();
        FMMainCharacter character = currentWorld.GetCharacter() as FMMainCharacter;

        PlayParticle("WorldParticle_Run", new Vector3(character.transform.position.x, 0.4f, character.transform.position.z));
    }

    //todo : Felix call this on hit obstacle
    public void AnimEvent_HitParticle()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene scene = currentScene as FMMainScene;
        FMWorld currentWorld = scene.GetCurrentWorldObject();
        FMMainCharacter character = currentWorld.GetCharacter() as FMMainCharacter;
        
        PlayParticle("WorldParticle_Hit", character.transform.position);
    }
    
    private void AddAnimationEventData(string actionName, Action actionFunction)
    {
        if (!animEventContainers.ContainsKey(actionName))
        {
            animEventContainers[actionName] = actionFunction;
        }
    }

    public Action GetAnimationEventData(string actionName)
    {
        if (animEventContainers.TryGetValue(actionName, out Action actionFunction))
        {
            return actionFunction;
        }
        else
        {
            return null;
        }
    }

    protected override void AddSceneData()
    {
        sceneData = new Dictionary<SceneState, VDScene>();
        int count = (int)SceneState.COUNT;
        for (int i = 0; i < count; i++)
        {
            SceneState sceneState = (SceneState)i;
            VDScene sceneObject = Resources.Load<VDScene>(string.Format(PATH_SCENE,sceneState));
            sceneData.Add(sceneState, sceneObject);
        }
    }

    public void ShowContinueConfirmation()
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.ContinueConfirmation, leaderboard, uiQuickAction);
    }

    protected override void InitGame()
    {
        DOTween.SetTweensCapacity(3000,50);

        animEventContainers = new Dictionary<string, Action>();
        AddAnimationEventData("RunParticle", AnimEvent_RunParticle);
        AddAnimationEventData("HitParticle", AnimEvent_HitParticle);

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        leaderboard = new FMLeaderboard();

        VDAdController.Get.Init();
        VDAdController.Get.LoadRewardedAd();

        FMUserDataService.Get().Init();
        //FMTrackingController.Get().Init();
        FMSoundController.Get().Init();
        FMMissionController.Get().Init();
        FMShopController.Get.Init(uiQuickAction);
        FMDailyLoginController.Get.Init();
        FMAssetFactory.InitAssets();

        FMUserDataService.Get().AssignLoginStamp();

#if ENABLE_BUNDLE && PLATFORM_ANDROID
        FMAssetBundleController.Get().DownloadBundles();
#endif

        uiQuickAction.Init(leaderboard);

#if ENABLE_CHEAT
        FMAssetFactory.GetDebugLog();

        VDDebugTool debugTool = FMAssetFactory.GetDebugTool();
        debugTool.Init(leaderboard);

        UIVersion uiVersion = FMAssetFactory.GetUIVersion();
        uiVersion.Root.SetParent(rootCanvas);
        uiVersion.Root.SetAsLastSibling();
        uiVersion.Root.offsetMin = Vector2.zero;
        uiVersion.Root.offsetMax = Vector2.zero;
#endif

        ChangeScene(SceneState.Lobby, uiQuickAction);
    }

    protected override void DoUpdate()
    {
        if (cinematicCallback != null)
        {
            cinematicCallback.Invoke();
        }
    }
}
