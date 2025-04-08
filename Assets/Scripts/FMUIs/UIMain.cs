using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public enum QTAResult
{
    Perfect,
    Good,
    Bad,
    Miss
}

public enum HandType
{
    NONE,
    Tap,
                                
}

public enum HandSwipeDirection
{
    Up,
    Right,
    Down,
    Left
}

public class UIMain : MonoBehaviour
{
    public Action<int> OnHealthChanged;
    public Action<int> OnCoinChanged;

    [Header("UI Content")]
    [SerializeField] private RectTransform rootContent;
    [SerializeField] private FMButton buttonPause;
    [SerializeField] private TextMeshProUGUI textDescription;
    [SerializeField] private TextMeshProUGUI missionTextDescription;
    [SerializeField] private List<string> missionTextList;
    [SerializeField] private RectTransform rootCurrency;
    [SerializeField] private RectTransform rootCoin;
    [SerializeField] private RectTransform rootDiamond;
    [SerializeField] private RectTransform rootHudGarbage;
    [SerializeField] private RectTransform rootUIParticle;
    [SerializeField] private RectTransform rootMissionMiniPopup;
    [SerializeField] private RectTransform rootInkParticle;
    [SerializeField] private Animator animInk;
    [SerializeField] private RectTransform rootPressSpace;
    private Coroutine queueMissionCoroutine;

    [Header("Game Timer")]
    [SerializeField] private RectTransform rootTimer;
    [SerializeField] private TextMeshProUGUI textTimer;

    [Header("Health")]
    [SerializeField] private RectTransform rootHealth;
    private List<HUDHealthIcon> hudHealthIcons;

    [Header("Score")]
    [SerializeField] private RectTransform rootScore;
    [SerializeField] private TextMeshProUGUI textScore;

    [Header("Static Message")]
    [SerializeField] private RectTransform rootStaticMessage;
    [SerializeField] private TextMeshProUGUI textStaticMessage;

    [Header("Speed Multiplier")]
    [SerializeField] private RectTransform rootSpeedMultiplier;
    [SerializeField] private Image imageFillBar;
    [SerializeField] private TextMeshProUGUI speedMultiplierTextLevel;
    [SerializeField] private TextMeshProUGUI speedMultiplierText;
    [SerializeField] private Animator animSpeedMultiplier;
    [SerializeField] private Tweener tweenCoinIcon;

    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private RectTransform rootCoinSpawn;
    [SerializeField] private RectTransform coinIcon;

    [Header("Diamond")]
    [SerializeField] private TextMeshProUGUI diamondText;

    [Header("QTA")]
    [SerializeField] private TextMeshProUGUI textQTAMessage;
    [SerializeField] private RectTransform rootExplanation;
    [SerializeField] private TextMeshProUGUI textExplanation;
    [SerializeField] private Transform qtaButton;
    [SerializeField] private Transform rootQTA;
    [SerializeField] private Transform buttonQTA;
    [SerializeField] private Transform rootPressIndicator;
    [SerializeField] private Transform timeIndicator;
    [SerializeField] private Transform perfectIndicator;
    [SerializeField] private Transform goodIndicator;
    [SerializeField] private Transform badIndicator;
    [SerializeField] private Transform rootQtaResult;
    [SerializeField] private List<GameObject> qtaResults;
    private Sequence qtaResultSequence;

    [Header("Distance Line")]
    [SerializeField] private RectTransform rootDistanceLine;
    [SerializeField] private Slider sliderDistance;
    [SerializeField] private Image imageBarDistance;

    [Header("Tutorial Content")]
    [SerializeField] private Button buttonOverlayTutorial;
    [SerializeField] private RectTransform uiStartGameRect;
    [SerializeField] private RectTransform rootTutorialEndTitle;
    [SerializeField] private GameObject startGameObject;
    private string tutorialSection;

    [Header("Fly Particle")]
    [SerializeField] private List<FMFlyParticle> flyParticleList;
    private FMFlyParticle flyParticle;

    [Header("Garbage")]
    [SerializeField] private GameObject garbageComplete;
    [SerializeField] private RectTransform rootParticle;
    [SerializeField] private Animator garbageAnimaton;
    private Dictionary<string, HUDGarbageItem> hudGarbages;

