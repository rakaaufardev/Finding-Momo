using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIVersion : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private TextMeshProUGUI textVersion;
    [SerializeField] private TextMeshProUGUI textFrameRate;
    [SerializeField] private float deltaTime;

    public RectTransform Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }

    void Start()
    {
        deltaTime = 0f;
        string version = Application.version;
        textVersion.SetText(version);
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float frameRate = 1.0f / deltaTime;
        string frameRateString = string.Format("{0:F0} FPS", frameRate);
        textFrameRate.SetText(frameRateString);    
    }
}
