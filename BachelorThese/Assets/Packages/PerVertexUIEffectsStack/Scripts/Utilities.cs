using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Text;

namespace Pinwheel.UIEffects
{
    public static class Utilities
    {
        public static void LogActionNoListener(Delegate action, string actionName = "")
        {
            if (action.GetInvocationList().Length == 1)
            {
                if (string.IsNullOrEmpty(actionName))
                {
                    Debug.Log("No listener");
                }
                else
                {
                    Debug.Log(actionName + ": No listener");
                }
            }
        }

        public static void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public static Vector3 ToWorldPosition(this Vector2 mousePosition)
        {
            return mousePosition.ToWorldPosition(Camera.main);
        }

        public static Vector3 ToWorldPosition(this Vector2 mousePosition, Camera cam)
        {
            return cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -cam.transform.position.z));
        }

        public static Vector2 ToViewportPosition(this Vector2 mousePosition)
        {
            return mousePosition.ToViewportPosition(Camera.main);
        }

        public static Vector2 ToViewportPosition(this Vector2 mousePosition, Camera cam)
        {
            return cam.ScreenToViewportPoint(mousePosition);
        }

        public static Vector2 ToScreenPosition(this Vector3 worldPoint)
        {
            return worldPoint.ToScreenPosition(Camera.main);
        }

        public static Vector2 ToScreenPosition(this Vector3 worldPoint, Camera cam)
        {
            return cam.WorldToScreenPoint(worldPoint);
        }

        public static Ray ToRay(this Vector2 mousePosition)
        {
            return mousePosition.ToRay(Camera.main);
        }

        public static Ray ToRay(this Vector2 mousePosition, Camera cam)
        {
            return cam.ScreenPointToRay(mousePosition);
        }

        public static int[] GetIndicesArray(int length)
        {
            int[] indices = new int[length];
            for (int i = 0; i < length; ++i)
            {
                indices[i] = i;
            }
            return indices;
        }

        public static int[] GetShuffleIndicesArray(int length)
        {
            int[] indices = GetIndicesArray(length);
            for (int i = 0; i < length - 1; ++i)
            {
                int j = UnityEngine.Random.Range(0, length);
                int tmp = indices[i];
                indices[i] = indices[j];
                indices[j] = tmp;
            }

            return indices;
        }

        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = value;
            }
        }

        public static void Fill<T>(this T[,] array2D, T value)
        {
            for (int i = 0; i < array2D.GetLength(0); ++i)
            {
                for (int j = 0; j < array2D.GetLength(1); ++j)
                {
                    array2D[i, j] = value;
                }
            }
        }

        public static void Fill<T>(this T[][] jaggedArray, T value)
        {
            for (int i = 0; i < jaggedArray.Length; ++i)
            {
                for (int j = 0; j < jaggedArray[i].Length; ++j)
                {
                    jaggedArray[i][j] = value;
                }
            }
        }

        public static List<T> ToList<T>(this T[] array)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; ++i)
            {
                list.Add(array[i]);
            }
            return list;
        }

        public static void BringToFront(this Transform t)
        {
            t.SetAsLastSibling();
        }

        public static void SendToBack(this Transform t)
        {
            t.SetAsFirstSibling();
        }

        public static void ClearAllChildren(this Transform t)
        {
            int childCount = t.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                GameObject.Destroy(t.GetChild(i).gameObject);
            }
        }

        public static string ToJsonSimple(this IDictionary d)
        {
            string json = "{}";
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            IDictionaryEnumerator i = d.GetEnumerator();
            if (i.MoveNext())
            {
                b.AppendFormat("\"{0}\":\"{1}\"",
                    i.Key != null ? i.Key.ToString() : "Null",
                    i.Value != null ? i.Value.ToString() : "Null");
            }
            while (i.MoveNext())
            {
                b.AppendFormat(", \"{0}\":\"{1}\"",
                    i.Key != null ? i.Key.ToString() : "Null",
                    i.Value != null ? i.Value.ToString() : "Null");
            }
            json = json.Insert(1, b.ToString());
            return json;
        }

        public static string ListElementsToString<T>(this IEnumerable<T> list, string separator)
        {
            IEnumerator<T> i = list.GetEnumerator();
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            if (i.MoveNext())
                s.Append(i.Current.ToString());
            while (i.MoveNext())
                s.Append(separator).Append(i.Current.ToString());
            return s.ToString();
        }

        public static bool HasElement<T>(this IEnumerable<T> list)
        {
            return list != null && list.GetEnumerator().MoveNext();
        }

        public static Dictionary<string, string> AddMany(this Dictionary<string, string> d, params object[] s)
        {
            if (s.Length % 2 != 0)
                throw new ArgumentException("Params is not fully paired");

            for (int i = 0; i < s.Length; i += 2)
            {
                d[s[i].ToString()] = s[i + 1].ToString();
            }

            return d;
        }

        public static string ToUpperFirstLetter(this string s)
        {
            if (s.Length > 1)
            {
                return s[0].ToString().ToUpper() + s.Substring(1);
            }
            else
            {
                return s.ToUpper();
            }
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static void MailTo(string receiverEmail, string subject, string body)
        {
            subject = EscapeURL(subject);
            body = EscapeURL(body);
            Application.OpenURL("mailto:" + receiverEmail + "?subject=" + subject + "&body=" + body);
        }

        public static string EscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public static UIVertex GetMidPoint(UIVertex v0, UIVertex v1)
        {
            UIVertex v = new UIVertex();
            v.position = Vector3.Lerp(v0.position, v1.position, 0.5f);
            v.color = Color.Lerp(v0.color, v1.color, 0.5f);
            v.normal = Vector3.Lerp(v0.normal, v1.normal, 0.5f);
            v.tangent = Vector3.Lerp(v0.tangent, v1.tangent, 0.5f);
            v.uv0 = Vector2.Lerp(v0.uv0, v1.uv0, 0.5f);
            v.uv1 = Vector2.Lerp(v0.uv1, v1.uv1, 0.5f);
            v.uv2 = Vector2.Lerp(v0.uv2, v1.uv2, 0.5f);
            v.uv3 = Vector2.Lerp(v0.uv3, v1.uv3, 0.5f);
            return v;
        }

        public static T EnsureComponentAttached<T>(GameObject g) where T : Component
        {
            T comp = g.GetComponent<T>();
            if (comp == null)
            {
                comp = g.AddComponent<T>();
                MoveComponentUp(comp);
            }
            return comp;
        }

        public static void MoveComponentUp(Component comp)
        {
#if UNITY_EDITOR
            UnityEditorInternal.ComponentUtility.MoveComponentUp(comp);
#endif
        }

        public static float GetNearestMultiple(float number, float multipleOf)
        {
            int multiplier = 0;
            while (multipleOf * multiplier < number)
            {
                multiplier += 1;
            }

            float floor = multipleOf * (multiplier - 1);
            float ceil = multipleOf * multiplier;
            float f0 = number - floor;
            float f1 = ceil - number;

            if (f0 < f1)
                return floor;
            else
                return ceil;
        }

        public static Gradient CreateFullWhiteGradient()
        {
            Gradient g = new Gradient();
            GradientColorKey color = new GradientColorKey(Color.white, 1);
            GradientAlphaKey alpha = new GradientAlphaKey(1, 1);
            g.SetKeys(new GradientColorKey[] { color }, new GradientAlphaKey[] { alpha });
            return g;
        }

        public static float FindMaxValue(List<UIVertex> stream, Func<UIVertex, float> getProperty)
        {
            float max = float.MinValue;
            foreach (UIVertex v in stream)
            {
                if (getProperty(v) > max)
                    max = getProperty(v);
            }
            return max;
        }

        public static float FindMinValue(List<UIVertex> stream, Func<UIVertex, float> getProperty)
        {
            float min = float.MaxValue;
            foreach (UIVertex v in stream)
            {
                if (getProperty(v) < min)
                    min = getProperty(v);
            }
            return min;
        }

        public static void Copy<T>(List<T> src, List<T> des)
        {
            des.Clear();
            for (int i = 0; i < src.Count; ++i)
            {
                des.Add(src[i]);
            }
        }
    }
}