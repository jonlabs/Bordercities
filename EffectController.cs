using UnityEngine;

namespace Bordercities
{
    public class EffectController : MonoBehaviour
    {
        public bool showSettingsPanel = false;
        private Rect windowRect = new Rect(64, 64, 803, 700); //was 64,250,803,466
        private float defaultHeight;
        private float defaultWidth;

        public Config config;
        private const string configPath = "BordercitiesConfig.xml";

        private EdgeDetection edge;
        private BloomOptimized bloom;

        public Config.Tab tab;
        public bool autoEdge;
        public bool firstTime;

        private bool hasClicked = false;

        public bool overrideFirstTime;

        private CameraController cameraController;
        private InfoManager infoManager;

        private bool autoEdgeActive;
        public bool subViewOnly;

        private string keystring = "";
        private string edgeKeyString = "";

        private bool automaticMode;

        private bool userWantsEdge;

        public Color currentColor;
        public float setR;
        public float setG;
        public float setB;
        public float colorMultiplier = 1f;
        private Color newColor;
        private float roundedR;
        private float roundedG;
        private float roundedB;
        private float roundedMult;

        void Awake()
        {
            cameraController = GetComponent<CameraController>();
            infoManager = InfoManager.instance;
            config = Config.Deserialize(configPath);
            if (config == null)
            {
                config = new Config();

                config.automaticMode = true;
                config.edgeEnabled = false;
                config.edgeMode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
                config.sensNorm = 1.63f;
                config.sensDepth = 2.12f;
                config.edgeExpo = 0.09f;
                config.edgeSamp = 1.0f;
                config.edgeOnly = 0;
                config.autoEdge = true;
                config.firstTime = true;
                config.subViewOnly = false;
                config.currentColor = new Color(0, 0, 0, 0);
                config.colorMultiplier = 1.0f;

                config.bloomEnabled = false;
                config.bloomThresh = 0.27f;
                config.bloomIntens = 0.39f;
                config.bloomBlurSize = 5.50f;

            }
            defaultWidth = windowRect.width;
            defaultHeight = windowRect.height;

        }

        void Start()
        {
            edge = GetComponent<EdgeDetection>();
            bloom = GetComponent<BloomOptimized>();

            LoadAllSettings();
            if (config.keyCode == KeyCode.None)
            {
                config.keyCode = KeyCode.LeftBracket;
            }
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
            userWantsEdge = config.edgeEnabled;
           
        }

        
        void MakeColor(float r, float g, float b)
        {
            float newR = r * colorMultiplier;
            float newG = g * colorMultiplier;
            float newB = b * colorMultiplier;
            newColor = new Color(newR, newG, newB, 0);
            edge.SetEdgeColor(newColor);
            currentColor = newColor;
        }

