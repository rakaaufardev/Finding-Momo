using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelMapButton : MonoBehaviour
{
    [SerializeField] private FMButton button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private RectTransform status;
    [SerializeField] private RectTransform overlay;
    [SerializeField] private KoreaCity koreaCity;
    private int buttonID;

    private string blue_color = "#2BE1FF";
    private string yellow_color = "#FFCE24";
    private string purple_color = "#E4A4FF";

    public RectTransform RectTransform
    {
        get => rectTransform;
    }

    public void InitButton(int inButtonID, Action<int, KoreaCity> callback)
    {
        buttonID = inButtonID;
        button.AddListener(() => callback(buttonID, koreaCity));
    }

    public void EnableButton(bool isEnable)
    {
        status.gameObject.SetActive(!isEnable);
        overlay.gameObject.SetActive(!isEnable);
    }

    public void SetText(string inText)
    {
        buttonText.SetText(inText);
    }

    public void SetKoreaCity(KoreaCity city)
    {
        koreaCity = city;
    }

    // TODO: ERASE AFTER GET UI ASSET
    public void SetColor()
    {
        string hexColor = null;
        int modulo = buttonID % 3;

        switch (modulo)
        {
            case 0:
                hexColor = blue_color;
                break;
            case 1:
                hexColor = yellow_color;
                break;
            case 2:
                hexColor = purple_color;
                break;
        }

        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            buttonImage.color = newColor;
        }
    }
}
