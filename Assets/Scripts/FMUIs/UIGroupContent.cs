using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGroupContent : MonoBehaviour
{
    [SerializeField] RectTransform root;
    [SerializeField] LayoutGroup layoutGroup;

    string tabId;

    public string TabId
    {
        get
        {
            return tabId;
        }
    }

    public RectTransform Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }

    public void SetTabId(string inId)
    {
        tabId = inId;
    }

    public LayoutGroup LayoutGroup
    {
        get
        {
            return layoutGroup;
        }
        set
        {
            layoutGroup = value;
        }
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
