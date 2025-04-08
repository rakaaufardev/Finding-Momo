using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UITitleSandbox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private RectTransform rootPositionName;
    [SerializeField] private RectTransform rootPositionFlag;

    public void ModifyTitleText(string text, bool isSnapping = false)
    {
        titleText.text = text;

        if (!isSnapping)
        {
            if (text == VDParameter.TITLE_CHANGE_NAME)
            {
                transform.DOLocalMoveY(rootPositionName.anchoredPosition.y, 0.5f);
            }
            else if (text == VDParameter.TITLE_CHANGE_FLAG)
            {
                transform.DOLocalMoveY(rootPositionFlag.anchoredPosition.y, 0.5f);
            }
        }
        else
        {
            if (text == VDParameter.TITLE_CHANGE_NAME)
            {
                transform.localPosition = rootPositionName.anchoredPosition;
            }
            else if (text == VDParameter.TITLE_CHANGE_FLAG)
            {
                transform.localPosition = rootPositionFlag.anchoredPosition;
            }
        }
    }
}
