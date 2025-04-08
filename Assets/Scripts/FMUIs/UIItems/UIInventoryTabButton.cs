using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIInventoryTabButton : MonoBehaviour
{
    private readonly Vector3 selectedLocalScale = new Vector3(1.2f, 1.2f, 1f);

    [SerializeField] private Image currentEquippedImage;
    [SerializeField] private FMButton buttonComponent;
    [SerializeField] private RectTransform rectTransform;

    public void AddListenerToButtonComponent(Action callback)
    {
        buttonComponent.AddListener(callback);
    }

    public void SetButtonSelectedState(bool isSelected)
    {
        rectTransform.localScale = isSelected ? selectedLocalScale : Vector3.one;
    }

    public void SetEquippedImage(UIInventoryItemData itemData)
    {
        switch (itemData.ItemType)
        {
            case RewardType.Costume:
                string costumeSpriteName = string.Format("Inventory_Costume_{0}", ((Costume)itemData.ItemID).ToString());
                Sprite costumeSprite = FMAssetFactory.GetInventoryItemSprite(costumeSpriteName);
                if (costumeSprite != null)
                {
                    currentEquippedImage.sprite = costumeSprite;
                }
                break;
            case RewardType.Surfboard:
                string surfboardSpriteName = string.Format("Inventory_Surfboard_{0}", ((Surfboard)itemData.ItemID).ToString());
                Sprite surfboardSprite = FMAssetFactory.GetInventoryItemSprite(surfboardSpriteName);
                if (surfboardSprite != null)
                {
                    currentEquippedImage.sprite = surfboardSprite;
                }
                break;
        }
    }
}
