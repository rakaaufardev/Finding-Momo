using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VD;

public class UIMissionPanel : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    private List<UIMissionItem> missionItems;
    private List<GameObject> groups;

    private const int MAX_CONTENT = 3;

    public void SetMissionItems(Dictionary<string,SurfMissionProgressData> dataContainer)
    {
        missionItems = new List<UIMissionItem>();
        groups = new List<GameObject>();

        int contentCount = 0;
        int count = dataContainer.Count;
        RectTransform rootMissionContent = null;
        for (int i = 0; i < count; i++)
        {
            if (contentCount <= 0)
            {
                contentCount = MAX_CONTENT;
                GameObject groupContent = FMAssetFactory.GetMissionGroup();
                groupContent.transform.SetParent(root);
                groupContent.transform.localScale = Vector3.one;
                rootMissionContent = groupContent.GetComponent<RectTransform>();

                groups.Add(groupContent);
            }

            contentCount--;

            UIMissionItem missionItem = null;
            SurfMissionProgressData data = dataContainer.GetValue(i);
            if (missionItems.Count >= dataContainer.Count)
            {
                missionItem = missionItems[i];
            }
            else
            {
                missionItem = FMAssetFactory.GetUISurfMissionItemVertical(rootMissionContent);
                VDUIWindow parentWindow = GetComponentInParent<VDUIWindow>();
                missionItem.Init(null, null, i, parentWindow);
                missionItem.SetImage(null,null, false);
                missionItems.Add(missionItem);
            }


            if (missionItem != null)
            {
                missionItem.SetMissionDetails(data);          
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public List<GameObject> GetGroups()
    {
        return groups;
    }

    public int GetGroupCount()
    {
        int result = groups.Count;
        return result;
    }
}
