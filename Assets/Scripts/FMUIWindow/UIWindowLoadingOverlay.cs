using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VD;
using DG.Tweening;
using TMPro;

public class UIWindowLoadingOverlay : VDUIWindow
{
    [SerializeField] private TextMeshProUGUI textLoading;

    private const string TEXT_LOADING = "Loading";

    public override void Show(object[] dataContainer)
    {
        int loopCount = 0;
        int loopTotal = 3;
        string textLoadingString = TEXT_LOADING;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            textLoadingString += ".";
            loopCount++;

            if (loopCount > loopTotal)
            {
                textLoadingString = TEXT_LOADING;
            }

            textLoading.SetText(textLoadingString);
        });       

        sequence.AppendInterval(0.25f);
        sequence.SetLoops(-1);
    }

    public override void Hide()
    {

    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }
}
