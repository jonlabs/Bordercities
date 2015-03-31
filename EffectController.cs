﻿using UnityEngine;
using ColossalFramework;
using System.Text.RegularExpressions;
using ICities;

namespace Bordercities
{
    public class EffectController : MonoBehaviour
    {

        public enum ActiveStockPreset
        {
            Bordercities,
            BordercitiesGritty,
            BordercitiesBright,
            Cartoon,
            Realism,
            HighEndPC,
            LowEndMain,
            LowEndAlt,
            Random,
            CartoonAlt,
            CartoonThree,
        }

        public bool showSettingsPanel = false;
        private Rect windowRect = new Rect(32, 32, 803, 700); //was 64,250,803,466
        private Rect dragBar = new Rect(0, 0, 803, 25);
        private Vector2 windowLoc;
        private float defaultHeight;
        private float defaultWidth;

        public Config config;
        public Preset preset;
        public PresetBank bank;

        private const string configPath = "BordercitiesConfig.xml";
        private const string bankPath = "BordercitiesPresetBank.xml";


        private EdgeDetection edge;
        private BloomOptimized bloom;
        private ToneMapping tonem;

        public Config.Tab tab;
        public bool autoEdge;
        public bool firstTime;
        private string displayTitle;
        private string displayText;
        private bool hasClicked = false;

        public bool overrideFirstTime;

        private CameraController cameraController;
        private InfoManager infoManager;
        public float oldToneMapBoost;
        public float toneMapBoost;
        public float oldGamma;
        public float toneMapGamma;

        private bool autoEdgeActive;
        public bool subViewOnly;
        public bool useInfoModeSpecific;

        private string keystring = "";
        private string edgeKeyString = "";

        private bool automaticMode;

        private bool showAdvanced = false;
        private bool showEdgeColors = false;
        private bool showMixColors = false;

        public bool wantsToneMapper = false;
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

        public Color mixCurrentColor;
        public float mixSetR;
        public float mixSetG;
        public float mixSetB;
        public float mixColorMultiplier = 1f;
        private Color mixNewColor;
        private float mixRoundedR;
        private float mixRoundedG;
        private float mixRoundedB;
        private float mixRoundedMult;

        private float defaultGamma;
        private float defaultBoost;
        private float prevGamma;
        private float prevBoost;


        public string[] presetEntries;

        private Color cartoonEdgeC;
        private Color cartoonMixC;
        private Color lowEndEdgeC;
        private Color realismEdgeC;

        private ActiveStockPreset activeStockPreset;


        private string infoBuildingLevel = "";
        private string infoConnections = "";
        private string infoCrime = "";
        private string infoDensity = "";
        private string infoDistricts = "";
        private string infoEducation = "";
        private string infoElectricity = "";
        private string infoEntertainment = "";
        private string infoFireSafety = "";
        private string infoGarbage = "";
        private string infoHappiness = "";
        private string infoHealth = "";
        private string infoLandValue = "";
        private string infoNaturalResources = "";
        private string infoNoisePollution = "";
        private string infoPollution = "";
        private string infoTerrainHeight = "";
        private string infoTraffic = "";
        private string infoTransport = "";
        private string infoWater = "";
        private string infoWind = "";

        private string inputInfoBuildingLevel = "";
        private string inputInfoConnections = "";
        private string inputInfoCrime = "";
        private string inputInfoDensity = "";
        private string inputInfoDistricts = "";
        private string inputInfoEducation = "";
        private string inputInfoElectricity = "";
        private string inputInfoEntertainment = "";
        private string inputInfoFireSafety = "";
        private string inputInfoGarbage = "";
        private string inputInfoHappiness = "";
        private string inputInfoHealth = "";
        private string inputInfoLandValue = "";
        private string inputInfoNaturalResources = "";
        private string inputInfoNoisePollution = "";
        private string inputInfoPollution = "";
        private string inputInfoTerrainHeight = "";
        private string inputInfoTraffic = "";
        private string inputInfoTransport = "";
        private string inputInfoWater = "";
        private string inputInfoWind = "";

        private InfoManager.InfoMode currentInfoMode;

        private bool isOn;
        private bool userIsPreviewing = false;
      
        

        void InitializeColors()
        {
            cartoonEdgeC = new Color(0.04f, 0.04f, 0.04f);
            lowEndEdgeC = new Color(0.08f, 0.08f, 0.06f);
            realismEdgeC = new Color(0.17f,0.17f,0.17f);
           
        }

        void Awake()
        {
            InitializeColors();
            
            cameraController = GetComponent<CameraController>();
            infoManager = InfoManager.instance;
            currentInfoMode = infoManager.CurrentMode;
            config = Config.Deserialize(configPath);
            bank = PresetBank.Deserialize(bankPath);
            
            defaultWidth = windowRect.width;
            defaultHeight = windowRect.height;

            InitializeFromConfig();
            InitializeBanking();
            InitializeExistBools();

        }

        

        void InitializeFromConfig()
        {
            if (config == null) 
            {
                config = new Config();
                #region parameters#
                config.automaticMode = true;
                config.edgeEnabled = false;
                config.edgeMode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                config.edgeSamp = 1.0f;
                config.edgeOnly = 0;
                //TriangleRoberts
                config.sensNorm = 1.63f;
                config.sensDepth = 2.12f;

                //Sobel
                config.edgeExpo = 0.09f;
                config.depthsAxis = 0.100f;
                config.depthsDiagonal = 1.000f;
                config.sobelMult1 = 1.000f;
                config.sobelMult2 = 1.000f;
                config.sobelMult3 = 1.000f;
                config.sobelMult4 = 1.000f;

                config.wantsTonemapper = false;

                config.autoEdge = true;
                config.firstTime = true;

                config.subViewOnly = false;
                config.useInfoModeSpecific = false;

                config.oldGamma = 2.2f;
                config.toneMapGamma = 1.6f;
                config.oldBoost = 1.15f;
                config.toneMapBoost = 5f;

                config.currentColor = new Color(0, 0, 0, 0);
                config.colorMultiplier = 1.0f;

                config.mixColorMultiplier = 1.0f;
                config.mixCurrentColor = new Color(1, 1, 1, 0);

                config.bloomEnabled = false;
                config.bloomThresh = 0.27f;
                config.bloomIntens = 0.39f;
                config.bloomBlurSize = 5.50f;

                config.cartoonMixColor = Color.white;
                config.activeStockPreset = ActiveStockPreset.LowEndAlt;
                config.windowLoc = new Vector2(32, 32);

                
                
                #endregion
            }
            else 
            {
                #region parameters
                if (IsNull(config.automaticMode))
                    config.automaticMode = true;
                if (IsNull(config.edgeEnabled))
                    config.edgeEnabled = false;
                if (IsNull(config.edgeMode))
                    config.edgeMode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                if (IsNull(config.edgeSamp))
                    config.edgeSamp = 1.0f;
                if (IsNull(config.edgeOnly))
                    config.edgeOnly = 0;

                if (IsNull(config.wantsTonemapper))
                    config.wantsTonemapper = false;

                if (IsNull(config.sensNorm))
                    config.sensNorm = 1.63f;
                if (IsNull(config.sensDepth))
                    config.sensDepth = 2.12f;


                if (IsNull(config.edgeExpo))
                    config.edgeExpo = 0.09f;
                if (IsNull(config.depthsDiagonal))
                    config.depthsDiagonal = 1.0f;
                if (IsNull(config.depthsAxis))
                    config.depthsAxis = 1.0f;
                if (IsNull(config.sobelMult1))
                    config.sobelMult1 = 1.000f;
                if (IsNull(config.sobelMult2))
                    config.sobelMult2 = 1.000f;
                if (IsNull(config.sobelMult3))
                    config.sobelMult3 = 1.000f;
                if (IsNull(config.sobelMult4))
                    config.sobelMult4 = 1.000f;



                if (IsNull(config.autoEdge))
                    config.autoEdge = true;
                if (IsNull(config.firstTime))
                    config.firstTime = true;
                if (IsNull(config.subViewOnly))
                    config.subViewOnly = false;
                if (IsNull(config.useInfoModeSpecific))
                    config.useInfoModeSpecific = false;
                if (IsNull(config.oldGamma))
                    config.oldGamma = 2.2f;
                if (IsNull(config.toneMapGamma))
                    config.toneMapGamma = 1.6f;
                if (IsNull(config.oldBoost))
                    config.oldBoost = 1.15f;
                if (IsNull(config.toneMapBoost))
                    config.toneMapBoost = 5f;

                if (IsNull(config.currentColor))
                    config.currentColor = new Color(0, 0, 0, 0);
                if (IsNull(config.colorMultiplier))
                    config.colorMultiplier = 1.0f;

                if (IsNull(config.mixColorMultiplier))
                    config.mixColorMultiplier = 1.0f;
                if (IsNull(config.mixCurrentColor))
                    config.mixCurrentColor = new Color(1, 1, 1, 0);

                if (IsNull(config.bloomEnabled))
                    config.bloomEnabled = false;
                if (IsNull(config.bloomThresh))
                    config.bloomThresh = 0.27f;
                if (IsNull(config.bloomIntens))
                    config.bloomIntens = 0.39f;
                if (IsNull(config.bloomBlurSize))
                    config.bloomBlurSize = 5.50f;
                if (IsNull(config.activeStockPreset))
                    config.activeStockPreset = ActiveStockPreset.LowEndAlt;
                if (IsNull(config.cartoonMixColor))
                    config.cartoonMixColor = new Color(1, 1, 1, 0);
                if (IsNull(config.windowLoc))
                    config.windowLoc = new Vector2(32,32);
                
                #endregion

            }
        }

