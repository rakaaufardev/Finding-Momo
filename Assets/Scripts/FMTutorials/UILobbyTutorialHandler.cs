using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VD;

public class UILobbyTutorialHandler : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform rootTextbox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private FMButton continueButton;
    [SerializeField] private FMButton skipButton;

    public void SetActive(bool isShow)
    {
        root.gameObject.SetActive(isShow);
    }

    public void SetTextboxActive(bool isShow)
    {
        rootTextbox.gameObject.SetActive(isShow);
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void AddContinueButtonListener(Action callback)
    {
        continueButton.AddListener(() => callback());
    }

    public void AddSkipButtonListener(Action callback)
    {
        skipButton.AddListener(() => callback());
    }

    public void SetButtonInteractable(bool isInteractable)
    {
        continueButton.interactable = isInteractable;
    }

    public void HideSkipButton()
    {
        skipButton.gameObject.SetActive(false);
    }
}
