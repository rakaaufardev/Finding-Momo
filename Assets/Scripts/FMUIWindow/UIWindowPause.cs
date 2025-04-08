using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWindowPause : VDUIWindow
{
    [Header("Content Main")]
    [SerializeField] private RectTransform rootContentMainGame;
    [SerializeField] private RectTransform rootButtonsMainGame;
    [SerializeField] private UIVolumeSetting volumeSetting;
    [SerializeField] private UIApplicationVersion applicationVersion;
    
    [Header("Content Mini Game")]
    [SerializeField] private RectTransform rootContentMiniGame;
    [SerializeField] private RectTransform rootButtonsMiniGame;
    [SerializeField] private UIMissionPanel missionPanel;
    [SerializeField] private SnapScroller snapScroller;

    [Header("Buttons")]
    [SerializeField] private RectTransform rootButtons;
    [SerializeField] private FMButton buttonHome;
    [SerializeField] private FMButton buttonContinue;
    [SerializeField] private FMButton buttonSetting;

    private FMMainScene mainScene;
    private UIQuickAction uiQuickAction;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];

        mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        WorldType worldType = mainScene.GetCurrentWorldType();

        bool isMainGame = worldType == WorldType.Main;
        bool isMiniGame = worldType == WorldType.Surf;
        rootContentMainGame.gameObject.SetActive(isMainGame);
        rootContentMiniGame.gameObject.SetActive(isMiniGame);

        switch (worldType)
        {
            case WorldType.Main:
                rootButtons.SetParent(rootButtonsMainGame);
                volumeSetting.Init(uiQuickAction);
                applicationVersion.Init();
                break;
            case WorldType.Surf:
                rootButtons.SetParent(rootButtonsMiniGame);
                missionPanel.SetMissionItems(FMMissionController.Get().SurfMissionProgressDictionary);
                volumeSetting.Init(uiQuickAction);
                int pageCount = missionPanel.GetGroupCount();
                List<GameObject> pages = missionPanel.GetGroups();
                //snapScroller.SetScroller(pageCount);
                //snapScroller.SetPages(pages);
                break;
        }

        rootButtons.localPosition = Vector3.zero;
        buttonHome.AddListener(OnClickHome);
        buttonContinue.AddListener(OnClickContinue);
        //buttonSetting.AddListener(OnClickSetting);
    }

    public override void Hide()
    {
        volumeSetting.SaveVolume();
    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }

    private void OnClickHome()
    {
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        currentWorld.ResetColliderObjects();

        FMSceneController.Get().SaveDisplayObjectToStorage();
        FMSceneController.Get().OnBackToLobby();
    }

    private void OnClickContinue()
    {
        FMWorld world = mainScene.GetCurrentWorldObject();
        world.PlayGame();
        FMUIWindowController.Get.CloseWindow();
    }
}