        void InitializeBanking()
        {
            presetEntries = new string[30];
            for (int i = 0; i < presetEntries.Length; i++)
            {
                presetEntries[i] = "";
            }

            if (bank == null)
            {
                bank = new PresetBank();
                bank.presetEntries = new string[30];

                for (int i = 0; i < presetEntries.Length; i++)
                {
                    bank.presetEntries[i] = "";
                }
            }
            LoadBank();
        }

        void ResetBank()
        {
            for (int i = 0; i < presetEntries.Length; i++)
            {
                presetEntries[i] = "";
            }
            SaveBank();
        }

        void LoadBank()
        {

            presetEntries = bank.presetEntries;
            for (int i = 0; i < presetEntries.Length; i++)
            {
                presetEntries[i] = bank.presetEntries[i];
            }
        }

        static bool IsNull(System.Object aObj)
        {
            return aObj == null || aObj.Equals(null);
        }

        void Start()
        {
            edge = GetComponent<EdgeDetection>();
            bloom = GetComponent<BloomOptimized>();
            tonem = GetComponent<ToneMapping>();
            defaultBoost = tonem.m_ToneMappingBoostFactor;
            defaultGamma = tonem.m_ToneMappingGamma;
            prevGamma = defaultGamma;
            prevBoost = defaultBoost;
            LoadConfig(false);
            
            if (config.keyCode == KeyCode.None)
            {
                config.keyCode = KeyCode.LeftBracket;
            }
            SaveBank();
            SaveConfig();
            if (firstTime && automaticMode)
                LowEndAutomatic();

            if (firstTime)
            {
                showSettingsPanel = true;
                tab = Config.Tab.Hotkey;
            }
            else
            {
                tab = Config.Tab.EdgeDetection;
            }
            toneMapGamma = config.toneMapGamma;
            toneMapBoost = config.toneMapBoost;
            if (config.edgeEnabled)
                ToggleBorderedSkylines(true);
            else
                ToggleBorderedSkylines(false);

            FixTonemapperIfZeroed();

        }



        void FixTonemapperIfZeroed()
        {
            if (tonem.m_ToneMappingGamma == 0)
                tonem.m_ToneMappingGamma = defaultGamma;
            if (tonem.m_ToneMappingBoostFactor == 0)
                tonem.m_ToneMappingBoostFactor = defaultBoost;
        }

        void EdgeColor(float r, float g, float b)
        {
            float newR = r * colorMultiplier;
            float newG = g * colorMultiplier;
            float newB = b * colorMultiplier;
            newColor = new Color(newR, newG, newB, 0);
            edge.SetEdgeColor(newColor);
            currentColor = newColor;
        }

        void MixColor(float r, float g, float b)
        {
            float mixNewR = r * mixColorMultiplier;
            float mixNewG = g * mixColorMultiplier;
            float mixNewB = b * mixColorMultiplier;
            mixNewColor = new Color(mixNewR, mixNewG, mixNewB, 0);
            edge.SetMixColor(mixNewColor);
            mixCurrentColor = mixNewColor;
        }

        void LoadConfig(bool falseIfInitializationLoadOnly)
        {
            if (!falseIfInitializationLoadOnly)
            {
                automaticMode = config.automaticMode;
                edge.enabled = config.edgeEnabled;
                subViewOnly = config.subViewOnly;
                useInfoModeSpecific = config.useInfoModeSpecific;
                firstTime = config.firstTime;
                windowLoc = config.windowLoc;
                windowRect.x = windowLoc.x;
                windowRect.y = windowRect.y;
                wantsToneMapper = config.wantsTonemapper;
            }
            toneMapGamma = config.toneMapGamma;
            toneMapBoost = config.toneMapBoost;
            cartoonMixC = config.cartoonMixColor;
            activeStockPreset = config.activeStockPreset;
            edge.mode = config.edgeMode;
            edge.edgesOnly = config.edgeOnly;
            edge.sampleDist = config.edgeSamp;
            //TriangleRoberts
            edge.sensitivityNormals = config.sensNorm;
            edge.sensitivityDepth = config.sensDepth;
            //Sobel
            edge.edgeExp = config.edgeExpo;
            edge.depthsDiagonal = config.depthsDiagonal;
            edge.depthsAxis = config.depthsAxis;
            edge.mult1 = config.sobelMult1;
            edge.mult2 = config.sobelMult2;
            edge.mult3 = config.sobelMult3;
            edge.mult4 = config.sobelMult4;
            autoEdge = config.autoEdge;
            currentColor = config.currentColor;
            edge.edgeColor = currentColor;
            colorMultiplier = config.colorMultiplier;
            mixCurrentColor = config.mixCurrentColor;
            edge.edgesOnlyBgColor = mixCurrentColor;
            mixColorMultiplier = config.mixColorMultiplier;
            setR = config.setR;
            setG = config.setG;
            setB = config.setB;
            mixSetR = config.mixSetR;
            mixSetG = config.mixSetG;
            mixSetB = config.mixSetB;
            bloom.enabled = config.bloomEnabled;
            bloom.threshold = config.bloomThresh;
            bloom.intensity = config.bloomIntens;
            bloom.blurSize = config.bloomBlurSize;
            

            if (automaticMode && !firstTime)
            {
                switch (activeStockPreset)
                {
                    case ActiveStockPreset.Cartoon:
                        CartoonAutomatic();
                        break;
                    case ActiveStockPreset.Bordercities:
                       BordercitiesAutomatic();
                        break;
                    case ActiveStockPreset.BordercitiesGritty:
                        BordercitiesGrittyAutomatic();
                        break;
                    case ActiveStockPreset.BordercitiesBright:
                        BordercitiesGrittyAutomatic();
                        break;
                    case ActiveStockPreset.LowEndMain:
                        LowEndAutomatic();
                        break;
                    case ActiveStockPreset.LowEndAlt:
                        LowEndAltAutomatic();
                        break;
                    case ActiveStockPreset.Realism:
                        RealismAutomatic();
                        break;
                    case ActiveStockPreset.Random:
                        RandomAutomatic();
                        break;
                    default:
                        LowEndAutomatic();
                        break;
                }
            }
        }



        public void SaveConfig()
        {
            config.subViewOnly = subViewOnly;
            config.useInfoModeSpecific = useInfoModeSpecific;

            config.wantsTonemapper = wantsToneMapper;
            config.automaticMode = automaticMode;
            config.edgeEnabled = edge.enabled;
            config.edgeMode = edge.mode;
            config.edgeOnly = edge.edgesOnly;
            config.edgeSamp = edge.sampleDist;
            config.autoEdge = autoEdge;
            //TriangleRobert
            config.sensNorm = edge.sensitivityNormals;
            config.sensDepth = edge.sensitivityDepth;
            //Sobel
            config.edgeExpo = edge.edgeExp;
            config.depthsDiagonal = edge.depthsDiagonal;
            config.depthsAxis = edge.depthsAxis;
            config.sobelMult1 = edge.mult1;
            config.sobelMult2 = edge.mult2;
            config.sobelMult3 = edge.mult3;
            config.sobelMult4 = edge.mult4;

            config.subViewOnly = subViewOnly;
            config.useInfoModeSpecific = useInfoModeSpecific;

            config.currentColor = currentColor;
            config.setR = setR;
            config.setG = setG;
            config.setB = setB;
            config.colorMultiplier = colorMultiplier;

            config.mixCurrentColor = mixCurrentColor;
            config.mixSetR = mixSetR;
            config.mixSetG = mixSetG;
            config.mixSetB = mixSetB;
            config.mixColorMultiplier = mixColorMultiplier;

            config.bloomEnabled = bloom.enabled;
            config.bloomThresh = bloom.threshold;
            config.bloomIntens = bloom.intensity;
            config.bloomBlurSize = bloom.blurSize;
            config.firstTime = firstTime;

            config.toneMapBoost = toneMapBoost;
            config.toneMapGamma = toneMapGamma;

            config.activeStockPreset = activeStockPreset;
            config.cartoonMixColor = cartoonMixC;

            config.windowLoc = windowLoc;

            


            Config.Serialize(configPath, config);
        }