    [Header("Hand")]
    [SerializeField] private RectTransform rootHandTap;
    [SerializeField] private RectTransform rootHandSwipe;
    [SerializeField] private Sequence handSequence;

    [Header("TopFrame")]
    [SerializeField] private RectTransform rootTopFrameHUD;
    UITopFrameHUD topFrameHUD;

    [Header("Visual Feedback Arrow")]
    [SerializeField] private RectTransform rootArrowDirection;
    [SerializeField] private bool isArrowDirectionEnabled = false;

    [Header("Power Item")]
    [SerializeField] private RectTransform rootPowerItem;
    private List<HUDPowerUp> powerUpItems;

    [SerializeField] private UIMiniGameSpinWheel uiSpinWheel;

    private UIQuickAction uiQuickAction;

    private Coroutine inkCoroutine;
    public Dictionary<string, HUDGarbageItem> HudGarbages
    {
        get
        {
            return hudGarbages;
        }
        set
        {
            hudGarbages = value;
        }
    }

    public Sequence QTAResultSequence
    {
        get
        {
            return qtaResultSequence;
        }
        set
        {
            qtaResultSequence = value;
        }
    }

    public RectTransform CoinIcon
    {
        get
        {
            return coinIcon;
        }
        set
        {
            coinIcon = value;
        }
    }

    public RectTransform RootUIParticle
    {
        get
        {
            return rootUIParticle;
        }
        set
        {
            rootUIParticle = value;
        }
    }
    
    public RectTransform RootInkParticle
    {
        get
        {
            return rootInkParticle;
        }
        set
        {
            rootInkParticle = value;
        }
    }

    public UIMiniGameSpinWheel UISpinWheel
    {
        get => uiSpinWheel;
    }

    public void Init(UIQuickAction inUiQuickAction)
    {
        uiQuickAction = inUiQuickAction;
        
        hudGarbages = new Dictionary<string, HUDGarbageItem>();
        hudHealthIcons = new List<HUDHealthIcon>();
        flyParticleList = new List<FMFlyParticle>();
        topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.Show(false);
        topFrameHUD.ShowInputName(false);
        buttonPause.AddListener(OnClickPause);

        float perfectResultPercentage = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_PERFECT;
        float goodResultPercentage = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_GOOD;
        float badResultPercentage = VDParameter.TRANSITION_BACK_TO_SIDE_QTE_BAD;
        perfectIndicator.localScale = new Vector2(perfectResultPercentage, perfectResultPercentage);
        goodIndicator.localScale = new Vector2(goodResultPercentage, goodResultPercentage);
        badIndicator.localScale = new Vector2(badResultPercentage, badResultPercentage);
        AddHudPowerUP();
    }

    public void ShowPressSpaceText(bool show)
    {
        rootPressSpace.gameObject.SetActive(show);
    }

    public void EnablePauseButton(bool isEnable)
    {
        buttonPause.enabled = isEnable;
    }

    public void RefreshHeader()
    {
        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        topFrameHUD.UpdateCoinText(userInfo.inventoryData.coin);
        topFrameHUD.UpdateDiamondText(userInfo.inventoryData.gems);
    }

    public void UpdateDistanceLine(float value)
    {
        float newProgress = value;
        imageBarDistance.DOFillAmount(newProgress, 0.5f);
        sliderDistance.DOValue(newProgress, 0.5f);
    }

    public void SetHUD(WorldType worldType)
    {
        bool isMainGame = worldType == WorldType.Main;
        bool isSurfGame = worldType == WorldType.Surf;

        rootScore.gameObject.SetActive(isMainGame);
        rootDistanceLine.gameObject.SetActive(isSurfGame);
        rootHealth.gameObject.SetActive(isMainGame);
        rootSpeedMultiplier.gameObject.SetActive(isMainGame);
        rootHudGarbage.gameObject.SetActive(isMainGame);
        rootCoin.gameObject.SetActive(isMainGame);
        rootPowerItem.gameObject.SetActive(isMainGame);
        rootTimer.gameObject.SetActive(isMainGame && VDParameter.GAME_TIME_ACTIVE);
    }

    public void ShowContent(bool isShow)
    {
        rootContent.gameObject.SetActive(isShow);
    }

