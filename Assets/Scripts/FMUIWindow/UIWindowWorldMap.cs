using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using VD;

public class UIWindowWorldMap : VDUIWindow
{
    [Header("Content")]
    [SerializeField] private RectTransform worldMapContainer;
    [SerializeField] private RectTransform mainWorldMap;
    [SerializeField] private RectTransform leftWorldMap;
    [SerializeField] private RectTransform rightWorldMap;
    [SerializeField] private List<UIWorldMapLandmark> buttonsNations;
    [SerializeField] private List<RectTransform> cloudsLocation;
    private List<RectTransform> nationsLocation;
    private float mapWidth;

    private Vector2 previousMousePosition;
    private bool isDragging;

    private Vector2 originalPosition;
    private float originalScale;
    private bool isZoomed;

    [Header("Tutorial")]
    [SerializeField] private UILobbyTutorialHandler lobbyTutorialHandler;
    [SerializeField] private List<RectTransform> closeUpImages;
    [SerializeField] private RectTransform arrow;
    [SerializeField] private int tutorialCount;
    private bool isTutorialFinished;
    private int dialogueIndex;
    private Vector2 arrowStartPosition;
    private Tween arrowTween;
    private Sequence imageSequence;

    private string dialogue_1 = "The Earth is currently polluted due to excessive waste and needs our help!";
    private string dialogue_2 = "Save the Earth through plogging!";
    private string dialogue_3 = "Let's begin in Korea!\nTap where the arrow is pointing!";

    private UITopFrameHUD uiTopFrameHUD;
    private UIQuickAction uiQuickAction;
    private FMTouchScreenController touchScreenController = new();

    public override void Show(params object[] dataContainer)
    {
        uiQuickAction = FMSceneController.Get().GetUIQuickAction;
        uiQuickAction.Hide();

        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.ShowInputName(false);
        uiTopFrameHUD.ShowCurrency(false);

        isTutorialFinished = FMUserDataService.Get().IsTutorialFinished();

        nationsLocation = new List<RectTransform>();

        originalPosition = worldMapContainer.anchoredPosition;
        originalScale = worldMapContainer.localScale.x;

        mapWidth = mainWorldMap.rect.width;
        leftWorldMap.anchoredPosition = mainWorldMap.anchoredPosition - new Vector2(mapWidth, 0);
        rightWorldMap.anchoredPosition = mainWorldMap.anchoredPosition + new Vector2(mapWidth, 0);

        int count = buttonsNations.Count;
        for (int i = 0; i < count; i++)
        {
            buttonsNations[i].SetName();

            RectTransform locationNation = buttonsNations[i].GetComponent<RectTransform>();
            nationsLocation.Add(locationNation);

            buttonsNations[i].SetLandmark();
            // TODO: CHANGE TO DICTIONARY FOR EASY READABILITY(?)
            if (isTutorialFinished) buttonsNations[i].Button.AddListener(() => ZoomToNation(locationNation)); // ALL NATIONS BUTTON
        }

        // TUTORIAL DIALOGUE TEXTBOX
        if (!isTutorialFinished)
        {
            // TODO: CHANGE TO DICTIONARY FOR EASY READABILITY(?)
            buttonsNations[0].Button.AddListener(() => ZoomToNation(nationsLocation[0])); // KOREA BUTTON

            uiTopFrameHUD.Show(false);

            dialogueIndex = 0;
            lobbyTutorialHandler.gameObject.SetActive(true);
            lobbyTutorialHandler.SetDialogueText(dialogue_1);
            lobbyTutorialHandler.AddContinueButtonListener(OnClickNext);
            lobbyTutorialHandler.AddSkipButtonListener(OnClickSkip);
        }
    }

    public override void DoUpdate()
    {
        if (touchScreenController.TouchScreen(TouchType.Down))
        {
            isDragging = true;
            previousMousePosition = Input.mousePosition;
        }

        if (touchScreenController.TouchScreen(TouchType.Up))
        {
            isDragging = false;
        }

        if (isDragging && !isZoomed && !lobbyTutorialHandler.gameObject.activeInHierarchy && !arrow.gameObject.activeInHierarchy)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            float deltaX = (currentMousePosition.x - previousMousePosition.x);

            mainWorldMap.anchoredPosition += new Vector2(deltaX, 0);
            leftWorldMap.anchoredPosition += new Vector2(deltaX, 0);
            rightWorldMap.anchoredPosition += new Vector2(deltaX, 0);

            int nationCount = nationsLocation.Count;
            for (int i = 0; i < nationCount; i++)
            {
                nationsLocation[i].anchoredPosition += new Vector2(deltaX, 0);
            }

            int cloudCount = cloudsLocation.Count;
            for (int i = 0; i < cloudCount; i++)
            {
                cloudsLocation[i].anchoredPosition += new Vector2(deltaX, 0);
            }

            previousMousePosition = currentMousePosition;
        }

