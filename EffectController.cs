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

        public Config.Tab tab;
        public bool autoEdge;
        public bool firstTime;

        public bool overrideFirstTime;

        private CameraController cameraController;
        private bool autoEdgeActive;
        private static int clickCount = 0;
        private bool clickedInappropriately = false;
        private string keystring = "";
        private string edgeKeyString = "";

        private bool automaticMode;
        void Awake()
        {
            cameraController = GetComponent<CameraController>();
            config = Config.Deserialize(configPath);
            if (config == null)
            {
                config = new Config();

                config.memoryTab = Config.Tab.Hotkey;
                config.automaticMode = true;
                config.edgeEnabled = false;
                config.edgeMode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
                config.sensNorm = 1.63f;
                config.sensDepth = 2.12f;
                config.edgeExpo = 0.09f;
                config.edgeSamp = 0.82f;
                config.edgeOnly = 0;
                config.autoEdge = true;
                config.firstTime = true;

                config.bloomEnabled = false;
                config.bloomThresh = 0.27f;
                config.bloomIntens = 0.39f;
                config.bloomBlurSize = 5.50f;

            }
            
        }

        void Start()
        {
            edge = GetComponent<EdgeDetection>();
            bloom = GetComponent<BloomOptimized>();

            LoadAllSettings();
            SaveConfig();

            if (firstTime)
            {
                showSettingsPanel = true;
                tab = Config.Tab.Hotkey;
            }
            else
            {
                tab = Config.Tab.EdgeDetection;
            }

        }

        void LoadAllSettings()
        {
            tab = config.memoryTab;
            automaticMode = config.automaticMode;
            edge.enabled = config.edgeEnabled;
            edge.mode = config.edgeMode;
            edge.sensitivityNormals = config.sensNorm;
            edge.sensitivityDepth = config.sensDepth;
            edge.edgeExp = config.edgeExpo;
            edge.edgesOnly = config.edgeOnly;
            edge.sampleDist = config.edgeSamp;
            autoEdge = config.autoEdge;
            
            firstTime = config.firstTime;

            bloom.enabled = config.bloomEnabled;
            bloom.threshold = config.bloomThresh;
            bloom.intensity = config.bloomIntens;
            bloom.blurSize = config.bloomBlurSize;
        }

        void LoadManualSettings()
        {
            edge.mode = config.edgeMode;
            edge.sensitivityNormals = config.sensNorm;
            edge.sensitivityDepth = config.sensDepth;
            edge.edgeExp = config.edgeExpo;
            edge.edgesOnly = config.edgeOnly;
            edge.sampleDist = config.edgeSamp;
            autoEdge = config.autoEdge;
            bloom.threshold = config.bloomThresh;
            bloom.intensity = config.bloomIntens;
            bloom.blurSize = config.bloomBlurSize;
        }

        

        public void SaveConfig()
        {
            config.automaticMode = automaticMode;
            config.edgeEnabled = edge.enabled;
            config.edgeMode = edge.mode;
            config.sensNorm = edge.sensitivityNormals;
            config.sensDepth = edge.sensitivityDepth;
            config.edgeExpo = edge.edgeExp;
            config.edgeOnly = edge.edgesOnly;
            config.edgeSamp = edge.sampleDist;
            config.autoEdge = autoEdge;

            config.bloomEnabled = bloom.enabled;
            config.bloomThresh = bloom.threshold;
            config.bloomIntens = bloom.intensity;
            config.bloomBlurSize = bloom.blurSize;
            config.firstTime = firstTime;

            Config.Serialize(configPath, config);
        }

        void RecommendedDefaults()
        {
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.sensitivityNormals = 0.63f;
            edge.sensitivityDepth = 2.12f;
            edge.edgeExp = 0.09f;
            edge.sampleDist = 0.82f;
            edge.edgesOnly = 0;
            autoEdge = true;

            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
        }

        void OnGUI()
        {
            if (firstTime)
                tab = Config.Tab.Hotkey;
            if (showSettingsPanel)
            {
                windowRect = GUI.Window(391435, windowRect, SettingsPanel, "Bordercities Configuration Panel");
            }
        }

        void SettingsPanel(int wnd)
        {
            GUILayout.BeginHorizontal();
            if (!firstTime)
            {
                if (GUILayout.Button("Edge Detection"))
                {
                    tab = Config.Tab.EdgeDetection;
                }
                if (GUILayout.Button("Bloom"))
                {
                    tab = Config.Tab.Bloom;
                }
                if (GUILayout.Button("Hotkey"))
                {
                    tab = Config.Tab.Hotkey;
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.Space(20f);

            if (tab == Config.Tab.EdgeDetection)
            {
                if (edge != null)
                {
                    if (!edge.enabled)
                        edge.enabled = GUILayout.Toggle(edge.enabled, "Click to enable Edge Detection.");
                    else
                        edge.enabled = GUILayout.Toggle(edge.enabled, "Click to disable Edge Detection.");
                    if (edge.enabled)
                    {
                        if (automaticMode)
                        {

                            if (!clickedInappropriately)
                            {
                                if (GUILayout.Button("'Plug & Play' Mode is on.  Enter Manual Mode with.."))
                                {
                                    clickCount++;
                                    if (clickCount >= 10)
                                    {
                                        clickedInappropriately = true;
                                    }
                                }
                            }
                            else
                            {
                                GUILayout.Button("Dude.  It doesn't do anything.");
                            }



                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("..current settings."))
                            {
                                automaticMode = false;
                                clickedInappropriately = false;
                                clickCount = 0;
                            }
                            if (GUILayout.Button("..your last saved settings."))
                            {
                                automaticMode = false;
                                clickedInappropriately = false;
                                clickCount = 0;
                                LoadManualSettings();
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
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
                                if (!autoEdge)
                                    edge.sensitivityDepth = GUILayout.HorizontalSlider(edge.sensitivityDepth, 0.000f, 50.000f, GUILayout.Width(570));
                                GUILayout.Label("Normal sensitivity: " + edge.sensitivityNormals.ToString());
                                if (!autoEdge)
                                    edge.sensitivityNormals = GUILayout.HorizontalSlider(edge.sensitivityNormals, 0.000f, 5.000f, GUILayout.Width(570));


                                autoEdge = GUILayout.Toggle(autoEdge, "Automatic Mode");
                                
                            }
                            if (edge.mode == EdgeDetection.EdgeDetectMode.SobelDepthThin || edge.mode == EdgeDetection.EdgeDetectMode.SobelDepth)
                            {
                                GUILayout.Label("Edge exponent: " + edge.edgeExp.ToString());
                                edge.edgeExp = GUILayout.HorizontalSlider(edge.edgeExp, 0.000f, 1.000f, GUILayout.Width(570));
                            }

                            if (GUILayout.Button("Advanced mode is on.  Switch to 'Plug & Play' mode."))
                            {
                                automaticMode = true;
                                RecommendedDefaults();
                            }
                        }
                    }
                }
            }

            if (tab == Config.Tab.Bloom)
            {
                if (!bloom.enabled)
                    bloom.enabled = GUILayout.Toggle(bloom.enabled, "Click to enable Bloom.");
                else
                    bloom.enabled = GUILayout.Toggle(bloom.enabled, "Click to disable Bloom.");

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
            }

            if (tab == Config.Tab.Hotkey)
            {
                if (firstTime || config.keyCode == KeyCode.None)
                {
                    GUILayout.Label("BORDERCITIES INITIAL HOTKEY CONFIG: (Will never popup again upon making choice below)");
                    GUILayout.Label("1) Choose below which key will open the Bordercities configuration window.");
                    GUILayout.Label("2) Press the same key on your keyboard.  If the window closes, setup is complete. If not, pick another hotkey.");
                    GUILayout.Label("NEW IN UPDATE 10: Edge detection can now be toggled via a hotkey of your choice!!  This option becomes available in the 'Hotkey' tab upon first confirming your 'Configuration Panel' hotkey here.");
                    GUILayout.Space(5f);
                    GUILayout.Label("More options for key choices coming soon!");
                    GUILayout.Space(10f);
                    GUILayout.Label("NOTE: This window will -never- automatically pop-up again as soon as you've confirmed your hotkey choice.   This initialization process ensures that all users, regardless of hardware, operating system, or current keyboard configuration, will be able to enjoy Bordercities.");
                
                    
                }


                if (!firstTime)
                    GUILayout.Label("WARNING: HOTKEY BUTTONS WILL SAVE UPON CLICK.  THIS INCLUDES YOUR EFFECTS SETTINGS.");
                KeyboardGrid(0);




                if (firstTime && config.keyCode != KeyCode.None)
                {
                    GUILayout.Space(3f);
                    GUILayout.Label("Hotkey '"+keystring+"' has been chosen and is active.  Confirm it now by using the hotkey.");
                    GUILayout.Space(3f);
                }

                if (!firstTime)
                {
                    if (config.keyCode != KeyCode.None)
                        GUILayout.Label("Current 'Config' hotkey: " + config.keyCode);
                    else
                        GUILayout.Label("No config hotkey is bound to Bordercities.");
                    GUILayout.Space(45f);
                    GUILayout.Label("Set edge toggle hotkey below: ");
                    KeyboardGrid(1);
                    if (config.edgeToggleKeyCode != KeyCode.None)
                        GUILayout.Label("Current 'Edge Enable' hotkey: " + config.edgeToggleKeyCode);
                    else
                        GUILayout.Label("No edge enable hotkey is bound to Bordercities.");
                    GUILayout.Space(5f);
                    GUILayout.Label("More key options coming soon!");
                }
                
            }

            
            if (!firstTime)
            {
                
                GUILayout.BeginHorizontal();
                if (tab != Config.Tab.Hotkey)
                {
                    if (GUILayout.Button("Save Settings"))
                    {
                        SaveConfig();
                    }
                    if (GUILayout.Button("Load From Save"))
                    {
                        LoadAllSettings();
                    }
                    if (GUILayout.Button("Recommended Defaults (Does not autosave on click.)"))
                    {
                        RecommendedDefaults();
                    }
                }
                
                if (GUILayout.Button("Close Window"))
                {
                    showSettingsPanel = false;
                    if (!overrideFirstTime)
                        overrideFirstTime = true;
                }

                GUILayout.EndHorizontal();
            }
            

        }

        void KeyboardGrid(int purpose)
        {
            GUILayout.BeginHorizontal();
            Hotkey("F5", KeyCode.F5, purpose);
            Hotkey("F6", KeyCode.F6, purpose);
            Hotkey("F7", KeyCode.F7, purpose);
            Hotkey("F8", KeyCode.F8, purpose);
            Hotkey("F9", KeyCode.F9, purpose);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Hotkey("F10", KeyCode.F10, purpose);
            Hotkey("F11", KeyCode.F11, purpose);
            Hotkey("F12", KeyCode.F12, purpose);
            Hotkey("[", KeyCode.LeftBracket, purpose);
            Hotkey("]", KeyCode.RightBracket, purpose);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Hotkey("=", KeyCode.Equals, purpose);
            Hotkey("Slash", KeyCode.Slash, purpose);
            Hotkey("Backslash", KeyCode.Backslash, purpose);
            Hotkey("Home", KeyCode.Home, purpose);
            Hotkey("End", KeyCode.End, purpose);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Hotkey("Numpad *", KeyCode.KeypadMultiply, purpose);
            Hotkey("Numpad -", KeyCode.KeypadMinus, purpose);
            Hotkey("Numpad =", KeyCode.KeypadEquals, purpose);
            Hotkey("Numpad +", KeyCode.KeypadPlus, purpose);
            Hotkey("Numpad /", KeyCode.KeypadDivide, purpose);
            GUILayout.EndHorizontal();

        }
        void Hotkey(string label, KeyCode keycode, int purpose)
        {
            if (GUILayout.Button(label, GUILayout.MaxWidth(100)))
            {
                switch (purpose)
                {
                    case 0:
                        config.keyCode = keycode;
                        keystring = label;
                        break;
                    case 1:
                        config.edgeToggleKeyCode = keycode;
                        edgeKeyString = label;
                        break;
                    default:
                        break;
                } 
                
                SaveConfig();
            }
        }

        

        public void Update()
        {
            if (Input.GetKeyUp(config.keyCode))
            {
                showSettingsPanel = !showSettingsPanel;
            }
            if (Input.GetKeyUp(KeyCode.Escape) && showSettingsPanel)
            {
                overrideFirstTime = true;
                showSettingsPanel = false;
            }
            if (firstTime && Input.GetKeyUp(config.keyCode))
            {
                firstTime = false;
                showSettingsPanel = false;
                SaveConfig();
                tab = Config.Tab.EdgeDetection;
            }
         
            if (firstTime)
            {
                if (!overrideFirstTime)
                    showSettingsPanel = true;
            }
            if (Input.GetKeyUp(config.edgeToggleKeyCode))
                edge.enabled = !edge.enabled;

            
        }

        void SizeCheck(bool value, float min, float max, float depthLimit)
        {
            float size = cameraController.m_currentSize;
            if (min < size && max > size)
            {
                if (value)
                {
                    if (edge.sensitivityDepth >= depthLimit)
                        edge.sensitivityDepth = depthLimit;
                }
                if (!value)
                {
                    edge.sensitivityNormals = Mathf.Lerp(edge.sensitivityNormals, depthLimit, 0.5f);
                    
                }
            }
        }

        void AutomaticAlgorithms()
        {
            SizeCheck(true,40f, 100f, 1.523f);
            SizeCheck(true,100, 200, 2.956f);
            SizeCheck(true,200, 300, 3.405f);
            SizeCheck(true,300, 400, 3.584f);
            SizeCheck(true,400, 500, 5.017f);
            SizeCheck(true,500, 600, 6.989f);
            SizeCheck(true,600, 700, 8.691f);
            SizeCheck(true,700, 800, 9.408f);
            SizeCheck(true,800, 1000, 12.186f);
            SizeCheck(true,1000, 1100, 15.681f);

            SizeCheck(false, 40, 222f, 0.65f);
            SizeCheck(false, 100, 476f, 0.833f);
            SizeCheck(false, 476f, 700f, 1.1827f);
            SizeCheck(false, 700, 1274, 1.2f);
            SizeCheck(false, 1274f, 4000, 1.24f);
            //cool mode


            float size = cameraController.m_currentSize;
            if (size < 222f)
            {
                edge.sensitivityDepth = size / 125;
                //edge.sensitivityNormals = Mathf.Lerp(0.57f, 0.93f, 1f);
            }
            if (size >= 222f)
            {
                edge.sensitivityDepth = size / 250;
            }

            




            if (edge.sensitivityDepth <= 0.44f)
                edge.sensitivityDepth = 0.44f;
            if (edge.sensitivityDepth >= 30f)
                edge.sensitivityDepth = 30f;
            if (edge.sensitivityNormals >= 1.29f)
                edge.sensitivityNormals = 1.29f;
            if (edge.sensitivityNormals <= 0.65f)
                edge.sensitivityNormals = 0.65f;
        }
        public void LateUpdate()
        {
            if (cameraController != null && autoEdge)
            {
                

                autoEdgeActive = true;
                AutomaticAlgorithms();
            }
            if (!autoEdge && autoEdgeActive)
            {
                edge.sensitivityDepth = config.sensDepth;
                autoEdgeActive = false;
            }
        }
    }
}