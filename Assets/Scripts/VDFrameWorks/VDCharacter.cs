using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VDCharacter : MonoBehaviour
{
    protected VDInputController inputController;
    protected VDActionMachine actionMachine;

    protected void Update()
    {
        DoUpdate();
    }

    protected void FixedUpdate()
    {
        DoFixedUpdate();
    }

    protected void LateUpdate()
    {
        DoLateUpdate();
    }

    public void UpdateHUDGarbage(string name)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        FMGarbageController garbageController = mainScene.GetGarbageController();

        float collectRemain = garbageController.GetGarbageRemain(name);
        float totalCount = garbageController.GetGarbageCount(name);
        uiMain.UpdateHudGarbage(name, collectRemain, totalCount);
        if (collectRemain > 0)
        {
            uiMain.PlayGarbageFillHUD(name);
        }
        else
        {
            uiMain.SetHudGarbageComplete(name);  
        }
    }

    public void HitGarbage(Garbage garbage, FMGarbageController garbageController, FMPlatformController platformController, FMScoreController scoreController)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        string garbageName = garbage.GetGarbageName();
        garbage.Show(false);

        //platformController.HideDuplicateGarbage(garbageName);
        //garbageController.CollectGarbage(garbageName);
        //garbageController.RemoveGarbagePool(garbageName);
        //garbageController.ResetGarbagePoolCounter();
        //garbageController.CheckGarbageCollectionStatus();

        platformController.StoreStashGarbageVisual(garbage);
        platformController.StoreStashRandomCollectible(garbage);
        garbage.ClearGarbage();

        garbage.Platform = null;

        //scoreController.UpdateGarbageScore();

        //bool isCollected = garbageController.GetGarbageStatus(garbageName);
        //float collectRemain = garbageController.GetGarbageRemain(name);
        //float totalCount = garbageController.GetGarbageCount(name);
        //uiMain.UpdateHudGarbage(garbageName, collectRemain, totalCount);

        FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);
    }

    public void HitInk(InkObstacle inkObstacle, Transform cameraCenterTransform)
    {
        inkObstacle.RootHitbox.gameObject.SetActive(false);
        Transform rootVisual = inkObstacle.RootVisual;
        rootVisual.SetParent(cameraCenterTransform);

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        StartCoroutine(mainScene.InkSequence(inkObstacle));
    }    

    public abstract void DoUpdate();
    public abstract void DoFixedUpdate();
    public abstract void DoLateUpdate();
    public VDInputController GetInputController()
    {
        return inputController;
    }

    public VDActionMachine GetActionMachine()
    {
        return actionMachine;
    }
}
