using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowRewardAlert : VDUIWindow
{
    [SerializeField] private RectTransform rootContent;
    [SerializeField] private FMButton closeButton;


    public override void Hide()
    {
    }

    /// <summary>
    /// data passed to this method should be of type UIRewardAlertItemData
    /// </summary>
    /// <param name="dataContainer"></param>
    public override void Show(params object[] dataContainer)
    {
        int itemCount = dataContainer.Length;
        for(int i = 0; i < itemCount; i++)
        {
            UIRewardAlertItemData data;

            //Guard clause if data in container isn't of type UIRewardAlertItemData
            if (dataContainer[i] is UIRewardAlertItemData)            
                data = (UIRewardAlertItemData)dataContainer[i];            
            else
                continue;            

            UIRewardAlertItem alertItem = FMAssetFactory.GetRewardAlertItem(rootContent);
            alertItem.SetItem(data);
        }

        closeButton.AddListener(OnClickClose);
    }

    public override void DoUpdate()
    {
    }
    public override void OnRefresh()
    {
    }

    private void OnClickClose()
    {
        FMUIWindowController.Get.CloseWindow();
    }
}

