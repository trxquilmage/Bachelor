using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    /// <summary>
    /// Utility class to get custom GUI style
    /// </summary>
    public static class GuiStyleUtilities
    {
        private static GUIStyle handlesLabelStyle;
        public static GUIStyle HandlesLabelStyle
        {
            get
            {
                if (handlesLabelStyle == null)
                {
                    handlesLabelStyle = new GUIStyle();
                    handlesLabelStyle.normal.textColor = Color.green;
                    handlesLabelStyle.alignment = TextAnchor.MiddleCenter;
                }
                return handlesLabelStyle;
            }
        }

        private static GUIStyle overlayInstructionStyle;
        public static GUIStyle OverlayInstructionStyle
        {
            get
            {
                if (overlayInstructionStyle == null)
                {
                    overlayInstructionStyle = new GUIStyle();
                    overlayInstructionStyle.alignment = TextAnchor.MiddleRight;
                    overlayInstructionStyle.normal.textColor = Color.white * 0.9f;
                    overlayInstructionStyle.richText = true;

                    Texture2D bg = new Texture2D(1, 1);
                    bg.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
                    bg.Apply();

                    overlayInstructionStyle.normal.background = bg;
                    overlayInstructionStyle.padding = new RectOffset(5, 10, 5, 5);
                }
                return overlayInstructionStyle;
            }
        }

        private static GUIStyle bigBoldWhiteLabel;
        public static GUIStyle BigBoldWhiteLabel
        {
            get
            {
                if (bigBoldWhiteLabel == null)
                {
                    bigBoldWhiteLabel = new GUIStyle();
                    bigBoldWhiteLabel.normal.textColor = Color.white * 0.9f;
                    bigBoldWhiteLabel.fontStyle = FontStyle.Bold;
                    bigBoldWhiteLabel.fontSize = 20;
                }
                return bigBoldWhiteLabel;
            }
        }

        private static GUIStyle inspectorGroupFoldout;
        public static GUIStyle InspectorGroupFoldout
        {
            get
            {
                if (inspectorGroupFoldout == null)
                {
                    inspectorGroupFoldout = new GUIStyle();
                    inspectorGroupFoldout.fontStyle = FontStyle.Bold;
                    inspectorGroupFoldout.alignment = TextAnchor.MiddleLeft;
                    inspectorGroupFoldout.padding = new RectOffset(3, 3, 0, 0);
                    inspectorGroupFoldout.normal.background = InspectorGroupBackground;
                    inspectorGroupFoldout.active.background = InspectorGroupBackgroundActive;
                    inspectorGroupFoldout.border = new RectOffset(2, 2, 2, 2);
                }
                return inspectorGroupFoldout;
            }
        }

        private static Texture2D inspectorGroupBackground;
        public static Texture2D InspectorGroupBackground
        {
            get
            {
                if (inspectorGroupBackground == null)
                {
                    int w = 32;
                    int h = 32;
                    Texture2D t = new Texture2D(w, h);
                    Color bgColor = new Color32(164, 164, 164, 255);
                    Color borderColor = new Color32(128, 128, 128, 255);

                    Color[] colors = new Color[w * h];
                    for (int x = 0; x < w; ++x)
                    {
                        for (int y = 0; y < h; ++y)
                        {
                            Color c = Color.clear;
                            if (y == 0 || y == h - 1)
                                c = borderColor;
                            else if (x == 0 || x == w - 1)
                                c = borderColor;
                            else
                                c = bgColor;
                            colors[y * w + x] = c;
                        }
                    }

                    t.SetPixels(colors);
                    t.Apply();
                    inspectorGroupBackground = t;
                }
                return inspectorGroupBackground;
            }
        }

        private static Texture2D inspectorGroupBackgroundActive;
        public static Texture2D InspectorGroupBackgroundActive
        {
            get
            {
                if (inspectorGroupBackgroundActive == null)
                {
                    int w = 32;
                    int h = 32;
                    Texture2D t = new Texture2D(w, h);
                    Color bgColor = new Color32(140, 140, 140, 255);
                    Color borderColor = new Color32(128, 128, 128, 255);
                    Color[] colors = new Color[w * h];
                    for (int x = 0; x < w; ++x)
                    {
                        for (int y = 0; y < h; ++y)
                        {
                            Color c = Color.clear;
                            if (y == 0 || y == h - 1)
                                c = borderColor;
                            else if (x == 0 || x == w - 1)
                                c = borderColor;
                            else
                                c = bgColor;
                            colors[y * w + x] = c;
                        }
                    }

                    t.SetPixels(colors);
                    t.Apply();
                    inspectorGroupBackgroundActive = t;
                }
                return inspectorGroupBackgroundActive;
            }
        }

        private static GUIStyle rightAlignedWhiteLabel;
        public static GUIStyle RightAlignedWhiteLabel
        {
            get
            {
                if (rightAlignedWhiteLabel == null)
                    rightAlignedWhiteLabel = new GUIStyle();
                rightAlignedWhiteLabel.normal.textColor = Color.white * 0.9f;
                rightAlignedWhiteLabel.alignment = TextAnchor.MiddleRight;
                return rightAlignedWhiteLabel;
            }
        }

        private static GUIStyle rightAlignedWhiteMiniLabel;
        public static GUIStyle RightAlignedWhiteMiniLabel
        {
            get
            {
                if (rightAlignedWhiteMiniLabel == null)
                    rightAlignedWhiteMiniLabel = new GUIStyle();
                rightAlignedWhiteMiniLabel.normal.textColor = Color.white * 0.9f;
                rightAlignedWhiteMiniLabel.alignment = TextAnchor.MiddleRight;
                rightAlignedWhiteMiniLabel.fontSize = 8;
                return rightAlignedWhiteMiniLabel;
            }
        }
    }
}
