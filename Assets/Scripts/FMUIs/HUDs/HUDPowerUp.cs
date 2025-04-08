using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HUDPowerUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountItemText;
    [SerializeField] private Image itemImageIcon;
    [SerializeField] private FMButton itemButton;

    public void SetPowerUpItem(float amountItem, Sprite itemImage, Action onButtonClick)
    {
        itemImageIcon.sprite = itemImage;
        SetItemAmount(amountItem);
        itemButton.interactable = amountItem > 0;
        itemButton.AddListener(onButtonClick.Invoke);
    }


    public void SetItemAmount(float amountItem)
    {
        amountItemText.text = "x" + amountItem.ToString();
        itemButton.interactable = amountItem > 0;
    }
}
