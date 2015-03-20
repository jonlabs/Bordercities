using UnityEngine;

namespace Bordercities
{
    public class EffectController : MonoBehaviour
    {
        public bool showSettingsPanel = false;
        private Rect windowRect = new Rect(64, 250, 803, 466);

        public Config config;
        private const string configPath = "BordercitiesConfig.xml";
        
        private EdgeDetection edge;
        private BloomOptimized bloom;
       
        void Awake()
        {
            config = Config.Deserialize(configPath);
            if (config == null)
            {
                config = new Config();
                config.edgeEnabled = false;
                config.edgeMode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
                config.sensNorm = 1.63f;
                config.sensDepth = 2.12f;
                config.edgeExpo = 0.09f;
                config.edgeSamp = 0.82f;
                config.edgeOnly = 0;
                config.bloomEnabled = false;
                config.bloomThresh = 0.27f;
                config.bloomIntens = 0.39f;
                config.bloomBlurSize = 5.50f;
            }
        }

        void LoadSettings()
        {
            edge.enabled = config.edgeEnabled;
            edge.mode = config.edgeMode;
            edge.sensitivityNormals = config.sensNorm;
            edge.sensitivityDepth = config.sensDepth;
            edge.edgeExp = config.edgeExpo;
            edge.edgesOnly = config.edgeOnly;
            edge.sampleDist = config.edgeSamp;

            bloom.enabled = config.bloomEnabled;
            bloom.threshold = config.bloomThresh;
            bloom.intensity = config.bloomIntens;
            bloom.blurSize = config.bloomBlurSize;
        }

        void Start()
        {
            edge = GetComponent<EdgeDetection>();
            bloom = GetComponent<BloomOptimized>();

            LoadSettings();
            SaveConfig();
        }
        
       public void SaveConfig()
        {
            config.edgeEnabled = edge.enabled;
            config.edgeMode = edge.mode;
            config.sensNorm = edge.sensitivityNormals;
            config.sensDepth = edge.sensitivityDepth;
            config.edgeExpo = edge.edgeExp;
            config.edgeOnly = edge.edgesOnly;
            config.edgeSamp = edge.sampleDist;

            config.bloomEnabled = bloom.enabled;
            config.bloomThresh = bloom.threshold;
            config.bloomIntens = bloom.intensity;
            config.bloomBlurSize = bloom.blurSize;

            Config.Serialize(configPath, config);
        }
        void OnGUI()
        {
            if (showSettingsPanel)
            {
                windowRect = GUI.Window(391435, windowRect, SettingsPanel, "Image Effects");
            }
        }

