using MeshTools.MeshKnife;
using UnityEditor;
using UnityEngine;

namespace MeshTools.Editor.MeshKnife
{
    [CustomEditor(typeof(MeshTools.MeshKnife.MeshKnife))]
    [CanEditMultipleObjects]
    public class MeshKnifeEditor : UnityEditor.Editor
    {
        private static IMeshKnife _target;
        
        private void Awake()
        {
            _target = target as IMeshKnife;
            if (_target == null)
                Debug.LogError($"Target is not a {nameof(MeshTools.MeshKnife.MeshKnife)}");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (!_target.Initialized && GUILayout.Button("Initialize"))
            {
                _target.Initialize();
            }

            if (_target.Initialized && GUILayout.Button("Cut"))
            {
                _target.Cut();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}