using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

namespace Pinwheel.UIEffects
{
    [CustomEditor(typeof(UIEffectsProfile))]
    public class UIEffectsProfileInspector : Editor
    {
        private UIEffectsProfile instance;

        private void OnEnable()
        {
            instance = (UIEffectsProfile)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(instance), typeof(MonoScript), false);
            EditorGUILayout.Space();
            GUI.enabled = true;

            DrawEffectFoldout(ref instance.showSubdivisionSettings, ref instance.useSubdivision, "Subdivision", instance.Subdivisor);
            if (instance.showSubdivisionSettings)
            {
                EditorGUI.indentLevel += 1;
                SubdivisionInspectorDrawer.DrawGUI(instance.Subdivisor);
                EditorGUI.indentLevel -= 1;
            }

            DrawEffectFoldout(ref instance.showDeformSettings, ref instance.useDeform, "Deform", instance.Deformer);
            if (instance.showDeformSettings)
            {
                EditorGUI.indentLevel += 1;
                DeformInspectorDrawer.DrawGUI(instance.Deformer);
                EditorGUI.indentLevel -= 1;
            }

            DrawEffectFoldout(ref instance.showShadowSettings, ref instance.useShadow, "Shadow", instance.Shadower);
            if (instance.showShadowSettings)
            {
                EditorGUI.indentLevel += 1;
                ShadowInspectorDrawer.DrawGUI(instance.Shadower);
                EditorGUI.indentLevel -= 1;
            }

            DrawEffectFoldout(ref instance.showOutlineSettings, ref instance.useOutline, "Outline", instance.Outliner);
            if (instance.showOutlineSettings)
            {
                EditorGUI.indentLevel += 1;
                OutlineInspectorDrawer.DrawGUI(instance.Outliner);
                EditorGUI.indentLevel -= 1;
            }

            DrawEffectFoldout(ref instance.showGradientSettings, ref instance.useGradient, "Gradient", instance.GradientBlender);
            if (instance.showGradientSettings)
            {
                EditorGUI.indentLevel += 1;
                GradientBlendInspectorDrawer.DrawGUI(instance);
                EditorGUI.indentLevel -= 1;
            }

            DrawEffectFoldout(ref instance.showMirrorSettings, ref instance.useMirror, "Mirror", instance.Mirror);
            if (instance.showMirrorSettings)
            {
                EditorGUI.indentLevel += 1;
                MirrorInspectorDrawer.DrawGUI(instance.Mirror);
                EditorGUI.indentLevel -= 1;
            }

            if (EditorGUI.EndChangeCheck())
            {
                instance.SetEffectsDirty();
            }

            EditorUtility.SetDirty(instance);
        }

        private void DrawEffectFoldout(ref bool foldOut, ref bool isUsed, string name, object effect)
        {
            string label = string.Format("   {0}", name);
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(EditorCommon.standardHeight));

            Rect toggleRect = new Rect(r.x, r.y, r.height, r.height);
            isUsed = EditorGUI.Toggle(toggleRect, isUsed);

            Rect resetRect = new Rect(r.max.x - 35, r.min.y, 35, r.height);
            if (GUI.Button(resetRect, ""))
            {
                ResetEffect(effect);
            }

            if (GUI.Button(r, label, GuiStyleUtilities.InspectorGroupFoldout))
            {
                foldOut = !foldOut;
            }

            EditorCommon.DrawToggleNonInteractive(toggleRect, isUsed);
            EditorGUI.LabelField(resetRect, "Reset", EditorStyles.miniLabel);
        }

        private void ResetEffect(object effect)
        {
            MethodInfo mi = effect.GetType().GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi != null)
            {
                mi.Invoke(effect, null);
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}
