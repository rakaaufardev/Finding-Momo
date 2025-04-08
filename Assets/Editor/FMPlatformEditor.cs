using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VD;

[CustomEditor(typeof(FMPlatform))]
public class FMPlatformEditor : Editor
{
    private RandomCollectible[] randomCollectibles;
    private Coin[] coins;
    private FMPlatform platform;
    private int randomCollectibleYPos;
    private int polygonCount;

    public void OnEnable()
    {
        platform = (FMPlatform)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Find All Coin"))
        {
            FindAllCoins();
        }

        if (GUILayout.Button("Find All Random Collectibles"))
        {
            FindAllRandomObstacle();
        }

        randomCollectibleYPos = EditorGUILayout.IntField("Random Collectible Y Pos", randomCollectibleYPos);
        if (GUILayout.Button("Set Random Collectible Y Position"))
        {
            SetRandomCollectibleYPos();
        }

        polygonCount = EditorGUILayout.IntField("Total Polygon Count", polygonCount);
        if (GUILayout.Button("Show Total Polygon in Platform"))
        {
            CountTotalPolygon();
        }

        if (GUILayout.Button("Save"))
        {
            Save();
        }
    }

    private void FindAllRandomObstacle()
    {
        randomCollectibles = platform.gameObject.GetComponentsInChildren<RandomCollectible>();
        platform.SetRandomCollectible(randomCollectibles);
    }
    
    private void FindAllCoins()
    {
        coins = platform.gameObject.GetComponentsInChildren<Coin>();
        platform.SetCoin(coins);
    }

    private void SetRandomCollectibleYPos()
    {
        int count = randomCollectibles.Length;
        for (int i = 0; i < count; i++)
        {
            randomCollectibles[i].transform.localPosition = new Vector3(0, randomCollectibleYPos, 0);
        }
    }

    private void CountTotalPolygon()
    {
        polygonCount = 0;

        Transform parentTransform = platform.transform;
        platform.GameObjectList.Clear();
        AddChildrenWithMeshFilter(parentTransform, platform.GameObjectList);
    }

    private void AddChildrenWithMeshFilter(Transform parent, List<GameObject> objectList)
    {
        foreach (Transform child in parent)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                polygonCount += mesh.triangles.Length / 3;
                objectList.Add(child.gameObject);
            }

            AddChildrenWithMeshFilter(child, objectList);
        }
    }

    private void Save()
    {
        //GameObject workspace = GameObject.Find("PlatformEditor");
        GameObject platformPrefab = platform.gameObject;
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(platformPrefab);

        bool pathFound = !string.IsNullOrEmpty(prefabPath);

        if (pathFound)
        {
            VDLog.Log("Local Path of Prefab: " + prefabPath);
            PrefabUtility.SaveAsPrefabAsset(platformPrefab, prefabPath, out bool success);

            if (success)
            {
                VDLog.Log("Save Prefab Success on Path: " + prefabPath);
            }
        }
        else
        {
            VDLog.LogError("Prefab local path not found");
        }
    }
}
