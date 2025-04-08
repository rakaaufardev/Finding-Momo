using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpData
{
    public string value;
    public string duration;
    public Action<string> callback;

    public PowerUpData(string value, string duration, Action<string> inCallback)
    {
        this.value = value;
        this.duration = duration;
        this.callback = inCallback;
    }
}
