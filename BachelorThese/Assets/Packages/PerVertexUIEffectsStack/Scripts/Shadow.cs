using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    [System.Serializable]
    public class Shadow
    {
        [SerializeField]
        private float opacity;
        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                opacity = value;
            }
        }

        [SerializeField]
        private Vector2 distance;
        public Vector2 Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        [SerializeField]
        private bool useGraphicAlpha;
        public bool UseGraphicAlpha
        {
            get
            {
                return useGraphicAlpha;
            }
            set
            {
                useGraphicAlpha = value;
            }
        }

        public Shadow()
        {
            Reset();
        }

        public void Reset()
        {
            opacity = 0.5f;
            distance = new Vector2(2, -2);
            useGraphicAlpha = true;
        }

        public void ModifyVertexStream(List<UIVertex> stream)
        {
            for (int i = 0; i < stream.Count; ++i)
            {
                UIVertex v = stream[i];
                v.position += (Vector3)Distance;
                float alpha = UseGraphicAlpha ?
                    (v.color.a * 1.0f / 255) * Opacity :
                    Opacity;

                v.color = new Color(0, 0, 0, alpha);
                stream[i] = v;
            }
        }
    }
}
