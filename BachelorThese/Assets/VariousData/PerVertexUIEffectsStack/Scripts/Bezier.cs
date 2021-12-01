using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Pinwheel.UIEffects
{
    /// <summary>
    /// Represent a Bezier curve
    /// </summary>
    public partial class Bezier
    {
#if UNITY_EDITOR
        /// <summary>
        /// Resolution of bezier curve in Scene view
        /// </summary>
        public const int GIZMOS_SEGMENTS_COUNT = 20;
#endif
         
        public delegate void ControlPointsChangedHandler(List<ControlPoint> poinst);
        /// <summary>
        /// Fired when any control points position has changed
        /// </summary>
        public event ControlPointsChangedHandler ControlPointsChanged;

        /// <summary>
        /// List of control points at 2 ends
        /// </summary>
        private List<ControlPoint> controlPoints;
        public List<ControlPoint> ControlPoints
        {
            get
            {
                if (controlPoints == null)
                {
                    CreateDefaultControlPoints();
                }
                return controlPoints;
            }
            set
            {
                if (value != null && value.Count != 2)
                    throw new System.ArgumentException("Invalid ControlPoints collection, must be 2 points.");
                List<ControlPoint> oldValue = controlPoints;
                List<ControlPoint> newValue = value;
                controlPoints = newValue;
                RegisterControlPointsModifiedEvent();
                if (oldValue != newValue && ControlPointsChanged != null)
                    ControlPointsChanged(controlPoints);
            }
        }

        public Bezier()
        {
            ControlPoints = null;
        }

        public Bezier(IEnumerable<ControlPoint> points)
        {
            ControlPoints = new List<ControlPoint>(points);
        }

        public Bezier(ControlPoint p0, ControlPoint p1)
        {
            ControlPoint[] points = new ControlPoint[2] { p0, p1 };
            ControlPoints = new List<ControlPoint>(points);
        }

        private void CreateDefaultControlPoints()
        {
            List<ControlPoint> tmpPoints = new List<ControlPoint>();
            tmpPoints.Add(new ControlPoint(Vector3.zero, new Vector3[2] { Vector3.left, Vector3.right }));
            tmpPoints.Add(new ControlPoint(Vector3.forward, new Vector3[2] { Vector3.forward + Vector3.left, Vector3.forward + Vector3.right }));
            ControlPoints = tmpPoints;
        }

        /// <summary>
        /// Register a notification when control points has been modified, such as re-position the handles
        /// </summary>
        private void RegisterControlPointsModifiedEvent()
        {
            for (int i = 0; i < ControlPoints.Count; ++i)
            {
                ControlPoints[i].Modified += OnControlPointModified;
            }
        }

        private void OnControlPointModified(ControlPoint sender)
        {
            if (ControlPointsChanged != null)
                ControlPointsChanged(ControlPoints);
        }

        public static Vector3 GetPoint(Vector3 cp0, Vector3 h1, Vector3 h2, Vector3 cp1, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1 - t;
            Vector3 p =
                oneMinusT * oneMinusT * oneMinusT * cp0 +
                3 * oneMinusT * oneMinusT * t * h1 +
                3 * oneMinusT * t * t * h2 +
                t * t * t * cp1;
            return p;
        }

        /// <summary>
        /// Get a point on the bezier
        /// </summary>
        /// <param name="t">Fraction, from 0 to 1</param>
        /// <returns></returns>
        public Vector3 GetPoint(float t)
        {
            Vector3 p0 = ControlPoints[0].Position;
            Vector3 p1 = ControlPoints[0].Handles[1];
            Vector3 p2 = ControlPoints[1].Handles[0];
            Vector3 p3 = ControlPoints[1].Position;

            t = Mathf.Clamp01(t);
            float oneMinusT = 1 - t;
            Vector3 p =
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3 * oneMinusT * oneMinusT * t * p1 +
                3 * oneMinusT * t * t * p2 +
                t * t * t * p3;
            return p;
        }

        /// <summary>
        /// Get a collection of points on the bezier, use to draw the bezier onto the screen
        /// </summary>
        /// <param name="segmentation"></param>
        /// <returns></returns>
        public Vector3[] GetPoints(int segmentation)
        {
            if (segmentation <= 0)
                throw new ArgumentException("Invalid segmentation, must >= 1");
            int pointCount = segmentation + 1;
            Vector3[] points = new Vector3[pointCount];
            float step = 1.0f / segmentation;
            Vector3 p0 = ControlPoints[0].Position;
            Vector3 p1 = ControlPoints[0].Handles[1];
            Vector3 p2 = ControlPoints[1].Handles[0];
            Vector3 p3 = ControlPoints[1].Position;
            float t = 0;

            for (int i = 0; i < pointCount; ++i)
            {
                t = Mathf.Clamp01(i * step);
                float oneMinusT = 1 - t;
                Vector3 p =
                    oneMinusT * oneMinusT * oneMinusT * p0 +
                    3 * oneMinusT * oneMinusT * t * p1 +
                    3 * oneMinusT * t * t * p2 +
                    t * t * t * p3;
                points[i] = p;
            }

            return points;
        }

        /// <summary>
        /// Draw the bezier in Scene view
        /// </summary>
        public void DrawHandles(Transform transform)
        {
#if UNITY_EDITOR
            Handles.color = Color.green;
            Vector3[] p = GetPoints(GIZMOS_SEGMENTS_COUNT);
            for (int i = 0; i < p.Length - 1; ++i)
            {
                Vector3 start = transform.TransformPoint(p[i]);
                Vector3 end = transform.TransformPoint(p[i + 1]);
                //Handles.DrawLine(p[i], p[i + 1]);
                Handles.DrawLine(start, end);
            }
            ControlPoints[0].DrawHandles(transform);
            ControlPoints[1].DrawHandles(transform);
#endif
        }
    }
}
