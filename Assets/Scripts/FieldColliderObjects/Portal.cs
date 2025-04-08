using System.Collections.Generic;
using UnityEngine;

public class Portal : PlatformColliderObject
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform rootVisual;
    [SerializeField] private Transform Line1;
    [SerializeField] private Transform Line2;
    [SerializeField] private Transform Line3;
    [SerializeField] private int platformId;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject inkPrefab;

    private List<Transform> lanes;

    public Transform Root
    {
        get { return root; }
        set { root = value; }
    }

    public int PlatformId
    {
        get { return platformId; }
        set { platformId = value; }
    }

    private void Start()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        WorldType currentWorldType = mainScene.GetCurrentWorldType();
        /*FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        FMSurfCharacter surfCharacter = currentWorld.GetCharacter() as FMSurfCharacter;*/

        if (currentWorldType == WorldType.Surf)
        {
            rootVisual.gameObject.SetActive(false);
        }

        /*if (surfCharacter != null)
        {
            if (surfCharacter.IsInTutorial) rootVisual.gameObject.SetActive(false);
        }*/

        lanes = new List<Transform> { Line1, Line2, Line3 };
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        WorldType worldType = mainScene.GetCurrentWorldType();

        List<Transform> availableLanes = new List<Transform>(lanes);

        //shuffle lane
        for (int i = availableLanes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = availableLanes[i];
            availableLanes[i] = availableLanes[randomIndex];
            availableLanes[randomIndex] = temp;
        }

        // first lane for portal
        Transform portalLane = availableLanes[0];
        root.SetParent(portalLane);
        root.localPosition = Vector3.zero;

        if (worldType == WorldType.Main)
        {
            //lane bomb/ink
            Transform obstacleLane = availableLanes[1];
            GameObject obstaclePrefab = (Random.value > 0.5f) ? bombPrefab : inkPrefab;
            Instantiate(obstaclePrefab, obstacleLane.position, Quaternion.identity, obstacleLane);
        }
    }

    public override void OnHitCollision(Collision collision)
    {
    }

    public override void OnHitTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FMMainCharacter character = other.gameObject.GetComponentInParent<FMMainCharacter>();
            if (character != null)
            {
                character.OnHitPortal(platformId);
            }
        }
    }
}
