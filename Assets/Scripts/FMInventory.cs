using UnityEngine;

public class FMInventory : VDSingleton<FMInventory>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SetCharacterCostume(Costume costumeEquiped)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        Costume costumeInUse = costumeEquiped;
        inventoryData.inUsedCostume = costumeEquiped; // Save the equipped costume
        FMUserDataService.Get().SaveInventoryData(inventoryData);
    }

    public void SetCharacterSurfboard(Surfboard surfboardEquiped)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        inventoryData.inUsedSurfboard = surfboardEquiped;
        FMUserDataService.Get().SaveInventoryData(inventoryData);
    }

    public void AddProductIdRecord(string productId)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        bool alreadyPurchase = inventoryData.productIds.Contains(productId);
        if (!alreadyPurchase)
        {
            inventoryData.productIds.Add(productId);
        }

        FMUserDataService.Get().SaveInventoryData(inventoryData);
    }

    public bool IsProductPurchased(string productId)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        bool result = inventoryData.productIds.Contains(productId);
        return result;
    }

    public void AddInventory(RewardType rewardType, int value, int amount)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        switch (rewardType)
        {
            case RewardType.Costume:
                Costume costume = (Costume)value;
                inventoryData.costumes.Add(costume);
                break;

            case RewardType.Surfboard:
                Surfboard surfboard = (Surfboard)value;
                inventoryData.surfboards.Add(surfboard);
                break;

            case RewardType.Item:
                ItemType itemType = (ItemType)value;
                ItemData itemData = inventoryData.GetItemData(itemType);

                if (itemData != null)
                {
                    itemData.amount = amount;
                }
                else
                {
                    ItemData newItemData = new ItemData()
                    {
                        itemType = itemType,
                        amount = amount,
                    };
                    inventoryData.items.Add(newItemData);
                }
                break;

            case RewardType.Coin:
                inventoryData.coin += amount;
                break;

            case RewardType.Gems:
                inventoryData.gems += amount;
                break;
        }

        FMUserDataService.Get().SaveInventoryData(inventoryData);
    }

}