        void LoadAllSettings()
        {
            automaticMode = config.automaticMode;
            edge.enabled = config.edgeEnabled;
            edge.mode = config.edgeMode;
            edge.sensitivityNormals = config.sensNorm;
            edge.sensitivityDepth = config.sensDepth;
            edge.edgeExp = config.edgeExpo;
            edge.edgesOnly = config.edgeOnly;
            edge.sampleDist = config.edgeSamp;
            autoEdge = config.autoEdge;
            subViewOnly = config.subViewOnly;
            currentColor = config.currentColor;
            edge.edgeColor = currentColor;
            colorMultiplier = config.colorMultiplier;
            setR = config.setR;
            setG = config.setG;
            setB = config.setB;

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
            subViewOnly = config.subViewOnly;
            currentColor = config.currentColor;
            colorMultiplier = config.colorMultiplier;
            setR = config.setR;
            setG = config.setG;
            setB = config.setB;
            edge.edgeColor = config.currentColor;

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
            config.subViewOnly = subViewOnly;
            config.currentColor = currentColor;
            config.setR = setR;
            config.setG = setG;
            config.setB = setB;
            config.colorMultiplier = colorMultiplier;

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
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0;
            autoEdge = true;
            subViewOnly = false;
            edge.edgeColor = Color.black;

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

        string KeyToString(KeyCode kc)
        {
            switch (kc)
            {
                case KeyCode.F5:
                    return "F5";
                case KeyCode.F6:
                    return "F6";
                case KeyCode.F7:
                    return "F7";
                case KeyCode.F8:
                    return "F8";
                case KeyCode.F9:
                    return "F9";
                case KeyCode.F10:
                    return "F10";
                case KeyCode.F11:
                    return "F11";
                case KeyCode.F12:
                    return "F12";
                case KeyCode.LeftBracket:
                    return "LeftBracket";
                case KeyCode.RightBracket:
                    return "RightBracket";
                case KeyCode.Equals:
                    return "=";
                case KeyCode.Slash:
                    return "Slash";
                case KeyCode.Backslash:
                    return "Backslash";
                case KeyCode.Home:
                    return "Home";
                case KeyCode.End:
                    return "End";
                case KeyCode.KeypadDivide:
                    return "Numpad /";
                case KeyCode.KeypadMultiply:
                    return "Numpad *";
                case KeyCode.KeypadMinus:
                    return "Numpad -";
                case KeyCode.KeypadPlus:
                    return "Numpad +";
                case KeyCode.KeypadEquals:
                    return "Numpad =";
                default:
                    return kc.ToString();
            }

        }

        void SettingsPanel(int wnd)
        {
            GUILayout.BeginHorizontal();
            if (!firstTime)
            {
                if (GUILayout.Button("The Bordercities Effect"))
                {
                    tab = Config.Tab.EdgeDetection;
                }
                if (GUILayout.Button("Bonus Effects"))
                {
                    tab = Config.Tab.Bloom;
                }
                if (GUILayout.Button("Hotkey Configuration"))
                {
                    tab = Config.Tab.Hotkey;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(20f);

            if (tab == Config.Tab.EdgeDetection)
            {
                ResizeDefaults();
                if (edge != null)
                {
                    if (!userWantsEdge)
                    {
                        ResizeWindow(803, 140);
                        userWantsEdge = GUILayout.Toggle(userWantsEdge, "Ready?  Click to enable the Bordercities effect!");
                    }
                    else
                        userWantsEdge = GUILayout.Toggle(userWantsEdge, "Click to disable the Bordercities effect.");
                    if (userWantsEdge)
                    {
                        subViewOnly = GUILayout.Toggle(subViewOnly, "Optional: Bordercities Effect enabled for Info Modes only.");

                        if (automaticMode)
                        {
                            ResizeWindow(803, 375);

                            GUILayout.Space(75f);
                            GUILayout.Label("'Plug & Play Mode' is on.  Simply close this window now, or, enter 'Advanced Mode' below using..");



                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("'Plug & Play' settings"))
                            {
                                automaticMode = false;
                            }
                            if (GUILayout.Button("Your last saved settings"))
                            {
                                automaticMode = false;
                                LoadManualSettings();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(75f);
                        }
                        else
                        {
                            GUILayout.Space(25f);
                            ResizeWindow(803, 625);
                            if (GUILayout.Button("Advanced mode is on.  Switch to 'Plug & Play' mode."))
                            {
                                automaticMode = true;
                                RecommendedDefaults();
                            }

                           


                            GUILayout.Space(20f);

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
                                GUILayout.Label("Edge Coloring: (0,0,0 for default black)");
                                GUILayout.Space(5f);


                                GUILayout.BeginHorizontal();
                                roundedR = Mathf.Round(setR * 100f) / 100f;
                                GUILayout.Label("R " + roundedR.ToString());

                                setR = GUILayout.HorizontalSlider(setR, 0.000f, 3.000f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();


                                GUILayout.BeginHorizontal();
                                roundedG = Mathf.Round(setG * 100f) / 100f;
                                GUILayout.Label("G " + roundedG.ToString());

                                setG = GUILayout.HorizontalSlider(setG, 0.000f, 3.000f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();


                                GUILayout.BeginHorizontal();
                                roundedB = Mathf.Round(setB * 100f) / 100f;
                                GUILayout.Label("B " + roundedB.ToString());

                                setB = GUILayout.HorizontalSlider(setB, 0.000f, 3.000f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();
                                
                                

                                GUILayout.Space(10f);

                                GUILayout.BeginHorizontal();
                                roundedMult = Mathf.Round(colorMultiplier * 100f) / 100f;
                                GUILayout.Label("Color multiplier: " + roundedMult.ToString());

                                colorMultiplier = GUILayout.HorizontalSlider(colorMultiplier, 0.0f, 10.0f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();



                                if (GUILayout.Button("Apply Color Settings (Does not overwrite saved color.)"))
                                {
                                    MakeColor(setR,setG,setB);
                                    MakeColor(setR,setG,setB); // double entry here is intentional, I am too in shock over how cool this is to do it totally right but this is acceptable enough for now
                                }

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


                        }
                    }
                }
            }

            if (tab == Config.Tab.Bloom)
            {
                ResizeWindow(803,265);
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
                if (firstTime)
                {
                    ResizeWindow(575, 336);
                    GUILayout.Label("BORDERCITIES FIRST-TIME INITIALIZATION (Never popups again after choice)");
                    GUILayout.Label("Choose and confirm your hotkey for Bordercities.  LeftBracket is default.");
                    GUILayout.Label("NOTE: Bordercities will -never- automatically pop-up again as soon as you've confirmed your hotkey choice.   This initialization process ensures that all users, regardless of hardware, operating system, or current keyboard configuration, will be able to enjoy Bordercities.");


                }


                if (!firstTime)
                {
                    ResizeWindow(538,512);
                    GUILayout.Label("WARNING: HOTKEY BUTTONS WILL SAVE UPON CLICK.  THIS INCLUDES YOUR EFFECTS SETTINGS.");

                }
                KeyboardGrid(0);




                if (firstTime && config.keyCode != KeyCode.None)
                {
                    GUILayout.Space(3f);
                    if (hasClicked)
                        GUILayout.Label("Hotkey '" + KeyToString(config.keyCode) + "' has been chosen and is active.  Confirm it now by using the hotkey.");
                    GUILayout.Space(10f);
                    GUILayout.Label("NOTE: Hotkey can be changed at anytime via the 'Hotkey' window tab in the config panel.");
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
                    if (GUILayout.Button("Reset All (Doesn't save)"))
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
                GUILayout.Label("Recommended External Settings: 175% or above with Dynamic Resolution & 'Tropical' CC");
            }


        }

        void ResizeDefaults()
        {
            windowRect.width = defaultWidth;
            windowRect.height = defaultHeight;
        }

        void SubviewModeState ()
        {
            if (infoManager.CurrentMode == InfoManager.InfoMode.None)
            {
                edge.enabled = false;
            }
            else
            {
                edge.enabled = true;
            }
        }


        
        void EffectState()
        {
            if (userWantsEdge)
            {
                edge.enabled = true;
                if (subViewOnly)
                    SubviewModeState();
            }
            else
            {
                edge.enabled = false;
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
                hasClicked = true;
                SaveConfig();
            }
        }

        void ResizeWindow(int width, int height)
        {
            if (windowRect.height != height)
                windowRect.height = height;
            if (windowRect.width != width)
                windowRect.width = width;
        }

        public void Update()
        {
            EffectState();
            
            if (Input.GetKeyUp(config.keyCode))
            {
                if (!showSettingsPanel)
                    tab = Config.Tab.EdgeDetection;
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
                tab = Config.Tab.EdgeDetection;
                SaveConfig();

            }

            if (firstTime)
            {
                if (!overrideFirstTime)
                    showSettingsPanel = true;
            }
            if (Input.GetKeyUp(config.edgeToggleKeyCode))
                userWantsEdge = !userWantsEdge;

         
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
            SizeCheck(true, 40f, 100f, 1.523f);
            SizeCheck(true, 100, 200, 2.956f);
            SizeCheck(true, 200, 300, 3.405f);
            SizeCheck(true, 300, 400, 3.584f);
            SizeCheck(true, 400, 500, 5.017f);
            SizeCheck(true, 500, 600, 6.989f);
            SizeCheck(true, 600, 700, 8.691f);
            SizeCheck(true, 700, 800, 9.408f);
            SizeCheck(true, 800, 1000, 12.186f);
            SizeCheck(true, 1000, 1100, 15.681f);

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