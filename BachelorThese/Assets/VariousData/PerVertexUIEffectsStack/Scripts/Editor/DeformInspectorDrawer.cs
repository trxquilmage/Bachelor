using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.UIEffects
{
    public static class DeformInspectorDrawer
    {
        public static void DrawGUI(Deform instance)
        {
            int toRemoveItemIndex = -1;
            int toMoveUpItemIndex = -1;
            int toMoveDownItemIndex = -1;

            for (int i = 0; i < instance.Settings.Count; ++i)
            {
                DeformSettings settings = instance.Settings[i];
                settings.enabled = EditorGUILayout.Toggle("Enabled", settings.enabled);
                settings.operation = (DeformOperation)EditorGUILayout.EnumPopup("Operation", settings.operation);
                if (settings.operation == DeformOperation.Wave)
                {
                    DrawWaveGUI(settings);
                }
                else if (settings.operation == DeformOperation.Warp)
                {
                    DrawWarpGUI(settings);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.enabled = i > 0;
                if (EditorCommon.TinyButton(EditorCommon.UP_TRIANGLE))
                {
                    toMoveUpItemIndex = i;
                }
                GUI.enabled = i < instance.Settings.Count - 1;
                if (EditorCommon.TinyButton(EditorCommon.DOWN_TRIANGLE))
                {
                    toMoveDownItemIndex = i;
                }
                GUI.enabled = true;
                if (EditorCommon.Button("Remove"))
                {
                    toRemoveItemIndex = ConfirmRemoveItemAt(i, settings);
                }
                EditorGUILayout.EndHorizontal();

                EditorCommon.Separator(true);
            }

            if (EditorCommon.RightAnchoredButton("Add"))
            {
                ShowAddContextMenu(instance);
            }

            if (toRemoveItemIndex >= 0 && toRemoveItemIndex < instance.Settings.Count)
            {
                instance.Settings.RemoveAt(toRemoveItemIndex);
            }
            if (toMoveUpItemIndex >= 0 && toMoveUpItemIndex < instance.Settings.Count)
            {
                instance.MoveItemAtIndexUp(toMoveUpItemIndex);
            }
            if (toMoveDownItemIndex >= 0 && toMoveDownItemIndex < instance.Settings.Count)
            {
                instance.MoveItemAtIndexDown(toMoveDownItemIndex);
            }
        }

        private static void ShowAddContextMenu(Deform instance)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Wave"),
                false,
                () => { instance.AddEffect(DeformOperation.Wave); });
            menu.AddItem(
                new GUIContent("Warp"),
                false,
                () => { instance.AddEffect(DeformOperation.Warp); });
            menu.ShowAsContext();
        }

        private static int ConfirmRemoveItemAt(int index, DeformSettings info)
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                string.Format("Remove {0}?", info.operation.ToString()),
                "OK", "Cancel"))
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        private static void DrawWaveGUI(DeformSettings settings)
        {
            settings.axis = (RectTransform.Axis)EditorGUILayout.EnumPopup("Axis", settings.axis);
            settings.seed = EditorGUILayout.FloatField("Seed", settings.seed);
            settings.amplitude = EditorGUILayout.FloatField("Amplitude", settings.amplitude);
            settings.frequency = EditorGUILayout.FloatField("Frequency", settings.frequency);
        }

        private static void DrawWarpGUI(DeformSettings settings)
        {
            settings.showControlPointsFieldInspector = EditorGUILayout.Toggle("Show Control Points Fields", settings.showControlPointsFieldInspector);
            if (settings.showControlPointsFieldInspector)
            {
                for (int i = 0; i < settings.controlPoints.Length - 2; i += 3)
                {
                    string name = 
                        i == 0 ? "Bottom Left" :
                        i == 3 ? "Top Left" :
                        i == 6 ? "Top Right" :
                        "Bottom Right";
                    EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
                    settings.controlPoints[i] = EditorGUILayout.Vector2Field("Position", settings.controlPoints[i]);
                    settings.controlPoints[i + 1] = EditorGUILayout.Vector2Field("Tangent 1", settings.controlPoints[i + 1]);
                    settings.controlPoints[i + 2] = EditorGUILayout.Vector2Field("Tangent 2", settings.controlPoints[i + 2]);
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Rect r = GUILayoutUtility.GetAspectRect(1, GUILayout.MaxWidth(400));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUI.DrawRect(r, EditorCommon.darkGrey);
            GUI.BeginGroup(r);
            DrawGrid(r, settings.inspectorControlPointsScale);
            DrawBeziers(r, settings);
            GUI.EndGroup();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(400));
            if (EditorCommon.Button("Template..."))
            {
                ShowApplyTemplateContextMenu(settings);
            }
            settings.inspectorControlPointsScale = EditorGUILayout.Slider(
                settings.inspectorControlPointsScale,
                settings.inspectorControlPointsScaleMin,
                settings.inspectorControlPointsScaleMax);
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawGrid(Rect r, float scale)
        {
            Vector2 min = Vector2.zero;
            Vector2 max = r.size;
            Vector2 center = (min + max) * 0.5f;
            float tileSize = r.width / (scale * 2 + 1);
            float offset = tileSize * 0.5f;
            int iMin = Mathf.FloorToInt(-scale);
            int iMax = Mathf.CeilToInt(scale);

            //Vertical lines
            for (int i = iMin; i <= iMax; ++i)
            {
                Vector2 start = new Vector2(center.x + i * tileSize - offset, min.y);
                Vector2 end = new Vector2(center.x + i * tileSize - offset, max.y);
                Color c = i == 0 ? Color.white * 0.9f : Color.white * 0.5f;
                EditorCommon.DrawLine(
                    FlipY(r, start),
                    FlipY(r, end),
                    c);

                Vector2 labelOffset = new Vector2(i < 0 ? -16 : -13, 3);
                Vector2 labelPos = FlipY(r, ProjectPoint(r, scale, new Vector2(i, 0))) + labelOffset;
                Color labelColor = i == 0 ? Color.white * 0.7f : Color.white * 0.5f;
                EditorCommon.Label(labelPos, i.ToString(), 12, TextAnchor.UpperLeft, FontStyle.Normal, labelColor);
            }

            //Horizontal lines
            for (int i = iMin; i <= iMax; ++i)
            {
                Vector2 start = new Vector2(min.x, center.y + i * tileSize - offset);
                Vector2 end = new Vector2(max.x, center.y + i * tileSize - offset);
                Color c = i == 0 ? Color.white * 0.9f : Color.white * 0.5f;
                EditorCommon.DrawLine(
                    FlipY(r, start),
                    FlipY(r, end),
                    c);

                Vector2 labelOffset = new Vector2(i < 0 ? -16 : -13, 3);
                Vector2 labelPos = FlipY(r, ProjectPoint(r, scale, new Vector2(0, i))) + labelOffset;
                Color labelColor = i == 0 ? Color.white * 0.7f : Color.white * 0.5f;
                EditorCommon.Label(labelPos, i.ToString(), 12, TextAnchor.UpperLeft, FontStyle.Normal, labelColor);
            }


        }

        private static void DrawBeziers(Rect r, DeformSettings settings)
        {
            float scale = settings.inspectorControlPointsScale;
            Color controlPointColor = Handles.yAxisColor;
            Color selectedControlPointColor = Handles.selectedColor;
            Color bezierColor = Handles.xAxisColor;
            if (settings.warpInfo == null)
                settings.warpInfo = new WarpInspectorInfo();
            WarpInspectorInfo warpInfo = settings.warpInfo;
            #region Bezier curves
            EditorCommon.DrawBezier(
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_LEFT])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_LEFT_H2])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_LEFT_H1])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_LEFT])),
                bezierColor);

            EditorCommon.DrawBezier(
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_LEFT])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_LEFT_H2])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_RIGHT_H1])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_RIGHT])),
                bezierColor);

            EditorCommon.DrawBezier(
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_RIGHT])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_TOP_RIGHT_H2])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_RIGHT_H1])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_RIGHT])),
                bezierColor);

            EditorCommon.DrawBezier(
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_RIGHT])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_RIGHT_H2])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_LEFT_H1])),
                FlipY(r, ProjectPoint(r, scale, settings.controlPoints[DeformSettings.CP_BOTTOM_LEFT])),
                bezierColor);
            #endregion

            for (int i = 0; i < settings.controlPoints.Length - 2; i += 3)
            {
                Vector2 controlPoint = settings.controlPoints[i];
                Vector2 projCP = ProjectPoint(r, scale, controlPoint);
                Vector2 flipProjCP = FlipY(r, projCP);
                EditorCommon.DrawSolidCircle(
                    flipProjCP,
                    8,
                    i == warpInfo.selectedRect ? selectedControlPointColor : controlPointColor);
                warpInfo.cpRects[i] = new Rect() { size = Vector2.one * 8 * 2, center = flipProjCP };

                Vector2 handle1 = settings.controlPoints[i + 1];
                Vector2 projH1 = ProjectPoint(r, scale, handle1);
                Vector2 flipProjH1 = FlipY(r, projH1);
                EditorCommon.DrawLine(flipProjCP, flipProjH1, controlPointColor);
                EditorCommon.DrawSolidCircleWithOutline(
                    flipProjH1,
                    4,
                    EditorCommon.darkGrey,
                    i == warpInfo.selectedRect ? selectedControlPointColor : controlPointColor);
                warpInfo.cpRects[i + 1] = new Rect() { size = Vector2.one * 4 * 2, center = flipProjH1 };

                Vector2 handle2 = settings.controlPoints[i + 2];
                Vector2 projH2 = ProjectPoint(r, scale, handle2);
                Vector2 flipProjH2 = FlipY(r, projH2);
                EditorCommon.DrawLine(flipProjCP, flipProjH2, controlPointColor);
                EditorCommon.DrawSolidCircleWithOutline(
                    flipProjH2,
                    4,
                    EditorCommon.darkGrey,
                    i == warpInfo.selectedRect ? selectedControlPointColor : controlPointColor);
                warpInfo.cpRects[i + 2] = new Rect() { size = Vector2.one * 4 * 2, center = flipProjH2 };
            }

            DrawControlPointPosition(settings);
            HandleBezierEditing(warpInfo);
            UpdateBezier(r, settings);
        }

        private static void DrawControlPointPosition(DeformSettings settings)
        {
            for (int i = 0; i < settings.warpInfo.cpRects.Length; ++i)
            {
                Rect r = settings.warpInfo.cpRects[i];
                if (r.Contains(Event.current.mousePosition))
                {
                    Vector2 cp = settings.controlPoints[i];
                    string label = string.Format(
                        "({0},{1})",
                        cp.x.ToString("0.00"),
                        cp.y.ToString("0.00"));
                    Vector2 offset = Vector2.one * 10;
                    EditorCommon.Label(
                        Event.current.mousePosition + offset,
                        label,
                        10,
                        TextAnchor.UpperLeft,
                        FontStyle.Normal,
                        Color.white * 0.9f);
                    return;
                }
            }
        }

        public static void HandleBezierEditing(WarpInspectorInfo warpInfo)
        {
            if (GuiEventUtilities.IsLeftMouseDown)
            {
                GetSelectedCpRect(warpInfo);
            }
            else if (GuiEventUtilities.IsLeftMouse && warpInfo.selectedRect >= 0)
            {
                Vector2 mouseDelta = Event.current.delta;
                warpInfo.cpRects[warpInfo.selectedRect].position += mouseDelta;
            }
            else if (GuiEventUtilities.IsLeftMouseUp)
            {
                Event.current.Use();
            }
        }

        public static void UpdateBezier(Rect referenceRect, DeformSettings settings)
        {
            if (!GuiEventUtilities.IsLeftMouse)
                return;
            WarpInspectorInfo warpInfo = settings.warpInfo;
            if (warpInfo.selectedRect >= 0)
            {
                Rect r = warpInfo.cpRects[warpInfo.selectedRect];
                float scale = settings.inspectorControlPointsScale;
                settings.controlPoints[warpInfo.selectedRect] = InverseProjectPoint(referenceRect, scale, FlipY(referenceRect, r.center));
                settings.controlPoints[warpInfo.selectedRect] = settings.ValidateControlPoint(settings.controlPoints[warpInfo.selectedRect]);
                GUI.changed = true;
            }
        }

        public static void GetSelectedCpRect(WarpInspectorInfo warpInfo)
        {
            warpInfo.selectedRect = -1;
            for (int i = warpInfo.cpRects.Length - 1; i >= 0; --i)
            {
                Rect r = warpInfo.cpRects[i];
                if (r.Contains(Event.current.mousePosition))
                {
                    warpInfo.selectedRect = i;
                    return;
                }
            }
        }

        private static Vector2 ProjectPoint(Rect r, float scale, Vector2 point)
        {
            Vector2 center = r.size * 0.5f;
            float tileCount = scale * 2 + 1;
            float tileSize = r.width / tileCount;
            Vector2 offset = point * tileSize;
            Vector2 result = center + offset - Vector2.one * tileSize * 0.5f;

            return result;
        }

        private static Vector2 InverseProjectPoint(Rect r, float scale, Vector2 projPoint)
        {
            Vector2 center = r.size * 0.5f;
            float tileCount = scale * 2 + 1;
            float tileSize = r.width / tileCount;
            Vector2 point = (projPoint - center + Vector2.one * tileSize * 0.5f) / tileSize;

            return point;
        }

        private static Vector2 FlipY(Rect r, Vector2 p)
        {
            float f = Mathf.InverseLerp(0, r.size.y, p.y);
            p.y = Mathf.Lerp(0, r.size.y, 1 - f);
            return p;
        }

        private static void ShowApplyTemplateContextMenu(DeformSettings settings)
        {
            List<WarpTemplates> templates = WarpTemplates.GetTemplates();
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < templates.Count; ++i)
            {
                int templateIndex = i;
                menu.AddItem(
                    new GUIContent(templates[i].Name),
                    false,
                    () =>
                    {
                        settings.ApplyTemplate(templates[templateIndex].Points);
                        GUI.changed = true;
                    });
            }
            menu.ShowAsContext();
        }
    }
}
