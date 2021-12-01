using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Pinwheel.UIEffects
{
    [System.Serializable]
    public class Mirror
    {
        [SerializeField]
        private RectTransform.Edge direction;
        public RectTransform.Edge Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        [SerializeField]
        private float offset;
        public float Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        [SerializeField]
        private AnimationCurve verticalFalloff;
        public AnimationCurve VerticalFalloff
        {
            get
            {
                if (verticalFalloff == null)
                {
                    verticalFalloff = AnimationCurve.Linear(0, 1, 1, 1);
                }
                return verticalFalloff;
            }
            set
            {
                verticalFalloff = value;
            }
        }

        [SerializeField]
        private AnimationCurve horiazontalFalloff;
        public AnimationCurve HorizontalFalloff
        {
            get
            {
                if (horiazontalFalloff == null)
                {
                    horiazontalFalloff = AnimationCurve.Linear(0, 1, 1, 1);
                }
                return horiazontalFalloff;
            }
            set
            {
                horiazontalFalloff = value;
            }
        }

        [SerializeField]
        private WrapMode falloffMode;
        public WrapMode FalloffMode
        {
            get
            {
                return falloffMode;
            }
            set
            {
                falloffMode = value;
            }
        }

        public Mirror()
        {
            Reset();
        }

        protected void Reset()
        {
            direction = RectTransform.Edge.Bottom;
            offset = 0;
            verticalFalloff = AnimationCurve.Linear(0, 1, 1, 1);
            horiazontalFalloff = AnimationCurve.Linear(0, 1, 1, 1);
        }

        public void ModifyVertexStream(List<UIVertex> baseStream, List<UIVertex> mirrorStream)
        {
            Vector2 bottomLeft = new Vector2(
                Utilities.FindMinValue(baseStream, (v) => v.position.x),
                Utilities.FindMinValue(baseStream, (v) => v.position.y));
            Vector2 topRight = new Vector2(
                Utilities.FindMaxValue(baseStream, (v) => v.position.x),
                Utilities.FindMaxValue(baseStream, (v) => v.position.y));
            Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
            Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);

            Vector2 lineStart =
                Direction == RectTransform.Edge.Left ? bottomLeft :
                Direction == RectTransform.Edge.Top ? topLeft :
                Direction == RectTransform.Edge.Right ? topRight :
                bottomRight;
            Vector2 lineEnd =
                Direction == RectTransform.Edge.Left ? topLeft :
                Direction == RectTransform.Edge.Top ? topRight :
                Direction == RectTransform.Edge.Right ? bottomRight :
                bottomLeft;
            Line2D line = new Line2D(lineStart, lineEnd);

            Vector2 translation =
                Direction == RectTransform.Edge.Left ? Vector2.left * Offset :
                Direction == RectTransform.Edge.Top ? Vector2.up * Offset :
                Direction == RectTransform.Edge.Right ? Vector2.right * Offset :
                Vector2.down * Offset;

            ApplyMirror(mirrorStream, line, translation);
        }

        private void ApplyMirror(List<UIVertex> stream, Line2D line, Vector2 translation)
        {
            if (FalloffMode == WrapMode.Position)
            {
                ApplyMirrorPositionFalloff(stream, line, translation);
            }
            else
            {
                ApplyMirrorUvFalloff(stream, line, translation);
            }
        }

        private void ApplyMirrorPositionFalloff(List<UIVertex> stream, Line2D line, Vector2 translation)
        {
            float minX = Utilities.FindMinValue(stream, v => v.position.x);
            float maxX = Utilities.FindMaxValue(stream, v => v.position.x);
            float minY = Utilities.FindMinValue(stream, v => v.position.y);
            float maxY = Utilities.FindMaxValue(stream, v => v.position.y);

            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float alphaHorizontal = HorizontalFalloff.Evaluate(Mathf.InverseLerp(minX, maxX, v.position.x));
                float alphaVertical = VerticalFalloff.Evaluate(Mathf.InverseLerp(minY, maxY, v.position.y));
                v.color.a = (byte)(v.color.a * alphaHorizontal * alphaVertical);
                v.position = line.Reflect(v.position) + translation;
                stream[i] = v;
            }
        }

        private void ApplyMirrorUvFalloff(List<UIVertex> stream, Line2D line, Vector2 translation)
        {
            float minX = Utilities.FindMinValue(stream, v => v.uv0.x);
            float maxX = Utilities.FindMaxValue(stream, v => v.uv0.x);
            float minY = Utilities.FindMinValue(stream, v => v.uv0.y);
            float maxY = Utilities.FindMaxValue(stream, v => v.uv0.y);

            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float alphaHorizontal = HorizontalFalloff.Evaluate(Mathf.InverseLerp(minX, maxX, v.uv0.x));
                float alphaVertical = VerticalFalloff.Evaluate(Mathf.InverseLerp(minY, maxY, v.uv0.y));
                v.color.a = (byte)(v.color.a * alphaHorizontal * alphaVertical);
                v.position = line.Reflect(v.position) + translation;
                stream[i] = v;
            }
        }
    }
}
