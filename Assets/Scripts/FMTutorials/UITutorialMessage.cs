using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITutorialMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMessage;
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform spaceImage;

    public void SetText(string inMessage)
    {
        textMessage.SetText(inMessage);
    }

    public void SetPosition(Vector2 position)
    {
        panel.localPosition = position;
        //textMessage.rectTransform.localPosition = position;
    }

    public void ShowSpaceImage(bool show)
    {
        spaceImage.gameObject.SetActive(show);
    }
}
