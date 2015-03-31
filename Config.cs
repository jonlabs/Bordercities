using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace Bordercities
{
    
    public class Config
    {
        public EdgeDetection.EdgeDetectMode edgeMode;
        public bool edgeEnabled;
        public float edgeSamp;
        public float edgeOnly;

        //Triangle/Roberts
        public float sensNorm;
        public float sensDepth;
        
        //Sobel
        public float edgeExpo;
        public float depthsDiagonal;
        public float depthsAxis;
        public float sobelMult1;
        public float sobelMult2;
        public float sobelMult3;
        public float sobelMult4;

        public float oldBoost;
        public float toneMapBoost;
        public float oldGamma;
        public float toneMapGamma;

        public bool firstTime = true;
        public KeyCode keyCode = KeyCode.LeftBracket;
        public KeyCode edgeToggleKeyCode;
       
        
        public bool subViewOnly;
        public bool useInfoModeSpecific;


        public bool autoEdge;

        public bool bloomEnabled;
        public float bloomThresh;
        public float bloomIntens;
        public float bloomBlurSize;

        public bool automaticMode;

        public Color currentColor;
        public float colorMultiplier;
        public float setR;
        public float setG;
        public float setB;

        public Color mixCurrentColor;
        public float mixColorMultiplier;
        public float mixSetR;
        public float mixSetG;
        public float mixSetB;

        public Color cartoonMixColor;
        public Vector2 windowLoc;

        

        public EffectController.ActiveStockPreset activeStockPreset;


        public enum Tab
        {
            EdgeDetection = 0,
            Bloom = 1,
            Hotkey = 2,
            Presets = 3,
            ViewModes = 4,
        }


        public static void Serialize(string filename, Config config)
        {
            var serializer = new XmlSerializer(typeof(Config));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, config);
            }
        }

        public static Config Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Config));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    var config = (Config)serializer.Deserialize(reader);
                    return config;
                }
            }
            catch { }
            return null;
        }

    }
}