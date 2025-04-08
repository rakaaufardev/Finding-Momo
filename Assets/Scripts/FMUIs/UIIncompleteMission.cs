using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VD;

public class UIIncompleteMission : MonoBehaviour
{
    [SerializeField] private RectTransform rootMission;

    [SerializeField] private List <FMButton> buttonGoToMissionList;

    [SerializeField] private List<UIMissionItem> missionItem;
    [SerializeField] private List<TextMeshProUGUI> missionDescription;

    private UIQuickAction uiQuickAction;

    private List<SurfMissionProgressData> incompleteMissions;
    private int firstIndex;
    private int secondIndex;
    private int thirdIndex;

    private string directionMovement;
    private const string moveUp = "Up";
    private const string moveDown = "Down";

    public void Init(UIQuickAction inUiQuickAction)
    {
        uiQuickAction = inUiQuickAction;

        for (int i = 0; i < buttonGoToMissionList.Count; i++)
        {
            buttonGoToMissionList[i].AddListener(OnClickMission);
        }
        //buttonScrollUp.AddListener(OnClickUp);
        //buttonScrollDown.AddListener(OnClickDown);

        firstIndex = 0;
        secondIndex = 1;
        thirdIndex = 2;

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        Dictionary<string, SurfMissionProgressData> missions = FMMissionController.Get().PermanentProgressDictionary;
        incompleteMissions = new List<SurfMissionProgressData>();

        int count = missions.Count;

        foreach (var mission in missions.Values)
        {
            bool isCompleted = mission.goalProgress == mission.goalValue;

            if (!isCompleted)
            {
                incompleteMissions.Add(mission);
            }
        }

        ShowIncompleteMissions();
    }

    private void OnClickMission()
    {
        uiQuickAction.ShowSelectedButton(UIWindowType.MissionList);
        FMUIWindowController.Get.OpenWindow(UIWindowType.MissionList);
    }

    private void OnClickUp()
    {
        directionMovement = moveUp;
        ShowIncompleteMissions();
    }
    
    private void OnClickDown()
    {
        directionMovement = moveDown;
        ShowIncompleteMissions();
    }

    private void ShiftMissionIndexData()
    {
        switch (directionMovement)
        {
            case moveUp:
                firstIndex += 1;
                secondIndex += 1;

                if (firstIndex > incompleteMissions.Count - 1)
                {
                    firstIndex = 0;
                }

                if (secondIndex > incompleteMissions.Count - 1)
                {
                    secondIndex = 0;
                }

                break;
            case moveDown:
                firstIndex -= 1;
                secondIndex -= 1;

                if (firstIndex < 0)
                {
                    firstIndex = incompleteMissions.Count - 1;
                }

                if (secondIndex < 0)
                {
                    secondIndex = incompleteMissions.Count - 1;
                }

                break;
        }
    }

    private void ShowIncompleteMissions()
    {
        int missionRemain = incompleteMissions.Count;

        if (missionRemain == 3)
        {
            SurfMissionProgressData firstMission = incompleteMissions[firstIndex];
            SurfMissionProgressData secondMission = incompleteMissions[secondIndex];
            SurfMissionProgressData thirdMission = incompleteMissions[thirdIndex];
            missionDescription[0].text = firstMission.missionDescription;
            missionDescription[1].text = secondMission.missionDescription;
            missionDescription[2].text = thirdMission.missionDescription;
        }
        else if (missionRemain == 2)
        {
            ShiftMissionIndexData();

            SurfMissionProgressData firstMission = incompleteMissions[firstIndex];
            SurfMissionProgressData secondMission = incompleteMissions[secondIndex];

            missionDescription[0].text = firstMission.missionDescription;
            missionDescription[1].text = secondMission.missionDescription;
            missionItem[2].gameObject.SetActive(false);
        }
        else if (missionRemain == 1)
        {
            SurfMissionProgressData firstMission = incompleteMissions[firstIndex];

            missionDescription[0].text = firstMission.missionDescription;
            missionItem[1].gameObject.SetActive(false);
            missionItem[2].gameObject.SetActive(false);
        }
        else
        {
            rootMission.gameObject.SetActive(false);
        }
    }
}
