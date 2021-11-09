using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    public class WarpInspectorInfo
    {
        public Rect[] cpRects;
        public int selectedRect;

        public WarpInspectorInfo()
        {
            cpRects = new Rect[12];
            selectedRect = -1;
        }
    }

    public enum DeformOperation
    {
        Wave, Warp
    }

    [System.Serializable]
    public class DeformSettings
    {
        public DeformOperation operation;
        public bool enabled;
        public RectTransform.Axis axis;
        public float seed;
        public float amplitude;
        public float frequency;
        public Vector2[] controlPoints;
        public bool showControlPointsFieldInspector;
        public float inspectorControlPointsScale;
        public float inspectorControlPointsScaleMin;
        public float inspectorControlPointsScaleMax;
        public WarpInspectorInfo warpInfo;

        public const int CP_BOTTOM_LEFT = 0;
        public const int CP_BOTTOM_LEFT_H1 = 1;
        public const int CP_BOTTOM_LEFT_H2 = 2;

        public const int CP_TOP_LEFT = 3;
        public const int CP_TOP_LEFT_H1 = 4;
        public const int CP_TOP_LEFT_H2 = 5;

        public const int CP_TOP_RIGHT = 6;
        public const int CP_TOP_RIGHT_H1 = 7;
        public const int CP_TOP_RIGHT_H2 = 8;

        public const int CP_BOTTOM_RIGHT = 9;
        public const int CP_BOTTOM_RIGHT_H1 = 10;
        public const int CP_BOTTOM_RIGHT_H2 = 11;

        public DeformSettings()
        {
            operation = DeformOperation.Wave;
            enabled = true;
            axis = RectTransform.Axis.Horizontal;
            seed = 0;
            amplitude = 10;
            frequency = 10;
            controlPoints = new Vector2[12];
            ApplyTemplate(WarpTemplates.GetDefaultTemplate().Points);
            showControlPointsFieldInspector = false;
            inspectorControlPointsScale = 1;
            inspectorControlPointsScaleMin = 0.5f;
            inspectorControlPointsScaleMax = 4;
            warpInfo = new WarpInspectorInfo();
        }

        public Vector2 ValidateControlPoint(Vector2 cp)
        {
            cp = new Vector2(
                Mathf.Clamp(cp.x, -inspectorControlPointsScaleMax, 1 + inspectorControlPointsScaleMax),
                Mathf.Clamp(cp.y, -inspectorControlPointsScaleMax, 1 + inspectorControlPointsScaleMax));
            return cp;
        }

        public void ApplyTemplate(Vector2[] points)
        {
            int maxIndex = Mathf.Min(controlPoints.Length, points.Length);
            for (int i = 0; i < maxIndex; ++i)
            {
                controlPoints[i] = ValidateControlPoint(points[i]);
            }
        }
    }

    [System.Serializable]
    public class Deform
    {
        [SerializeField]
        private List<DeformSettings> settings;
        public List<DeformSettings> Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new List<DeformSettings>();
                }
                return settings;
            }
            private set
            {
                settings = value;
            }
        }

        public Deform()
        {
            Reset();
        }

        private void Reset()
        {
            Settings.Clear();
        }

        public void ModifyVertexStream(List<UIVertex> stream)
        {
            for (int i = 0; i < Settings.Count; ++i)
            {
                bool enabled = Settings[i].enabled;
                if (!enabled)
                    continue;
                DeformOperation operation = Settings[i].operation;
                if (operation == DeformOperation.Wave)
                {
                    ApplyWave(stream, Settings[i]);
                }
                else if (operation == DeformOperation.Warp)
                {
                    ApplyWarp(stream, Settings[i]);
                }
            }
        }

        public void AddEffect(DeformOperation type)
        {
            DeformSettings s = new DeformSettings();
            s.operation = type;
            Settings.Add(s);
        }

        private void ApplyWave(List<UIVertex> stream, DeformSettings settings)
        {
            float minX = Utilities.FindMinValue(stream, v => v.position.x);
            float maxX = Utilities.FindMaxValue(stream, v => v.position.x);
            float minY = Utilities.FindMinValue(stream, v => v.position.y);
            float maxY = Utilities.FindMaxValue(stream, v => v.position.y);

            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                if (settings.axis == RectTransform.Axis.Vertical)
                {
                    float rad = settings.seed + Mathf.InverseLerp(minY, maxY, v.position.y) * settings.frequency;
                    v.position.x += Mathf.Sin(rad) * settings.amplitude;
                }
                else if (settings.axis == RectTransform.Axis.Horizontal)
                {
                    float rad = settings.seed + Mathf.InverseLerp(minX, maxX, v.position.x) * settings.frequency;
                    v.position.y += Mathf.Sin(rad) * settings.amplitude;
                }
                stream[i] = v;
            }
        }

        private void ApplyWarp(List<UIVertex> stream, DeformSettings settings)
        {
            float minX = Utilities.FindMinValue(stream, v => v.position.x);
            float maxX = Utilities.FindMaxValue(stream, v => v.position.x);
            float minY = Utilities.FindMinValue(stream, v => v.position.y);
            float maxY = Utilities.FindMaxValue(stream, v => v.position.y);
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float fX = Mathf.InverseLerp(minX, maxX, v.position.x);
                float fY = Mathf.InverseLerp(minY, maxY, v.position.y);
                Warp(settings, ref fX, ref fY);
                float x = minX + fX * (maxX - minX);
                float y = minY + fY * (maxY - minY);
                float z = v.position.z;
                v.position = new Vector3(x, y, z);
                stream[i] = v;
            }
        }

        private void Warp(DeformSettings settings, ref float fX, ref float fY)
        {
            Vector2[] cp = settings.controlPoints;

            Vector2 x0Original = new Vector2(fX, 0);
            Vector2 x0 = Bezier.GetPoint(
                cp[DeformSettings.CP_BOTTOM_RIGHT],
                cp[DeformSettings.CP_BOTTOM_RIGHT_H2],
                cp[DeformSettings.CP_BOTTOM_LEFT_H1],
                cp[DeformSettings.CP_BOTTOM_LEFT],
                1 - fX);
            Vector2 x0Offset = (x0 - x0Original) * (1 - fY);

            Vector2 x1Original = new Vector2(fX, 1);
            Vector2 x1 = Bezier.GetPoint(
                cp[DeformSettings.CP_TOP_LEFT],
                cp[DeformSettings.CP_TOP_LEFT_H2],
                cp[DeformSettings.CP_TOP_RIGHT_H1],
                cp[DeformSettings.CP_TOP_RIGHT],
                fX);
            Vector2 x1Offset = (x1 - x1Original) * fY;

            Vector2 y0Original = new Vector2(0, fY);
            Vector2 y0 = Bezier.GetPoint(
                cp[DeformSettings.CP_BOTTOM_LEFT],
                cp[DeformSettings.CP_BOTTOM_LEFT_H2],
                cp[DeformSettings.CP_TOP_LEFT_H1],
                cp[DeformSettings.CP_TOP_LEFT],
                fY);
            Vector2 y0Offset = (y0 - y0Original) * (1 - fX);

            Vector2 y1Original = new Vector2(1, fY);
            Vector2 y1 = Bezier.GetPoint(
                cp[DeformSettings.CP_TOP_RIGHT],
                cp[DeformSettings.CP_TOP_RIGHT_H2],
                cp[DeformSettings.CP_BOTTOM_RIGHT_H1],
                cp[DeformSettings.CP_BOTTOM_RIGHT],
                1 - fY);
            Vector2 y1Offset = (y1 - y1Original) * fX;

            Vector2 origin = new Vector2(fX, fY);
            Vector2 quadDeformOffset = origin - QuadInterpolateXY(
                cp[DeformSettings.CP_BOTTOM_LEFT],
                cp[DeformSettings.CP_TOP_LEFT],
                cp[DeformSettings.CP_TOP_RIGHT],
                cp[DeformSettings.CP_BOTTOM_RIGHT],
                fX, fY);
            Vector2 p = origin + x0Offset + x1Offset + y0Offset + y1Offset + quadDeformOffset;

            fX = p.x;
            fY = p.y;
        }

        private Vector2 QuadInterpolateXY(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float tX, float tY)
        {
            Vector2 x0 = Vector2.Lerp(p0, p3, tX);
            Vector2 x1 = Vector2.Lerp(p1, p2, tX);
            Vector2 z0 = Vector2.Lerp(p0, p1, tY);
            Vector2 z1 = Vector2.Lerp(p3, p2, tY);

            Line2D lineX = new Line2D(x0.x, x0.y, x1.x, x1.y);
            Line2D lineY = new Line2D(z0.x, z0.y, z1.x, z1.y);

            Vector2 intersect = Vector2.zero;
            Line2D.Intersect(lineX, lineY, out intersect);

            return intersect;
        }

        public void MoveItemAtIndexUp(int index)
        {
            if (index <= 0 || index >= Settings.Count)
                return;
            DeformSettings s0 = Settings[index - 1];
            DeformSettings s1 = Settings[index];
            Settings[index - 1] = s1;
            Settings[index] = s0;
        }

        public void MoveItemAtIndexDown(int index)
        {
            if (index < 0 || index >= Settings.Count - 1)
                return;
            DeformSettings s0 = Settings[index];
            DeformSettings s1 = Settings[index + 1];
            Settings[index] = s1;
            Settings[index + 1] = s0;
        }
    }
}
