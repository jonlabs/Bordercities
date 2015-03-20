using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bordercities
{

    // NOTE: The contents of EdgeDetection.cs as well as the corresponding shader, may only be publicly used 
    // within the context of furthering the development of a game built with Unity.
    //
    // Cities:Skylines is a game made with Unity.  The effects used in this package are from Unity's freely available 
    // "Image Effects" suite.  I am essentially adding features to the game that would otherwise have been available to the
    // development team, however, were chosen not to be included or not thought of.
    //
    // Provided you understand this, you can gain access to the functionality of this effect by compiling the 'Hidden\EdgeDetectNormals' shader
    // from within Unity's 'Standard Assets/Image Effects' package, and then add and git-ignore a script
    // containing the shader, such as:
    // 
    // public class HiddenShaderText
    //{
    //   public const string edgeDetectShader = @"insert shader text here";
    //}
    //
    // to access it.  Do note that you will need to do a Replace All within the shader script to replace single quotations
    // with double quotations.
    //
    // Remember: You can only publicly use the contents of this script in the context of Unity development.
    //
    // This script is useless without the shader.  By compiling the shader for yourself, you are acknowledging that you
    // understand that you may only use EdgeDetection.cs as well its shader in the context of furthering the development
    // of a game built with Unity.


    public class EdgeDetection : PostEffectsBase
    {
        public enum EdgeDetectMode
        {
            TriangleDepthNormals = 0,
            RobertsCrossDepthNormals = 1,
            SobelDepth = 2,
            SobelDepthThin = 3,
            TriangleLuminance = 4,
        }
        public EdgeDetectMode mode = EdgeDetectMode.RobertsCrossDepthNormals;
        private EdgeDetectMode oldMode = EdgeDetectMode.RobertsCrossDepthNormals;
        public float sensitivityDepth = 1.0f;
        public float sensitivityNormals = 1.0f;
        public float lumThreshold = 0.2f;
        public float edgeExp = 1.0f;
        public float sampleDist = 1.0f;
        public float edgesOnly = 0.0f;
        public Color edgesOnlyBgColor = Color.white;
        private Material edgeDetectMaterial = null;



        public override bool CheckResources()
        {
            CheckSupport(true);


           

            edgeDetectMaterial = CreateMaterialFromString(edgeDetectMaterial, HiddenShaderText.edgeDetectShader);// This is where you'll want to plug the string in.
            
         

            if (mode != oldMode)
                SetCameraFlag();

            oldMode = mode;

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        new void Start()
        {
            oldMode = mode;

        }

        public void SetCameraFlag()
        {
            if (mode == EdgeDetectMode.SobelDepth || mode == EdgeDetectMode.SobelDepthThin)
                GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
            else if (mode == EdgeDetectMode.TriangleDepthNormals || mode == EdgeDetectMode.RobertsCrossDepthNormals)
                GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
        }

        void OnEnable()
        {
            SetCameraFlag();
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);  // Might need to remove this block and go back to just CheckResources()
                return;
            }

            Vector2 sensitivity = new Vector2(sensitivityDepth, sensitivityNormals);
            edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
            edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
            edgeDetectMaterial.SetFloat("_SampleDistance", sampleDist);
            edgeDetectMaterial.SetVector("_BgColor", edgesOnlyBgColor);
            edgeDetectMaterial.SetFloat("_Exponent", edgeExp);
            edgeDetectMaterial.SetFloat("_Threshold", lumThreshold);
            Graphics.Blit(source, destination, edgeDetectMaterial, (int)mode);
            UnityEngine.Object shadTrash = edgeDetectMaterial.shader;
            UnityEngine.Object matTrash = edgeDetectMaterial;
            DestroyImmediate(shadTrash, true);
            DestroyImmediate(matTrash, true);
        }
    }
}

