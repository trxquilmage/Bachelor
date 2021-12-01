using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using EditorHandles = UnityEditor.Handles;
#endif

namespace Pinwheel.UIEffects
{
    public partial class Bezier
    {
        /// <summary>
        /// Represent a control point of the bezier curve
        /// </summary>
        public class ControlPoint
        {
            /// <summary>
            /// Fired when its position or handles position has been modified
            /// </summary>
            /// <param name="sender"></param>
            public delegate void ModifiedHandler(ControlPoint sender);
            public event ModifiedHandler Modified;

            /// <summary>
            /// How many serializable property contains in this class, used with a custom serialize mechanic
            /// </summary>
            public const int PROPERTY_COUNT = 3;

            /// <summary>
            /// Position of the control point in local space
            /// </summary>
            private Vector3 position;
            public Vector3 Position
            {
                get
                {
                    return position;
                }
                set
                {
                    Vector3 oldValue = position;
                    Vector3 newValue = value;
                    position = newValue;
                    if (oldValue != newValue && Modified != null)
                        Modified(this);
                }
            }

            /// <summary>
            /// Position of the handles in local space
            /// </summary>
            private Vector3[] handles;
            public Vector3[] Handles
            {
                get
                {
                    if (handles == null || handles.Length < 2)
                        handles = new Vector3[2];
                    return handles;
                }
                set
                {

                    if (value != null && value.Length != 2)
                        throw new System.ArgumentException("Invalid Handles array, only accept 2 handles");
                    Vector3[] oldValue = handles;
                    Vector3[] newValue = value;
                    handles = newValue;
                    if (oldValue != newValue && Modified != null)
                        Modified(this);
                }
            }

            public ControlPoint(Vector3 position, Vector3[] handles)
            {
                Position = position;
                Handles = handles;
            }

            public ControlPoint(Vector3 position, Vector3 handle0, Vector3 handle1)
            {
                Position = position;
                Handles = new Vector3[2] { handle0, handle1 };
            }

            /// <summary>
            /// Translate a control point, this will also moves its handles
            /// </summary>
            /// <param name="translation"></param>
            public void Translate(Vector3 translation)
            {
                Position += translation;
                for (int i = 0; i < Handles.Length; ++i)
                {
                    SetHandle(i, Handles[i] + translation);
                }
            }

            /// <summary>
            /// Move the control point to a new location, also move its handles relatively
            /// </summary>
            /// <param name="destination"></param>
            public void MoveTo(Vector3 destination)
            {
                Translate(destination - Position);
            }

            /// <summary>
            /// Set a handle's position
            /// </summary>
            /// <param name="index"></param>
            /// <param name="position"></param>
            public void SetHandle(int index, Vector3 position)
            {
                Vector3 oldValue = Handles[index];
                Vector3 newValue = position;
                Handles[index] = newValue;
                if (oldValue != newValue && Modified != null)
                    Modified(this);
            }

            /// <summary>
            /// Draw the control point onto Scene view
            /// </summary>
            public void DrawHandles(Transform transform)
            {
#if UNITY_EDITOR
                bool isCtrlKeyHold = Event.current != null && Event.current.control;
                bool isShiftKeyHold = Event.current != null && Event.current.shift;

                //drag to move control point's position
                EditorHandles.color = Color.blue;
                Vector3 oldPosition = transform.TransformPoint(Position);
                Vector3 newPosition = EditorHandles.FreeMoveHandle(
                    oldPosition,
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(oldPosition) * 0.2f,
                    Vector3.zero,
                    EditorHandles.CircleHandleCap);
                newPosition.z = oldPosition.z; //restrict Z-axis movement
                if (newPosition != oldPosition)
                {
                    if (isShiftKeyHold)
                    {
                        newPosition = SnapPosition(newPosition);
                    }
                    if (isCtrlKeyHold)
                    {
                        Translate(newPosition - oldPosition);
                    }
                    else
                    {
                        Position = transform.InverseTransformPoint(newPosition);
                    }
                }

                //drag to move handle's position
                EditorHandles.color = Color.red;
                for (int i = 0; i < Handles.Length; ++i)
                {
                    Vector3 handleOldPosition = transform.TransformPoint(Handles[i]);
                    Vector3 handleNewPosition = EditorHandles.FreeMoveHandle(
                        handleOldPosition,
                        Quaternion.identity,
                        HandleUtility.GetHandleSize(handleOldPosition) * 0.1f,
                        Vector3.zero,
                        EditorHandles.RectangleHandleCap);
                    handleNewPosition.z = handleOldPosition.z; //restrict Z-axis movement

                    if (handleOldPosition != handleNewPosition)
                    {
                        if (isShiftKeyHold)
                        {
                            handleNewPosition = SnapHandle(handleNewPosition);
                        }
                        SetHandle(i, transform.InverseTransformPoint(handleNewPosition));
                        if (isCtrlKeyHold)
                        {
                            Vector3 reflectedPosition = handleNewPosition + 2 * (transform.TransformPoint(Position) - handleNewPosition);
                            SetHandle(Handles.Length - 1 - i, transform.InverseTransformPoint(reflectedPosition));
                        }
                    }
                    EditorHandles.DrawLine(handleNewPosition, transform.TransformPoint(Position));
                }
#endif
            }

            /// <summary>
            /// Snap handle position
            /// </summary>
            /// <param name="newPosition"></param>
            /// <returns></returns>
            public Vector3 SnapPosition(Vector3 newPosition)
            {
                newPosition.x = Mathf.Round(newPosition.x);
                newPosition.y = Mathf.Round(newPosition.y);
                newPosition.z = Mathf.Round(newPosition.z);
                return newPosition;
            }

            /// <summary>
            /// Snap handles angle
            /// </summary>
            /// <param name="handleNewPosition"></param>
            /// <param name="snapAngle"></param>
            /// <returns></returns>
            public Vector3 SnapHandle(Vector3 handleNewPosition, float snapAngle = 15)
            {
                Vector3 handleDirection = handleNewPosition - Position;
                float angle = 0;
                float dot = Vector3.Dot(handleDirection, Vector3.right);
                if (dot >= 0)
                {
                    angle = (dot < 0 ? 180 : 0) + Vector3.Angle(Vector3.forward, handleDirection);
                }
                else
                {
                    angle = 180 + Vector3.Angle(Vector3.back, handleDirection);
                }
                angle = Utilities.GetNearestMultiple(angle, snapAngle);

                float length = Mathf.Max(
                    Mathf.Abs(handleNewPosition.x - Position.x),
                    Mathf.Abs(handleNewPosition.z - Position.z));
                handleNewPosition.x = Mathf.Cos(angle * Mathf.Deg2Rad) * length + Position.x;
                handleNewPosition.z = Mathf.Sin(angle * Mathf.Deg2Rad) * length + Position.z;

                return handleNewPosition;
            }
        }
    }
}
