using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIWorldMapLandmark : MonoBehaviour
{
    [SerializeField] private FMButton button;
    [SerializeField] private RectTransform cleanLandmark;
    [SerializeField] private RectTransform dirtyLandmark;
    [SerializeField] private TextMeshProUGUI nationName;
    [SerializeField] private List<RectTransform> cleanMap;
    [SerializeField] private List<Image> clouds;
    [SerializeField] private WorldMissionCountry map;
    private bool isCompleted;

    private string darkColor = "#6B8E9F";
    private string whiteColor = "#FFFFFF";

    private Tween textTween;

    public FMButton Button
    {
        get => button;
    }

    public WorldMissionCountry Map
    {
        get => map;
    }

    public void SetName()
    {
        nationName.SetText(gameObject.name);
    }

    public void SetLandmark()
    {
        bool isCompleted = FMMissionController.Get().IsMapMissionAllCompleted(map);

        cleanLandmark.gameObject.SetActive(isCompleted);
        dirtyLandmark.gameObject.SetActive(!isCompleted);

        if (cleanMap != null && cleanMap.Count > 0)
        {
            int count = cleanMap.Count;
            for (int i = 0; i < count; i++)
            {
                cleanMap[i].gameObject.SetActive(isCompleted);
            }
        }

        string hexColor = isCompleted ? whiteColor : darkColor;
        if (clouds != null && clouds.Count > 0)
        {
            int count = clouds.Count;
            for (int i = 0; i < count; i++)
            {
                if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
                {
                    clouds[i].color = newColor;
                }
            }
        }
    }

    public void SetTextBlinking()
    {
        textTween = nationName.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopTextBlinking()
    {
        textTween?.Kill();

        Color textColor = nationName.color;
        textColor.a = 1;
        nationName.color = textColor;
    }
}
