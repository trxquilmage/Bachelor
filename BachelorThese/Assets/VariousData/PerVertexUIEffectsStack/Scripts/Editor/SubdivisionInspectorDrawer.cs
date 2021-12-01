using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
	public static class SubdivisionInspectorDrawer
	{
        public static void DrawGUI(Subdivision target)
        {
            target.SubDivision = EditorGUILayout.IntSlider("Step", target.SubDivision, 0, 5);
        }
	}
}
