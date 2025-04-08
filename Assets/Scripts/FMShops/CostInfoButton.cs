using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostInfoButton : MonoBehaviour
{
    [SerializeField] private RectTransform rootCost;
    [SerializeField] private RectTransform rootText;
    [SerializeField] private Image buttonSprite;
    [SerializeField] private TextMeshProUGUI textButton;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private TextMeshProUGUI textDiscountCost;
    [SerializeField] private Image imageButton;
    [SerializeField] private FMButton buttonBuy;

    private int buttonId;
    private float cost;
    private string spriteTag;


    public void InitButton(int inButtonid, Action callback)
    {
        buttonId = inButtonid;
        buttonBuy.AddListener(callback);
    }

    public void EnableText(bool isEnable)
    {
        rootText.gameObject.SetActive(isEnable);
        rootCost.gameObject.SetActive(!isEnable);
    }

    public void SetText(string inText)
    {
        textButton.SetText(inText);
    }

    public void SetTextMaterial(bool useBlue)
    {
        Material fontMaterial = FMAssetFactory.GetShopPriceFontMaterial(useBlue);
        textButton.fontSharedMaterial = fontMaterial;
        textCost.fontSharedMaterial = fontMaterial;
        textDiscountCost.fontSharedMaterial = fontMaterial;
    }

    public void UpdateTextCost(float inCost)
    {
        cost = inCost;

        string costString = spriteTag + cost.ToString();
        textCost.SetText(costString);
    }

    public void SetTextCost(float inCost, ShopCurrencyType shopCurrencyType)
    {
        cost = inCost;
        spriteTag = GetSpriteTag(shopCurrencyType);

        string costString = spriteTag + cost.ToString();
        textCost.SetText(costString);
    }

    public void SetDiscountTextCost(float inDiscountCost, ShopCurrencyType shopCurrencyType)
    {
        cost = inDiscountCost;
        spriteTag = GetSpriteTag(shopCurrencyType);
        
        string costString = spriteTag + cost.ToString();
        textDiscountCost.SetText(costString);
    }

    public void EnableDiscount(bool isEnable)
    {
        textDiscountCost.gameObject.SetActive(isEnable);
    }

    public void EnableButton(bool isEnable)
    {
        imageButton.enabled = isEnable;
        buttonBuy.enabled = isEnable;
    }   

    public void SetButtonSprite(bool isOwned)
    {
        Material fontMaterial = FMAssetFactory.GetShopEquipFontMaterial(isOwned);
        textButton.fontSharedMaterial = fontMaterial;
        buttonSprite.sprite = FMAssetFactory.GetShopEquipSprite(isOwned);

    }

    private string GetSpriteTag(ShopCurrencyType shopCurrencyType)
    {
        string result = null;
        string spriteTagName = null;
        string spriteText = "<sprite name={0}>";
        switch (shopCurrencyType)
        {
            case ShopCurrencyType.Coin:
                spriteTagName = "coin";
                result = string.Format(spriteText, spriteTagName);
                break;
            case ShopCurrencyType.Gems:
                spriteTagName = "diamond";
                result = string.Format(spriteText, spriteTagName);
                break;
            case ShopCurrencyType.RealMoney:
                //todo: split real world currency
                spriteTagName = "$";
                result = spriteTagName;
                break;
        }

        return result;
    }
}
