using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITopFrameHUD : MonoBehaviour
{
    [SerializeField] private RectTransform rootParent;
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform rootInputName;
    [SerializeField] private RectTransform rootCoinHUD;
    [SerializeField] private RectTransform rootDiamondHUD;
    [SerializeField] private Image flagImage;
    [SerializeField] private TextMeshProUGUI textCoin;
    [SerializeField] private TextMeshProUGUI textDiamond;
    [SerializeField] private TextMeshProUGUI currentPlayerNameText;
    [SerializeField] private FMButton buttonInputName;
    [SerializeField] private FMButton buttonSetting;
    [SerializeField] private FMButton buttonClose;

    private UILobby uiLobby;
    private UIQuickAction uiQuickAction;
    private RectTransform lobbyParent;

    public void Init(UIQuickAction inUiQuickAction)
    {
        LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
        uiLobby = lobbyScene.GetUI();
        uiQuickAction = inUiQuickAction;

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        UpdateCoinText(inventoryData.coin);
        UpdateDiamondText(inventoryData.gems);

        buttonSetting.onClick.RemoveAllListeners();
        buttonClose.onClick.RemoveAllListeners();
        buttonInputName.AddListener(OnClickInputName);
        buttonSetting.AddListener(OnButtonSettingPressed);
        buttonClose.AddListener(OnButtonClosePressed);

        SetPlayerName();
    }

    public void UpdateCoinText(int coin)
    {
        textCoin.SetText(coin.ToString());
    }
    public void UpdateDiamondText(int diamond)
    {
        textDiamond.SetText(diamond.ToString());
    }

    public void Show(bool isShow)
    {
        content.gameObject.SetActive(isShow);
    }

    public void ShowInputName(bool isShow)
    {
        rootInputName.gameObject.SetActive(isShow);
    }

    public void ShowButtons(bool isShow)
    {
        buttonSetting.gameObject.SetActive(isShow);
        buttonClose.gameObject.SetActive(isShow);
    }

    public void ShowCurrency(bool isShow)
    {
        rootCoinHUD.gameObject.SetActive(isShow);
        rootDiamondHUD.gameObject.SetActive(isShow);
    }

    private void OnClickInputName()
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.InputName, uiQuickAction);
        UIWindowInputName popup = FMUIWindowController.Get.CurrentWindow as UIWindowInputName;
        popup.ImmediateShowFlag();
    }

    private void OnButtonSettingPressed()
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.Setting, uiQuickAction);
        ShowInputName(false);
    }

    private void OnButtonClosePressed()
    {
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Main)
        {
            FMUIWindowController.Get.CloseCurrentWindow();
            Show(false);
            uiQuickAction.Hide();
            
        }
        else if(sceneState == SceneState.Lobby) 
        {
            if (FMUIWindowController.Get.CurrentWindow == null)
            {
                Application.Quit();
            }
            else
            {
                FMUIWindowController.Get.CloseCurrentWindow();
                uiQuickAction.OnCloseWindow(false);
            }
        }
    }

    public void SetPlayerName()
    {
        bool isPlayerNameExist = FMUserDataService.Get().IsPlayerNameExist();
        if (isPlayerNameExist)
        {
            UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
            currentPlayerNameText.text = userInfo.currentPlayerName;
            string playerFlag = userInfo.currentFlag;
            flagImage.sprite = FMAssetFactory.GetFlag(playerFlag);
        }
    }

    public void MoveToParent()
    {
        lobbyParent = uiLobby.RootTopFrameHUD;
        root.SetParent(lobbyParent);
    }

    public void ReturnToOriginalParent()
    {
        root.SetParent(rootParent);
    }
}
