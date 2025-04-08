using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FMPlatform : MonoBehaviour
{
    [SerializeField] private Portal portal;
    [SerializeField] private Transform nextConnector;
    [SerializeField] private Transform obstacleInSpawnPoint;
    [SerializeField] private Transform characterSpawnPoint;
    [SerializeField] private Transform endTrackPoint;
    [SerializeField] private Transform rootObstacles;
    [SerializeField] private Transform edgeEnvironment;
    [SerializeField] private Transform[] areaToHideList;
    [SerializeField] private Transform[] areaToShowList;
    [SerializeField] private Transform[] obstacleToHideList;
    [SerializeField] private Transform[] arrowTransitions;
    [SerializeField] private RandomCollectible[] randomCollectibles;
    [SerializeField] private Coin[] coins;
    [SerializeField] private int platformId;
    [SerializeField] private FMRandomStartSign randomStartSign;
    [SerializeField] private List<Transform> hideEdgeObjects;
    [SerializeField] private List<Transform> showEdgeObjects;

    [Header("Debug Purposes")]
    [SerializeField] private List<GameObject> gameObjectList;
    [SerializeField] private TextMeshPro platformName;

    public List<GameObject> GameObjectList
    {
        get
        {
            return gameObjectList;
        }
        set
        {
            gameObjectList = value;
        }
    }

    public Vector3 GetNextConnectorPosition()
    {
        Vector3 result = nextConnector.transform.position;
        return result;
    }

    public Vector3 GetEndTrackPoint()
    {
        Vector3 result = endTrackPoint.transform.position;
        return result;
    }

    public Vector3[] GetArrowTransitionPositions()
    {
        int count = arrowTransitions.Length;
        Vector3[] result = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            Transform arrow = arrowTransitions[i];
            result[i] = arrow.transform.position;
        }

        return result;
    }

    public void SetPlatformId(int id)
    {
        platformId = id;
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void ShowObstacles(bool isShow)
    {
        rootObstacles.gameObject.SetActive(isShow);
    }

    public void ShowArrowTransition(bool isShow)
    {
        int count = arrowTransitions.Length;
        for (int i = 0; i < count; i++)
        {
            Transform arrow = arrowTransitions[i];
            arrow.gameObject.SetActive(isShow);
        }
    }

    public void ShowPortal(bool isShow)
    {
        if (portal != null)
        {
            portal.gameObject.SetActive(isShow);
            portal.PlatformId = platformId;
        }
    }

    public void HideSideViewTransitionEnvironment(bool isShow)
    {
        int count = areaToHideList.Length;
        for (int i = 0; i < count; i++)
        {
            Transform hideEnvironment = areaToHideList[i];
            hideEnvironment.gameObject.SetActive(!isShow);
        }
    }
    
    public void HideSideViewTransitionObstacles(bool isShow)
    {
        int count = obstacleToHideList.Length;
        for (int i = 0; i < count; i++)
        {
            Transform hideEnvironment = obstacleToHideList[i];
            hideEnvironment.gameObject.SetActive(!isShow);
        }
    }

    public void ShowSideViewTransitionEnvironment(bool isShow)
    {
        int count = areaToShowList.Length;
        for (int i = 0; i < count; i++)
        {
            Transform showEnvironment = areaToShowList[i];
            showEnvironment.gameObject.SetActive(isShow);
        }
    }

    public void SetCharacterSpawnPoint(bool isShow)
    {
        characterSpawnPoint.gameObject.SetActive(isShow);
    }

    public void SetRandomStartLogo()
    {
        int hideCount = hideEdgeObjects.Count;
        int showCount = showEdgeObjects.Count;
        if (randomStartSign != null && hideCount > 0)
        {
            randomStartSign.GenerateStartSign();
            randomStartSign.gameObject.SetActive(true);

            for (int i = 0; i < hideCount; i++)
            {
                hideEdgeObjects[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < showCount; i++)
            {
                showEdgeObjects[i].gameObject.SetActive(true);
            }
        }
    }

    public void ShowObjectInSpawnPoint(bool isShow)
    {
        obstacleInSpawnPoint.gameObject.SetActive(isShow);

        if (edgeEnvironment != null)
        {
            edgeEnvironment.gameObject.SetActive(!isShow);
        }
    }

    public Transform GetCharacterSpawnPoint()
    {
        return characterSpawnPoint;
    }

    public Portal GetPortal()
    {
        return portal;
    }

    public bool IsPortalExist()
    {
        bool result = portal != null && portal.gameObject.activeInHierarchy;
        return result;
    }

    public Vector3 GetPortalPosition()
    {
        return portal.Root.position;
    }


    public void SetPortal(Portal portal)
    {
        this.portal = portal;
    }

    public int GetRandomCollectibleCount()
    {
        return randomCollectibles.Length;
    }

    public PlatformColliderObject[] GetRandomCollectibleObjects()
    {
        int count = randomCollectibles.Length;
        PlatformColliderObject[] result = new PlatformColliderObject[count];
        for (int i = 0; i < count; i++)
        {
            RandomCollectible randomCollectible = randomCollectibles[i];
            PlatformColliderObject platformColliderObject = randomCollectible.GetCollectibleObject();
            result[i] = platformColliderObject;
        }

        return result;
    }

    public Coin[] GetCoinObjects()
    {
        return coins;
    }

    public void ClearRandomCollectible(PlatformColliderObject checkCollectible)
    {
        int count = randomCollectibles.Length;
        for (int i = 0; i < count; i++)
        {
            RandomCollectible randomCollectible = randomCollectibles[i];
            PlatformColliderObject collectible = randomCollectible.GetCollectibleObject();
            if (collectible == checkCollectible)
            {
                randomCollectible.ClearCollectible();
            }
        }
    }

    public void SetRandomCollectibleObject(PlatformColliderObject collectible, int index)
    {
        RandomCollectible randomCollectible = randomCollectibles[index];
        randomCollectible.SetCollectibleObject(collectible);
        collectible.Platform = this;
    }

    public RandomCollectible GetRandomCollectibleObject(int index)
    {
        RandomCollectible randomCollectible = randomCollectibles[index];
        return randomCollectible;
    }

    public void SetCoinModels(string coinModelName, ViewMode viewMode)
    {
        StartCoroutine(SetCoinModelsAsync(coinModelName, viewMode));
    }

    private IEnumerator SetCoinModelsAsync(string coinModelName, ViewMode viewMode)
    {
        int count = coins.Length;
        for (int i = 0; i < count; i++)
        {
            GameObject coinModel = FMPlatformController.Get().GetCoinVisual(coinModelName);

            Coin coin = coins[i];
            coin.ClearCoinModel();
            coin.SetCoinModel(coinModel, viewMode);
            coin.SetCoinName(coinModelName);
            coin.SetCoinOriginalPosition();
            yield return coin.SetCoinRotation();

            yield return null;
        }
    }

    public void Awake()
    {
        SetPlatformName();
    }

    private void SetPlatformName()
    {
        if (platformName != null) platformName.text = this.name;
    }

#if UNITY_EDITOR
    public void SetRandomCollectible(RandomCollectible[] inRandomCollectibles)
    {
        randomCollectibles = inRandomCollectibles;
    }

    public void SetCoin(Coin[] inCoin)
    {
        coins = inCoin;
    }
#endif
}
