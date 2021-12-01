using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
    public static class MirrorInspectorDrawer
    {
        public static void DrawGUI(Mirror target)
        {
            target.Direction = (RectTransform.Edge)EditorGUILayout.EnumPopup("Direction", target.Direction);
            target.FalloffMode = (WrapMode)EditorGUILayout.EnumPopup("Falloff Mode", target.FalloffMode);
            Rect range = new Rect(Vector2.zero, Vector2.one);
            target.VerticalFalloff = EditorGUILayout.CurveField("Vertical Falloff", target.VerticalFalloff, Handles.yAxisColor, range);
            target.HorizontalFalloff = EditorGUILayout.CurveField("Horizontal Falloff", target.HorizontalFalloff, Handles.xAxisColor, range);
            target.Offset = EditorGUILayout.FloatField("Offset", target.Offset);
        }
    }
}
