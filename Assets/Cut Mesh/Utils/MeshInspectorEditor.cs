using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshInspector))]
[CanEditMultipleObjects]
public class MeshInspectorEditor : Editor
{

    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        var t = (target as MeshInspector);

        if (GUILayout.Button("Next vertex"))
        {
            t.NextVertex();
        }
        if (GUILayout.Button("Next triangle"))
        {
            t.NextTriangle();
        }

        EditorGUILayout.LabelField($"Triangle: {t.TrinangleIndex}");
        EditorGUILayout.LabelField($"Vertex: {t.VertexIndex}");

        serializedObject.ApplyModifiedProperties();
    }
}