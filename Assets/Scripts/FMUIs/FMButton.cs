using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FMButton : Button
{
    public void AddListener(Action callback)
    {
        callback += () =>
        {
            FMSoundController.Get().PlaySFX(SFX.SFX_Button);
        };
        onClick.AddListener(callback.Invoke);
    }

    public void RemoveListener(Action callback)
    {
        callback += () =>
        {
            FMSoundController.Get().PlaySFX(SFX.SFX_Button);
        };
        onClick.RemoveListener(callback.Invoke);
    }

    public void RemoveAllListeners()
    {
        onClick.RemoveAllListeners();
    }
}
