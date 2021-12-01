using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;

namespace Pinwheel.UIEffects
{
    internal class CompileLogger
    {
        private const string PACKAGE_NAME = "Per-Vertex UI Effects Stack";
        private const string PACKAGE_NAME_PLACEHOLDER = "${PACKAGE_NAME}";

        private const string WEBSITE = "http://pinwheel.studio";
        private const string WEBSITE_PLACEHOLDER = "${WEBSITE}";

        private const string PATREON = "https://www.patreon.com/tntam";
        private const string PATREON_PLACEHOLDER = "${PATREON}";

        private const string LINK_COLOR = "blue";
        private const string LINK_COLOR_PLACEHOLDER = "${LC}";

        private const float LOG_MESSAGE_PROBABIILITY = 0.02F;


        private static void ValidatePackageAndNamespace()
        {
            bool isPackageNameInvalid = PACKAGE_NAME.Equals("PACKAGE_NAME");
            bool isNamespaceInvalid = typeof(CompileLogger).Namespace.Contains("PACKAGE_NAME");
            if (isPackageNameInvalid)
            {
                string message = "<color=red>Invalid PACKAGE_NAME in CompileLogger, fix it before release!</color>";
                Debug.Log(message);
            }
            if (isNamespaceInvalid)
            {
                string message = "<color=red>Invalid NAMESPACE in CompileLogger, fix it before release!</color>";
                Debug.Log(message);
            }
        }
    }
}