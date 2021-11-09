using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.UIEffects
{
    /// <summary>
    /// Represent a line
    /// </summary>
    public struct Line2D
    {
        private Vector2 startPoint;
        public Vector2 StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                if (value == endPoint)
                    throw new ArgumentException("Invalid startPoint");
                startPoint = value;
            }
        }

        private Vector2 endPoint;
        public Vector2 EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                if (value == startPoint)
                    throw new ArgumentException("Invalid endPoint");
                endPoint = value;
            }
        }

        public Vector2 Direction
        {
            get
            {
                return (EndPoint - StartPoint).normalized;
            }
        }

        public Line2D(Vector2 start, Vector2 end)
        {
            if (start == end)
                throw new ArgumentException("Invalid startPoint and endPoint");
            startPoint = start;
            endPoint = end;
        }

        public Line2D(float x1, float y1, float x2, float y2)
        {
            Vector2 start = new Vector2(x1, y1);
            Vector2 end = new Vector2(x2, y2);
            if (start == end)
                throw new ArgumentException("Invalid startPoint and endPoint");
            startPoint = start;
            endPoint = end;
        }

        public float GetX(float y)
        {
            Vector2 dir = EndPoint - StartPoint;
            float a = -dir.y;
            float b = dir.x;
            float c = -(a * StartPoint.x + b * StartPoint.y);
            float x = (-b * y - c) / a;
            return x;
        }

        public float GetY(float x)
        {
            Vector2 dir = EndPoint - StartPoint;
            float a = -dir.y;
            float b = dir.x;
            float c = -(a * StartPoint.x + b * StartPoint.y);
            float y = (-a * x - c) / b;
            return y;
        }

        public static bool Intersect(Line2D l1, Line2D l2, out Vector2 point)
        {
            bool result = false;
            float x1 = l1.StartPoint.x;
            float x2 = l1.EndPoint.x;
            float x3 = l2.StartPoint.x;
            float x4 = l2.EndPoint.x;
            float y1 = l1.StartPoint.y;
            float y2 = l1.EndPoint.y;
            float y3 = l2.StartPoint.y;
            float y4 = l2.EndPoint.y;

            float denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (denominator == 0)
            {
                point = Vector2.zero;
                result = false;
            }
            else
            {
                float xNumerator = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
                float yNumerator = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
                point = new Vector2(xNumerator / denominator, yNumerator / denominator);
                result = true;
            }

            return result;
        }

        public Vector2 Reflect(Vector2 point)
        {
            Vector2 projectedPoint = Project(point);
            Vector2 reflectedPoint = 2 * projectedPoint - point;
            return reflectedPoint;
        }

        public Vector2 Project(Vector2 point)
        {
            Vector2 unnormalizedDirection = EndPoint-StartPoint;
            Vector2 normal = new Vector2(-unnormalizedDirection.y, unnormalizedDirection.x);
            Line2D perpendicularLine = new Line2D(point, point + normal);
            Vector2 intersection = Vector2.zero;
            Intersect(this, perpendicularLine, out intersection);
            return intersection;
        }
    }
}
