using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    public partial class VertexWarper
    {

        public enum Template
        {
            Default, Custom, Ellipse, Pillow, Barrel, Pincushion, ArcForward, ArcBackward, RoundedDiamond
        }

        /// <summary>
        /// Utility class to create warping template
        /// </summary>
        public static class TemplateMaker
        {
            /// <summary>
            /// Get a template using its name
            /// </summary>
            /// <param name="template"></param>
            /// <param name="y"></param>
            /// <param name="minX"></param>
            /// <param name="maxX"></param>
            /// <param name="minZ"></param>
            /// <param name="maxZ"></param>
            /// <returns></returns>
            public static Vector3[] GetTemplate(Template template, float y, float minX, float maxX, float minZ, float maxZ)
            {
                if (template == Template.Default)
                    return GetDefaultTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.Ellipse)
                    return GetEllipseTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.Pillow)
                    return GetPillowTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.Barrel)
                    return GetBarrelTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.Pincushion)
                    return GetPincushionTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.ArcForward)
                    return GetArcForwardTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.ArcBackward)
                    return GetArcBackwardTemplate(y, minX, maxX, minZ, maxZ);
                else if (template == Template.RoundedDiamond)
                    return GetRoundedDiamondTemplate(y, minX, maxX, minZ, maxZ);
                else
                    return null;
            }

            public static Vector3[] GetDefaultTemplate(float z, float minX, float maxX, float minY, float maxY)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                Vector3 bottomLeft = new Vector3(minX, minY, z);
                Vector3 topLeft = new Vector3(minX, maxY, z);
                Vector3 topRight = new Vector3(maxX, maxY, z);
                Vector3 bottomRight = new Vector3(maxX, minY, z);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeY = Mathf.Abs(maxY - minY);
                Vector3 xOffset = Vector3.right * sizeX / 3.0f;
                Vector3 yOffset = Vector3.up * sizeY / 3.0f;

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottomLeft + xOffset;
                points[i * controlPointDataCount + 2] = bottomLeft + yOffset;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = topLeft - yOffset;
                points[i * controlPointDataCount + 2] = topLeft + xOffset;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = topRight - xOffset;
                points[i * controlPointDataCount + 2] = topRight - yOffset;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = bottomRight + yOffset;
                points[i * controlPointDataCount + 2] = bottomRight - xOffset;

                return points;
            }

            public static Vector3[] GetEllipseTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                Vector3 bottomLeft = new Vector3(minX, y, minZ);
                Vector3 topLeft = new Vector3(minX, y, maxZ);
                Vector3 topRight = new Vector3(maxX, y, maxZ);
                Vector3 bottomRight = new Vector3(maxX, y, minZ);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeZ = Mathf.Abs(maxZ - minZ);

                float whRatio = sizeX / sizeZ;
                float betaAngle = 90 / (1 + whRatio);
                float alphaAngle = 90 - betaAngle;
                float tetaRadian;

                tetaRadian = alphaAngle * Mathf.Deg2Rad / 2;
                float lengthMultiplier = (1 - Mathf.Sin(tetaRadian) / Mathf.Sin(3 * tetaRadian));

                Vector3 left = (bottomLeft + topLeft) * 0.5f + Vector3.left * sizeZ * 0.5f * Mathf.Tan(alphaAngle * Mathf.Deg2Rad);
                Vector3 left0 = Vector3.Lerp(bottomLeft, left, lengthMultiplier);
                Vector3 left1 = Vector3.Lerp(topLeft, left, lengthMultiplier);

                Vector3 right = (bottomRight + topRight) * 0.5f + Vector3.right * sizeZ * 0.5f * Mathf.Tan(alphaAngle * Mathf.Deg2Rad);
                Vector3 right0 = Vector3.Lerp(topRight, right, lengthMultiplier);
                Vector3 right1 = Vector3.Lerp(bottomRight, right, lengthMultiplier);

                tetaRadian = betaAngle * Mathf.Deg2Rad / 2;
                lengthMultiplier = (1 - Mathf.Sin(tetaRadian) / Mathf.Sin(3 * tetaRadian));

                Vector3 top = (topLeft + topRight) * 0.5f + Vector3.forward * sizeX * 0.5f * Mathf.Tan(betaAngle * Mathf.Deg2Rad);
                Vector3 top0 = Vector3.Lerp(topLeft, top, lengthMultiplier);
                Vector3 top1 = Vector3.Lerp(topRight, top, lengthMultiplier);

                Vector3 bottom = (bottomLeft + bottomRight) * 0.5f + Vector3.back * sizeX * 0.5f * Mathf.Tan(betaAngle * Mathf.Deg2Rad);
                Vector3 bottom0 = Vector3.Lerp(bottomRight, bottom, lengthMultiplier);
                Vector3 bottom1 = Vector3.Lerp(bottomLeft, bottom, lengthMultiplier);

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottom1;
                points[i * controlPointDataCount + 2] = left0;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = left1;
                points[i * controlPointDataCount + 2] = top0;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = top1;
                points[i * controlPointDataCount + 2] = right0;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = right1;
                points[i * controlPointDataCount + 2] = bottom0;

                return points;
            }

            public static Vector3[] GetPillowTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                Vector3 bottomLeft = new Vector3(minX, y, minZ);
                Vector3 topLeft = new Vector3(minX, y, maxZ);
                Vector3 topRight = new Vector3(maxX, y, maxZ);
                Vector3 bottomRight = new Vector3(maxX, y, minZ);

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottomLeft;
                points[i * controlPointDataCount + 2] = bottomLeft;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = topLeft;
                points[i * controlPointDataCount + 2] = topLeft;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = topRight;
                points[i * controlPointDataCount + 2] = topRight;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = bottomRight;
                points[i * controlPointDataCount + 2] = bottomRight;

                return points;
            }

            public static Vector3[] GetBarrelTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                Vector3 bottomLeft = new Vector3(minX, y, minZ);
                Vector3 topLeft = new Vector3(minX, y, maxZ);
                Vector3 topRight = new Vector3(maxX, y, maxZ);
                Vector3 bottomRight = new Vector3(maxX, y, minZ);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeZ = Mathf.Abs(maxZ - minZ);
                Vector3 xOffset = Vector3.right * sizeX / 3.0f;
                Vector3 zOffset = Vector3.forward * sizeZ / 3.0f;
                Vector3 xMinorOffset = xOffset / 3.0f;
                Vector3 zMinorOffset = zOffset / 3.0f;

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottomLeft + xOffset - zMinorOffset;
                points[i * controlPointDataCount + 2] = bottomLeft + zOffset - xMinorOffset;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = topLeft - zOffset - xMinorOffset;
                points[i * controlPointDataCount + 2] = topLeft + xOffset + zMinorOffset;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = topRight - xOffset + zMinorOffset;
                points[i * controlPointDataCount + 2] = topRight - zOffset + xMinorOffset;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = bottomRight + zOffset + xMinorOffset;
                points[i * controlPointDataCount + 2] = bottomRight - xOffset - zMinorOffset;

                return points;
            }

            public static Vector3[] GetPincushionTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                Vector3 bottomLeft = new Vector3(minX, y, minZ);
                Vector3 topLeft = new Vector3(minX, y, maxZ);
                Vector3 topRight = new Vector3(maxX, y, maxZ);
                Vector3 bottomRight = new Vector3(maxX, y, minZ);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeZ = Mathf.Abs(maxZ - minZ);
                Vector3 xOffset = Vector3.right * sizeX / 3.0f;
                Vector3 zOffset = Vector3.forward * sizeZ / 3.0f;
                Vector3 xMinorOffset = xOffset / 3.0f;
                Vector3 zMinorOffset = zOffset / 3.0f;

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottomLeft + xOffset + zMinorOffset;
                points[i * controlPointDataCount + 2] = bottomLeft + zOffset + xMinorOffset;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = topLeft - zOffset + xMinorOffset;
                points[i * controlPointDataCount + 2] = topLeft + xOffset - zMinorOffset;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = topRight - xOffset - zMinorOffset;
                points[i * controlPointDataCount + 2] = topRight - zOffset - xMinorOffset;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = bottomRight + zOffset - xMinorOffset;
                points[i * controlPointDataCount + 2] = bottomRight - xOffset + zMinorOffset;

                return points;
            }

            public static Vector3[] GetArcForwardTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                float sin = Mathf.Sin(45 * Mathf.Deg2Rad);
                float cos = Mathf.Cos(45 * Mathf.Deg2Rad);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeZ = Mathf.Abs(maxZ - minZ);
                Vector3 bottomLeft = new Vector3(minX, y, minZ);
                Vector3 bottomRight = new Vector3(maxX, y, minZ);
                Vector3 topLeft = bottomLeft + new Vector3(-cos, 0, sin) * sizeZ;
                Vector3 topRight = bottomRight + new Vector3(cos, 0, sin) * sizeZ;

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottomLeft + new Vector3(cos, 0, sin) * Mathf.Min(sizeX, sizeZ) / 3.0f;
                points[i * controlPointDataCount + 2] = bottomLeft + new Vector3(-cos, 0, sin) * sizeZ / 3.0f;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = topLeft + new Vector3(cos, 0, -sin) * sizeZ / 3.0f;
                points[i * controlPointDataCount + 2] = topLeft + new Vector3(cos, 0, sin) * sizeZ;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = topRight + new Vector3(-cos, 0, sin) * sizeZ;
                points[i * controlPointDataCount + 2] = topRight + new Vector3(-cos, 0, -sin) * sizeZ / 3.0f;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = bottomRight + new Vector3(cos, 0, sin) * sizeZ / 3.0f;
                points[i * controlPointDataCount + 2] = bottomRight + new Vector3(-cos, 0, sin) * Mathf.Min(sizeX, sizeZ) / 3.0f;

                return points;
            }

            public static Vector3[] GetArcBackwardTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                float sin = Mathf.Sin(45 * Mathf.Deg2Rad);
                float cos = Mathf.Cos(45 * Mathf.Deg2Rad);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeZ = Mathf.Abs(maxZ - minZ);
                Vector3 topLeft = new Vector3(minX, y, maxZ);
                Vector3 topRight = new Vector3(maxX, y, maxZ);
                Vector3 bottomLeft = topLeft + new Vector3(-cos, 0, -sin) * sizeZ;
                Vector3 bottomRight = topRight + new Vector3(cos, 0, -sin) * sizeZ;

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottomLeft + new Vector3(cos, 0, -sin) * sizeZ;
                points[i * controlPointDataCount + 2] = bottomLeft + new Vector3(cos, 0, sin) * sizeZ / 3.0f;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = topLeft + new Vector3(-cos, 0, -sin) * sizeZ / 3.0f;
                points[i * controlPointDataCount + 2] = topLeft + new Vector3(cos, 0, -sin) * Mathf.Min(sizeX, sizeZ) / 3.0f;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = topRight + new Vector3(-cos, 0, -sin) * Mathf.Min(sizeX, sizeZ) / 3.0f;
                points[i * controlPointDataCount + 2] = topRight + new Vector3(cos, 0, -sin) * sizeZ / 3.0f;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = bottomRight + new Vector3(-cos, 0, sin) * sizeZ / 3.0f;
                points[i * controlPointDataCount + 2] = bottomRight + new Vector3(-cos, 0, -sin) * sizeZ;

                return points;
            }

            public static Vector3[] GetRoundedDiamondTemplate(float y, float minX, float maxX, float minZ, float maxZ)
            {
                int controlPointCount = VertexWarper.CONTROL_POINT_COUNT;
                int controlPointDataCount = Bezier.ControlPoint.PROPERTY_COUNT;
                Vector3[] points = new Vector3[controlPointCount * controlPointDataCount];

                Vector3 bottomLeft = new Vector3(minX, y, minZ);
                Vector3 topLeft = new Vector3(minX, y, maxZ);
                Vector3 topRight = new Vector3(maxX, y, maxZ);
                Vector3 bottomRight = new Vector3(maxX, y, minZ);
                float sizeX = Mathf.Abs(maxX - minX);
                float sizeZ = Mathf.Abs(maxZ - minZ);

                float whRatio = sizeX / sizeZ;
                float betaAngle = 90 / (1 + whRatio);
                float alphaAngle = 90 - betaAngle;

                Vector3 left = (bottomLeft + topLeft) * 0.5f + Vector3.left * sizeZ * 0.5f * Mathf.Tan(alphaAngle * Mathf.Deg2Rad);
                Vector3 right = (bottomRight + topRight) * 0.5f + Vector3.right * sizeZ * 0.5f * Mathf.Tan(alphaAngle * Mathf.Deg2Rad);

                Vector3 top = (topLeft + topRight) * 0.5f + Vector3.forward * sizeX * 0.5f * Mathf.Tan(betaAngle * Mathf.Deg2Rad);
                Vector3 bottom = (bottomLeft + bottomRight) * 0.5f + Vector3.back * sizeX * 0.5f * Mathf.Tan(betaAngle * Mathf.Deg2Rad);

                int i;
                i = 0;
                points[i * controlPointDataCount + 0] = bottomLeft;
                points[i * controlPointDataCount + 1] = bottom;
                points[i * controlPointDataCount + 2] = left;

                i = 1;
                points[i * controlPointDataCount + 0] = topLeft;
                points[i * controlPointDataCount + 1] = left;
                points[i * controlPointDataCount + 2] = top;

                i = 2;
                points[i * controlPointDataCount + 0] = topRight;
                points[i * controlPointDataCount + 1] = top;
                points[i * controlPointDataCount + 2] = right;

                i = 3;
                points[i * controlPointDataCount + 0] = bottomRight;
                points[i * controlPointDataCount + 1] = right;
                points[i * controlPointDataCount + 2] = bottom;

                return points;
            }
        }
    }
}
