using UnityEngine;
using VD;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIWindowInventory : VDUIWindow
{
    [Header("Side Tab Selection")]
    [SerializeField] private UIInventoryTabButton tabButtonCostumeInventory;
    [SerializeField] private UIInventoryTabButton tabButtonSurfboardInventory;

    [Header("Item Info")]
    [SerializeField] private TextMeshProUGUI textItemName;
    [SerializeField] private FMButton buttonEquip;
    [SerializeField] private FMButton buttonEquipped;
    [SerializeField] private FMButton buttonLocked;

    [Header("Inventory")]
    [SerializeField] private FMButton costumeTabDarkOverlay;
    [SerializeField] private FMButton surfboardTabDarkOverlay;
    [SerializeField] private RectTransform costumeTab;
    [SerializeField] private RectTransform surfboardTab;
    [SerializeField] private ScrollRect scrollRectCostumeContent;
    [SerializeField] private ScrollRect scrollRectSurfboardContent;

    [Header("3D Model View")]    
    [SerializeField] private Transform root3DModelCostume;
    [SerializeField] private Transform root3DModelSurfboard;
    [SerializeField] private GameObject object3DModel;

    private FMTouchScreenController touchScreenController = new();

    private ScrollRect currentShownContent;
    private UIInventoryTabButton currentSelectedTabButton;
    private UIInventoryItem currentSelectedCostume;  
    private UIInventoryItem currentSelectedSurfboard;  
    private UIInventoryItem currentEquippedCostume;  
    private UIInventoryItem currentEquippedSurfboard;
    private UITopFrameHUD topFrameHUD;

    private Sprite cachedLockedIcon, cachedEquippedIcon;

    public override void Show(params object[] dataContainer)
    {
        topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.ShowInputName(false);

        InitContents();
        InitTabButtons();
        buttonEquip.AddListener(EquipItem);

        scrollRectSurfboardContent.gameObject.SetActive(false);
        scrollRectCostumeContent.gameObject.SetActive(false);

        OnTabButtonCostume();
    }

    public void OnItemSelected(UIInventoryItem selectedItem)
    {
        selectedItem.SetSelectedState(true);
        SetItemInfo(selectedItem);

        switch (selectedItem.GetData().ItemType)
        {
            case RewardType.Costume:
                if (currentSelectedCostume == selectedItem) return;
                if(currentSelectedCostume != null)
                    currentSelectedCostume.SetSelectedState(false);               
                currentSelectedCostume = selectedItem;
                Set3DModelCostume((Costume)selectedItem.GetData().ItemID);
                break;
            case RewardType.Surfboard:
                if (currentSelectedSurfboard == selectedItem) return;
                if (currentSelectedSurfboard != null)
                    currentSelectedSurfboard.SetSelectedState(false);
                currentSelectedSurfboard = selectedItem;
                Set3DModelSurfboard((Surfboard)selectedItem.GetData().ItemID);
                break;
            default:
                break;
        }

    }

    private void SetItemInfo(UIInventoryItem item)
    {
        SetItemInfoButton(item.ItemStatus);

        var itemData = item.GetData();
        switch (itemData.ItemType)
        {
            case RewardType.Costume:
                textItemName.text = ((Costume)itemData.ItemID).ToString();                
                break;
            case RewardType.Surfboard:
                textItemName.text = ((Surfboard)itemData.ItemID).ToString();
                break;
        }
    }

    private void SetItemInfoButton(InventoryItemStatus itemStatus)
    {
        switch (itemStatus)
        {
            case InventoryItemStatus.Equipped:
                buttonEquip.gameObject.SetActive(false);
                buttonEquipped.gameObject.SetActive(true);
                buttonLocked.gameObject.SetActive(false);
                break;
            case InventoryItemStatus.Unlocked:
                buttonEquip.gameObject.SetActive(true);
                buttonEquipped.gameObject.SetActive(false);
                buttonLocked.gameObject.SetActive(false);
                break;
            case InventoryItemStatus.Locked:
                buttonEquip.gameObject.SetActive(false);
                buttonEquipped.gameObject.SetActive(false);
                buttonLocked.gameObject.SetActive(true);
                break;
        }
    }

    private void EquipItem()
    {
        if (currentSelectedTabButton == tabButtonCostumeInventory)
        {
            FMInventory.Get.SetCharacterCostume((Costume)currentSelectedCostume.GetData().ItemID);
            currentSelectedCostume.SetStatus(InventoryItemStatus.Equipped);
            currentEquippedCostume.SetStatus(InventoryItemStatus.Unlocked);
            currentEquippedCostume = currentSelectedCostume;
            SetItemInfoButton(currentEquippedCostume.ItemStatus);
            tabButtonCostumeInventory.SetEquippedImage(currentEquippedCostume.GetData());
        }
        else if(currentSelectedTabButton == tabButtonSurfboardInventory)
        {
            FMInventory.Get.SetCharacterSurfboard((Surfboard)currentSelectedSurfboard.GetData().ItemID);
            currentSelectedSurfboard.SetStatus(InventoryItemStatus.Equipped);
            currentEquippedSurfboard.SetStatus(InventoryItemStatus.Unlocked);
            currentEquippedSurfboard = currentSelectedSurfboard;
            SetItemInfoButton(currentEquippedSurfboard.ItemStatus);
            tabButtonSurfboardInventory.SetEquippedImage(currentEquippedSurfboard.GetData());
        }

    }

    private void Set3DModelCostume(Costume costumeID)
    {        
        if(object3DModel != null)
        {
            Destroy(object3DModel);
        }
        object3DModel = FMAssetFactory.GetCostume(costumeID, WorldType.Main).gameObject;
        object3DModel.transform.SetParent(root3DModelCostume, false);
    }

    private void Set3DModelSurfboard(Surfboard surfboardID)
    {
        if (object3DModel != null)
        {
            Destroy(object3DModel);
        }
        object3DModel = FMAssetFactory.GetSurfboard(surfboardID).gameObject;
        object3DModel.transform.SetParent(root3DModelSurfboard, false);
    }

    private void InitTabButtons()
    {
        tabButtonCostumeInventory.AddListenerToButtonComponent(OnTabButtonCostume);
        tabButtonCostumeInventory.SetEquippedImage(currentEquippedCostume.GetData());

        tabButtonSurfboardInventory.AddListenerToButtonComponent(OnTabButtonSurfboard);
        tabButtonSurfboardInventory.SetEquippedImage(currentEquippedSurfboard.GetData());

        costumeTabDarkOverlay.AddListener(OnTabButtonCostume);
        surfboardTabDarkOverlay.AddListener(OnTabButtonSurfboard);
    }

    private void OnTabButtonCostume()
    {        
        if (currentSelectedTabButton == tabButtonCostumeInventory) return;

        if(currentSelectedTabButton != null)
            currentSelectedTabButton.SetButtonSelectedState(false);
        tabButtonCostumeInventory.SetButtonSelectedState(true);
        currentSelectedTabButton = tabButtonCostumeInventory;

        if (currentShownContent != null)
            currentShownContent.gameObject.SetActive(false);
        scrollRectCostumeContent.gameObject.SetActive(true);
        currentShownContent = scrollRectCostumeContent;

        if (currentSelectedSurfboard != null)
            currentSelectedSurfboard.SetSelectedState(false);
        currentSelectedSurfboard = null;

        costumeTab.gameObject.SetActive(true);
        surfboardTab.gameObject.SetActive(false);

        OnItemSelected(currentEquippedCostume);
    }

    private void OnTabButtonSurfboard()
    {
        if (currentSelectedTabButton == tabButtonSurfboardInventory) return;

        if (currentSelectedTabButton != null)
            currentSelectedTabButton.SetButtonSelectedState(false);
        tabButtonSurfboardInventory.SetButtonSelectedState(true);
        currentSelectedTabButton = tabButtonSurfboardInventory;

        if (currentShownContent != null)
            currentShownContent.gameObject.SetActive(false);
        scrollRectSurfboardContent.gameObject.SetActive(true);
        currentShownContent = scrollRectSurfboardContent;

        if (currentSelectedCostume != null)
            currentSelectedCostume.SetSelectedState(false);
        currentSelectedCostume = null;

        surfboardTab.gameObject.SetActive(true);
        costumeTab.gameObject.SetActive(false);

        OnItemSelected(currentEquippedSurfboard);
    }

    private void InitContents()
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        HashSet<Costume> unlockedCostumes = new HashSet<Costume>(inventoryData.costumes);
        HashSet<Surfboard> unlockedSurfboard = new HashSet<Surfboard>(inventoryData.surfboards);

        //Add Costumes
        for(int i = 0; i < (int)Costume.COUNT; i++)
        {
            Costume costumeID = (Costume)i;

            var inventoryItem = FMAssetFactory.GetInventoryItem(scrollRectCostumeContent.content);
            bool isEquipped = inventoryData.inUsedCostume == costumeID;

            InventoryItemStatus itemStatus = 
                isEquipped ? 
                InventoryItemStatus.Equipped : 
                unlockedCostumes.Contains(costumeID) || costumeID == Costume.Casual ? 
                InventoryItemStatus.Unlocked : InventoryItemStatus.Locked;            

            inventoryItem.SetItem(this, new UIInventoryItemData(RewardType.Costume, i), itemStatus);
            inventoryItem.name = "Costume " + costumeID.ToString(); 

            inventoryItem.SetSelectedState(false);
            if (isEquipped)
            {
                currentEquippedCostume = inventoryItem;            
            }           
        }

        //Add Surfboards
        for (int i = 0; i < (int)Surfboard.COUNT; i++)
        {
            Surfboard surfboardID = (Surfboard)i;

            var inventoryItem = FMAssetFactory.GetInventoryItem(scrollRectSurfboardContent.content);
            bool isEquipped = inventoryData.inUsedSurfboard == surfboardID;

            InventoryItemStatus itemStatus =
                isEquipped ?
                InventoryItemStatus.Equipped :
                unlockedSurfboard.Contains(surfboardID) || surfboardID == Surfboard.Cool ?
                InventoryItemStatus.Unlocked : InventoryItemStatus.Locked;

            inventoryItem.SetItem(this, new UIInventoryItemData(RewardType.Surfboard, i), itemStatus);
            inventoryItem.name = "Surfboard " + surfboardID.ToString();

            inventoryItem.SetSelectedState(false);
            if (isEquipped)
            {
                currentEquippedSurfboard = inventoryItem;
            }
        }
    }

    public Sprite GetCachedItemStatusIcon(InventoryItemStatus itemStatus)
    {
        switch(itemStatus)
        {
            case InventoryItemStatus.Equipped:
                if(cachedEquippedIcon == null)
                {
                    cachedEquippedIcon = FMAssetFactory.GetIcon("Checkmark");
                }
                return cachedEquippedIcon;                
            case InventoryItemStatus.Locked:
                if (cachedLockedIcon == null)
                {
                    cachedLockedIcon = FMAssetFactory.GetIcon("Locked");
                }
                return cachedLockedIcon;
            default:
                return null;                
        }
    }

    public override void DoUpdate()
    {
        touchScreenController.DoUpdateInShop();

        float rotateValue = touchScreenController.GetRotate3DModelViewerValue();
        object3DModel.transform.Rotate(Vector3.up, rotateValue, Space.World);
    }

    public override void Hide()
    {
        topFrameHUD.ShowInputName(true);
    }

    public override void OnRefresh()
    {
    }



}
