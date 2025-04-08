using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWindowSurfPause : VDUIWindow
{
    [SerializeField] private List<TextMeshProUGUI> missionDescription;
    [SerializeField] private List<TextMeshProUGUI> missionReward;
    [SerializeField] private List<TextMeshProUGUI> missionTextProgress;
    [SerializeField] private List<Slider> missionBarProgress;
    [SerializeField] private FMButton buttonHome;
    [SerializeField] private FMButton buttonContinue;

    private UIQuickAction uiQuickAction;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];

        buttonHome.AddListener(OnClickHome);
        buttonContinue.AddListener(OnClickContinue);

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        int count = missionDescription.Count;
        List<KeyValuePair<string, SurfMissionProgressData>> missionList = FMMissionController.Get().SurfMissionProgressDictionary.ToList();
        
        for (int i = 0; i < count && i < missionList.Count; i++)
        {
            int index = i;

            var missionData = missionList[index];

            missionDescription[index].text = missionData.Value.missionDescription;
            missionReward[index].text = "Clear Reward\n" + missionData.Value.rewardScoreValue.ToString();

            missionBarProgress[index].maxValue = missionData.Value.goalValue;
            missionBarProgress[index].value = missionData.Value.goalProgress;

            bool isCompleted = Mathf.Approximately(missionBarProgress[index].value, missionBarProgress[index].maxValue);

            if (isCompleted)
            {
                missionTextProgress[index].text = "Completed";
            }
            else
            {
                if (missionData.Value.missionType == SurfMissionType.TimePlayed || missionData.Value.missionType == SurfMissionType.AirTime)
                {
                    missionTextProgress[index].text = missionData.Value.goalProgress.ToString("F1") + "s / " + missionData.Value.goalValue.ToString("F1") + "s";
                }
                else if (missionData.Value.missionType == SurfMissionType.SurfDistance)
                {
                    missionTextProgress[index].text = missionData.Value.goalProgress.ToString("F0") + "m / " + missionData.Value.goalValue.ToString("F0") + "m";
                }
                else
                {
                    missionTextProgress[index].text = missionData.Value.goalProgress.ToString("F0") + " / " + missionData.Value.goalValue.ToString("F0");
                }
            }
        }
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

    void OnClickHome()
    {
        FMUIWindowController.Get.CloseWindow();
        FMSceneController.Get().ChangeScene(SceneState.Lobby, uiQuickAction);
    }

    void OnClickContinue()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        SurfWorld surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
        surfWorld.PlayGame();
        FMUIWindowController.Get.CloseWindow();
    }

    /*public void DoUpdate()
    {
        int count = mission.Count;
        var collectMissionArray = missionController.MissionProgressDictionary.Values.ToArray();

        for (int i = 0; i < count; i++)
        {
            int index = i;
            var missionBar = collectMissionArray[i];
            missionProgress[index].value = missionBar.goalProgress;
        }
    }*/
}
