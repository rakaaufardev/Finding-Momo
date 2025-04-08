using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowChooseFlag : VDUIWindow
{
    [SerializeField] private List<GameObject> rootFlagContents;
    [SerializeField] private SnapScroller snapScroller;
    [SerializeField] private RectTransform rootContent;
    [SerializeField] private FMButton buttonBack;
    UITopFrameHUD topFrameHUD;
    UITitleSandbox titleSandbox;

    const int MAX_FLAG_CONTENT = 18;

    public override void Show(object[] dataContainer)
    {
        int currentRootIndex = 0;
        GameObject rootFlagContent = rootFlagContents[currentRootIndex];
        int flagCount = 0;
        int maxPage = rootFlagContents.Count;
        int count = VDParameter.FLAG_NAMES.Length;
        for (int i = 0; i < count; i++)
        {
            string flagName = VDParameter.FLAG_NAMES[i];

            UIFlagItem uiFlagItem = FMAssetFactory.GetUIFlagItem(rootFlagContent.transform);
            Sprite spriteFlag = FMAssetFactory.GetFlag(flagName);
            uiFlagItem.SetItem(spriteFlag,flagName);

            flagCount++;
            if (flagCount >= MAX_FLAG_CONTENT)
            {
                flagCount = 0;
                currentRootIndex++;

                if (currentRootIndex < maxPage) 
                {
                    rootFlagContent = rootFlagContents[currentRootIndex];
                }
            }
        }

        topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.Show(false);

        if (titleSandbox == null)
        {
            titleSandbox = FindFirstObjectByType<UITitleSandbox>();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootContent);

        snapScroller.SetScroller(maxPage);
        snapScroller.SetPages(rootFlagContents);
        
        buttonBack.AddListener(OnClickBack);

    }

    public override void Hide()
    {
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();

        if (sceneState == SceneState.Lobby)
        {
            LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
            UILobby uiLobby = lobbyScene.GetUI();
            topFrameHUD.Show(false);
        }

        if(titleSandbox !=null)
        {
            titleSandbox.ModifyTitleText(VDParameter.TITLE_CHANGE_NAME);
        }
    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }

    void OnClickBack()
    {
        FMUIWindowController.Get.CloseWindow();
    }
}
