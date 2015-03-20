using System;
using UnityEngine;

namespace Bordercities
{

    // NOTE: The contents of BloomOptimized.cs as well as the corresponding shader, may only be publicly used 
    // within the context of furthering the development of a game built with Unity.
    //
    // Cities:Skylines is a game made with Unity.  The effects used in this package are from Unity's freely available 
    // "Image Effects" suite.  I am essentially adding features to the game that would otherwise have been available to the
    // development team, however, were chosen not to be included or not thought of.
    //
    // Provided you understand this, you can gain access to the functionality of this effect by compiling the 'Hidden\FastBloom' shader
    // from within Unity's 'Standard Assets/Image Effects' package, and then add and git-ignore a script
    // containing the shader, such as:
    // 
    // public class HiddenShaderText
    //{
    //   public const string bloomShader = @"insert shader text here";
    //}
    //
    // to access it.  Do note that you will need to do a Replace All within the shader script to replace single quotations
    // with double quotations.
    //
    // Remember: You can only publicly use the contents of this script in the context of Unity development.
    //
    // This script is useless without the shader.  By compiling the shader for yourself, you are acknowledging that you
    // understand that you may only use BloomOptimized.cs as well its shader in the context of furthering the development
    // of a game built with Unity.


    [RequireComponent(typeof(Camera))]
    public class BloomOptimized : PostEffectsBase
    {

        public enum Resolution
        {
            Low = 0,
            High = 1,
        }

        public enum BlurType
        {
            Standard = 0,
            Sgx = 1,
        }

        [Range(0.0f, 1.5f)]
        public float threshold = 0.25f;
        [Range(0.0f, 2.5f)]
        public float intensity = 0.75f;

        [Range(0.25f, 5.5f)]
        public float blurSize = 1.0f;

        Resolution resolution = Resolution.High;
        [Range(1, 4)]
        public int blurIterations = 2;

        public BlurType blurType = BlurType.Standard;

        private Material fastBloomMaterial = null;


        public override bool CheckResources()
        {
            CheckSupport(false);

            fastBloomMaterial = CreateMaterialFromString(fastBloomMaterial, HiddenShaderText.bloomShader);// This is where you'll want to plug the string in.
           
            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnDisable()
        {
            if (fastBloomMaterial)
                DestroyImmediate(fastBloomMaterial);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            int divider = resolution == Resolution.Low ? 4 : 2;
            float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

            fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshold, intensity));
            source.filterMode = FilterMode.Bilinear;

            var rtW = source.width / divider;
            var rtH = source.height / divider;

            // downsample
            RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, rt, fastBloomMaterial, 1);

            var passOffs = blurType == BlurType.Standard ? 0 : 2;

            for (int i = 0; i < blurIterations; i++)
            {
                fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, threshold, intensity));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 2 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                // horizontal blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 3 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            fastBloomMaterial.SetTexture("_Bloom", rt);

            Graphics.Blit(source, destination, fastBloomMaterial, 0);
            UnityEngine.Object shadTrash = fastBloomMaterial.shader;
            UnityEngine.Object matTrash = fastBloomMaterial;
            DestroyImmediate(shadTrash, true);
            DestroyImmediate(matTrash, true);
            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
