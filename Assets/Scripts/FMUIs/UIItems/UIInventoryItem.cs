using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    private readonly Color borderNormalColor = new Color(1f, 1f, 1f);
    private readonly Color borderSelectedColor = new Color(0.9882354f, 1f, 0.01176471f);
    private readonly Color borderLockedColor = new Color(0.2f, 0.2f, 0.2f);

    private readonly Color imageNormalColor = new Color(1f, 1f, 1f);
    private readonly Color imageLockedColor = new Color(0.3f, 0.3f, 0.3f);

    [SerializeField] private Image selectedGlowEffect;
    [SerializeField] private Image statusIcon;
    [SerializeField] private Image border;
    [SerializeField] private Image imageItem;
    [SerializeField] private FMButton buttonComponent;

    private UIInventoryItemData itemData;
    private UIWindowInventory windowInventory;    

    public InventoryItemStatus ItemStatus { get; private set; }    

    public void SetItem(UIWindowInventory windowInventory, UIInventoryItemData itemData, InventoryItemStatus itemStatus)
    {
        this.windowInventory = windowInventory;
        this.itemData = itemData;
        ItemStatus = itemStatus;
        buttonComponent.AddListener(OnSelectItem);

        switch (itemData.ItemType)
        {
            case RewardType.Costume:
                string costumeSpriteName = string.Format("Inventory_Costume_{0}", ((Costume)itemData.ItemID).ToString());
                Sprite costumeSprite = FMAssetFactory.GetInventoryItemSprite(costumeSpriteName);
                if (costumeSprite != null)
                {
                    imageItem.sprite = costumeSprite;
                }
                break;
            case RewardType.Surfboard:
                string surfboardSpriteName = string.Format("Inventory_Surfboard_{0}", ((Surfboard)itemData.ItemID).ToString());
                Sprite surfboardSprite = FMAssetFactory.GetInventoryItemSprite(surfboardSpriteName);
                if (surfboardSprite != null)
                {
                    imageItem.sprite = surfboardSprite;
                }
                break;
            default:
                break;
        }
    }

    private void OnSelectItem()
    {        
        windowInventory.OnItemSelected(this);
    }

    public void SetSelectedState(bool isSelected)
    {
        selectedGlowEffect.enabled = isSelected;
        UpdateBorder(isSelected);
        UpdateImageColor();
        UpdateStatusIcon();
    }

    public void UpdateBorder(bool isSelected)
    {
        if (isSelected)
        {
            border.color = borderSelectedColor;
        }
        else if (ItemStatus == InventoryItemStatus.Locked)
        {
            border.color = borderLockedColor;
        }
        else
        {
            border.color = borderNormalColor;
        }
    }

    public void UpdateImageColor()
    {
        if (ItemStatus == InventoryItemStatus.Locked)
        {
            imageItem.color = imageLockedColor;
            statusIcon.color = imageLockedColor;
        }
        else
        {
            imageItem.color = imageNormalColor;
            statusIcon.color = imageNormalColor;
        }
    }

    public void UpdateStatusIcon()
    {
        Sprite icon = windowInventory.GetCachedItemStatusIcon(ItemStatus);
        statusIcon.sprite = icon;
        statusIcon.enabled = icon != null;
    }

    public void SetStatus(InventoryItemStatus status)
    {
        ItemStatus = status;
        UpdateStatusIcon();
    }

    public UIInventoryItemData GetData()
    {
        return itemData;
    }    

}

public enum InventoryItemStatus
{
    Locked,
    Unlocked,
    Equipped
}

public struct UIInventoryItemData
{
    public RewardType ItemType { get; private set; }
    public int ItemID { get; private set; }

    public UIInventoryItemData(RewardType itemType, int itemID)
    {
        ItemType = itemType;
        ItemID = itemID;
    }
}
