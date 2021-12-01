using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Pinwheel.UIEffects
{
    [System.Serializable]
    public class GradientBlend
    {
        [SerializeField]
        private RectTransform.Axis direction;
        public RectTransform.Axis Direction
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
        private WrapMode wrappingMode;
        public WrapMode WrappingMode
        {
            get
            {
                return wrappingMode;
            }
            set
            {
                wrappingMode = value;
            }
        }

        [SerializeField]
        private BlendMode blendingMode;
        public BlendMode BlendingMode
        {
            get
            {
                return blendingMode;
            }
            set
            {
                blendingMode = value;
            }
        }

        [SerializeField]
        private Gradient colors;
        public Gradient Colors
        {
            get
            {
                if (colors == null)
                    colors = new Gradient();
                return colors;
            }
            set
            {
                colors = value;
            }
        }

        public GradientBlend()
        {
            Reset();
        }

        protected void Reset()
        {
            direction = RectTransform.Axis.Vertical;
            wrappingMode = WrapMode.Position;
            blendingMode = BlendMode.Multiply;
            colors = new Gradient();
            GradientColorKey colorStart = new GradientColorKey(Color.white, 0);
            GradientColorKey colorEnd = new GradientColorKey(Color.white, 1);
            GradientAlphaKey alphaStart = new GradientAlphaKey(1, 0);
            GradientAlphaKey alphaEnd = new GradientAlphaKey(0, 1);
            colors.SetKeys(new GradientColorKey[] { colorStart, colorEnd }, new GradientAlphaKey[] { alphaStart, alphaEnd });
        }

        public void ModifyVertexStream(List<UIVertex> stream, List<UIVertex> overlayStream)
        {
            if (BlendingMode == BlendMode.Multiply)
            {
                ApplyGradient(stream);
            }
            else
            {
                ApplyGradient(overlayStream);
            }
        }

        private void ApplyGradient(List<UIVertex> stream)
        {
            if (Direction == RectTransform.Axis.Horizontal)
            {
                if (WrappingMode == WrapMode.Position)
                {
                    ApplyGradientHorizontalPosition(stream);
                }
                else
                {
                    ApplyGradientHorizontalUV(stream);
                }
            }
            else
            {
                if (WrappingMode == WrapMode.Position)
                {
                    ApplyGradientVerticalPosition(stream);
                }
                else
                {
                    ApplyGradientVerticalUV(stream);
                }
            }
        }

        private void ApplyGradientHorizontalPosition(List<UIVertex> stream)
        {
            float minX = Utilities.FindMinValue(stream, (v) => v.position.x);
            float maxX = Utilities.FindMaxValue(stream, (v) => v.position.x);
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float f = Mathf.InverseLerp(minX, maxX, v.position.x);
                Color c = Colors.Evaluate(f);
                v.color *= c;
                stream[i] = v;
            }
        }

        private void ApplyGradientHorizontalUV(List<UIVertex> stream)
        {
            float minX = Utilities.FindMinValue(stream, (v) => v.uv0.x);
            float maxX = Utilities.FindMaxValue(stream, (v) => v.uv0.x);
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float f = Mathf.InverseLerp(minX, maxX, v.uv0.x);
                Color c = Colors.Evaluate(f);
                v.color *= c;
                stream[i] = v;
            }
        }

        private void ApplyGradientVerticalPosition(List<UIVertex> stream)
        {
            float minY = Utilities.FindMinValue(stream, (v) => v.position.y);
            float maxY = Utilities.FindMaxValue(stream, (v) => v.position.y);
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float f = Mathf.InverseLerp(minY, maxY, v.position.y);
                Color c = Colors.Evaluate(f);
                v.color *= c;
                stream[i] = v;
            }
        }

        private void ApplyGradientVerticalUV(List<UIVertex> stream)
        {
            float minY = Utilities.FindMinValue(stream, (v) => v.uv0.y);
            float maxY = Utilities.FindMaxValue(stream, (v) => v.uv0.y);
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                float f = Mathf.InverseLerp(minY, maxY, v.uv0.y);
                Color c = Colors.Evaluate(f);
                v.color *= c;
                stream[i] = v;
            }
        }

        private void ResetUV(List<UIVertex> stream)
        {
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                v.uv0 = -Vector2.one;
                stream[i] = v;
            }
        }
    }
}
