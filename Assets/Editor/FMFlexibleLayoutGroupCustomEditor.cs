using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FMFlexibleLayoutGroup))]
public class FMFlexibleLayoutGroupCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FMFlexibleLayoutGroup flg = (this.target as FMFlexibleLayoutGroup);
        flg.padding = new RectOffset(EditorGUILayout.IntField("P Left", flg.padding.left), flg.padding.right, flg.padding.top, flg.padding.bottom);
        flg.padding = new RectOffset(flg.padding.left, EditorGUILayout.IntField("P Right", flg.padding.right), flg.padding.top, flg.padding.bottom);
        flg.padding = new RectOffset(flg.padding.left, flg.padding.right, EditorGUILayout.IntField("P Top", flg.padding.top), flg.padding.bottom);
        flg.padding = new RectOffset(flg.padding.left, flg.padding.right, flg.padding.top, EditorGUILayout.IntField("P Bottom", flg.padding.bottom));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Spacing"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_StartCorner"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_StartAxis"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_ChildAlignment"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("cellsPerLine"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("aspectRatio"));
        GUI.enabled = false;
        EditorGUILayout.Vector2Field("Cell size", flg.cellSize);
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();

    }
}