using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VD;

[CustomEditor(typeof(FMCostume))]
public class FMCostumeEditor : Editor
{
    private FMCostume costume;
    private int polygonCount;

    public void OnEnable()
    {
        costume = (FMCostume)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        polygonCount = EditorGUILayout.IntField("Total Character Polygon Count", polygonCount);

        if (GUILayout.Button("Show Character Total Polygon"))
        {
            CountTotalPolygon();
        }
    }

    private void CountTotalPolygon()
    {
        polygonCount = 0;

        int count = costume.CostumeParts.Count;

        for (int i = 0; i < count; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = costume.CostumeParts[i].GetComponent<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer != null)
            {
                Mesh mesh = skinnedMeshRenderer.sharedMesh;
                polygonCount += mesh.triangles.Length / 3;
            }
            else
            {
                VDLog.Log("Skinned Mesh Renderer for " + costume.CostumeParts[i].name + " not found!");
            }
        }

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
}