        void SettingsPanel(int wnd)
        {
            if (edge != null)
            {
                edge.enabled = GUILayout.Toggle(edge.enabled, "Edge Detection");
                if (edge.enabled)
                {
                    GUILayout.Label("Edge sample distance: " + edge.sampleDist.ToString());
                    edge.sampleDist = GUILayout.HorizontalSlider(edge.sampleDist, 1, 5, GUILayout.Width(570));
                    GUILayout.Label("Edge mix: " + edge.edgesOnly.ToString());
                    edge.edgesOnly = GUILayout.HorizontalSlider(edge.edgesOnly, 0.000f, 1.000f, GUILayout.Width(570));
                    GUILayout.Space(5f);
                    switch (edge.mode)
                    {
                        case EdgeDetection.EdgeDetectMode.TriangleDepthNormals:
                            {
                                GUILayout.Label("Edge mode: Triangle Depth Normals");
                                break;
                            }
                        case EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals:
                            {
                                GUILayout.Label("Edge mode: Roberts Cross Depth Normals");
                                break;
                            }
                        case EdgeDetection.EdgeDetectMode.SobelDepth:
                            {
                                GUILayout.Label("Edge mode: Sobel Depth");
                                break;
                            }
                        case EdgeDetection.EdgeDetectMode.SobelDepthThin:
                            {
                                GUILayout.Label("Edge mode: Sobel Depth Thin");
                                break;
                            }
                        default:
                            {
                                GUILayout.Label("Edge mode:");
                                break;
                            }
                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Triangle Depth Normals"))
                    {
                        
                        edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
                        edge.SetCameraFlag();
                    }
                    if (GUILayout.Button("Roberts Cross Depth Normals"))
                    {
                        edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
                        edge.SetCameraFlag();
                    }
                    if (GUILayout.Button("Sobel Depth"))
                    {
                        edge.mode = EdgeDetection.EdgeDetectMode.SobelDepth;
                        edge.SetCameraFlag();
                    }
                    if (GUILayout.Button("Sobel Depth Thin"))
                    {
                        edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                        edge.SetCameraFlag();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5f);

                    if (edge.mode == EdgeDetection.EdgeDetectMode.TriangleDepthNormals || edge.mode == EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals)
                    {
                        GUILayout.Label("Depth sensitivity: " + edge.sensitivityDepth.ToString());
                        edge.sensitivityDepth = GUILayout.HorizontalSlider(edge.sensitivityDepth, 0.000f, 15.000f, GUILayout.Width(570));
                        GUILayout.Label("Normal sensitivity: " + edge.sensitivityNormals.ToString());
                        edge.sensitivityNormals = GUILayout.HorizontalSlider(edge.sensitivityNormals, 0.000f, 15.000f, GUILayout.Width(570));
                    }
                    if (edge.mode == EdgeDetection.EdgeDetectMode.SobelDepthThin || edge.mode == EdgeDetection.EdgeDetectMode.SobelDepth)
                    {
                        GUILayout.Label("Edge exponent: " + edge.edgeExp.ToString());
                        edge.edgeExp = GUILayout.HorizontalSlider(edge.edgeExp, 0.000f, 1.000f, GUILayout.Width(570));
                    }

                    
                    
                }
                bloom.enabled = GUILayout.Toggle(bloom.enabled, "Bloom");
                {
                    if (bloom.enabled)
                    {
                        GUILayout.Label("Threshold: " + bloom.threshold.ToString());
                        bloom.threshold = GUILayout.HorizontalSlider(bloom.threshold, 0.00f, 1.50f, GUILayout.Width(570));
                        GUILayout.Label("Intensity: " + bloom.intensity.ToString());
                        bloom.intensity = GUILayout.HorizontalSlider(bloom.intensity, 0.00f, 2.50f, GUILayout.Width(570));
                        GUILayout.Label("Blur size: " + bloom.blurSize.ToString());
                        bloom.blurSize = GUILayout.HorizontalSlider(bloom.blurSize, 0.00f, 5.50f, GUILayout.Width(570));
                    }
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save Settings"))
                {
                    SaveConfig();
                }
                if (GUILayout.Button("Load Previous"))
                {
                    LoadSettings();
                }
                if (GUILayout.Button("Recommended Defaults (Does not autosave on click.)"))
                {
                    edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
                    edge.sensitivityNormals = 1.63f;
                    edge.sensitivityDepth= 2.12f;
                    edge.edgeExp = 0.09f;
                    edge.sampleDist = 0.82f;
                    edge.edgesOnly = 0;

                    bloom.threshold = 0.27f;
                    bloom.intensity = 0.39f;
                    bloom.blurSize = 5.50f;
                }
                GUILayout.EndHorizontal();
               
            }
            else
            {
                edge = GetComponent<EdgeDetection>();
                bloom = GetComponent<BloomOptimized>();
            }
        }
        public void Update()
        {

            if (Input.GetKeyUp(KeyCode.Backslash))
            {
                showSettingsPanel = !showSettingsPanel;
            }
            if (Input.GetKeyUp(KeyCode.Escape) && showSettingsPanel)
            {
                showSettingsPanel = false;
            }
        }
    }
}
