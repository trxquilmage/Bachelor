using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
	public static class ShadowInspectorDrawer
	{
        public static void DrawGUI(Shadow target)
        {
            target.Opacity = EditorGUILayout.Slider("Opacity", target.Opacity, 0f, 1f);
            target.Distance = EditorGUILayout.Vector2Field("Distance", target.Distance);
            target.UseGraphicAlpha = EditorGUILayout.Toggle("Use Graphic Alpha", target.UseGraphicAlpha);
        }
	}
}
