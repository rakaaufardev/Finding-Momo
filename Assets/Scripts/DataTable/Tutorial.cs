using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tutorial
{
    public string TutorialSection;
    public int Step;
    public string ActionType;
    public string Message;
    public string Target;

    public Tutorial(string inTutorialSection, string inStep, string inActionType, string inMessage, string inTarget)
    {
        TutorialSection = inTutorialSection;
        Step = Convert.ToInt32(inStep);
        ActionType = inActionType;
        Message = inMessage;
        Target = inTarget;
    }
}

[Serializable]
public class TutorialWrapper
{
    public Tutorial[] tutorial;

    public TutorialWrapper(Tutorial[] inTutorial)
    {
        tutorial = inTutorial;
    }
}
