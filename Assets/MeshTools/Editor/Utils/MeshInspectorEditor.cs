using UnityEditor;
using UnityEngine;

namespace MeshTools.Editor.Utils
{
    [CustomEditor(typeof(MeshInspector))]
    [CanEditMultipleObjects]
    public class MeshInspectorEditor : UnityEditor.Editor
    {
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

            EditorGUILayout.LabelField($"Triangle: {t.TriangleIndex}");
            EditorGUILayout.LabelField($"Vertex: {t.VertexIndex}");

            serializedObject.ApplyModifiedProperties();
        }
    }
}