using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMDirectionalLight : MonoBehaviour
{
    [SerializeField] private Transform lightTransform;

    public Transform LightTransform
    {
        get
        {
            return lightTransform;
        }
    }

    public void RotateDirectionalLight(ViewMode inViewMode)
    {
        if (inViewMode == ViewMode.SideView)
        {
            lightTransform.DORotate(new Vector3(50f, -120f, 0f), 1.5f);
        }
        else if (inViewMode == ViewMode.BackView)
        {
            lightTransform.DORotate(new Vector3(50f, -30f, 0f), 1.5f);
        }
    }
}
