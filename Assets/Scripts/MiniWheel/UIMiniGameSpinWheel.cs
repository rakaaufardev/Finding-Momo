using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMiniGameSpinWheel : MonoBehaviour
{
    [Header("Content")]
    [SerializeField] private RectTransform rootContent;
    [SerializeField] private RectTransform rootTutorialContent;
    [SerializeField] private RectTransform arrowPointer;
    [SerializeField] private RectTransform failedText;
    [SerializeField] private GameObject panelClick;
    [SerializeField] private GameObject rewardPopUp;
    [SerializeField] private Image matchingGarbage;
    private Vector2 arrowStartPosition;
    private Tween arrowTween;

    [Header("Spin Wheel Slots")]
    [SerializeField] private SpinWheelThreeSlots wheelThreeSlots;
    [SerializeField] private SpinWheelFourSlots wheelFourSlots;
    [SerializeField] private SpinWheelFiveSlots wheelFiveSlots;
    [SerializeField] private SpinWheelSixSlots wheelSixSlots;
    private List<SpinWheelSlots> wheelSlotsList;

    private string targetGarbage;
    private int currentWheel;

    private FMMainScene mainScene;
    private MainWorld mainWorld;

    private void Awake()
    {
        wheelSlotsList = new List<SpinWheelSlots>
        {
            wheelThreeSlots,
            wheelFourSlots,
            wheelFiveSlots,
            wheelSixSlots
        };
    }

    private void Update()
    {
        foreach (var slotsType in wheelSlotsList)
        {
            slotsType.SpinWheel();
        }
    }
    
    public void Show(bool isShow)
    {
        rootContent.gameObject.SetActive(isShow);
        matchingGarbage.sprite = FMAssetFactory.GetGarbageIcon(targetGarbage);

        if (isShow)
        {
            arrowStartPosition = arrowPointer.anchoredPosition;
            arrowTween = arrowPointer.DOAnchorPos(arrowStartPosition + new Vector2(0, 25), 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            arrowTween?.Kill();
            arrowPointer.anchoredPosition = arrowStartPosition;
        }
    }

    public void SpawnWheel(string garbageName, int wheelType)
    {
        targetGarbage = garbageName;

        mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        rootTutorialContent.gameObject.SetActive(character.IsTutorial);
        panelClick.SetActive(true);

        if (wheelType == 3)
        {
            wheelThreeSlots.Show(true, targetGarbage);
            currentWheel = 3;
        }
        else if (wheelType == 4)
        {
            wheelFourSlots.Show(true, targetGarbage);
            currentWheel = 4;
        }
        else if (wheelType == 5)
        {
            wheelFiveSlots.Show(true, targetGarbage);
            currentWheel = 5;
        }
        else if (wheelType == 6)
        {
            wheelSixSlots.Show(true, targetGarbage);
            currentWheel = 6;
        }
    }

    // ASSIGNED TO EVENT TRIGGER
    public void SpinResult()
    {
        bool maxSpeed = false;

        foreach (var slotsType in wheelSlotsList)
        {
            if (slotsType.IsMaxSpeed)
            {
                maxSpeed = true;
                break;
            }
        }

        if (!maxSpeed)
        {
            return;
        }

        switch (currentWheel)
        {
            case 3:
                wheelThreeSlots.HasPressed = true;
                GetReward(wheelThreeSlots.SpinResult(targetGarbage));
                break;
            case 4:
                wheelFourSlots.HasPressed = true;
                GetReward(wheelFourSlots.SpinResult(targetGarbage));
                break;
            case 5:
                wheelFiveSlots.HasPressed = true;
                GetReward(wheelFiveSlots.SpinResult(targetGarbage));
                break;
            case 6:
                wheelSixSlots.HasPressed = true;
                GetReward(wheelSixSlots.SpinResult(targetGarbage));
                break;
        }
    }

    public void GetReward(bool isTrue)
    {
        StartCoroutine(ShowSpinResults(isTrue));
    }

    private IEnumerator ShowSpinResults(bool isTrue)
    {
        FMScoreController scoreController = mainScene.GetScoreController();
        UIMain uiMain = mainScene.GetUI();
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;

        panelClick.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        rootTutorialContent.gameObject.SetActive(!character.IsTutorial);
        Show(false);

        if (isTrue)
        {
            rewardPopUp.SetActive(true);
            character.CoinCollected += 250;
            scoreController.UpdateCoinScore(character.CoinCollected);

            RectTransform flyParticleDestination = uiMain.CoinIcon;
            uiMain.PlayFlyParticle("FlyParticle_Coin", Vector2.zero, flyParticleDestination, 3f, 50, true, Ease.InCubic, () =>
            {
                uiMain.PlayCoinIconBouncing();
                uiMain.UpdateCoinText(character.CoinCollected);
            });
        }
        else
        {
            failedText.gameObject.SetActive(true);
            failedText.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => failedText.localScale = Vector3.one);
        }


        foreach (var slotsType in wheelSlotsList)
        {
            slotsType.Show(false, null);
            slotsType.HasPressed = false;
        }

        float waitTime = isTrue ? 2f : 1f;

        yield return new WaitForSeconds(waitTime);

        rewardPopUp.SetActive(false);
        failedText.gameObject.SetActive(false);
        failedText.localScale = Vector3.zero;

        character.DisableOrEnableJump(false);
        uiMain.EnablePauseButton(true);
        mainWorld.PlayGame();
    }
}
