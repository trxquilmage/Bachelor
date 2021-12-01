using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
	public static class GradientBlendInspectorDrawer
	{
        public static void DrawGUI(UIEffectsProfile profile)
        {
            profile.GradientBlender.Direction = (RectTransform.Axis)EditorGUILayout.EnumPopup("Direction", profile.GradientBlender.Direction);
            profile.GradientBlender.BlendingMode = (BlendMode)EditorGUILayout.EnumPopup("Blend Mode", profile.GradientBlender.BlendingMode);
            profile.GradientBlender.WrappingMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", profile.GradientBlender.WrappingMode);

            SerializedObject so = new SerializedObject(profile);
            SerializedProperty sp = so.FindProperty("gradientBlender").FindPropertyRelative("colors");
            EditorGUILayout.PropertyField(sp, new GUIContent("Colors"));
            so.ApplyModifiedProperties();
        }
	}
}
