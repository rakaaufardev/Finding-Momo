using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWindowSurfMission : VDUIWindow
{
    [SerializeField] private UIMissionPanel missionPanel;
    [SerializeField] private RectTransform rootContent;
    [SerializeField] private RectTransform rootButton;
    [SerializeField] private FMButton confirmButton;
    [SerializeField] private SnapScroller snapScroller;

    public override void Show(object[] dataContainer)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        missionPanel.SetMissionItems(FMMissionController.Get().SurfMissionProgressDictionary);

        int pageCount = missionPanel.GetGroupCount();
        List<GameObject> pages = missionPanel.GetGroups();
        //snapScroller.SetScroller(pageCount);
        //snapScroller.SetPages(pages);


#if UNITY_STANDALONE
        rootText.gameObject.SetActive(true);
#elif UNITY_ANDROID
        rootButton.gameObject.SetActive(true);
        confirmButton.AddListener(OnConfirm);
#endif
    }

    public override void Hide()
    {

    }

    public override void OnRefresh()
    {

    }

    public override void DoUpdate()
    {

    }

    public void OnTutorial()
    {
        rootContent.localPosition = new Vector3(0, 187, 0);
    }

    private void OnConfirm()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        SurfWorld surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
        surfWorld.SpawnAnimal();
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        FMSurfCharacter surfCharacter = currentWorld.GetCharacter() as FMSurfCharacter;
        FMSurfCharacterActionMachine characterActionMachine = surfCharacter.GetActionMachine() as FMSurfCharacterActionMachine;
        FMSurfCharacterActionMachine_Idle actionIdle = (characterActionMachine).GetCurrentAction() as FMSurfCharacterActionMachine_Idle;
        actionIdle.SetStateToRun();

        FMUIWindowController.Get.CloseWindow();
    }
}
