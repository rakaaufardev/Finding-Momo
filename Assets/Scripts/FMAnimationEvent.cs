using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FMAnimationEvent : MonoBehaviour
{
    private FMSceneController sceneController;

    public void PlayAnimationEvent(string animEventName)
    {
        Action eventCallback = FMSceneController.Get().GetAnimationEventData(animEventName);
        eventCallback?.Invoke();
    }
}
