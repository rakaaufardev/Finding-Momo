using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VD;


public class UIApplicationVersion : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    public void Init()
    {
        string version = Application.version;
        versionText.SetText(string.Format("Current Version: {0}", version));
    }
}
