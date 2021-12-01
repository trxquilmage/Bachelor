using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
    [CustomEditor(typeof(UIEffectStack))]
	public class UIEffectsStackInspector : Editor
	{
        UIEffectStack instance;

        private void OnEnable()
        {
            instance = (UIEffectStack)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(instance), typeof(MonoScript), false);
            GUI.enabled = true;

            instance.Profile = EditorGUILayout.ObjectField("Profile", instance.Profile, typeof(UIEffectsProfile), false) as UIEffectsProfile;
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(instance);
                instance.SendMessage("OnValidate", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
