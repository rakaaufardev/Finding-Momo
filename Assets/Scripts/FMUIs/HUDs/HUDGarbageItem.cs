using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HUDGarbageItem : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private Image imageIcon;
    [SerializeField] private Image imageBackground;
    [SerializeField] private Image imageBackgroundFill;
    [SerializeField] private RectTransform rootMultipleScore;
    [SerializeField] private TextMeshProUGUI textMultipleGarbage;

    private Tweener tween;
    bool isActive;

    public bool IsActive
    {
        get
        {
            return isActive;
        }
    }

    public RectTransform Root
    {
        get
        {
            return root;
        }
    }

    public void SetIcon(Sprite sprite)
    {
        imageIcon.sprite = sprite;
    }

    public void SetSize(Vector3 sizeRoot)
    {
        root.localScale = sizeRoot;
    }

    public void SetActive(bool inIsActive)
    {
        Color color = inIsActive ? new Color(1, 1, 1, VDParameter.HEX_HUD_GARBAGE_ALPHA_ACTIVE) : new Color(0.5f, 0.5f, 0.5f, VDParameter.HEX_HUD_GARBAGE_ALPHA_INACTIVE);
        imageIcon.color = color;

        isActive = inIsActive;
    }

    public void SetFill(float currentValue, float maxValue)
    {
        float value = 1f - ((currentValue / maxValue) * 1f);
        imageBackgroundFill.fillAmount = value;
    }

    public void PlayFillEffect(bool isComplete,Vector3 scaleDefault)
    {
        float duration = isComplete ? 1f : 0.1f;
        float targetScale = isComplete ? 0.25f : 0.15f;

        if (tween != null)
        {
            tween.Restart();
        }

        root.localScale = Vector3.one;
        tween = root.DOPunchScale(new Vector3(targetScale, targetScale, targetScale), duration,0).SetEase(Ease.Linear).OnComplete(() =>
        {
            root.localScale = scaleDefault;
            tween = null;
        });
    }


    public void SetActiveScoreMultipy(int multipleGarbage)
    {
        rootMultipleScore.gameObject.SetActive(true);
        textMultipleGarbage.text = "x"+multipleGarbage.ToString();
    }
}
