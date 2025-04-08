using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

//public enum TutorialActionType
//{
//    None,
//    Talk,
//    Click,
//    InputKey
//}
public enum TutorialState
{
    Intro,
    Coin,
    Jump,
    DoubleJump,
    Slide,
    TransitionBackView,
    Portal,
    MoveRight,
    MoveLeft,
    MiniWheel,
    ManHole,
    Rhythm,
    Finish,
    PressTransitionForce,
    JumpBackSand,
    DoubleJumpBackSand,
    SlideBackSand,
    Bomb,
    Collectible,
    Ink,
    SlideSideCity,
    TrashBinLeft,
    TrashBinMiddle,
    TrashBinRight,
    JumpBackCity,
    Truck,
    SlideBackCity,
    Trampoline,
    NONE
}
public class UITutorial : MonoBehaviour
{
    private static UITutorialMessage textMessage;
    private static RectTransform rootTutorial;
    private static List<Tutorial> tutorialSteps;
    private static int currentStep;
    private static string currentTutorialSection;
    private static bool isTutorialActiveDialogue;

    public static void Create(string inTutorialSection)
    {
        Init(inTutorialSection);
    }

    public static string GetCurrentTutorialSection()
    {
        return currentTutorialSection;
    }

    public static int GetCurrentTutorialStep()
    {
        return currentStep;
    }

    public static bool IsTutorialActive()
    {
        return isTutorialActiveDialogue;
    }
   
    
    public static void OnNext()
    {
        Next();
    }

    public static bool IsDone(string inTutorialSection)
    {
        bool result = FMUserDataService.Get().GetTutorialDoneFlags(inTutorialSection);
        
        return result;
    }

    public static void SkipAllTutorial()
    {
        Dictionary<string, List<Tutorial>> tutorialDatabase = FMTutorialDatabase.GetTutorialDatabase();
        int count = tutorialDatabase.Count;
        string[] tutorialSections = new string[count];
        tutorialDatabase.Keys.CopyTo(tutorialSections, 0);
        for (int i = 0; i < count; i++)
        {
            string tutorialSection = tutorialSections[i];
            FMUserDataService.Get().UpdateTutorialDoneFlags(tutorialSection, true);
        }
    }

    public static void SkipTutorial(string section)
    {
        FMUserDataService.Get().UpdateTutorialDoneFlags(section, true);
    }

    public static void ShowSpaceImage(bool show)
    {
        textMessage.ShowSpaceImage(show);
    }
    
    private static void Init(string inTutorialSection)
    {
        currentStep = -1;
        currentTutorialSection = inTutorialSection;

        Dictionary<string, List<Tutorial>> tutorialDatabase = FMTutorialDatabase.GetTutorialDatabase();
        tutorialSteps = tutorialDatabase[currentTutorialSection];

        Next();
    }

    private static void Next()
    {
        currentStep++;
        if (currentStep < tutorialSteps.Count)
        {
            Tutorial tutorial = tutorialSteps[currentStep];
            string actionType = tutorial.ActionType;
            string messageTag = tutorial.Message;
            Enum.TryParse(messageTag, out CopyTag copyTag);
            string message = VDCopy.GetCopy(copyTag);
            object target = tutorial.Target;
            DoAction(actionType, message, target);
         }
        else
        {
            End();
        }
    }

    private static void DoAction(string inActionType, string inMessage, object inTarget)
    {
        isTutorialActiveDialogue = true;
        if (rootTutorial == null)
        {
            rootTutorial = GameObject.FindObjectOfType<UITutorial>().GetComponent<RectTransform>();
        }

        switch (inActionType)
        {
            case "Text":
                if (textMessage == null)
                {
                    textMessage = FMAssetFactory.GetUiTutorialMessage(rootTutorial);
                    textMessage.tag = "UITutorial_ContentTutorial";
                }
                textMessage.SetText(inMessage);
                string target = (string)inTarget;
                switch (target)
                {
                    case "UpCenter":
                        textMessage.SetPosition(new Vector2(0,300));
                        break;
                    case "BottomCenter":
                        textMessage.SetPosition(new Vector2(0,-300));
                        break;
                    case "Center":
                        textMessage.SetPosition(Vector2.zero);
                        break;
                }

                textMessage.gameObject.SetActive(true);

                break;
            case "SpotLight":
                break;
            
        }
        
    }

    private static void End()
    {
        FMUserDataService.Get().UpdateTutorialDoneFlags(currentTutorialSection, true);
        textMessage.gameObject.SetActive(false);
        isTutorialActiveDialogue = false;
    }
}
