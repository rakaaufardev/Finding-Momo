using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIWindowDownloadChecker : VDUIWindow
{
    [SerializeField] private Slider sliderFill;
    [SerializeField] private TextMeshProUGUI textLoading;
    private float currentProgress;

    public override void Show(object[] dataContainer)
    {
        currentProgress = 0;
        sliderFill.value = 0;

        UITopFrameHUD topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.Show(false);
    }

    public override void Hide()
    {
        UITopFrameHUD topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.Show(true);
    }

    public override void OnRefresh()
    {

    }

    public override void DoUpdate()
    {
        float lastProgress = currentProgress;
        float newProgress = FMDownloadProgressChecker.GetCheckingProgress();
        DOTween.To(() => currentProgress, x => currentProgress = x, newProgress, 0.5f).SetEase(Ease.Linear);

        float progressPercentage = currentProgress * 100f;
        sliderFill.DOValue(currentProgress, 0.5f).SetEase(Ease.Linear);

        string progressString = string.Format("Downloading...{0:F0}%", progressPercentage);
        textLoading.SetText(progressString);
    }
}
