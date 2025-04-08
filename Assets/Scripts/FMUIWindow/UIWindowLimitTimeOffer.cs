using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWindowLimitTimeOffer : VDUIWindow
{
    [SerializeField] Image imageThumbnail;
    [SerializeField] TextMeshProUGUI textTag;
    [SerializeField] TextMeshProUGUI textOfferName;
    [SerializeField] TextMeshProUGUI textDesc;
    [SerializeField] RectTransform root;
    [SerializeField] RectTransform tagDiscount;
    [SerializeField] RectTransform rootCostInfo;
    [SerializeField] FMButton buttonProceed;
    [SerializeField] FMButton buttonClose;
    List<CostInfoButton> costInfos;

    private UIQuickAction uiQuickAction;
    private UITopFrameHUD uiTopFrameHUD;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.MoveToParent();
        uiTopFrameHUD.ShowInputName(false);

        costInfos = new List<CostInfoButton>();
        buttonProceed.AddListener(OnProceedButtonPressed);
        buttonClose.AddListener(OnCloseButtonPressed);

        uiQuickAction.SetInteractableButton(false);
    }

    public override void DoUpdate()
    {

    }

    public override void Hide()
    {
        uiTopFrameHUD.ReturnToOriginalParent();
        uiTopFrameHUD.ShowInputName(true);
    }

    public override void OnRefresh()
    {

    }

    public void RefreshOffer(ShopData shopData)
    {
        List<CostData> costDatas = shopData.costTypes;
        bool enableDiscountTag = shopData.enableDiscount;
        string thumbnailNameString = shopData.thumbnailName;
        string textTagString = shopData.discountTag;
        string textTitleString = shopData.title.ToString();
        string textDesccriptionString = shopData.description.ToString();
        Sprite thumbnailSprite = FMShopController.Get.GetSpriteAtlasShop(thumbnailNameString);

        int count = costInfos.Count;
        for (int i = 0; i < count; i++)
        {
            CostInfoButton costInfo = costInfos[i];
            costInfo.gameObject.SetActive(false);
        }

        tagDiscount.gameObject.SetActive(enableDiscountTag);

        imageThumbnail.sprite = thumbnailSprite;
        textTag.SetText(textTagString);

        count = costDatas.Count;
        for (int i = 0; i < count; i++)
        {
            CostData costData = costDatas[i];
            bool isDiscount = costData.isDiscount;
            float discountData = costData.cost;
            float cost = isDiscount ? costData.discountCost : costData.cost;
            ShopCurrencyType shopCurrencyType = costData.shopCurrencyType;

            CostInfoButton costInfo = FMAssetFactory.GetCostInfoButton(rootCostInfo);
            costInfo.SetTextCost(cost, shopCurrencyType);
            costInfo.SetDiscountTextCost(discountData, shopCurrencyType);
            costInfo.EnableDiscount(isDiscount);
            costInfo.EnableButton(false);
            costInfo.EnableText(false);

            costInfos.Add(costInfo);
        }

        textOfferName.SetText(textTitleString);
        textDesc.SetText(textDesccriptionString);

        imageThumbnail.SetNativeSize();
        uiQuickAction.SetInteractableButton(false);
    }

    void OnProceedButtonPressed()
    {
        FMShopController.Get.ShowOffer = false;
        FMShopController.Get.OfferContinue = false;
        FMUIWindowController.Get.CloseAllWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Shop, uiQuickAction);
        uiTopFrameHUD.ReturnToOriginalParent();
        uiQuickAction.SetInteractableButton(true);
    }

    void OnCloseButtonPressed()
    {
        FMShopController.Get.ShowOffer = false;
        uiQuickAction.SetInteractableButton(true);
    }
}
