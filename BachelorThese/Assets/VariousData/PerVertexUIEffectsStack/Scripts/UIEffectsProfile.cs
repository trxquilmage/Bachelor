using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.UIEffects
{
    [CreateAssetMenu(fileName = "New UI Effects Profile", menuName = "UI Effects Profile")]
    public class UIEffectsProfile : ScriptableObject
    {
        public delegate void ModifiedHandler();
        public event ModifiedHandler Modified;

        [SerializeField]
        private Material material;
        public Material Material
        {
            get
            {
                if (material == null)
                {
                    material = new Material(Shader.Find("UIFX/Unlit"));
                }
                return material;
            }
        }

        #region Subdivision
        public bool useSubdivision;
        public bool showSubdivisionSettings;
        [SerializeField]
        private Subdivision subdivisor;
        public Subdivision Subdivisor
        {
            get
            {
                if (subdivisor == null)
                    subdivisor = new Subdivision();
                return subdivisor;
            }
        }
        #endregion

        #region Deform
        public bool useDeform;
        public bool showDeformSettings;
        [SerializeField]
        private Deform deformer;
        public Deform Deformer
        {
            get
            {
                if (deformer == null)
                    deformer = new Deform();
                return deformer;
            }
        }
        #endregion

        #region Shadow
        public bool useShadow;
        public bool showShadowSettings;
        [SerializeField]
        private Shadow shadower;
        public Shadow Shadower
        {
            get
            {
                if (shadower == null)
                    shadower = new Shadow();
                return shadower;
            }
        }
        #endregion

        #region Outline
        public bool useOutline;
        public bool showOutlineSettings;
        [SerializeField]
        private Outline outliner;
        public Outline Outliner
        {
            get
            {
                if (outliner == null)
                    outliner = new Outline();
                return outliner;
            }
        }
        #endregion

        #region Gradient
        public bool useGradient;
        public bool showGradientSettings;
        [SerializeField]
        private GradientBlend gradientBlender;
        public GradientBlend GradientBlender
        {
            get
            {
                if (gradientBlender == null)
                    gradientBlender = new GradientBlend();
                return gradientBlender;
            }
        }
        #endregion

        #region Mirror
        public bool useMirror;
        public bool showMirrorSettings;
        [SerializeField]
        private Mirror mirror;
        public Mirror Mirror
        {
            get
            {
                if (mirror == null)
                    mirror = new Mirror();
                return mirror;
            }
        }
        #endregion

        public void ApplyEffects(List<UIVertex> stream)
        {
            List<UIVertex> baseStream = new List<UIVertex>();
            Utilities.Copy(stream, baseStream);

            if (useSubdivision)
            {
                Subdivisor.ModifyVertexStream(baseStream);
            }

            if (useDeform)
            {
                Deformer.ModifyVertexStream(baseStream);
            }

            List<UIVertex> shadowStream = new List<UIVertex>();
            if (useShadow)
            {
                Utilities.Copy(baseStream, shadowStream);
                Shadower.ModifyVertexStream(shadowStream);
            }

            List<UIVertex> outlineStream = new List<UIVertex>();
            if (useOutline)
            {
                Utilities.Copy(baseStream, outlineStream);
                Outliner.ModifyVertexStream(outlineStream);
            }

            List<UIVertex> gradientOverlayStream = new List<UIVertex>();
            if (useGradient)
            {
                if (GradientBlender.BlendingMode == BlendMode.Overlay)
                    Utilities.Copy(baseStream, gradientOverlayStream);
                GradientBlender.ModifyVertexStream(baseStream, gradientOverlayStream);
            }

            List<UIVertex> mirrorStream = new List<UIVertex>();
            if (useMirror)
            {
                Utilities.Copy(baseStream, mirrorStream);
                Mirror.ModifyVertexStream(baseStream, mirrorStream);
            }

            stream.Clear();
            stream.AddRange(shadowStream);
            stream.AddRange(outlineStream);
            stream.AddRange(baseStream);
            stream.AddRange(gradientOverlayStream);
            stream.AddRange(mirrorStream);
        }

        public void SetEffectsDirty()
        {
            if (Modified != null)
                Modified();
        }
    }
}
