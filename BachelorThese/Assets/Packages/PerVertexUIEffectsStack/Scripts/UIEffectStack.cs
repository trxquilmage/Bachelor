using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Pinwheel.UIEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Graphic))]
    public class UIEffectStack : BaseMeshEffect
    {
        [SerializeField]
        private UIEffectsProfile profile;
        public UIEffectsProfile Profile
        {
            get
            {
                return profile;
            }
            set
            {
                if (profile == null && value != null)
                {
                    profile = value;
                    profile.Modified += OnProfileModified;
                    if (graphic!=null)
                    {
                        //graphic.material = profile.Material;
                        //graphic.SetMaterialDirty();
                        graphic.SetVerticesDirty();
                    }
                }
                else if (profile != null && value == null)
                {
                    profile.Modified -= OnProfileModified;
                    profile = value;
                    if (graphic != null)
                    {
                        //graphic.material = null;
                        //graphic.SetMaterialDirty();
                        graphic.SetVerticesDirty();
                    }
                }
                else if (profile != null && value != profile)
                {
                    profile.Modified -= OnProfileModified;
                    profile = value;
                    profile.Modified += OnProfileModified;
                    if (graphic != null)
                    {
                        //graphic.material = profile.Material;
                        //graphic.SetMaterialDirty();
                        graphic.SetVerticesDirty();
                    }
                }
            }
        }

        protected override void OnEnable()
        {
            if (Profile != null)
            {
                Profile.Modified += OnProfileModified;
                if (graphic != null)
                {
                    //graphic.material = profile.Material;
                    //graphic.SetMaterialDirty();
                }
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (Profile != null)
            {
                Profile.Modified -= OnProfileModified;
                if (graphic != null)
                {
                    //graphic.material = null;
                    //graphic.SetMaterialDirty();
                }
            }
            base.OnDisable();
        }

        /*protected override void OnValidate()
        {
            Profile = profile;
            base.OnValidate();
        }*/

        private void OnProfileModified()
        {
            if (graphic != null)
                graphic.SetVerticesDirty();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || Profile == null)
            {
                return;
            }

            List<UIVertex> stream = new List<UIVertex>();
            vh.GetUIVertexStream(stream);
            Profile.ApplyEffects(stream);
            vh.Clear();
            vh.AddUIVertexTriangleStream(stream);
        }
    }
}
