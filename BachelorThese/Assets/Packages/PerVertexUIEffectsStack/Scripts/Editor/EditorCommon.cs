using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pinwheel.UIEffects
{
    public static class EditorCommon
    {
        public const string UP_TRIANGLE = "▲";
        public const string DOWN_TRIANGLE = "▼";
        public const string LEFT_TRIANGLE = "◄";
        public const string RIGHT_TRIANGLE = "►";

        public static float standardHeight = EditorGUIUtility.singleLineHeight;
        public static float tinyHeight = EditorGUIUtility.singleLineHeight;

        public static float standardWidth = 100;
        public static float standardWidthExtent = 200;
        public static float tinyWidth = EditorGUIUtility.singleLineHeight * 1.5f;

        public static Color oddItemColor = new Color32(190, 190, 190, 255);
        public static Color evenItemColor = new Color32(180, 180, 180, 255);
        public static Color selectedItemColor = new Color32(100, 120, 255, 255);
        public static Color criticalItemColor = new Color32(200, 0, 0, 255);
        public static Color lightGrey = new Color32(176, 176, 176, 255);
        public static Color midGrey = new Color32(128, 128, 128, 255);
        public static Color darkGrey = new Color32(64, 64, 64, 255);
        public static Color linkColor = new Color(0, 0, 238, 255); //#0000EE

        private static GUIStyle centeredLabel;
        public static GUIStyle CenteredLabel
        {
            get
            {
                if (centeredLabel == null)
                {
                    centeredLabel = new GUIStyle();
                    centeredLabel.alignment = TextAnchor.MiddleCenter;
                }

                return centeredLabel;
            }
        }

        private static GUIStyle centeredWhiteLabel;
        public static GUIStyle CenteredWhiteLabel
        {
            get
            {
                if (centeredWhiteLabel == null)
                {
                    centeredWhiteLabel = new GUIStyle();
                    centeredWhiteLabel.alignment = TextAnchor.MiddleCenter;
                    centeredWhiteLabel.normal.textColor = Color.white;
                }

                return centeredWhiteLabel;
            }
        }

        private static GUIStyle linkLabel;
        public static GUIStyle LinkLabel
        {
            get
            {
                if (linkLabel == null)
                {
                    linkLabel = new GUIStyle(CenteredLabel);
                    linkLabel.normal.textColor = linkColor;
                }
                return linkLabel;
            }
        }

        public static GUIStyle BoldLabel
        {
            get
            {
                return EditorStyles.boldLabel;
            }
        }

        private static GUIStyle italicLabel;
        public static GUIStyle ItalicLabel
        {
            get
            {
                if (italicLabel == null)
                {
                    italicLabel = new GUIStyle();
                    italicLabel.fontStyle = FontStyle.Italic;
                    italicLabel.alignment = TextAnchor.MiddleLeft;
                }
                return italicLabel;
            }
        }

        private static GUIStyle rightAlignedItalicLabel;
        public static GUIStyle RightAlignedItalicLabel
        {
            get
            {
                if (rightAlignedItalicLabel == null)
                {
                    rightAlignedItalicLabel = new GUIStyle(ItalicLabel);
                    rightAlignedItalicLabel.alignment = TextAnchor.MiddleRight;
                }
                return rightAlignedItalicLabel;
            }
        }

        private static Texture2D sliderBackgroundTexture;
        public static Texture2D SliderBackgroundTexture
        {
            get
            {
                if (sliderBackgroundTexture == null)
                {
                    //sliderBackgroundTexture = EditorGUIUtility.Load("icons/sv_icon_dot0_sml.png") as Texture2D;
                    sliderBackgroundTexture = Texture2D.whiteTexture;
                }
                return sliderBackgroundTexture;
            }
        }

        private static Texture2D warningIcon;
        public static Texture2D WarningIcon
        {
            get
            {
                if (warningIcon == null)
                {
                    warningIcon = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
                }
                return warningIcon;
            }
        }

        private static Texture2D oddItemTexture1x1;
        public static Texture2D OddItemTexture1x1
        {
            get
            {
                if (oddItemTexture1x1 == null)
                {
                    oddItemTexture1x1 = new Texture2D(1, 1);
                    oddItemTexture1x1.SetPixels(new Color[] { oddItemColor });
                    oddItemTexture1x1.Apply();
                }
                return oddItemTexture1x1;
            }
        }

        private static Texture2D oddItemHoveredTexture1x1;
        public static Texture2D OddItemHoveredTexture1x1
        {
            get
            {
                if (oddItemHoveredTexture1x1 == null)
                {
                    oddItemHoveredTexture1x1 = new Texture2D(1, 1);
                    oddItemHoveredTexture1x1.SetPixels(new Color[] { oddItemColor * 1.2f });
                    oddItemHoveredTexture1x1.Apply();
                }
                return oddItemHoveredTexture1x1;
            }
        }

        private static Texture2D oddItemClickedTexture1x1;
        public static Texture2D OddItemClickedTexture1x1
        {
            get
            {
                if (oddItemClickedTexture1x1 == null)
                {
                    oddItemClickedTexture1x1 = new Texture2D(1, 1);
                    oddItemClickedTexture1x1.SetPixels(new Color[] { selectedItemColor * 1.2f });
                    oddItemClickedTexture1x1.Apply();
                }
                return oddItemClickedTexture1x1;
            }
        }

        private static Texture2D evenItemTexture1x1;
        public static Texture2D EvenItemTexture1x1
        {
            get
            {
                if (evenItemTexture1x1 == null)
                {
                    evenItemTexture1x1 = new Texture2D(1, 1);
                    evenItemTexture1x1.SetPixels(new Color[] { evenItemColor });
                    evenItemTexture1x1.Apply();
                }
                return evenItemTexture1x1;
            }
        }

        private static Texture2D evenItemHoveredTexture1x1;
        public static Texture2D EvenItemHoveredTexture1x1
        {
            get
            {
                if (evenItemHoveredTexture1x1 == null)
                {
                    evenItemHoveredTexture1x1 = new Texture2D(1, 1);
                    evenItemHoveredTexture1x1.SetPixels(new Color[] { evenItemColor * 1.2f });
                    evenItemHoveredTexture1x1.Apply();
                }
                return evenItemHoveredTexture1x1;
            }
        }

        private static Texture2D evenItemClickedTexture1x1;
        public static Texture2D EvenItemClickedTexture1x1
        {
            get
            {
                if (evenItemClickedTexture1x1 == null)
                {
                    evenItemClickedTexture1x1 = new Texture2D(1, 1);
                    evenItemClickedTexture1x1.SetPixels(new Color[] { selectedItemColor * 1.2f });
                    evenItemClickedTexture1x1.Apply();
                }
                return evenItemClickedTexture1x1;
            }
        }

        private static Texture2D selectedItemTexture1x1;
        public static Texture2D SelectedItemTexture1x1
        {
            get
            {
                if (selectedItemTexture1x1 == null)
                {
                    selectedItemTexture1x1 = new Texture2D(1, 1);
                    selectedItemTexture1x1.SetPixels(new Color[] { selectedItemColor });
                    selectedItemTexture1x1.Apply();
                }
                return selectedItemTexture1x1;
            }
        }

        private static Texture2D selectedItemHoveredTexture1x1;
        public static Texture2D SelectedItemHoveredTexture1x1
        {
            get
            {
                if (selectedItemHoveredTexture1x1 == null)
                {
                    selectedItemHoveredTexture1x1 = new Texture2D(1, 1);
                    selectedItemHoveredTexture1x1.SetPixels(new Color[] { selectedItemColor * 1.2f });
                    selectedItemHoveredTexture1x1.Apply();
                }
                return selectedItemHoveredTexture1x1;
            }
        }

        private static Texture2D selectedItemClickedTexture1x1;
        public static Texture2D SelectedItemClickedTexture1x1
        {
            get
            {
                if (selectedItemClickedTexture1x1 == null)
                {
                    selectedItemClickedTexture1x1 = new Texture2D(1, 1);
                    selectedItemClickedTexture1x1.SetPixels(new Color[] { selectedItemColor * 1.4f });
                    selectedItemClickedTexture1x1.Apply();
                }
                return selectedItemClickedTexture1x1;
            }
        }

        private static GUIStyle oddFlatButton;
        public static GUIStyle OddFlatButton
        {
            get
            {
                if (oddFlatButton == null)
                {
                    oddFlatButton = new GUIStyle();
                    oddFlatButton.alignment = TextAnchor.MiddleCenter;
                    oddFlatButton.normal.background = OddItemTexture1x1;
                    oddFlatButton.hover.background = OddItemHoveredTexture1x1;
                    oddFlatButton.active.background = OddItemClickedTexture1x1;
                }
                return oddFlatButton;
            }
        }

        private static GUIStyle evenFlatButton;
        public static GUIStyle EvenFlatButton
        {
            get
            {
                if (evenFlatButton == null)
                {
                    evenFlatButton = new GUIStyle();
                    evenFlatButton.alignment = TextAnchor.MiddleCenter;
                    evenFlatButton.normal.background = EvenItemTexture1x1;
                    evenFlatButton.hover.background = EvenItemHoveredTexture1x1;
                    evenFlatButton.active.background = EvenItemClickedTexture1x1;
                }
                return evenFlatButton;
            }
        }

        private static GUIStyle selectedFlatButton;
        public static GUIStyle SelectedFlatButton
        {
            get
            {
                if (selectedFlatButton == null)
                {
                    selectedFlatButton = new GUIStyle();
                    selectedFlatButton.alignment = TextAnchor.MiddleCenter;
                    selectedFlatButton.normal.background = SelectedItemTexture1x1;
                    selectedFlatButton.hover.background = SelectedItemHoveredTexture1x1;
                    selectedFlatButton.active.background = SelectedItemClickedTexture1x1;
                }
                return selectedFlatButton;
            }
        }

        private static Texture2D criticalItemTexture1x1;
        public static Texture2D CriticalItemTexture1x1
        {
            get
            {
                if (criticalItemTexture1x1 == null)
                {
                    criticalItemTexture1x1 = new Texture2D(1, 1);
                    criticalItemTexture1x1.SetPixels(new Color[] { criticalItemColor });
                    criticalItemTexture1x1.Apply();
                }
                return criticalItemTexture1x1;
            }
        }

        private static Texture2D criticalItemHoveredTexture1x1;
        public static Texture2D CriticalItemHoveredTexture1x1
        {
            get
            {
                if (criticalItemHoveredTexture1x1 == null)
                {
                    criticalItemHoveredTexture1x1 = new Texture2D(1, 1);
                    criticalItemHoveredTexture1x1.SetPixels(new Color[] { criticalItemColor * 1.2f });
                    criticalItemHoveredTexture1x1.Apply();
                }
                return criticalItemHoveredTexture1x1;
            }
        }

        private static Texture2D criticalItemClickedTexture1x1;
        public static Texture2D CriticalItemClickedTexture1x1
        {
            get
            {
                if (criticalItemClickedTexture1x1 == null)
                {
                    criticalItemClickedTexture1x1 = new Texture2D(1, 1);
                    criticalItemClickedTexture1x1.SetPixels(new Color[] { criticalItemColor * 1.4f });
                    criticalItemClickedTexture1x1.Apply();
                }
                return criticalItemClickedTexture1x1;
            }
        }

        private static GUIStyle criticalFlatButton;
        public static GUIStyle CriticalFlatButton
        {
            get
            {
                if (criticalFlatButton == null)
                {
                    criticalFlatButton = new GUIStyle();
                    criticalFlatButton.alignment = TextAnchor.MiddleCenter;
                    criticalFlatButton.normal.background = CriticalItemTexture1x1;
                    criticalFlatButton.hover.background = CriticalItemHoveredTexture1x1;
                    criticalFlatButton.active.background = CriticalItemClickedTexture1x1;
                }
                return criticalFlatButton;
            }
        }

        private static string projectName;
        public static string ProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    string[] s = Application.dataPath.Split('/', '\\');
                    projectName = s[s.Length - 2];
                }
                return projectName;
            }

        }

        public const string CHECKMARK_ICON_PATH = "Assets/PerVertexUIEffectsStack/Textures/EditorIcons/checkmark.png";

        public static string GetProjectRelatedEditorPrefsKey(params string[] keyElements)
        {
            System.Text.StringBuilder b = new System.Text.StringBuilder(ProjectName);
            for (int i = 0; i < keyElements.Length; ++i)
            {
                b.Append("-").Append(keyElements[i]);
            }
            return b.ToString();
        }

        /// <summary>
        /// Draw a tab view and return the currently selected tab
        /// </summary>
        /// <param name="current"></param>
        /// <param name="tabLabels"></param>
        /// <returns></returns>
        public static int Tabs(int current, params string[] tabLabels)
        {
            if (tabLabels.Length == 0)
                return current;
            int selectedTab = current;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < tabLabels.Length; ++i)
            {
                GUIStyle style =
                    i == 0 && tabLabels.Length > 1 ? EditorStyles.miniButtonLeft :
                    i == tabLabels.Length - 1 && tabLabels.Length > 1 ? EditorStyles.miniButtonRight :
                    i > 0 && i < tabLabels.Length - 1 && tabLabels.Length > 1 ? EditorStyles.miniButtonMid :
                    EditorStyles.miniButton;
                GUI.backgroundColor = i == current ? Color.gray : Color.white;
                if (GUILayout.Button("", style))
                {
                    selectedTab = i;
                }
                GUI.backgroundColor = Color.white;

                GUIStyle labelStyle = i == current ? CenteredWhiteLabel : CenteredLabel;
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), tabLabels[i], labelStyle);
            }
            EditorGUILayout.EndHorizontal();
            return selectedTab;
        }

        public static int Tabs(int current, params GUIContent[] tabContents)
        {
            if (tabContents.Length == 0)
                return current;
            float maxIconHeight = EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < tabContents.Length; ++i)
            {
                if (tabContents[i].image != null && tabContents[i].image.height > maxIconHeight)
                    maxIconHeight = tabContents[i].image.height;
            }
            int selectedTab = current;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < tabContents.Length; ++i)
            {
                GUIStyle style =
                    i == 0 && tabContents.Length > 1 ? EditorStyles.miniButtonLeft :
                    i == tabContents.Length - 1 && tabContents.Length > 1 ? EditorStyles.miniButtonRight :
                    i > 0 && i < tabContents.Length - 1 && tabContents.Length > 1 ? EditorStyles.miniButtonMid :
                    EditorStyles.miniButton;
                GUI.backgroundColor = i == current ? Color.gray : Color.white;
                if (GUILayout.Button("", style, GUILayout.Height(maxIconHeight)))
                {
                    selectedTab = i;
                }
                GUI.backgroundColor = Color.white;

                GUIStyle labelStyle = i == current ? CenteredWhiteLabel : CenteredLabel;
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), tabContents[i], labelStyle);
            }
            EditorGUILayout.EndHorizontal();
            return selectedTab;
        }

        public static int Tabs(int current, float height, params GUIContent[] tabContents)
        {
            if (tabContents.Length == 0)
                return current;

            int selectedTab = current;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < tabContents.Length; ++i)
            {
                GUIStyle style =
                    i == 0 && tabContents.Length > 1 ? EditorStyles.miniButtonLeft :
                    i == tabContents.Length - 1 && tabContents.Length > 1 ? EditorStyles.miniButtonRight :
                    i > 0 && i < tabContents.Length - 1 && tabContents.Length > 1 ? EditorStyles.miniButtonMid :
                    EditorStyles.miniButton;
                GUI.backgroundColor = i == current ? Color.gray : Color.white;
                if (GUILayout.Button("", style, GUILayout.Height(height)))
                {
                    selectedTab = i;
                }
                GUI.backgroundColor = Color.white;

                GUIStyle labelStyle = i == current ? CenteredWhiteLabel : CenteredLabel;
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), tabContents[i], labelStyle);
            }
            EditorGUILayout.EndHorizontal();
            return selectedTab;
        }

        /// <summary>
        /// Add space between controls
        /// </summary>
        /// <param name="amount"></param>
        public static void Space(int amount = 1)
        {
            for (int i = 0; i < amount; ++i)
            {
                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Draw a horizontal line
        /// </summary>
        public static void Separator(bool indented = false)
        {
            Rect r = EditorGUILayout.GetControlRect();
            if (indented)
                r = EditorGUI.IndentedRect(r);
            Vector2 start = new Vector2(r.min.x, (r.min.y + r.max.y) / 2);
            Vector2 end = new Vector2(r.max.x, (r.min.y + r.max.y) / 2);
            Handles.BeginGUI();
            Handles.color = Color.gray;
            Handles.DrawLine(start, end);
            Handles.EndGUI();
        }

        /// <summary>
        /// Draw a button which anchors to the right edge of the window
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool RightAnchoredButton(string label)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool btnClicked = GUILayout.Button(label, GUILayout.Width(standardWidth), GUILayout.Height(standardHeight));
            EditorGUILayout.EndHorizontal();
            return btnClicked;
        }

        public static bool RightAnchoredButton(string label, float width, float height)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool btnClicked = GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(height));
            EditorGUILayout.EndHorizontal();
            return btnClicked;
        }

        public static bool Button(string label)
        {
            return GUILayout.Button(label, GUILayout.Width(standardWidth), GUILayout.Height(standardHeight));
        }

        /// <summary>
        /// Draw a tiny square button
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool TinyButton(string label)
        {
            return GUILayout.Button(label, GUILayout.Width(tinyWidth), GUILayout.Height(tinyHeight));
        }

        /// <summary>
        /// Draw a folder browser
        /// </summary>
        /// <param name="label">The prefix label</param>
        /// <param name="result">The currently selected folder, this will be modified after user selects another folder</param>
        public static void BrowseFolder(string label, ref string result)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.LabelField(result, ItalicLabel, GUILayout.MinWidth(100));
            if (GUILayout.Button("Browse", GUILayout.Width(standardWidth), GUILayout.Height(standardHeight)))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("Select folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectedFolder))
                    result = FileUtil.GetProjectRelativePath(selectedFolder);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw a file browser
        /// </summary>
        /// <param name="label">The prefix label</param>
        /// <param name="result">The currently selected file path, this will be modified after selecting another file</param>
        /// <param name="filter">File extension filter</param>
        public static void BrowseFile(string label, ref string result, params string[] filter)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.LabelField(result, ItalicLabel, GUILayout.MinWidth(100));
            if (GUILayout.Button("Browse", GUILayout.Width(standardWidth), GUILayout.Height(standardHeight)))
            {
                string selectedFile = EditorUtility.OpenFilePanelWithFilters("Select file", Application.dataPath, filter);
                if (!string.IsNullOrEmpty(selectedFile))
                    result = selectedFile;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw a toggle group in horizontal layout, only one item can be selected at a time
        /// </summary>
        /// <param name="label">The prefix label</param>
        /// <param name="selected">Currently selected item index</param>
        /// <param name="toggleWidth">Width of each toggle</param>
        /// <param name="toggleLabels">Label for each toggle</param>
        /// <returns>Index of the selected item</returns>
        public static int ToggleGroup(string label, int selected, float toggleWidth, params string[] toggleLabels)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            Rect r = EditorGUILayout.GetControlRect();
            Rect toggleRect = new Rect(r.x, r.y, toggleWidth, r.height);

            for (int i = 0; i < toggleLabels.Length; ++i)
            {
                toggleRect.position = r.position + new Vector2(i * toggleWidth, 0);
                if (EditorGUI.ToggleLeft(toggleRect, toggleLabels[i], i == selected))
                {
                    selected = i;
                }
            }

            EditorGUILayout.EndHorizontal();
            return selected;
        }

        /// <summary>
        /// Draw a toggle group in vertical layout, only one item can be selected at a time
        /// </summary>
        /// <param name="label">The prefix label</param>
        /// <param name="selected">Currently selected item index</param>
        /// <param name="toggleHeight">Height of each toggle</param>
        /// <param name="toggleLabels">Label for each toggle</param>
        /// <returns>Index of the selected item</returns>
        public static int ToggleGroupVertical(string label, int selected, float toggleHeight, params string[] toggleLabels)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            Rect r = EditorGUILayout.GetControlRect(false, toggleHeight * toggleLabels.Length);
            Rect toggleRect = new Rect(r.x, r.y, r.width, toggleHeight);

            for (int i = 0; i < toggleLabels.Length; ++i)
            {
                toggleRect.position = r.position + new Vector2(0, i * toggleHeight);
                if (EditorGUI.ToggleLeft(toggleRect, toggleLabels[i], i == selected))
                {
                    selected = i;
                }
            }

            EditorGUILayout.EndHorizontal();
            return selected;
        }

        /// <summary>
        /// Draw a button looks like a hyperlink
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool LinkButton(string label, float width, float height)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(width), GUILayout.Height(height));
            return LinkButton(r, label);
        }

        /// <summary>
        /// Draw a button looks like a hyperlink
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool LinkButton(string label)
        {
            return LinkButton(label, standardWidth, standardHeight);
        }

        /// <summary>
        /// Draw a button looks like a hyperlink
        /// </summary>
        /// <param name="r"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool LinkButton(Rect r, string label)
        {
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
            return GUI.Button(r, label, LinkLabel);
        }

        public static void DrawOutlineBox(Rect r, Color c)
        {
            Handles.BeginGUI();
            using (var scope = new Handles.DrawingScope(c))
            {
                Vector2 p1 = new Vector2(r.xMin, r.yMin);
                Vector2 p2 = new Vector2(r.xMax, r.yMin);
                Vector2 p3 = new Vector2(r.xMax, r.yMax);
                Vector2 p4 = new Vector2(r.xMin, r.yMax);
                Handles.DrawLines(new Vector3[]
                {
                    p1,p2,
                    p2,p3,
                    p3,p4,
                    p4,p1
                });
            }
            Handles.EndGUI();
        }

        public static void DrawToggleNonInteractive(Rect r, bool isOn)
        {
            Handles.BeginGUI();
            Handles.color = Color.white * 0.9f;
            Handles.DrawSolidDisc(r.center, Vector3.forward, Mathf.Min(r.width, r.height) * 0.3f);
            if (isOn)
            {
                GUIContent checkmark = EditorGUIUtility.IconContent(CHECKMARK_ICON_PATH);
                EditorGUI.LabelField(r, checkmark);
            }
            Handles.EndGUI();
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color c)
        {
            Handles.color = c;
            Handles.BeginGUI();
            Handles.DrawLine(start, end);
            Handles.EndGUI();
        }

        public static void DrawDottedLine(Vector2 start, Vector2 end, float size, Color c)
        {
            Handles.color = c;
            Handles.BeginGUI();
            Handles.DrawDottedLine(start, end, size);
            Handles.EndGUI();
        }

        public static void DrawSolidCircle(Vector2 position, float radius, Color c)
        {
            Handles.color = c;
            Handles.BeginGUI();
            Handles.DrawSolidDisc(position, Vector3.forward, radius);
            Handles.EndGUI();
        }

        public static void DrawWiredCircle(Vector2 position, float radius, Color c)
        {
            Handles.color = c;
            Handles.BeginGUI();
            Handles.DrawWireDisc(position, Vector3.forward, radius);
            Handles.EndGUI();
        }

        public static void DrawSolidCircleWithOutline(Vector2 position, float radius, Color bgColor, Color outlineColor)
        {
            DrawSolidCircle(position, radius, bgColor);
            DrawWiredCircle(position, radius, outlineColor);
        }

        public static void DrawBezier(Vector2 cp1, Vector2 h1, Vector2 h2, Vector2 cp2, Color c)
        {
            Handles.BeginGUI();
            Handles.DrawBezier(
                cp1,
                cp2,
                h1,
                h2,
                c, Texture2D.whiteTexture, 1);
            Handles.EndGUI();
        }

        public static void Label(Vector3 position, string text, int size, TextAnchor alignment, FontStyle fontStyle, Color c)
        {
            Handles.BeginGUI();
            GUIStyle style = new GUIStyle();
            style.fontSize = size;
            style.alignment = alignment;
            style.fontStyle = fontStyle;
            style.normal.textColor = c;
            Handles.Label(position, text, style);
            Handles.EndGUI();
        }
    }
}