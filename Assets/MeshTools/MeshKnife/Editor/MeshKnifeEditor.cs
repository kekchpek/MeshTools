using System;
using UnityEditor;
using UnityEngine;

namespace MeshTools.MeshKnife.Editor
{
    [CustomEditor(typeof(global::MeshTools.MeshTools.MeshKnife.MeshKnife))]
    [CanEditMultipleObjects]
    public class MeshKnifeEditor : UnityEditor.Editor
    {

        private const string AskCutConfirmationInEditModeKey = nameof(AskCutConfirmationInEditModeKey);
        
        private static IMeshKnife _target;

        private bool _askCutConfirmationInEditMode;
        
        private void Awake()
        {
            _target = target as IMeshKnife;
            if (_target == null)
                Debug.LogError($"Target is not a {nameof(MeshTools.MeshKnife.MeshKnife)}");
            
            _askCutConfirmationInEditMode = Convert.ToBoolean(PlayerPrefs.GetInt(AskCutConfirmationInEditModeKey, 1));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (!_target.Initialized && GUILayout.Button("Initialize"))
            {
                _target.Initialize();
            }

            var prevVal = _askCutConfirmationInEditMode;
            _askCutConfirmationInEditMode =
                GUILayout.Toggle(_askCutConfirmationInEditMode, "Ask confirmation in EditMode");

            if (_askCutConfirmationInEditMode != prevVal)
            {
                PlayerPrefs.SetInt(AskCutConfirmationInEditModeKey, Convert.ToInt32(_askCutConfirmationInEditMode));
            }

            if (_target.Initialized && GUILayout.Button("Cut"))
            {
                if (_askCutConfirmationInEditMode)
                {
                    if (EditorUtility.DisplayDialog("Confirm cut in edit mode", 
                        "Are you sure you want to cut a mesh in edit mode?", "Cut", "Cancel"))
                    {
                        _target.Cut();
                    }
                }
                else
                {
                    _target.Cut();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}