    public void UpdateGameTimer(float timer)
    {
        string timerString = VDTimeUtility.GetTimeStringFormat(timer);
        textTimer.SetText(timerString);
    }

    public bool IsQTAUIActive()
    {
        bool result = rootQTA.gameObject.activeSelf;
        return result;
    }

    public void ShowQTA(bool show, bool isTutorial)
    {
        string qtaMessage = VDCopy.GetCopy(CopyTag.BACK_VIEW_TRANSITION_PRESS);
        textQTAMessage.SetText(qtaMessage);

        rootQTA.gameObject.SetActive(show);

        if (!show)
        {
            timeIndicator.gameObject.SetActive(true);
            rootPressIndicator.gameObject.SetActive(true);
            buttonQTA.gameObject.SetActive(true);
            textDescription.gameObject.SetActive(true);
            int count = qtaResults.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject qtaResult = qtaResults[i];
                qtaResult.gameObject.SetActive(false);
            }
        }
        else
        {
#if UNITY_STANDALONE
            qtaButton.gameObject.SetActive(true);
#elif UNITY_ANDROID
            qtaButton.gameObject.SetActive(false);
            Vector3 position = isTutorial ? new Vector3(640, -150) : new Vector3(640, -35);
            ShowHandTap(position);
#endif
        }
    }

    public void SetQTAExplanation(bool isShow)
    {
        textExplanation.text = VDCopy.GetCopy(CopyTag.QTA_EXPLANATION);
        rootExplanation.gameObject.SetActive(isShow);
    }

    public void UpdateTimeQTEIndicator(float timePercentage)
    {
        timeIndicator.localScale = new Vector2(timePercentage,timePercentage);
    }

    public Coroutine InkCoroutine()
    {
        return inkCoroutine;
    }
    public void SetResultsText(QTAResult result)
    {
        timeIndicator.gameObject.SetActive(false);
        rootPressIndicator.gameObject.SetActive(false);
        buttonQTA.gameObject.SetActive(false);
        textDescription.gameObject.SetActive(false);
        rootExplanation.gameObject.SetActive(false);

        int count = qtaResults.Count;
        for (int i = 0; i < count; i++)
        {
            bool showResult = i == (int)result;
            GameObject qtaResult = qtaResults[i];
            qtaResult.gameObject.SetActive(showResult);
        }

        qtaResultSequence = DOTween.Sequence();

        int index = (int)result;
        qtaResultSequence.Append(qtaResults[index].transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f).SetEase(Ease.OutElastic));
        qtaResultSequence.AppendInterval(0.25f);
        qtaResultSequence.Append(qtaResults[index].transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.25f).SetEase(Ease.InBack)).OnComplete(() =>
        {
            ShowQTA(false, false);
            qtaResultSequence = null;
        });
    }

    public void SkipQtaResult()
    {
        qtaResultSequence.Complete();
        qtaResultSequence = null;
    }

    public void AddHealthIcon(int amount)
    {
        int iconCount = hudHealthIcons.Count;
        if (amount > iconCount)
        {
            int totalAdd = amount - iconCount;
            for (int i = 0; i < totalAdd; i++)
            {
                HUDHealthIcon healthIcon = FMAssetFactory.GetHUDHealthIcon();
                healthIcon.transform.SetParent(rootHealth);
                healthIcon.transform.localScale = Vector3.one;
                hudHealthIcons.Add(healthIcon);
            }
        }        
    } 

    public void UpdateHealthIcon()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        int currentHealth = character.CurrentHealth;
        int maxHealth = character.MaxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            bool isActivate = i < currentHealth;
            hudHealthIcons[i].Activate(isActivate);
        }
    }

    public void UpdateTextScore(int score,bool isMultiply)
    {
        if (isMultiply)
        {
            textScore.SetText(score.ToString() + "m <size=100><color=#FF0000><b>x2</b></color></size>");
        }
        else
        {
            textScore.SetText(score.ToString() + "m");
        }
    }  

    public void UpdateCoinText(int value)
    {
        coinText.SetText(value.ToString());
    }
    
    public void PlayFlyParticle(string particleName, Vector2 startPosition, RectTransform rootDestination, float scale, int amount, bool isRandom, Ease ease, Action endCallback)
    {
        if (amount > VDParameter.MAXIMUM_FLY_PARTICLE)
        {
            amount = VDParameter.MAXIMUM_FLY_PARTICLE;
        }

        int particleCount = flyParticleList.Count;
        for (int i = 0; i < amount; i++)
        {
            FMFlyParticle newParticle = null;
            if (i >= particleCount)
            {
                newParticle = InstantiateFlyParticle(particleName);
                flyParticleList.Add(newParticle);
            }
            else
            {
                for (int j = 0; j < particleCount; j++)
                {
                    FMFlyParticle particle = flyParticleList[j];
                    if (!particle.gameObject.activeSelf)
                    {
                        newParticle = particle;
                        break;
                    }
                }

                if (newParticle == null) // Only instantiate if no inactive particle was found
                {
                    newParticle = InstantiateFlyParticle(particleName);
                    flyParticleList.Add(newParticle);
                }
            }


            newParticle.transform.localPosition = startPosition;
            newParticle.transform.localScale = new Vector3(scale, scale, scale);
            newParticle.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();

            if (isRandom)
            {
                float rangeX = 750;
                float rangeY = 500;
                Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-rangeX, rangeX), UnityEngine.Random.Range(-rangeY, 200), 0);
                seq.Append(newParticle.transform.DOMove(randomPosition + newParticle.transform.position, 0.4f).SetEase(Ease.OutQuart));
            }

            float duration = 0.4f;
            seq.Append(newParticle.transform.DOMove(rootDestination.transform.position, duration).SetEase(ease));
            //seq.Join(newParticle.transform.DOScale(Vector3.one, duration).SetEase(Ease.InCubic));
            seq.AppendCallback(() =>
            {
                endCallback.Invoke();
                newParticle.gameObject.SetActive(false);
            });

            particleCount = flyParticleList.Count;
        }
    }

    public void PlayCoinIconBouncing()
    {
        if (tweenCoinIcon == null)
        {
            Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
            coinIcon.localScale = Vector3.one;
            tweenCoinIcon = coinIcon.DOPunchScale(targetScale, 0.1f, 1, 0).SetEase(Ease.Linear).OnComplete(() => {
                coinIcon.localScale = Vector3.one;
                tweenCoinIcon = null;
            });
        }        
    }

    private FMFlyParticle InstantiateFlyParticle(string particleName)
    {
        FMFlyParticle result = null;
        result = FMAssetFactory.GetFlyParticle(particleName);
        result.transform.SetParent(rootCoinSpawn.transform);

        return result;
    }

    public void UpdateHudGarbage(string name,float currentValue, float maxValue)
    {
        hudGarbages[name].SetFill(currentValue, maxValue);
        //if (isActive)
        //{
        //    string log = string.Format("Check Garbage : {0} active from {1}",name,activeFrom);
        //    Debug.Log(log);
        //    FMSceneController.Get().PlayParticle("UIParticle_Garbage", hudGarbages[name].Root.localPosition + Vector3.up * 435, false);
        //    hudGarbages[name].transform.DOPunchScale(new Vector3(1.25f, 1.25f, 1.25f), 1f, 1).SetEase(Ease.OutElastic).OnComplete(() =>
        //    {
        //        hudGarbages[name].transform.localScale = Vector3.one;
        //    });
        //}
    }

    public void SetHudGarbageComplete(string name)
    {
        HUDGarbageItem item = hudGarbages[name];

        if (!item.IsActive)
        {
            item.SetActive(true);
            item.PlayFillEffect(true,Vector3.one);
            FMSceneController.Get().PlayParticle("UIParticle_Garbage", item.Root, Vector3.zero);
            FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);
        }
    }

    public void PlayGarbageFillHUD(string name)
    {
        HUDGarbageItem item = hudGarbages[name];
        item.PlayFillEffect(false,Vector3.one);
    }

    public void CompleteGarbageSequence(bool isAddHealth, bool flyToHealth)
    {
        StartCoroutine(StartCompleteGarbageSequence(isAddHealth, flyToHealth));
    }

    IEnumerator StartCompleteGarbageSequence(bool isAddHealth, bool flyToHealth)
    {
        bool shouldContinue = false;

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;

        HUDHealthIcon hudHealthIcon = null;
        Transform hudHealthRoot = null;

        if (flyToHealth)
        {
            FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

            int currentHealth = character.CurrentHealth;
            int maxHealth = character.MaxHealth;
            int lastIndex = hudHealthIcons.Count - 1;
            if (currentHealth == maxHealth)
            {
                hudHealthIcon = hudHealthIcons[lastIndex];
                hudHealthRoot = hudHealthIcon.Root;
            }
            else
            {
                hudHealthIcon = hudHealthIcons[currentHealth - 1];
                hudHealthRoot = hudHealthIcon.Root;
            }

            if (isAddHealth)
            {
                hudHealthIcon.RootVisual.gameObject.SetActive(false);
            }
            else
            {
                hudHealthIcon.HealthIcon.gameObject.SetActive(false);
            }
        }

        foreach (var hudGarbage in hudGarbages)
        {
            hudGarbage.Value.gameObject.SetActive(false);
        }

        garbageComplete.gameObject.SetActive(true);
        garbageAnimaton.Play("GarbageGather");

        while (!shouldContinue)
        {
            shouldContinue = garbageAnimaton.GetCurrentAnimatorStateInfo(0).IsName("Finish");
            yield return new WaitForEndOfFrame();
        }

        if (flyToHealth)
        {
            Vector2 offset = new Vector2(50, 100);
            garbageComplete.transform.SetParent(hudHealthRoot);
            garbageComplete.transform.DOLocalMove(Vector2.zero + offset, 0.3f).OnComplete(() =>
            {
                garbageComplete.transform.SetParent(rootHudGarbage.transform);
                garbageComplete.transform.localPosition = new Vector3(0, 0, 0);
                garbageComplete.SetActive(false);
                foreach (var hudGarbage in hudGarbages)
                {
                    hudGarbage.Value.gameObject.SetActive(true);
                }

                hudHealthIcon.RootVisual.gameObject.SetActive(true);
                hudHealthIcon.HealthIcon.gameObject.SetActive(true);
                Vector3 offset = new Vector3(50, -50, 0);
                FMSceneController.Get().PlayParticle("UIParticle_HeartCreate", hudHealthRoot, offset);
            });
        }
        else
        {

            foreach (var hudGarbage in hudGarbages)
            {
                hudGarbage.Value.gameObject.SetActive(true);
            }

            garbageComplete.SetActive(false);
        }
    }

    public void SetHudGarbage()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();
        int count = garbageController.GarbageNames.Count;
        for (int i = 0; i < count; i++)
        {
            string name = garbageController.GarbageNames[i];
            Sprite sprite = FMAssetFactory.GetGarbageIcon(name);

            if (!hudGarbages.ContainsKey(name))
            {
                HUDGarbageItem item = FMAssetFactory.GetHUDGarbageItem();
                item.SetIcon(sprite);
                item.Root.SetParent(rootHudGarbage);
                item.transform.localScale = Vector3.one;
                item.SetActive(false);

                hudGarbages.Add(name, item);
            }

            float collectRemain = garbageController.GetGarbageRemain(name);
            float totalCount = garbageController.GetGarbageCount(name);
            UpdateHudGarbage(name, collectRemain, totalCount);
        }
    }

    public void ClearAllGarbageHUD()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();
        List<string> garbageCollected = garbageController.GetGarbageNames();
        foreach (string names in garbageCollected)
        {
            string name = names;
            float collectRemain = garbageController.GetGarbageRemain(name);
            float totalCount = garbageController.GetGarbageCount(name);
            UpdateHudGarbage(name, collectRemain, totalCount);
            
            hudGarbages[name].SetActive(false);
        }
    }

    public void UpdateSpeedMultiplierTimer(float timer, float maxDuration)
    {
        float barValue = (maxDuration - timer) / maxDuration;
        imageFillBar.fillAmount = barValue;
    }

    public void UpdateSpeedMultiplierLevel(int level)
    {
        speedMultiplierTextLevel.SetText(level.ToString());
        speedMultiplierTextLevel.transform.DOPunchScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f, 1).SetEase(Ease.OutElastic).OnComplete(() =>
        {
            speedMultiplierTextLevel.transform.localScale = new Vector3(1f, 1f, 1f);
        });
    }

    private void SetSpeedMultiplierText(string text)
    {
        speedMultiplierText.SetText(text);
    }

    public void TriggerSpeedMultiplierAnim(string trigger)
    {
        animSpeedMultiplier.SetTrigger(trigger);
    }

    public void ShowStaticMessage(bool isShow, string message = "")
    {
        rootStaticMessage.gameObject.SetActive(isShow);
        textStaticMessage.SetText(message);
    }

    public void ShowMissionNotice(string missionDescription)
    {
        missionTextList.Add(missionDescription);

        if (queueMissionCoroutine == null)
        {
            queueMissionCoroutine = StartCoroutine(QueueMission());
        }
    }

    public void SwitchCurrencyHUD(bool isCoin)
    {
        rootCoin.gameObject.SetActive(isCoin);
        rootDiamond.gameObject.SetActive(!isCoin);
    }

    public void UpdateDiamondText(int amount)
    {
        diamondText.SetText(amount.ToString());
    }

    private IEnumerator QueueMission()
    {
        while (missionTextList != null && missionTextList.Count > 0)
        {
            missionTextDescription.text = missionTextList[0];

            Sequence seq = DOTween.Sequence();
            seq.Append(rootMissionMiniPopup.DOAnchorPosY(0, 0.4f).SetEase(Ease.InCubic))
                 .AppendInterval(2f)
                 .Append(rootMissionMiniPopup.DOAnchorPosY(115f, 0.4f).SetEase(Ease.InCubic))
                 .AppendCallback(() => 
                 {
                     missionTextList.RemoveAt(0);

                     if (missionTextList.Count == 0)
                     {
                         queueMissionCoroutine = null;
                     }
                 });

            yield return seq.WaitForCompletion();
        }
    }

    public IEnumerator InkParticleSequence(float inkTimer)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        float time = 0f;

        if (rootInkParticle == null) yield break;
        rootInkParticle.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        if (animInk != null)
            animInk.SetTrigger("Start");

        yield return new WaitForSeconds(0.25f);

        if (animInk != null)
            animInk.SetTrigger("Show");

        while (time < inkTimer)
        {
            if (mainScene == null) yield break;

            if (mainScene.GameStatus == GameStatus.Pause)
            {
                yield return new WaitUntil(() => mainScene.GameStatus != GameStatus.Pause);
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (animInk != null)
            animInk.SetTrigger("End");

        inkCoroutine = null;

        yield return new WaitForSeconds(0.25f);

        if (rootInkParticle != null)
            rootInkParticle.gameObject.SetActive(false);
    }

    public void StopInkParticleSequence()
    {
        if (inkCoroutine != null)
        {
            StopCoroutine(inkCoroutine);
            inkCoroutine = null;
        }
    }


    public void ShowMaxSpeedMultiplier()
    {
        SetSpeedMultiplierText("Max Speed!");
        TriggerSpeedMultiplierAnim("active");
    }

    public void ShowLevelUpSpeedMultiplier()
    {
        SetSpeedMultiplierText("Speed Up!");

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            TriggerSpeedMultiplierAnim("show");
        });

        seq.AppendInterval(2f);

        seq.AppendCallback(() =>
        {
            TriggerSpeedMultiplierAnim("hide");
        });
    }

    private void OnClickPause()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMWorld world = mainScene.GetCurrentWorldObject();

        if (mainScene.GameStatus != GameStatus.Finish)
        {
            world.PauseGame();
            FMUIWindowController.Get.OpenWindow(UIWindowType.Pause, uiQuickAction);
        }
    }

    public void ShowTutorialEndTitle()
    {
        rootTutorialEndTitle.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Append(rootTutorialEndTitle.DOScale(Vector3.one * 1.1f, 3f).SetEase(Ease.OutElastic));
    }

    public void ShowStartGameUI()
    {
        startGameObject.SetActive(true); 
        
        uiStartGameRect.transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f).SetEase(Ease.OutElastic);
    }

    public void HideStartGameUI() 
    {
        uiStartGameRect.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.25f).SetEase(Ease.InBack).OnComplete(() => startGameObject.SetActive(false));
    }

    public void ShowHandTap(Vector3 localPosition)
    {
#if UNITY_ANDROID
        rootHandTap.gameObject.SetActive(true);
        rootHandTap.localPosition = localPosition;
#endif
    }

    public void HideHandTap()
    {
#if UNITY_ANDROID
        rootHandTap.gameObject.SetActive(false);
#endif
    }

    public void ShowHandSwipe(HandSwipeDirection direction, Vector3 localPosition)
    {
#if UNITY_ANDROID
        rootHandSwipe.gameObject.SetActive(true);

        float targetMoveX = localPosition.x;
        float targetMoveY = localPosition.y;
        switch (direction)
        {
            case HandSwipeDirection.Up:
                targetMoveY = localPosition.y + 250;
                break;
            case HandSwipeDirection.Down:
                targetMoveY = localPosition.y - 250;
                break;
            case HandSwipeDirection.Right:
                targetMoveX = localPosition.x + 250;
                break;
            case HandSwipeDirection.Left:
                targetMoveX = localPosition.x - 250;
                break;
        }

        Vector3 targetMove = new Vector3(targetMoveX, targetMoveY, localPosition.z);
        if (handSequence == null)
        {
            rootHandSwipe.localPosition = localPosition;
            handSequence = DOTween.Sequence();
            handSequence.Append(rootHandSwipe.DOLocalMove(targetMove, 0.75f).SetEase(Ease.InQuint));
            handSequence.AppendInterval(0.15f);
            handSequence.SetLoops(-1, LoopType.Restart);
            handSequence.Play();
        }
#endif
    }

    public void HideHandSwipe()
    {
#if UNITY_ANDROID
        if (handSequence != null)
        {
            handSequence.Kill();
            handSequence = null;
        }

        rootHandSwipe.gameObject.SetActive(false);
#endif
    }

    public void EnableArrowDirection(bool enable)
    {
        isArrowDirectionEnabled = enable;
        if (!enable)
        {
            HideArrowDirection();
        }
    }

    public void RotateArrowDirection(SwipeDirection direction)
    {
        if (!isArrowDirectionEnabled) return;

        rootArrowDirection.gameObject.SetActive(true);
        switch (direction)
        {
            case SwipeDirection.left:
                rootArrowDirection.transform.localRotation = Quaternion.Euler(0, -180, 0);
                break;
            case SwipeDirection.Right:
                rootArrowDirection.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case SwipeDirection.Up:
                rootArrowDirection.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case SwipeDirection.Down:
                rootArrowDirection.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
        }
        StartCoroutine(DelayHideArrowDirection());
    }

    public void HideArrowDirection()
    {
        if (!isArrowDirectionEnabled) return;

        StopCoroutine(DelayHideArrowDirection());
        rootArrowDirection.gameObject.SetActive(false);
    }

    IEnumerator DelayHideArrowDirection()
    {
        yield return new WaitForSeconds(1);
        HideArrowDirection();
    }

    private void AddHudPowerUP()
    {
        ItemType[] itemTypes = Enum.GetValues(typeof(ItemType))
            .Cast<ItemType>()
            .Where(item => item != ItemType.Continue && item != ItemType.COUNT)
            .ToArray();

        powerUpItems = new List<HUDPowerUp>();

        for (int i = 0; i < itemTypes.Length; i++)
        {
            ItemType itemType = itemTypes[i];

            InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
            ItemData itemData = inventoryData.GetItemData(itemType);

            if (itemData == null)
            {
                Debug.LogError($"ItemData is null for itemType: {itemType}");
                continue;
            }

            HUDPowerUp powerUpItem = FMAssetFactory.GetHUDPowerUp();
            if (powerUpItem == null)
            {
                Debug.LogError($"FMAssetFactory.GetHUDPowerUp() returned null for itemType: {itemType}");
                continue;
            }

            string iconName = itemType.ToString();
            Sprite powerUpIcon = FMAssetFactory.GetPowerUpIcon(iconName);

            powerUpItem.transform.SetParent(rootPowerItem);
            powerUpItem.SetPowerUpItem(itemData.amount, powerUpIcon, () => OnPowerUpButtonClick(itemType, itemData, powerUpItem));

            powerUpItems.Add(powerUpItem);
        }
    }

    private void OnPowerUpButtonClick(ItemType itemType,ItemData itemData,HUDPowerUp powerUpItem)
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;
        character.AddPowerUpStats(itemType, itemData, powerUpItem, this);
    }

    public void HidePowerUpUI()
    {
        rootPowerItem.gameObject.SetActive(false);
    }
}