        WrapWorldMap(mainWorldMap);
        WrapWorldMap(leftWorldMap);
        WrapWorldMap(rightWorldMap);
        WrapButtons(nationsLocation);
        WrapButtons(cloudsLocation);
    }

    public override void Hide()
    {
        uiTopFrameHUD.ShowInputName(true);
        uiTopFrameHUD.ShowCurrency(true);
        uiQuickAction.Show();
    }

    public override void OnRefresh()
    {

    }

    private void WrapWorldMap(RectTransform worldMap)
    {
        if (worldMap.anchoredPosition.x <= -mapWidth * 1.5f)
        {
            worldMap.anchoredPosition += new Vector2(mapWidth * 3, 0);
        }
        else if (worldMap.anchoredPosition.x >= mapWidth * 1.5f)
        {
            worldMap.anchoredPosition -= new Vector2(mapWidth * 3, 0);
        }
    }

    private void WrapButtons(List<RectTransform> button)
    {
        int count = button.Count;
        for (int i = 0; i < count; i++)
        {
            if (button[i].anchoredPosition.x < -mapWidth / 2)
            {
                button[i].anchoredPosition += new Vector2(mapWidth, 0);
            }
            if (button[i].anchoredPosition.x > mapWidth / 2)
            {
                button[i].anchoredPosition -= new Vector2(mapWidth, 0);
            }
        }
    }

    private void ZoomToNation(RectTransform targetButton)
    {
        uiTopFrameHUD.ShowButtons(isZoomed);

        if (isZoomed)
        {
            ResetZoom();
            return;
        }

        string nationName = targetButton.name;
        VDLog.Log("Nation Name: " + nationName);
        RectTransform targetMap = mainWorldMap;
        if (RectTransformUtility.RectangleContainsScreenPoint(leftWorldMap, targetButton.position, null))
        {
            targetMap = leftWorldMap;
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(rightWorldMap, targetButton.position, null))
        {
            targetMap = rightWorldMap;
        }

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(targetMap, targetButton.position, null, out localPoint))
        {
            Vector2 mapSize = targetMap.rect.size;
            float percentX = (localPoint.x / mapSize.x) + 0.5f;
            float percentY = (localPoint.y / mapSize.y) + 0.5f;

            float adjustedX = targetMap.anchoredPosition.x + (percentX - 0.5f) * targetMap.rect.width;
            float adjustedY = targetMap.anchoredPosition.y + (percentY - 0.5f) * targetMap.rect.height;

            float offsetX = (worldMapContainer.anchoredPosition.x - adjustedX) * VDParameter.WORLD_MAP_ZOOM_MULTIPLIER;
            float offsetY = (worldMapContainer.anchoredPosition.y - adjustedY) * VDParameter.WORLD_MAP_ZOOM_MULTIPLIER;

            Vector2 zoomedPosition = new Vector2(offsetX, offsetY);

            StartCoroutine(SmoothZoom(zoomedPosition, VDParameter.WORLD_MAP_ZOOM_MULTIPLIER, nationName));
            isZoomed = true;

            // TODO: NEED TO REFACTOR LATER
            buttonsNations[0].StopTextBlinking();
            arrow.gameObject.SetActive(false);
            arrow.anchoredPosition = arrowStartPosition;
            arrowTween?.Kill();
        }
    }

    public void ResetZoom()
    {
        StartCoroutine(SmoothZoom(originalPosition, originalScale, null));
        isZoomed = false;
    }

    IEnumerator SmoothZoom(Vector2 targetPosition, float targetScale, string? nationName)
    {
        float elapsedTime = 0;
        Vector2 startPosition = worldMapContainer.anchoredPosition;
        float startScale = worldMapContainer.localScale.x;

        while (elapsedTime < VDParameter.WORLD_MAP_ZOOM_SPEED)
        {
            float t = elapsedTime / VDParameter.WORLD_MAP_ZOOM_SPEED;
            worldMapContainer.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            worldMapContainer.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        worldMapContainer.anchoredPosition = targetPosition;
        worldMapContainer.localScale = Vector3.one * targetScale;

        if (nationName != null)
        {
            string nationMap = nationName + "Map";
            if (Enum.TryParse(nationMap, out UIWindowType windowEnum))
            {
                FMUIWindowController.Get.OpenWindow(windowEnum);
                /*ResetZoom();*/
            }
        }
    }

    private void OnClickNext()
    {
        lobbyTutorialHandler.SetButtonInteractable(false);

        switch (dialogueIndex)
        {
            case 0:
                lobbyTutorialHandler.SetTextboxActive(false);

                imageSequence?.Kill();
                imageSequence = DOTween.Sequence();
                imageSequence.Append(closeUpImages[0].DOAnchorPos(Vector2.zero, 1f))
                    .Join(closeUpImages[0].DOScale(Vector3.one, 1f))
                    .AppendInterval(0.6f)
                    .AppendCallback(() => closeUpImages[0].gameObject.SetActive(false))
                    .Append(closeUpImages[1].DOAnchorPos(Vector2.zero, 1f))
                    .Join(closeUpImages[1].DOScale(Vector3.one, 1f))
                    .AppendInterval(0.6f)
                    .AppendCallback(() => closeUpImages[1].gameObject.SetActive(false))
                    .Append(closeUpImages[2].DOAnchorPos(Vector2.zero, 1f))
                    .Join(closeUpImages[2].DOScale(Vector3.one, 1))
                    .AppendInterval(0.6f)
                    .OnComplete(() =>
                    {
                        closeUpImages[2].gameObject.SetActive(false);
                        lobbyTutorialHandler.SetTextboxActive(true);
                        lobbyTutorialHandler.SetDialogueText(dialogue_2);

                        lobbyTutorialHandler.SetButtonInteractable(true);
                    });
                break;
            case 1:
                lobbyTutorialHandler.SetDialogueText(dialogue_3);
                lobbyTutorialHandler.SetButtonInteractable(true);
                break;
            case 2:
                lobbyTutorialHandler.SetActive(false);
                buttonsNations[0].SetTextBlinking();

                arrow.gameObject.SetActive(true);
                arrowStartPosition = arrow.anchoredPosition;
                arrowTween = arrow.DOAnchorPos(arrowStartPosition - new Vector2(50, 0), 0.5f).SetLoops(-1, LoopType.Yoyo);
                break;
        }
        dialogueIndex++;
    }

    private void OnClickSkip()
    {
        dialogueIndex = 3;
        imageSequence?.Kill();
        lobbyTutorialHandler.SetActive(false);
        ZoomToNation(nationsLocation[0]);
    }
}
