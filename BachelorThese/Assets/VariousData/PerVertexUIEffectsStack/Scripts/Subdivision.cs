using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.UIEffects;

namespace Pinwheel.UIEffects
{
    [System.Serializable]
    public class Subdivision
    {
        [SerializeField]
        private int subDivision;
        public int SubDivision
        {
            get
            {
                return subDivision;
            }
            set
            {
                subDivision = Mathf.Clamp(value, 0, 5);
            }
        }

        public Subdivision()
        {
            Reset();
        }

        public void Reset()
        {
            subDivision = 1;
        }

        public void ModifyVertexStream(List<UIVertex> stream)
        {
            for (int subDivideIndex = 0; subDivideIndex < subDivision; ++subDivideIndex)
            {
                List<UIVertex> newStream = new List<UIVertex>();
                int trisCount = stream.Count / 3;
                for (int trisIndex = 0; trisIndex < trisCount; ++trisIndex)
                {
                    UIVertex v0 = stream[trisIndex * 3 + 0];
                    UIVertex v1 = stream[trisIndex * 3 + 1];
                    UIVertex v2 = stream[trisIndex * 3 + 2];
                    UIVertex v01 = Utilities.GetMidPoint(v0, v1);
                    UIVertex v12 = Utilities.GetMidPoint(v1, v2);
                    UIVertex v20 = Utilities.GetMidPoint(v2, v0);

                    newStream.AddRange(new UIVertex[]
                    {
                        v0, v01, v20,
                        v1, v12, v01,
                        v2, v20, v12,
                        v01, v12, v20
                    });
                }
                stream.Clear();
                stream.AddRange(newStream);
            }
        }
    }
}
