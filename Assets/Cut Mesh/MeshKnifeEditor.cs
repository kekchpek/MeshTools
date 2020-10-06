using System.Numerics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshKnife))]
[CanEditMultipleObjects]
public class MeshKnifeEditor : Editor
{

    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        var t = (target as MeshKnife);

        if (!t.Initialized && GUILayout.Button("Initalize"))
        {
            t.Initialize();
        }

        if (t.Initialized && GUILayout.Button("Cut"))
        {
            t.Cut();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
    }
}