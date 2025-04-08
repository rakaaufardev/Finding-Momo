using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDHealthIcon : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform healthIcon;
    [SerializeField] private RectTransform rootVisual;

    public RectTransform Root
    {
        get => root;
        set => root = value;
    }

    public RectTransform RootVisual
    {
        get => rootVisual;
        set => rootVisual = value;
    }

    public RectTransform HealthIcon
    {
        get => healthIcon;
        set => healthIcon = value;
    }

    public void Activate(bool isActive)
    {
        healthIcon.gameObject.SetActive(isActive);
    }
}
