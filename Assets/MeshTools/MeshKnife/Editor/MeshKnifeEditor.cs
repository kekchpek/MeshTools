using System;
using MeshTools.MeshKnife.Components;
using UnityEditor;
using UnityEngine;

namespace MeshTools.MeshKnife.Editor
{
    [CustomEditor(typeof(MeshKnifeBehaviour))]
    [CanEditMultipleObjects]
    public class MeshKnifeEditor : UnityEditor.Editor
    {

        private const string AskCutConfirmationInEditModeKey = nameof(AskCutConfirmationInEditModeKey);
        
        private static IMeshKnifeBehaviour _target;

        private bool _askCutConfirmationInEditMode;
        
        private void Awake()
        {
            _target = target as IMeshKnifeBehaviour;
            if (_target == null)
                Debug.LogError($"Target is not a {nameof(MeshKnife)}");
            
            _askCutConfirmationInEditMode = Convert.ToBoolean(PlayerPrefs.GetInt(AskCutConfirmationInEditModeKey, 1));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (_target == null)
            {
                return;
            }
            
            if (!_target.BasePointsSet && GUILayout.Button("Initialize"))
            {
                _target.CreateBasePoints();
            }

            var prevVal = _askCutConfirmationInEditMode;
            _askCutConfirmationInEditMode =
                GUILayout.Toggle(_askCutConfirmationInEditMode, "Ask confirmation in EditMode");

            if (_askCutConfirmationInEditMode != prevVal)
            {
                PlayerPrefs.SetInt(AskCutConfirmationInEditModeKey, Convert.ToInt32(_askCutConfirmationInEditMode));
            }

            if (_target.BasePointsSet && GUILayout.Button("Cut"))
            {
                if (Application.isEditor && !EditorApplication.isPlaying && _askCutConfirmationInEditMode)
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