using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIDailyLoginItem : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform rootChecklist;
    [SerializeField] private TextMeshProUGUI textDay;
    [SerializeField] private TextMeshProUGUI textAmount;
    [SerializeField] private Image imageIcon;
    [SerializeField] private Image imageItem;
    [SerializeField] private FMButton button;

    public RectTransform Root
    {
        get
        {
            return root;
        }
    }

    public void SetButtonCallback(Action callback)
    {
        button.AddListener(callback);
    }

    public void SetItem(int day, int amount, Sprite icon)
    {
        string dayString = string.Format("Day {0}", day);
        string amountString = amount.ToString();

        textDay.SetText(dayString);
        textAmount.SetText(amountString);
        imageIcon.sprite = icon;
    }

    public void SetButtonState(Sprite sprite, string hexColor, DailyLoginState state)
    {
        ColorUtility.TryParseHtmlString(hexColor, out Color color);

        textDay.color = color;
        textAmount.color = color;

        rootChecklist.gameObject.SetActive(state == DailyLoginState.Claimed);
        imageItem.sprite = sprite;
    }

    public void PlayClaim(Action endCallback)
    {
        rootChecklist.gameObject.SetActive(true);
        Vector2 punchScaleTarget = new Vector2(-0.75f, -0.75f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rootChecklist.DOPunchScale(punchScaleTarget, 0.5f,1));
        sequence.AppendInterval(1f);
        sequence.AppendCallback(()=> 
        {
            endCallback.Invoke();
        });
    }

    public void DisableButton()
    {
        button.interactable = false;
    }
}
