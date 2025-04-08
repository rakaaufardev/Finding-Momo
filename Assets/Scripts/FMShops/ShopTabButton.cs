using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VD;


public class ShopTabButton : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private TextMeshProUGUI textButton;
    [SerializeField] private Image imageButton;
    [SerializeField] private FMButton buttonTab;

    private string buttonId;

    public string ButtonId
    {
        get
        {
            return buttonId;
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

    public void SetButtonId(string inButtonId)
    {
        buttonId = inButtonId;
    }

    public void SetButton(string tabName, Action callback)
    {
        textButton.SetText(tabName);
        buttonTab.AddListener(callback);
    }

    public void UpdateButtonState(bool isActive)
    {
        root.localScale = isActive ? new Vector2(1.1f, 1.1f) : Vector2.one;
    }

    public void Refresh()
    {
        root.localScale = Vector3.one;
    }
}
