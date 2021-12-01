using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    [System.Serializable]
    public class Outline
    {
        [SerializeField]
        private Color outlineColor;
        public Color OutlineColor
        {
            get
            {
                return outlineColor;
            }
            set
            {
                outlineColor = value;
            }
        }

        [SerializeField]
        private float thickness;
        public float Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = Mathf.Max(0, value);
            }
        }

        public Outline()
        {
            Reset();
        }

        public void Reset()
        {
            outlineColor = Color.white;
            thickness = 2;
        }

        public void ModifyVertexStream(List<UIVertex> stream)
        {
            List<UIVertex> bottomLeftStream = new List<UIVertex>();
            List<UIVertex> topLeftStream = new List<UIVertex>();
            List<UIVertex> topRightStream = new List<UIVertex>();
            List<UIVertex> bottomRightStream = new List<UIVertex>();

            Utilities.Copy(stream, bottomLeftStream);
            Utilities.Copy(stream, topLeftStream);
            Utilities.Copy(stream, topRightStream);
            Utilities.Copy(stream, bottomRightStream);

            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex vBottomLeft = bottomLeftStream[i];
                vBottomLeft.position += new Vector3(-1, -1) * Thickness;
                vBottomLeft.color = OutlineColor;
                bottomLeftStream[i] = vBottomLeft;

                UIVertex vTopLeft = topLeftStream[i];
                vTopLeft.position += new Vector3(-1, 1) * Thickness;
                vTopLeft.color = OutlineColor;
                topLeftStream[i] = vTopLeft;

                UIVertex vTopRight = topRightStream[i];
                vTopRight.position += new Vector3(1, 1) * Thickness;
                vTopRight.color = OutlineColor;
                topRightStream[i] = vTopRight;

                UIVertex vBottomRight = bottomRightStream[i];
                vBottomRight.position += new Vector3(1, -1) * Thickness;
                vBottomRight.color = OutlineColor;
                bottomRightStream[i] = vBottomRight;
            }

            stream.Clear();
            stream.AddRange(bottomLeftStream);
            stream.AddRange(topLeftStream);
            stream.AddRange(topRightStream);
            stream.AddRange(bottomRightStream);
        }
    }
}
