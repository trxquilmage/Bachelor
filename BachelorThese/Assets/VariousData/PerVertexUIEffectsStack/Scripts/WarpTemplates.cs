using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    public class WarpTemplates
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private Vector2[] points;
        public Vector2[] Points
        {
            get
            {
                return points;
            }
        }

        public WarpTemplates(string name, Vector2[] points)
        {
            this.name = name;
            this.points = new Vector2[12];
            int maxIndex = Mathf.Min(this.points.Length, points.Length);
            for (int i = 0; i < maxIndex; ++i)
            {
                this.points[i] = points[i];
            }
        }

        public static List<WarpTemplates> GetTemplates()
        {
            List<WarpTemplates> templates = new List<WarpTemplates>();
            templates.Add(GetDefaultTemplate());
            templates.Add(GetEllipseTemplate());
            templates.Add(GetPillowTemplate());
            templates.Add(GetBarrelTemplate());
            templates.Add(GetPincushionTemplate());
            templates.Add(GetArcUpwardTemplate());
            templates.Add(GetArcDownwardTemplate());
            templates.Add(GetRoundedDiamondTemplate());

            return templates;
        }

        public static WarpTemplates GetDefaultTemplate()
        {
            float f = 1f / 3f;

            string name = "Default";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(f, 0), new Vector2(0, f),
                new Vector2(0,1), new Vector2(0, 1-f), new Vector2(f, 1),
                new Vector2(1,1), new Vector2(1-f, 1), new Vector2(1, 1-f),
                new Vector2(1,0), new Vector2(1, f), new Vector2(1-f, 0)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetEllipseTemplate()
        {
            float f = 0.28f;

            string name = "Ellipse";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(f, -f), new Vector2(-f, f),
                new Vector2(0,1), new Vector2(-f, 1-f), new Vector2(f, 1+f),
                new Vector2(1,1), new Vector2(1-f, 1+f), new Vector2(1+f, 1-f),
                new Vector2(1,0), new Vector2(1+f, f), new Vector2(1-f, -f)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetPillowTemplate()
        {
            string name = "Pillow";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(0,1), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(1,1), new Vector2(1, 1), new Vector2(1, 1),
                new Vector2(1,0), new Vector2(1, 0), new Vector2(1, 0)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetBarrelTemplate()
        {
            float f = 1f / 3f;

            string name = "Barrel";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(f, -f/3), new Vector2(-f/3, f),
                new Vector2(0,1), new Vector2(-f/3, 1-f), new Vector2(f, 1+f/3),
                new Vector2(1,1), new Vector2(1-f, 1+f/3), new Vector2(1+f/3, 1-f),
                new Vector2(1,0), new Vector2(1+f/3, f), new Vector2(1-f, -f/3)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetPincushionTemplate()
        {
            float f = 1f / 3f;

            string name = "Pincushion";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(f, f/3), new Vector2(f/3, f),
                new Vector2(0,1), new Vector2(f/3, 1-f), new Vector2(f, 1-f/3),
                new Vector2(1,1), new Vector2(1-f, 1-f/3), new Vector2(1-f/3, 1-f),
                new Vector2(1,0), new Vector2(1-f/3, f), new Vector2(1-f, f/3)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetArcUpwardTemplate()
        {
            float f = 1f / 3f;
            float f3 = f * 3;
            float sine45 = Mathf.Sin(45 * Mathf.Deg2Rad);
            string name = "Arc Upward";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(f*sine45, f*sine45), new Vector2(-f*sine45, f*sine45),
                new Vector2(-f3*sine45,f3*sine45), new Vector2(-f3*sine45+f*sine45,f3*sine45-f*sine45), new Vector2(-f3*sine45+f3*sine45,f3*sine45+f3*sine45),
                new Vector2(1+f3*sine45,f3*sine45), new Vector2(1+f3*sine45-f3*sine45,f3*sine45+f3*sine45), new Vector2(1+f3*sine45-f*sine45,f3*sine45-f*sine45),
                new Vector2(1,0), new Vector2(1+f*sine45, f*sine45), new Vector2(1-f*sine45, f*sine45)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetArcDownwardTemplate()
        {
            float f = 1f / 3f;
            float f3 = f * 3;
            float sine45 = Mathf.Sin(45 * Mathf.Deg2Rad);
            string name = "Arc Downward";
            Vector2[] points = new Vector2[]
            {
                new Vector2(-f3*sine45,1-f3*sine45), new Vector2(-f3*sine45+f3*sine45,1-f3*sine45-f3*sine45), new Vector2(-f3*sine45+f*sine45,1-f3*sine45+f*sine45),
                new Vector2(0,1), new Vector2(-f*sine45,1-f*sine45), new Vector2(f*sine45,1-f*sine45),
                new Vector2(1,1), new Vector2(1-f*sine45, 1-f*sine45), new Vector2(1+f*sine45,1-f*sine45),
                new Vector2(1+f3*sine45, 1-f3*sine45), new Vector2(1+f3*sine45-f*sine45, 1-f3*sine45+f*sine45), new Vector2(1+f3*sine45-f3*sine45, 1-f3*sine45-f3*sine45)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }

        public static WarpTemplates GetRoundedDiamondTemplate()
        {
            float f = 0.5f;

            string name = "Rounded Diamond";
            Vector2[] points = new Vector2[]
            {
                new Vector2(0,0), new Vector2(f, -f), new Vector2(-f, f),
                new Vector2(0,1), new Vector2(-f, 1-f), new Vector2(f, 1+f),
                new Vector2(1,1), new Vector2(1-f, 1+f), new Vector2(1+f, 1-f),
                new Vector2(1,0), new Vector2(1+f, f), new Vector2(1-f, -f)
            };
            WarpTemplates t = new WarpTemplates(name, points);
            return t;
        }
    }
}
