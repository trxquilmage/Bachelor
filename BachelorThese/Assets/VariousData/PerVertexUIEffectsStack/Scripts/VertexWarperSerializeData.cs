using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    /// <summary>
    /// Wrap everything into a class to deal with serailization
    /// </summary>
    [System.Serializable]
    public class VertexWarperSerializeData
    {
        public bool modifySilently;
        public float y;
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public Vector3[] controlPointsInfo;

        public static bool IsFresh(VertexWarperSerializeData data)
        {
            return 
                data == null ||
                data.controlPointsInfo == null ||
                data.controlPointsInfo.Length == 0;
        }
    }

}
