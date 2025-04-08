using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIMissionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMissionProgress;
    [SerializeField] private TextMeshProUGUI textMissionDesc;
    [SerializeField] private TextMeshProUGUI textRewardAmount;
    [SerializeField] private TextMeshProUGUI claimButtonText;
    [SerializeField] private TextMeshProUGUI equipButtonText;
    [SerializeField] private Slider sliderProgress;
    [SerializeField] private Image rewardImage;
    [SerializeField] private Image missionImage;
    [SerializeField] private Image frameRewardImage;
    [SerializeField] private FMButton claimButton;
    [SerializeField] private FMButton equipButton;
    [SerializeField] private RectTransform claimNotificationIcon;
    [SerializeField] private RectTransform rootFrameImage;
    [SerializeField] private RectTransform rootFrame;
    [SerializeField] private RectTransform rootFrameScore;
    [SerializeField] private RectTransform scoreOverlay;
    [SerializeField] private RectTransform scoreNotificationIcon;
    [SerializeField] private Material enabledFont;
    [SerializeField] private Material disabledFont;

    private int missionIndex;
    private VDUIWindow currentWindow;

    public int MissionIndex => missionIndex;

    public void Init(Action<int> onClaimReward, Action<int> onEquip, int inMissionindex, VDUIWindow parentWindow)
    {
        missionIndex = inMissionindex;
        currentWindow = parentWindow;

        if (currentWindow is UIWindowMissionList)
        {
            claimButton.AddListener(() => onClaimReward(missionIndex));
            equipButton.AddListener(() => onEquip(missionIndex));
            equipButtonText.fontSharedMaterial = disabledFont;
            SetLayoutState(true);
        }
        else if (currentWindow is UIWindowSurfMission || currentWindow is UIWindowPause)
        {
            textMissionDesc.rectTransform.sizeDelta = new Vector2(textMissionDesc.rectTransform.sizeDelta.x, 200f);
            SetLayoutState(false);
        }
    }

    public void SetMissionDetails(SurfMissionProgressData missionProgressData)
    {
        int score = missionProgressData.rewardScoreValue;
        float goalProgress = missionProgressData.goalProgress;
        float goalValue = missionProgressData.goalValue;
        string missionDescription = missionProgressData.missionDescription;

        textMissionDesc.SetText(missionDescription);
        textRewardAmount.SetText($"Score\n+{score}");
        textMissionProgress.SetText($"{goalProgress}/{goalValue}");
        sliderProgress.value = goalProgress / goalValue;

        scoreOverlay.gameObject.SetActive(goalProgress < goalValue);
        scoreNotificationIcon.gameObject.SetActive(goalProgress >= goalValue);
        if (goalProgress >= goalValue)
        {
            if (frameRewardImage != null)
            {
                frameRewardImage.color = Color.white;
                textRewardAmount.color = Color.white;
            }
        }
        else
        {
            if (frameRewardImage != null)
            {
                Color completeColor;
                ColorUtility.TryParseHtmlString("#395EBC", out completeColor);
                frameRewardImage.color = completeColor;
                
            }
        }
    }

    public void SetImage(Sprite rewardSprite,Sprite missionSprite, bool isActive)
    {
        if (rewardSprite == null || rootFrame == null) return;
        rewardImage.sprite = rewardSprite;
        rewardImage.preserveAspect = true;
        missionImage.sprite = missionSprite;
        missionImage.preserveAspect = true;
        rootFrame.gameObject.SetActive(isActive);
    }

    public void SetButtonState(bool isClaimable)
    {
        claimButton.gameObject.SetActive(isClaimable);
        equipButton.gameObject.SetActive(!isClaimable);
    }

    public void SetRewardClaimable(bool isClaimable)
    {
        claimButton.interactable = isClaimable;
        claimButtonText.text = "CLAIM";
        claimNotificationIcon.gameObject.SetActive(isClaimable);
        equipButtonText.fontSharedMaterial = enabledFont;
    }

    public void SetEquipmentState(bool ownEquipment, bool isEquip)
    {
        Color lockedColor;
        ColorUtility.TryParseHtmlString("#7DA5DF", out lockedColor);
        equipButton.interactable = ownEquipment && !isEquip;
        equipButtonText.SetText(isEquip ? "EQUIPPED" : ownEquipment ? "Equip" : "LOCKED");
        if (!isEquip && !ownEquipment)
        {
            equipButtonText.color = lockedColor;
        }
        else if (isEquip)
        {
            equipButtonText.color = lockedColor;
        }
        else
        {
            equipButtonText.color = Color.white;
            equipButtonText.fontSize = 40f;
        }
    }

    private void SetLayoutState(bool isPermanentMission)
    {
        if (rootFrameImage == null) return;
        rootFrameImage.gameObject.SetActive(isPermanentMission);
        rootFrameScore.gameObject.SetActive(!isPermanentMission);
        claimButton.gameObject.SetActive(isPermanentMission);
    }
}