        public void SaveBank()
        {
            bank.presetEntries = presetEntries;
            for (int i = 0; i < presetEntries.Length; i++ )
            {
                bank.presetEntries[i] = presetEntries[i];
            }
            PresetBank.Serialize(bankPath, bank);
        }

        

        

        void LowEndAutomatic()
        {
            displayTitle = "Low End PC - REQUIRES (1280x720)-(1920x1080) AND (NO DR)-(150% DR) FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.LowEndMain;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.sampleDist = 1f;
            edge.edgesOnly = 0.01f;
            edge.edgeExp = 0.43f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "Suited for users of low-end hardware who cannot afford the performance hit of supersampling via nlight's Dynamic Resolution.  This mode was fine-tuned in 720p with all settings as low as possible. NOTE 3/30:  Coming soon (likely later tonight/early-morning), a 'Part 2' update with an algorithm to keep the effect balanced at all zoom levels!";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            edge.depthsDiagonal = 1f;
            edge.axisVsCenter = 0.1f;
            edge.mult1 = 0.850f;
            edge.mult3 = 0.752f;
            edge.mult2 = 0.501f;
            edge.mult4 = 2.437f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void LowEndAltAutomatic()
        {
            displayTitle = "Low End Alternative - REQUIRES (1280x720)-(1920x1080) AND (NO DR)-(150% DR) FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.LowEndAlt;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.sampleDist = 1f;
            autoEdge = true;
            edge.edgeExp = 0.5f;
            edge.edgesOnly = 0.01f;
            edge.edgeColor = lowEndEdgeC;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "Suited for low-end PC users who desire a greater effect than what 'Low End PC' provides, this mode is configured for appropriate line size at low resolutions.  This mode has been specifically tweaked while playing in 720p with the very possible lowest settings.  If edges appear to be too jagged and bothersome, try turning your graphics settings down and/or switching to 720p -- it will look cleaner as there are less lines to be picked up by the edge detection.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void BordercitiesAutomatic()
        {
            displayTitle = "Bordercities Classic - REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.Bordercities;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "Bordercities very specifically attempts to capture the vibe of Borderlands & XIII.  This is the original Bordercities preset, before the gamma/boost settings were set per preset.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void BordercitiesGrittyAutomatic()
        {
            displayTitle = "Bordercities Gritty - REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.BordercitiesGritty;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "Bordercities-Gritty adds a bit of extra grit over the normal Bordercities preset.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 1.123f;
                    toneMapBoost = 3.061f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void BordercitiesBrightAutomatic()
        {
            displayTitle = "Bordercities Bright - REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.BordercitiesBright;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "Bordercities-Bright brings a little bit more fun to the world than its vanilla-based and grittier counterparts.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 1.881f;
                    toneMapBoost = 2.849f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void RealismAutomatic()
        {
            displayTitle = "Realism - REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.Realism;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.sampleDist = 1;
            edge.edgesOnly = 0;
            edge.edgeExp = 0.5f;
            edge.depthsDiagonal = 1.0f;
            edge.axisVsCenter = 0.1f;
            edge.mult1 = 0.018f;
            edge.mult3 = 0.501f;
            edge.mult2 = 2.580f;
            edge.mult4 = 1.451f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "This preset is designed for those who wish to use 'Edge Detection' for use as a visual enhancement tool, not an effect.  As an enhancement tool, 'Edge Detection' maintains the visual detection of distant shapes which would otherwise be lost by the renderer at a distance - such as street lamps.  Of course, nature doesn't draw lines around everything, nor may your monitor look like mine, thus, you may need to tweak this further to your own satisfaction.  I strongly recommend using 'Mix' to your benefit so that you can dial in the right amount of settings. Regardless, do keep in mind that Bordered Skylines is not an ambient occlusion mod.  If you are looking for -shadowing- at -edge intersections-, try Ulysius' Ambient Occlusion.  NOTE 3/30:  Coming soon (likely later tonight/early-morning), a 'Part 2' update with an algorithm to keep the effect balanced at all zoom levels!";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void CartoonAutomatic()
        {
            displayTitle = "Cartoon|Retro REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.Cartoon;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.sampleDist = 1.09f;
            edge.edgesOnly = 0.28f;
            autoEdge = false;
            edge.sensitivityDepth = 0;
            edge.sensitivityNormals = 1.68f;
            mixColorMultiplier = 1.0f;
            edge.edgeColor = cartoonEdgeC;
            edge.edgesOnlyBgColor = cartoonMixC;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            displayText = "No explanation required!  Optionally, add a random color theme by clicking the below button until satisfied.  Make sure to save your color once you've found it!  If you prefer to tweak this manually, simply convert your 'Plug & Play' into your own personalized preset by entering Advanced Mode via 'currently displayed settings', tweak to your liking, and save!";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 3.064f;
                    toneMapBoost = 0.537f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void CartoonAltAutomatic()
        {
            displayTitle = "Cartoon|Colorful - REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.CartoonAlt;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.sampleDist = 1.09f;
            edge.edgesOnly = 0.067f;
            autoEdge = false;
            edge.sensitivityDepth = 0;
            edge.sensitivityNormals = 1.68f;
            mixColorMultiplier = 1.0f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = cartoonMixC;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            displayText = "No explanation required!  Optionally, add a random color theme by clicking the below button until satisfied.  Make sure to save your color once you've found it!  If you prefer to tweak this manually, simply convert your 'Plug & Play' into your own personalized preset by entering Advanced Mode via 'currently displayed settings', tweak to your liking, and save!";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 5.376f;
                    toneMapBoost = 0.128f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }
        void CartoonThreeAutomatic()
        {
            displayTitle = "Cartoon|Clean - REQUIRES 1920x1080 AND DYNAMIC RESOLUTION 175-250% FOR INTENDED LOOK.";
            activeStockPreset = ActiveStockPreset.CartoonThree;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.sampleDist = 1;
            edge.edgesOnly = 0f;
            autoEdge = false;
            edge.sensitivityDepth = 0;
            edge.sensitivityNormals = 1.68f;
            mixColorMultiplier = 1.0f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = cartoonMixC;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            displayText = "No explanation required!  Unlike the other cartoon presets, this one does not use a color theme.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 3.978f;
                    toneMapBoost = 0.376f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void RandomAutomatic()
        {
            displayTitle = "Random - NO SETTINGS PREFERENCE";
            activeStockPreset = ActiveStockPreset.Random;
            automaticMode = true;
            edge.mode = (EdgeDetection.EdgeDetectMode)Random.Range(0, 3);
            edge.sampleDist = Random.Range(0.00f,5.00f);
            edge.edgesOnlyBgColor = new Color(Random.Range(0.0f,5.0f),Random.Range(0.0f,5.0f), Random.Range(0.0f,5.0f));
            mixColorMultiplier = 1.0f;
            edge.edgeColor = new Color(Random.Range(0.0f,5.0f),Random.Range(0.0f,5.0f), Random.Range(0.0f,5.0f));
            edge.edgesOnly = Random.Range(0.00f, 1.00f);
            autoEdge = true;
            edge.edgeExp = 0.5f;
            displayText = "This preset was generated randomly.  Enter Advanced Mode below to tweak further.   Note that you will lose this effect if you fail to save it before returning to 'Plug & Play' mode.";
            bloom.enabled = false;
            
        }

        void UltraAutomatic()
        {
            automaticMode = true;
            displayTitle = "Ultra - REQUIRES 1920x1080 + DR 250-300% + 'AMBIENT OCC' + 'SUN SHAFTS' + 'POSTPROCESSFX' WITH BLOOM, MOTION BLUR, AND DLAA ANTI-ALIASING FOR INTENDED LOOK.";
            displayText = "This mode looks AB-SO-FREAKING-LUTE-LY SPECTACULAR!!!!!!!!!!!!!  ..if you can actually run it.  Tweaked at 2FPS on 300%, this mode will lend itself to absolutely STELLAR screenshots,  though unless you've got 980s in SLI (random guess based on my 7970 GHz performance vs. benchmarks) is likely not going to be actually playable for the forseeable future.";
            edge.sampleDist = 2.0f;
            activeStockPreset = ActiveStockPreset.HighEndPC;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            autoEdge = true;
            edge.edgesOnly = 0f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            MatchColorsOnGUI();
            bloom.enabled = false;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapBoost = defaultBoost;
                    toneMapGamma = defaultGamma;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
            


        }


        void OnGUI()
        {
            GUI.backgroundColor = Color.gray;
            if (firstTime)
                tab = Config.Tab.Hotkey;
            if (showSettingsPanel)
            {
                windowRect = GUI.Window(391435, windowRect, SettingsPanel, "Bordered Skylines (NEW: Draggable Window!)");
            }
        }

        void MatchColorsOnGUI()
        {
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
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

        void SavePreset(string name, bool falseIfInfoMode)
        {
            Preset.MakeFolderIfNonexistent();
            Preset presetToSave;
            if (falseIfInfoMode)
                presetToSave = Preset.Deserialize(name);
            else
                presetToSave = Preset.DeserializeInfoMode(name);
            if (presetToSave == null) // If no presets file, create one.
            {
                presetToSave = new Preset();
            }
            presetToSave.edgeMode = edge.mode;
            presetToSave.sensNorm = edge.sensitivityNormals;
            presetToSave.sensDepth = edge.sensitivityDepth;
            presetToSave.edgeOnly = edge.edgesOnly;
            presetToSave.edgeSamp = edge.sampleDist;
            presetToSave.autoEdge = autoEdge;
            presetToSave.subViewOnly = subViewOnly;

            presetToSave.edgeExpo = edge.edgeExp;
            presetToSave.depthsAxis = edge.depthsAxis;
            presetToSave.depthsDiagonal = edge.depthsDiagonal;
            presetToSave.sobelMult1 = edge.mult1;
            presetToSave.sobelMult2 = edge.mult2;
            presetToSave.sobelMult3 = edge.mult3;
            presetToSave.sobelMult4 = edge.mult4;

            presetToSave.currentColor = currentColor;
            presetToSave.setR = setR;
            presetToSave.setG = setG;
            presetToSave.setB = setB;
            presetToSave.colorMultiplier = colorMultiplier;

            presetToSave.mixCurrentColor = mixCurrentColor;
            presetToSave.mixSetR = mixSetR;
            presetToSave.mixSetG = mixSetG;
            presetToSave.mixSetB = mixSetB;
            presetToSave.mixColorMultiplier = mixColorMultiplier;

            presetToSave.bloomEnabled = bloom.enabled;
            presetToSave.bloomThresh = bloom.threshold;
            presetToSave.bloomIntens = bloom.intensity;
            presetToSave.bloomBlurSize = bloom.blurSize;

            presetToSave.toneMapBoost = tonem.m_ToneMappingBoostFactor;
            presetToSave.toneMapGamma = tonem.m_ToneMappingGamma;

            if (falseIfInfoMode)
                Preset.Serialize(name, presetToSave);
            else
                Preset.SerializeInfoMode(name, presetToSave);
        }


        void SettingsPanel(int wnd)
        {
            
            GUI.DragWindow(dragBar);
            #region Top Navigation Buttons
            GUILayout.BeginHorizontal();
            if (!firstTime || !automaticMode)
            {
                if (GUILayout.Button("Main"))
                {
                    tab = Config.Tab.EdgeDetection;
                }
                if (!automaticMode)
                {
                    if (GUILayout.Button("Bonus Effects"))
                    {
                        tab = Config.Tab.Bloom;
                    }
                    if (GUILayout.Button("Presets"))
                    {
                        LoadBank();
                        tab = Config.Tab.Presets;

                    }
                }
                if (useInfoModeSpecific)
                {
                    if (GUILayout.Button("Info Modes"))
                    {
                        tab = Config.Tab.ViewModes;
                    }
                }
                if (GUILayout.Button("Hotkey Configuration"))
                {
                    tab = Config.Tab.Hotkey;
                }
                
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(20f);
#endregion
            #region Tab - Edge Detection
            if (tab == Config.Tab.EdgeDetection)
            {
                ResizeDefaults();
                if (edge != null)
                {
                    if (!isOn)
                    {
                        ResizeWindow(803, 190);
                        GUILayout.Space(30f);
                        if (GUILayout.Button("Enable Bordered Skylines"))
                        {
                            ToggleBorderedSkylines(true);
                        }
                        GUILayout.Space(30f);
                        
                    }
                    if (isOn)
                    {

                        if (automaticMode)
                        {
                            ResizeWindow(803, 690);

                            GUILayout.Space(5f);
                            GUILayout.Label("Pick a preset!  Or, jump into Advanced Mode down below to start tweaking for yourself!");
                            GUILayout.Space(10f);
                            wantsToneMapper = GUILayout.Toggle(wantsToneMapper, "A large part of the immersion of the below presets is their usage of the internal brightness/gamma settings.  They can be reset at any time via the below 'Reset Brightness' button.  Use these settings??");
                            GUILayout.Space(10f);
                            
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Low End PC",GUILayout.MaxWidth(120f)))
                            {
                                LowEndAutomatic();
                            }
                            if (GUILayout.Button("Low End Alt.", GUILayout.MaxWidth(120f)))
                            {
                                LowEndAltAutomatic();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Cartoon|Clean", GUILayout.MaxWidth(110f)))
                            {
                                CartoonThreeAutomatic();
                            }
                            if (GUILayout.Button("Cartoon|Retro", GUILayout.MaxWidth(110f)))
                            {
                                CartoonAutomatic();
                            }
                            if (GUILayout.Button("Cartoon|Colorful", GUILayout.MaxWidth(110f)))
                            {
                                CartoonAltAutomatic();
                            }
                            GUILayout.EndHorizontal();
                            
                            
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Bordercities|Classic", GUILayout.MaxWidth(160f)))
                            {
                                BordercitiesAutomatic();
                            }
                            if (GUILayout.Button("Bordercities|Bright", GUILayout.MaxWidth(270f)))
                            {
                                BordercitiesBrightAutomatic();
                            }
                            if (GUILayout.Button("Bordercities|Gritty", GUILayout.MaxWidth(160f)))
                            {
                                BordercitiesGrittyAutomatic();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Realism", GUILayout.MaxWidth(160f)))
                            {
                                RealismAutomatic();
                            }
                            if (GUILayout.Button("Ultra", GUILayout.MaxWidth(90f)))
                            {
                                UltraAutomatic();
                            }
                            GUILayout.EndHorizontal();
                            if (GUILayout.Button("Random (WARNING: Could potentially be very bright!", GUILayout.MaxWidth(330)))
                            {
                                RandomAutomatic();
                            }
                            GUILayout.Space(10f);


                            GUILayout.Space(10f);
                            if (displayText != null && displayTitle != null)
                            {
                                GUILayout.Label("CURRENT MODE: "+displayTitle);
                                GUILayout.Space(3f);
                                GUILayout.Label(displayText);
                            }
                            if (activeStockPreset == ActiveStockPreset.Cartoon || activeStockPreset == ActiveStockPreset.CartoonAlt)
                            {
                                if (GUILayout.Button("CARTOON SPECIFIC: Randomize Color Theme)"))
                                {
                                    Color tempColor = new Color(Random.Range(0.00f, 1.00f), Random.Range(0.00f, 1.00f), Random.Range(0.00f, 1.00f));
                                    cartoonMixC = tempColor;
                                    edge.edgesOnlyBgColor = cartoonMixC;
                                    mixSetR = edge.edgesOnlyBgColor.r;
                                    mixSetG = edge.edgesOnlyBgColor.g;
                                    mixSetB = edge.edgesOnlyBgColor.b;
                                    setR = edge.edgeColor.r;
                                    setG = edge.edgeColor.g;
                                    setB = edge.edgeColor.b;
                                    mixColorMultiplier = 1.0f;
                                    colorMultiplier = 1.0f;
                                }
                                if (GUILayout.Button("Reset Cartoon Color"))
                                {
                                    cartoonMixC = Color.white;
                                    edge.edgesOnlyBgColor = cartoonMixC;
                                    mixSetR = 1.0f;
                                    mixSetG = 1.0f;
                                    mixSetB = 1.0f;
                                    setR = 1.0f;
                                    setG = 1.0f;
                                    setB = 1.0f;
                                    mixColorMultiplier = 1.0f;
                                    colorMultiplier = 1.0f;
                                }
                            }
                            GUILayout.Space(20f);
                            GUILayout.Label("If you are unsatisfied with these stock presets, or wish to build from scratch, enter Advanced Mode below with:");
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Currently displayed settings"))
                            {
                                automaticMode = false;

                            }
                            if (GUILayout.Button("Your previously saved settings"))
                            {
                                automaticMode = false;
                                LoadConfig(true);
                            }
                            GUILayout.EndHorizontal();
                            if (!firstTime)
                            {
                                GUILayout.Space(5f);
                                if (config.edgeToggleKeyCode == KeyCode.None)
                                    GUILayout.Label("Hey! Listen! You can set a hotkey for toggling the Bordercities Effect! It would appear you haven't done this yet: simply press the 'Hotkey Configuration' tab and choose a hotkey from the bottom keyboard grid!");
                                GUILayout.Space(5f);
                            }
                            else
                                GUILayout.Space(15f);

                            

                        }
                        else
                        {
                            GUILayout.Space(10f);
                            ResizeWindow(803, 1000);
                            GUILayout.Label("Sigh.. the GUI is a wreck.  Housecleaning is ever more-so a top priority with each update!");

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
                                        GUILayout.Label("Edge mode: Legacy 'Sobel Depth'");
                                        break;
                                    }
                                case EdgeDetection.EdgeDetectMode.SobelDepthThin:
                                    {
                                        GUILayout.Label("Edge mode: 'Sobel Skylines' (Upgrades & combines both old Sobels into one)");
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
                            if (GUILayout.Button("Legacy Sobel"))
                            {
                                edge.mode = EdgeDetection.EdgeDetectMode.SobelDepth;
                                edge.SetCameraFlag();
                            }
                            if (GUILayout.Button("'Sobel Skylines' (New)"))
                            {
                                edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                                edge.SetCameraFlag();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Label("Edge distance: " + edge.sampleDist.ToString());
                            edge.sampleDist = GUILayout.HorizontalSlider(edge.sampleDist, 1, 5, GUILayout.MaxWidth(100));
                            GUILayout.Label("Mix: " + edge.edgesOnly.ToString());
                            edge.edgesOnly = GUILayout.HorizontalSlider(edge.edgesOnly, 0.000f, 1.000f, GUILayout.MaxWidth(500));                            
                            

                            if (edge.mode == EdgeDetection.EdgeDetectMode.TriangleDepthNormals || edge.mode == EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals)
                            {
                                


                                GUILayout.Label("Depth sensitivity: " + edge.sensitivityDepth.ToString());
                                if (!autoEdge)
                                    edge.sensitivityDepth = GUILayout.HorizontalSlider(edge.sensitivityDepth, 0.000f, 50.000f, GUILayout.Width(570));
                                GUILayout.Label("Normal sensitivity: " + edge.sensitivityNormals.ToString());
                                if (!autoEdge)
                                    edge.sensitivityNormals = GUILayout.HorizontalSlider(edge.sensitivityNormals, 0.000f, 5.000f, GUILayout.Width(570));


                                autoEdge = GUILayout.Toggle(autoEdge, "Automatic Sensitivity");

                            }
                            if (edge.mode == EdgeDetection.EdgeDetectMode.SobelDepthThin)
                            {
                                showAdvanced = GUILayout.Toggle(showAdvanced, "Show advanced settings");
                                if (showAdvanced)
                                {
                                    
                                    GUILayout.Label("Diagonal Depth: " + edge.depthsDiagonal.ToString());
                                    edge.depthsDiagonal = GUILayout.HorizontalSlider(edge.depthsDiagonal, 0.000f, 1.000f, GUILayout.Width(570));
                                    
                                    GUILayout.Label("Axis/Center: " + edge.axisVsCenter.ToString());
                                    edge.axisVsCenter = GUILayout.HorizontalSlider(edge.axisVsCenter, 0.001f, 0.100f, GUILayout.Width(570));
                                    GUILayout.Space(10f);
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Horizontal:" + edge.mult1.ToString());
                                    edge.mult1 = GUILayout.HorizontalSlider(edge.mult1, 0.000f, 10.100f, GUILayout.Width(570));
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Fine-tune H:" + edge.mult3.ToString());
                                    edge.mult3 = GUILayout.HorizontalSlider(edge.mult3, 0.000f, 10.000f, GUILayout.Width(570));
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Vertical:" + edge.mult2.ToString());
                                    edge.mult2 = GUILayout.HorizontalSlider(edge.mult2, 0.000f, 10.000f, GUILayout.Width(570));
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Fine-tune V:" + edge.mult4.ToString());
                                    edge.mult4 = GUILayout.HorizontalSlider(edge.mult4, 0.000f, 10.000f, GUILayout.Width(570));
                                    //edge.diagonalVsCenter = GUILayout.HorizontalSlider(edge.diagonalVsCenter, 0.000f, 2.000f, GUILayout.Width(570)); 
                                    GUILayout.EndHorizontal();
                                }
                                
                               

                            }
                            if (edge.mode == EdgeDetection.EdgeDetectMode.SobelDepth)
                            {
                                GUILayout.Label("Edge exponent: " + edge.edgeExp.ToString());
                                edge.edgeExp = GUILayout.HorizontalSlider(edge.edgeExp, 0.004f, 1.000f, GUILayout.Width(570));
                            }
                            showEdgeColors = GUILayout.Toggle(showEdgeColors, "Show edge colors");
                            if (showEdgeColors)
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

                                GUILayout.BeginHorizontal();
                                roundedMult = Mathf.Round(colorMultiplier * 100f) / 100f;
                                GUILayout.Label("Color multiplier: " + roundedMult.ToString());
                                colorMultiplier = GUILayout.HorizontalSlider(colorMultiplier, 0.0f, 10.0f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();
                                
                                if (GUILayout.Button("Apply Edge Color"))
                                {
                                    EdgeColor(setR, setG, setB);
                                    EdgeColor(setR, setG, setB); // double entry here is intentional, I am too in shock over how cool this is to do it totally right but this is acceptable enough for now
                                }
                            }
                            
                            GUILayout.Space(10f);

                            showMixColors = GUILayout.Toggle(showMixColors, "Show mix colors");
                            if (showMixColors)
                            {
                                GUILayout.Label("Mix Coloring: (1,1,1 for default white)");
                                GUILayout.Space(5f);


                                GUILayout.BeginHorizontal();
                                mixRoundedR = Mathf.Round(mixSetR * 100f) / 100f;
                                GUILayout.Label("R " + mixRoundedR.ToString());

                                mixSetR = GUILayout.HorizontalSlider(mixSetR, 0.001f, 3.000f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();


                                GUILayout.BeginHorizontal();
                                mixRoundedG = Mathf.Round(mixSetG * 100f) / 100f;
                                GUILayout.Label("G " + mixRoundedG.ToString());

                                mixSetG = GUILayout.HorizontalSlider(mixSetG, 0.000f, 3.000f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();


                                GUILayout.BeginHorizontal();
                                mixRoundedB = Mathf.Round(mixSetB * 100f) / 100f;
                                GUILayout.Label("B " + mixRoundedB.ToString());

                                mixSetB = GUILayout.HorizontalSlider(mixSetB, 0.000f, 3.000f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();



                                GUILayout.Space(10f);

                                GUILayout.BeginHorizontal();
                                mixRoundedMult = Mathf.Round(mixColorMultiplier * 100f) / 100f;
                                GUILayout.Label("Color multiplier: " + mixRoundedMult.ToString());

                                mixColorMultiplier = GUILayout.HorizontalSlider(mixColorMultiplier, 0.0f, 10.0f, GUILayout.Width(570));
                                GUILayout.EndHorizontal();


                                if (GUILayout.Button("Apply Edge Mix Color"))
                                {
                                    MixColor(mixSetR, mixSetG, mixSetB);
                                    MixColor(mixSetR, mixSetG, mixSetB); // double entry here is intentional, I am too in shock over how cool this is to do it totally right but this is acceptable enough for now
                                }
                            }
                            


                            



                           
                            if (infoManager.CurrentMode == InfoManager.InfoMode.None)
                            {
                                GUILayout.Label("Gamma: " + toneMapGamma.ToString());
                                toneMapGamma = GUILayout.HorizontalSlider(toneMapGamma, 0.0f, 30.0f, GUILayout.Width(570));
                                GUILayout.Label("Boost: " + toneMapBoost.ToString());
                                toneMapBoost = GUILayout.HorizontalSlider(toneMapBoost, 0.0f, 30.0f, GUILayout.Width(570));
                            }
                            else
                            {
                                GUILayout.Label("Gamma and boost settings have no effect in View Modes.");
                            }
                            GUILayout.Space(15f);

                            if (GUILayout.Button("Advanced mode is on.  Switch back to 'Plug & Play' mode."))
                            {
                                automaticMode = true;
                                switch (activeStockPreset)
                                {
                                    case ActiveStockPreset.Cartoon:
                                        CartoonAutomatic();
                                        break;
                                    case ActiveStockPreset.Bordercities:
                                        BordercitiesAutomatic();
                                        break;
                                    case ActiveStockPreset.LowEndMain:
                                        LowEndAutomatic();
                                        break;
                                    case ActiveStockPreset.LowEndAlt:
                                        LowEndAltAutomatic();
                                        break;
                                    case ActiveStockPreset.Realism:
                                        RealismAutomatic();
                                        break;
                                    case ActiveStockPreset.Random:
                                        RandomAutomatic();
                                        break;
                                    default:
                                        LowEndAutomatic();
                                        break;
                                }
                            }

                            GUILayout.Space(15);
                        }
                    }
                }
            }
            #endregion
            #region Tab - Bloom
            if (tab == Config.Tab.Bloom)
            {
                ResizeWindow(803, 415);
                GUILayout.Label("NOTE: There is already a Bloom effect in Cities Skylines, and it is quite better than what Bordercities provides here.  However, because they both achieve a different effect, the Bordercities Bloom has been maintained in the event you wish to stack the bloom effects.");
                GUILayout.Space(5f);
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
            #endregion
            #region Tab - Hotkey
            if (tab == Config.Tab.Hotkey)
            {
                if (firstTime)
                {
                    ResizeWindow(575, 400);
                    GUILayout.Label("BORDERCITIES FIRST-TIME INITIALIZATION (Never popups again after choice)");
                    GUILayout.Label("Choose and confirm your hotkey for Bordercities.  LeftBracket is default.");
                    GUILayout.Label("NOTE: Bordercities will -never- automatically pop-up again as soon as you've confirmed your hotkey choice.   This initialization process ensures that all users, regardless of hardware, operating system, or current keyboard configuration, will be able to enjoy Bordercities.");


                }


                if (!firstTime)
                {
                    ResizeWindow(538, 600);
                    GUILayout.Label("WARNING: HOTKEY BUTTONS WILL SAVE UPON CLICK.  THIS INCLUDES YOUR EFFECTS SETTINGS.  If you wish to create a safe backup of your active Effect, navigate to the 'Presets' tab above and create a permanent copy before selecting a hotkey here.  This will not be like this for much longer.");

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
            #endregion
            #region Tab - Presets
            if (tab == Config.Tab.Presets)
            {
                ResizeWindow(600, 750);
                GUILayout.Label("Coming soon: Fourth 'Preset Bind' tab where you can allocate these assigned entries to Info Modes and/or hotkeys!");
                GUILayout.Label("NOTE: Your preset bank list is saved automatically, and as a whole, upon either saving any of your fields, or, proper exit of the game.  Note that your changes to this list will NOT be saved in the event of an Alt-F4 or other similarly graceless exit.  You will the preset files stored within steamapps/common/Cities_Skylines/BordercitiesPresets.");

                GUILayout.Space(2f);

                for (int i = 0; i < presetEntries.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    
                    presetEntries[i] = GUILayout.TextField(presetEntries[i], 31, GUILayout.MaxWidth(280));
                    presetEntries[i] = Regex.Replace(presetEntries[i], @"[^a-zA-Z0-9 ]", "");
                    if (GUILayout.Button("Save", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        if (IsValidFilename(presetEntries[i]))
                        {
                            SavePreset(presetEntries[i], true);
                            SaveBank();
                        }
                        
                    }
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(60),GUILayout.MaxHeight(25)))
                    {
                        LoadPreset(presetEntries[i], true);
                    }

                    i++;

                    presetEntries[i] = GUILayout.TextField(presetEntries[i], 31, GUILayout.MaxWidth(280));
                    presetEntries[i] = Regex.Replace(presetEntries[i], @"[^a-zA-Z0-9 ]", "");
                    if (GUILayout.Button("Save", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        if (IsValidFilename(presetEntries[i]))
                        {
                            SavePreset(presetEntries[i], true);
                        }
                    }
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        LoadPreset(presetEntries[i], true);
                    }
                    
                    GUILayout.EndHorizontal();  
                }

                GUILayout.Label("Support for presets is sort of thrown together for the moment. The name of the input field must match the filename, and is case sensitive.  Having a nicer preset browser is on the todo list.");

                if (GUILayout.Button("Reset Bank (This will save!)"))
                {
                    ResetBank();
                }

            }
            #endregion
            #region View Modes
            if (tab == Config.Tab.ViewModes)
            {
                ResizeWindow(600, 900);

                ViewModeGUI(inputInfoBuildingLevel, infoBuildingLevel, "Building Level", "BuildingLevel", existBuildingLevel);
                ViewModeGUI(inputInfoConnections, infoConnections, "Connections", existBuildingLevel);
                ViewModeGUI(inputInfoCrime, infoCrime, "Crime", existCrimeRate);
                ViewModeGUI(inputInfoDensity, infoDensity, "Density", existDensity);
                ViewModeGUI(inputInfoDistricts, infoDistricts, "Districts", existDistricts);
                ViewModeGUI(inputInfoEducation, infoEducation, "Education", existEducation);
                ViewModeGUI(inputInfoElectricity, infoElectricity, "Electricity", existElectricity);
                ViewModeGUI(inputInfoEntertainment, infoEntertainment, "Entertainment", existEntertainment);
                ViewModeGUI(inputInfoFireSafety, infoFireSafety, "Fire Safety", "FireSafety", existFireSafety);
                ViewModeGUI(inputInfoGarbage, infoGarbage, "Garbage", existGarbage);
                ViewModeGUI(inputInfoHappiness, infoHappiness, "Happiness", existHappiness);
                ViewModeGUI(inputInfoHealth, infoHealth, "Health", existHealth);
                ViewModeGUI(inputInfoLandValue, infoLandValue, "Land Value", "LandValue", existLandValue);
                ViewModeGUI(inputInfoNaturalResources, infoNaturalResources, "Natural Resources", "NaturalResources", existNaturalResources);
                ViewModeGUI(inputInfoNoisePollution, infoNoisePollution, "Noise Pollution","NoisePollution", existNoisePollution);
                ViewModeGUI(inputInfoPollution, infoPollution, "Pollution", existPollution);
                ViewModeGUI(inputInfoTerrainHeight, infoTerrainHeight, "Terrain Height", "TerrainHeight", existTerrainHeight);
                ViewModeGUI(inputInfoTraffic, infoTraffic, "Traffic", existTraffic);
                ViewModeGUI(inputInfoTransport, infoTransport, "Transport", existTransport);
                ViewModeGUI(inputInfoWater, infoWater, "Water", existWater);
                ViewModeGUI(inputInfoWind, infoWind, "Wind", existWind);

                GUILayout.Space(2);
                GUILayout.Label("Any Info Mode preset is generated and saved to disk ('steamapps/common/Cities_Skylines/BordercitiesPresets/InfoModes') in its own dedicated XML file, performed upon any click of a Info Mode's respective 'Set' button.  When generated/updated, whichever settings are currently on-screen will be the settings saved to your desired Info Mode's preset -- regardless of whether or not you had first pressed 'Save Settings'.  For ease of understanding, know that your main config is entirely separate from the Info-Mode-specific presets assignable here as well as separate from the general-purpose-presets located in the 'Presets' tab.  The 'Main Config' (the one that auto-loads when you start the game via 'Save Settings') is the configuration that will be automatically loaded upon exiting an Info Mode (if you are also not using 'Effect Enabled in Info Modes Only.')");

                if (infoManager.CurrentMode != InfoManager.InfoMode.None)
                {
                    if (GUILayout.Button("Quick-Set currently displayed settings to this Info Mode ("+GetCurrentInfoModeString()+")"))
                    {
                        QuicksaveActiveViewMode();
                    }
                }
                else
                {
                    if (GUILayout.Button("Enter an Info Mode to quicksave for it, or, set above!"))
                    {

                    }
                }

                GUILayout.Space(4);
                if (GUILayout.Button("Return to configuration tab"))
                {
                    tab = Config.Tab.EdgeDetection;
                }
                GUILayout.Space(30);


            }





            #endregion

            #region Bottom Navigation
            if (!firstTime)
            {
                if (tab == Config.Tab.EdgeDetection && isOn)
                {
                    subViewOnly = GUILayout.Toggle(subViewOnly, "Optional: Effect enabled within 'Info Modes' only. (Auto-disables/enables accordingly)");
                    useInfoModeSpecific = GUILayout.Toggle(useInfoModeSpecific, "Optional: Use 'Info-Mode'-specific Presets (upon activation, 'Info Modes' tab will appear above.)");
                    if (automaticMode && useInfoModeSpecific)
                    {
                        GUILayout.Space(5);

                        GUILayout.Label("Info-Mode presets must first be created in 'Advanced Mode,' or downloaded from the web and placed in the appropriate folder (see tab), in order to function in 'Plug & Play' mode.  Coming soon: Stock-provided Info-Mode styles (with optional overriding of your own/downloaded!)!");
                        GUILayout.Space(5);
                    }
                    if (isOn)
                    {
                        if (GUILayout.Button("Disable Bordered Skylines"))
                        {
                            ToggleBorderedSkylines(false);
                        }
                    }
                }
                GUILayout.BeginHorizontal();
                if (tab != Config.Tab.Hotkey)
                {

                    if (GUILayout.Button("Reset Brightness (Tonemapper)") && tab != Config.Tab.Presets)
                    {
                        ResetTonemapper();
                    }
                    if (!automaticMode)
                    {
                        if (tab != Config.Tab.Presets)
                        {
                            if (GUILayout.Button("Load Settings"))
                            {
                                LoadConfig(true);
                            }
                        }
                        
                    }
                    if (tab != Config.Tab.Presets)
                    {
                        if (automaticMode && tab == Config.Tab.EdgeDetection)
                            GUILayout.Space(40f);
                        if (GUILayout.Button("Save Settings"))
                        {
                            SaveConfig(); 
                        }
                    }
                    else
                    {
                        
                        if (GUILayout.Button("Save Visible to Main Save"))
                        {
                            SaveConfig(); 
                        }
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
            #endregion
            dragBar.width = windowRect.width;
            windowLoc.x = windowRect.x;
            windowLoc.y = windowRect.y;

        }

       
        bool LoadPreset(string name, bool falseIfInfoMode)
        {
            Preset presetToLoad;
            if (falseIfInfoMode)
                presetToLoad = Preset.Deserialize(name);
            else
                presetToLoad = Preset.DeserializeInfoMode(name);
            if (presetToLoad == null)
            {
                return false;
            }
            else
            {
                edge.mode = presetToLoad.edgeMode;
                edge.sensitivityNormals = presetToLoad.sensNorm;
                edge.sensitivityDepth = presetToLoad.sensDepth;
                edge.edgeExp = presetToLoad.edgeExpo;
                edge.depthsAxis = presetToLoad.depthsAxis;
                edge.depthsDiagonal = presetToLoad.depthsDiagonal;
                edge.mult1 = presetToLoad.sobelMult1;
                edge.mult2 = presetToLoad.sobelMult2;
                edge.mult3 = presetToLoad.sobelMult3;
                edge.mult4 = presetToLoad.sobelMult4;
                edge.edgesOnly = presetToLoad.edgeOnly;
                edge.sampleDist = presetToLoad.edgeSamp;
                autoEdge = presetToLoad.autoEdge;
                currentColor = presetToLoad.currentColor;
                edge.edgeColor = currentColor;
                colorMultiplier = presetToLoad.colorMultiplier;
                mixCurrentColor = presetToLoad.mixCurrentColor;
                edge.edgesOnlyBgColor = mixCurrentColor;
                mixColorMultiplier = presetToLoad.mixColorMultiplier;
                setR = presetToLoad.setR;
                setG = presetToLoad.setG;
                setB = presetToLoad.setB;
                mixSetR = presetToLoad.mixSetR;
                mixSetG = presetToLoad.mixSetG;
                mixSetB = presetToLoad.mixSetB;
                tonem.m_ToneMappingBoostFactor = presetToLoad.toneMapBoost;
                tonem.m_ToneMappingGamma = presetToLoad.toneMapGamma;
                bloom.enabled = presetToLoad.bloomEnabled;
                bloom.threshold = presetToLoad.bloomThresh;
                bloom.intensity = presetToLoad.bloomIntens;
                bloom.blurSize = presetToLoad.bloomBlurSize;
                return true;
            }
        }

        bool IsValidFilename(string testName)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(testName))
                return false;
            return true;
        }

        void ResizeDefaults()
        {
            windowRect.width = defaultWidth;
            windowRect.height = defaultHeight;
        }

        void QuicksaveActiveViewMode()
        {
            string saveName = GetCurrentInfoModeString();
            SavePreset(saveName, false);
        }

        string GetCurrentInfoModeString()
        {
            switch (infoManager.CurrentMode)
            {
                case InfoManager.InfoMode.BuildingLevel:
                    return "BuildingLevel";
                case InfoManager.InfoMode.Connections:
                    return "Connections";
                case InfoManager.InfoMode.CrimeRate:
                    return "CrimeRate";
                case InfoManager.InfoMode.Density:
                    return "Density";
                case InfoManager.InfoMode.Districts:
                    return "Districts";
                case InfoManager.InfoMode.Education:
                    return "Education";
                case InfoManager.InfoMode.Electricity:
                    return "Electricity";
                case InfoManager.InfoMode.Entertainment:
                    return "Entertainment";
                case InfoManager.InfoMode.FireSafety:
                    return "FireSafety";
                case InfoManager.InfoMode.Garbage:
                    return "Garbage";
                case InfoManager.InfoMode.Happiness:
                    return "Happiness";
                case InfoManager.InfoMode.Health:
                    return "Health";
                case InfoManager.InfoMode.LandValue:
                    return "LandValue";
                case InfoManager.InfoMode.NaturalResources:
                    return "NaturalResources";
                case InfoManager.InfoMode.NoisePollution:
                    return "NoisePollution";
                case InfoManager.InfoMode.None:
                    return "None";
                case InfoManager.InfoMode.Pollution:
                    return "Pollution";
                case InfoManager.InfoMode.TerrainHeight:
                    return "TerrainHeight";
                case InfoManager.InfoMode.Traffic:
                    return "Traffic";
                case InfoManager.InfoMode.Transport:
                    return "Transport";
                case InfoManager.InfoMode.Water:
                    return "Water";
                case InfoManager.InfoMode.Wind:
                    return "Wind";
                default:
                    return "Default";
            }
        }

        private bool existBuildingLevel = false;
        private bool existConnections = false;
        private bool existCrimeRate = false;
        private bool existDensity = false;
        private bool existDistricts = false;
        private bool existEducation = false;
        private bool existElectricity = false;
        private bool existEntertainment = false;
        private bool existFireSafety = false;
        private bool existGarbage = false;
        private bool existHappiness = false;
        private bool existHealth = false;
        private bool existLandValue = false;
        private bool existNaturalResources = false;
        private bool existNoisePollution = false;
        private bool existPollution = false;
        private bool existTerrainHeight = false;
        private bool existTraffic = false;
        private bool existTransport = false;
        private bool existWater = false;
        private bool existWind = false;
        void InitializeExistBools()
        {
            existBuildingLevel = Preset.CheckIfExists("BuildingLevel");
            existConnections = Preset.CheckIfExists("Connections");
            existCrimeRate = Preset.CheckIfExists("CrimeRate");
            existDensity = Preset.CheckIfExists("Density");
            existDistricts = Preset.CheckIfExists("Districts");
            existEducation = Preset.CheckIfExists("Education");
            existElectricity = Preset.CheckIfExists("Electricity");
            existEntertainment = Preset.CheckIfExists("Entertainment");
            existFireSafety = Preset.CheckIfExists("FireSafety");
            existGarbage = Preset.CheckIfExists("Garbage");
            existHappiness = Preset.CheckIfExists("Happiness");
            existHealth = Preset.CheckIfExists("Health");
            existLandValue = Preset.CheckIfExists("LandValue");
            existNaturalResources = Preset.CheckIfExists("NaturalResources");
            existNoisePollution = Preset.CheckIfExists("NoisePollution");
            existPollution = Preset.CheckIfExists("Pollution");
            existTerrainHeight = Preset.CheckIfExists("TerrainHeight");
            existTraffic = Preset.CheckIfExists("Traffic");
            existTransport = Preset.CheckIfExists("Transport");
            existWater = Preset.CheckIfExists("Water");
            existWind = Preset.CheckIfExists("Wind");
        }

        void ResetTonemapper()
        {
            toneMapBoost = defaultBoost;
            toneMapGamma = defaultGamma;
        }
        void SetTonemapper(float gam, float boost)
        {
            tonem.m_ToneMappingBoostFactor = boost;
            tonem.m_ToneMappingGamma = gam;
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

        bool CheckTonemapper()
        {
            if (tonem.m_ToneMappingGamma != defaultGamma || tonem.m_ToneMappingBoostFactor != defaultBoost)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        void SetActiveLookToTemporary()
        {
            userIsPreviewing = true;
            tempAutoEdge = autoEdge;
            tempEdgeBgrdColor = mixCurrentColor;
            tempEdgeColor = currentColor;
            tempEdgeExpo = edge.edgeExp;
            tempEdgeMode = edge.mode;
            tempEdgeOnly = edge.edgesOnly;
            tempEdgeSamp = edge.sampleDist;
            tempSensDepth = edge.sensitivityDepth;
            tempSensNorm = edge.sensitivityNormals;
            tempToneMapBoost = toneMapBoost;
            tempToneMapGamma = toneMapGamma;
        }

        void RestoreFromTemporary()
        {
            if (userIsPreviewing)
            {
                autoEdge = tempAutoEdge;
                mixCurrentColor = tempEdgeBgrdColor;
                currentColor = tempEdgeColor;
                edge.edgeExp = tempEdgeExpo;
                edge.mode = tempEdgeMode;
                edge.edgesOnly = tempEdgeOnly;
                edge.sampleDist = tempEdgeSamp;
                edge.sensitivityDepth = tempSensDepth;
                edge.sensitivityNormals = tempSensNorm;
                toneMapBoost = tempToneMapBoost;
                toneMapGamma = tempToneMapGamma;
            }
            userIsPreviewing = false;
        }


        void ViewModeGUI(string inputField, string presetName, string label, bool exists)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
            {
                SavePreset(label, false);
                InitializeExistBools();
            }
            if (exists)
            {
                if (GUILayout.Button("Activate " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    LoadPreset(label, false);
                }
            }
            else
            {
                if (GUILayout.Button("No " + label + " found!", GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    
                }
            }
            GUILayout.EndHorizontal();
        }
        void ViewModeGUI(string inputField, string presetName, string label, string doubleWord, bool exists)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
            {
                SavePreset(doubleWord, false);
                InitializeExistBools();
            }
            if (exists)
            {
                if (GUILayout.Button("Activate " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    LoadPreset(doubleWord, false);
                }
            }
            else
            {
                if (GUILayout.Button("No " + label + " found!", GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {

                }
            }
            GUILayout.EndHorizontal();
        }

        bool ShouldWeExitPreview(Preset infoModePreset)
        {
            if (config.toneMapBoost != infoModePreset.toneMapBoost)
                return false;
            if (config.toneMapGamma != infoModePreset.toneMapGamma)
                return false;
            if (config.autoEdge != infoModePreset.autoEdge)
                return false;
            if (config.edgeExpo != infoModePreset.edgeExpo)
                return false;
            if (config.edgeMode != infoModePreset.edgeMode)
                return false;
            if (config.edgeOnly != infoModePreset.edgeOnly)
                return false;
            if (config.edgeSamp != infoModePreset.edgeSamp)
                return false;
            if (config.sensDepth != infoModePreset.sensDepth)
                return false;
            if (config.sensNorm != infoModePreset.sensNorm)
                return false;
            return true;
        }

        public float tempToneMapBoost;
        public float tempToneMapGamma;
        public bool tempAutoEdge;
        public float tempEdgeExpo;
        public EdgeDetection.EdgeDetectMode tempEdgeMode;
        public float tempEdgeOnly;
        public float tempEdgeSamp;
        public float tempSensDepth;
        public float tempSensNorm;
        public Color tempEdgeColor;
        public Color tempEdgeBgrdColor;
        


        void InfoModes()
        {

            if (infoManager.CurrentMode != currentInfoMode)
            {
                if (infoManager.CurrentMode == InfoManager.InfoMode.None)
                {
                    currentInfoMode = infoManager.CurrentMode;
                    LoadConfig(true);
                    return;
                }
                ViewModeCheckAndSet(InfoManager.InfoMode.BuildingLevel, "BuildingLevel");
                ViewModeCheckAndSet(InfoManager.InfoMode.Connections, "Connections");
                ViewModeCheckAndSet(InfoManager.InfoMode.CrimeRate, "Crime");
                ViewModeCheckAndSet(InfoManager.InfoMode.Density, "Density");
                ViewModeCheckAndSet(InfoManager.InfoMode.Districts, "Districts");
                ViewModeCheckAndSet(InfoManager.InfoMode.Education, "Education");
                ViewModeCheckAndSet(InfoManager.InfoMode.Electricity, "Electricity");
                ViewModeCheckAndSet(InfoManager.InfoMode.Entertainment, "Entertainment");
                ViewModeCheckAndSet(InfoManager.InfoMode.FireSafety, "Fire Safety");
                ViewModeCheckAndSet(InfoManager.InfoMode.Garbage, "Garbage");
                ViewModeCheckAndSet(InfoManager.InfoMode.Happiness, "Happiness");
                ViewModeCheckAndSet(InfoManager.InfoMode.Health, "Health");
                ViewModeCheckAndSet(InfoManager.InfoMode.LandValue, "LandValue");
                ViewModeCheckAndSet(InfoManager.InfoMode.NaturalResources, "NaturalResources");
                ViewModeCheckAndSet(InfoManager.InfoMode.NoisePollution, "Noise Pollution");
                ViewModeCheckAndSet(InfoManager.InfoMode.Pollution, "Pollution");
                ViewModeCheckAndSet(InfoManager.InfoMode.TerrainHeight, "TerrainHeight");
                ViewModeCheckAndSet(InfoManager.InfoMode.Traffic, "Traffic");
                ViewModeCheckAndSet(InfoManager.InfoMode.Transport, "Transport");
                ViewModeCheckAndSet(InfoManager.InfoMode.Water, "Water");
                ViewModeCheckAndSet(InfoManager.InfoMode.Wind, "Wind");
                GUILayout.Space(10);
                if (GUILayout.Button("Load Main Panel Configuration"))
                {
                    LoadConfig(true);
                }
                GUILayout.Space(10);
            }
        }

        void ViewModeCheckAndSet(InfoManager.InfoMode infoMode, string preset)
        {
            if (infoManager.CurrentMode == infoMode)
            {
                currentInfoMode = infoManager.CurrentMode;
                LoadPreset(preset, false);
            }
        }

        public void Update()
        {
            if (edge.depthsAxis != 1.000f)
            {
                edge.depthsAxis = 1;
            }
            EffectState();
            if (useInfoModeSpecific)
                InfoModes();
            if (isOn)
            {
                tonem.m_ToneMappingBoostFactor = toneMapBoost;
                tonem.m_ToneMappingGamma = toneMapGamma;
            }
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
                LowEndAutomatic();
                SaveConfig();

            }

            if (firstTime)
            {
                if (!overrideFirstTime)
                    showSettingsPanel = true;
            }


            if (Input.GetKeyUp(config.edgeToggleKeyCode))
            {
                if (isOn)
                {
                    ToggleBorderedSkylines(false);
                }
                else
                {
                    ToggleBorderedSkylines(true);
                }
                
                
            }


        }

        void ToggleBorderedSkylines(bool state)
        {
            if (state)
            {
                isOn = true;
                edge.enabled = true;
                prevGamma = tonem.m_ToneMappingGamma;
                prevBoost = tonem.m_ToneMappingBoostFactor;
                tonem.m_ToneMappingGamma = toneMapGamma;
                tonem.m_ToneMappingBoostFactor = toneMapBoost;
                FixTonemapperIfZeroed();
            }
            else
            {
                isOn = false;
                edge.enabled = false;
                tonem.m_ToneMappingGamma = prevGamma;
                tonem.m_ToneMappingBoostFactor = prevBoost;
                FixTonemapperIfZeroed();
            }
        }
        
        void EffectState()
        {
            if (isOn)
            {
                if (subViewOnly)
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
                else
                    if (!edge.enabled)
                        edge.enabled = true;

            }
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