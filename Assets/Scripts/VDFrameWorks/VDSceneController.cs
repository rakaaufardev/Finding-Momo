using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum SceneState
{
    None,
    Lobby,
    Main,
    COUNT
}

public abstract class VDSceneController : MonoBehaviour
{
    protected Dictionary<SceneState, VDScene> sceneData;
    protected SceneState currentSceneState;
    protected VDScene currentSceneObject;
    protected Action cinematicCallback;
    [SerializeField] protected CinemachineBrain cameraCinemachineBrain;
    [SerializeField] protected Transform rootScene;
    [SerializeField] protected Transform rootUI;
    /*[SerializeField] protected Transform rootQuickAction;*/

    protected void Update()
    {
        DoUpdate();
    }

    protected void Start()
    {
        AddSceneData();
        InitGame();
    }

    protected abstract void AddSceneData();
    protected abstract void InitGame();
    protected abstract void DoUpdate();

    public void ChangeScene(SceneState sceneState, params object[] dataContainer)
    {
        StartCoroutine(ChangeSceneSequence(sceneState, dataContainer));
    }

    private IEnumerator ChangeSceneSequence(SceneState sceneState, params object[] dataContainer)
    {
        //DON'T CHANGE ANY VALUE IN HERE !
        if (sceneState != SceneState.None && sceneState != SceneState.COUNT)
        {
            if (currentSceneObject != null) 
            {
                yield return currentSceneObject.Exit();
                Destroy(currentSceneObject.gameObject);
            }
        }

        VDScene scenePrefab = sceneData[sceneState];
        currentSceneObject = Instantiate(scenePrefab,rootScene);
        yield return currentSceneObject.Enter(dataContainer);
        currentSceneState = sceneState;
    }

    public VDScene GetCurrentScene()
    {
        VDScene result = currentSceneObject;
        return result;
    }

    public SceneState GetCurrentSceneState()
    {
        return currentSceneState;
    }

    public Transform GetRootUI()
    {
        return rootUI;
    }

    /*public Transform GetRootQuickAction()
    {
        return rootQuickAction;
    }*/
}
