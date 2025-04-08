using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlagItem : MonoBehaviour
{
    [SerializeField] private FMButton buttonFlag;
    [SerializeField] private Image imageFlag;
    private string chosenFlag;

    public void SetItem(Sprite sprite, string flagName)
    {
        imageFlag.sprite = sprite;
        chosenFlag = flagName;
        buttonFlag.AddListener(OnClickFlag);
    }

    void OnClickFlag()
    {
        LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
        UILobby uiLobby = lobbyScene.GetUI();
        uiLobby.CacheFlagChosen = chosenFlag;
        FMUIWindowController.Get.CloseWindow();
    }
}
