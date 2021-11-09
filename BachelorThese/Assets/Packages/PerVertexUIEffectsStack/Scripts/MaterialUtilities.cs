using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
	public static class MaterialUtilities
	{
        private static Material uifxDefault;
        public static Material UIFXDefault
        {
            get
            {
                if (uifxDefault==null)
                {
                    uifxDefault = new Material(Shader.Find("UIFX/Default"));
                }
                return uifxDefault;
            }
        }
	}
}
