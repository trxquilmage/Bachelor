using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
    public static class OutlineInspectorDrawer
    {
        public static void DrawGUI(Outline target)
        {
            target.OutlineColor = EditorGUILayout.ColorField("Color", target.OutlineColor);
            target.Thickness = EditorGUILayout.FloatField("Thickness", target.Thickness);
        }
    }
}
