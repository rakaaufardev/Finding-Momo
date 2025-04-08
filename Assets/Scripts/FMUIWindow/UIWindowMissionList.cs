using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VD;


public class UIWindowMissionList : VDUIWindow
{
    [SerializeField] private RectTransform rootItems;
    [SerializeField] private FMButton buttonClaimAll;
    [SerializeField] private TextMeshProUGUI textClaimAll;
    [SerializeField] private SnapScroller snapScroller;
    [SerializeField] private Material enabledFont;
    [SerializeField] private Material disabledFont;

    private List<UIMissionItem> items;

    private UITopFrameHUD uiTopFrameHUD;

    private const int MAX_CONTENT = 3;
    
    public override void Show(object[] dataContainer)
    {
        items = new List<UIMissionItem>();
        List<GameObject> groups = new List<GameObject>();

        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.ShowInputName(false);

        Dictionary<string, SurfMissionProgressData> missions = FMMissionController.Get().PermanentProgressDictionary;
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        RectTransform rootContent = null;
        int contentCount = 0;
        int count = missions.Count;
        for (int i = 0; i < count; i++)
        {
            if (contentCount <= 0)
            {
                contentCount = MAX_CONTENT;
                GameObject groupContent = FMAssetFactory.GetMissionGroup();                
                groupContent.transform.SetParent(rootItems);
                rootContent = groupContent.GetComponent<RectTransform>();
                rootContent.localScale = Vector3.one;

                groups.Add(groupContent);
            }

            contentCount--;

            SurfMissionProgressData data = missions.GetValue(i);
            UIMissionItem missionItem = FMAssetFactory.GetUIMissionItemVertical(rootContent);

            missionItem.Init(OnClaimReward, OnEquip, i, this);

            missionItem.SetMissionDetails(data);

            string thumbnailRewardName = string.Format("Thumbnail_Shop_Surfboard_{0}", data.surfboardReward);
            string thumbnailMissionName = string.Format("Thumbnail_Mission_Surfboard_{0}", data.missionType);
            Sprite thumbnailRewardSprite = FMAssetFactory.GetShopImageThumbnail(thumbnailRewardName);
            Sprite thumbnailMissionSprite = FMAssetFactory.GetMissionImageThumbnail(thumbnailMissionName);
            missionItem.SetImage(thumbnailRewardSprite,thumbnailMissionSprite,true);
                        
            bool isClaimable = FMMissionController.Get().IsClaimable(inventoryData, data.surfboardReward, data.goalProgress, data.goalValue);
            missionItem.SetButtonState(isClaimable);

            if (isClaimable)
            {
                missionItem.SetRewardClaimable(true);
            }
            else
            {
                bool isEquip = inventoryData.inUsedSurfboard == data.surfboardReward;
                bool ownEquipment = FMMissionController.Get().IsOwnEquipment(inventoryData, data.surfboardReward);
                missionItem.SetEquipmentState(ownEquipment, isEquip);
            }

            items.Add(missionItem);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rootContent);
        }


        LayoutRebuilder.ForceRebuildLayoutImmediate(rootItems);

        int groupCount = Mathf.CeilToInt((float)count / MAX_CONTENT);
        snapScroller.SetScroller(groupCount);
        snapScroller.SetPages(groups);

        buttonClaimAll.interactable = FMMissionController.Get().IsClaimableMissionExist();
        if (buttonClaimAll.IsInteractable())
        {
            textClaimAll.fontSharedMaterial = enabledFont;
        }
        buttonClaimAll.onClick.AddListener(OnClaimAll);
    }

    public override void DoUpdate()
    {

    }

    public override void Hide()
    {
        uiTopFrameHUD.ShowInputName(true);
    }

    public override void OnRefresh()
    {

    }

    private void OnClaimReward(int missionIndex)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> missions = inventoryData.userPermanentMissionProgress;
        SurfMissionProgressData mission = missions[missionIndex];
        mission.hasClaimed = true;

        FMInventory.Get.AddInventory(RewardType.Surfboard, (int)mission.surfboardReward, 0);
        FMUserDataService.Get().SaveInventoryData(inventoryData);

        UIMissionItem item = items[missionIndex];
        item.SetButtonState(false);
        item.SetRewardClaimable(false);
        item.SetEquipmentState(true, false);

        FMSceneController.Get().CheckClaimableMissionReward();

        FMUIWindowController.Get.OpenWindow(UIWindowType.RewardAlert,
            new UIRewardAlertItemData[]
            {
                new UIRewardAlertItemData(RewardType.Surfboard, (int)mission.surfboardReward, 0)
            });
    }

    private void OnEquip(int missionIndex)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> missions = inventoryData.userPermanentMissionProgress;
        SurfMissionProgressData selectedMission = missions[missionIndex];

        FMInventory.Get.SetCharacterSurfboard(selectedMission.surfboardReward);

        int count = items.Count;
        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData checkMission = missions[i];
            bool isClaimable = FMMissionController.Get().IsClaimable(inventoryData, checkMission.surfboardReward, checkMission.goalProgress, checkMission.goalValue);
            bool isOwnEquipment = FMMissionController.Get().IsOwnEquipment(inventoryData, checkMission.surfboardReward);
            bool isEquip = inventoryData.inUsedSurfboard == checkMission.surfboardReward;

            UIMissionItem item = items[i];
            item.SetEquipmentState(isOwnEquipment, isEquip);
            item.SetButtonState(isClaimable);
        }
    }

    private void OnClaimAll()
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> savedPermanentMissions = inventoryData.userPermanentMissionProgress;
        FMMissionController.Get().CompleteAllPermanentMissions();

        int count = items.Count;
        for (int i = 0; i < count; i++)
        {
            UIMissionItem item = items[i];
            int missionIndex = item.MissionIndex;
            SurfMissionProgressData savedMission = savedPermanentMissions[missionIndex];
            bool isClaimable = FMMissionController.Get().IsClaimable(inventoryData,savedMission.surfboardReward,savedMission.goalProgress,savedMission.goalValue);
            item.SetButtonState(isClaimable);
        }

        buttonClaimAll.interactable = FMMissionController.Get().IsClaimableMissionExist();
        if (!buttonClaimAll.IsInteractable())
        {
            textClaimAll.fontSharedMaterial = disabledFont;
        }
    }
}
