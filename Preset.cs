using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace Bordercities
{
    
    public class Preset
    {
        private const string presetPath = "BordercitiesPresets/";
        private const string infoModePath = "BordercitiesPresets/InfoModes/";

        public string presetName;
        
        public EdgeDetection.EdgeDetectMode edgeMode;
        public float sensNorm;
        public float sensDepth;
        public float edgeSamp;
        public float edgeOnly;
        public float toneMapBoost;
        public float toneMapGamma;
        public bool subViewOnly;
        public bool autoEdge;

        public float edgeExpo;
        public float depthsDiagonal;
        public float depthsAxis;
        public float sobelMult1;
        public float sobelMult2;
        public float sobelMult3;
        public float sobelMult4;

        public bool bloomEnabled;
        public float bloomThresh;
        public float bloomIntens;
        public float bloomBlurSize;

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


        public static void Serialize(string filename, Preset preset)
        {
            var serializer = new XmlSerializer(typeof(Preset));

            using (var writer = new StreamWriter(presetPath + "/" + filename + ".xml"))
            {
                serializer.Serialize(writer, preset);
            }
        }

        public static void SerializeInfoMode(string filename, Preset preset)
        {
            var serializer = new XmlSerializer(typeof(Preset));

            using (var writer = new StreamWriter(infoModePath + "/" + filename + ".xml"))
            {
                serializer.Serialize(writer, preset);
            }
        }

        public static Preset Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Preset));

            try
            {
                using (var reader = new StreamReader(presetPath + "/" + filename + ".xml"))
                {
                    var preset = (Preset)serializer.Deserialize(reader);
                    return preset;
                }
            }
            catch { }
            return null;
        }

        public static Preset DeserializeInfoMode(string filename)
        {
            var serializer = new XmlSerializer(typeof(Preset));

            try
            {
                using (var reader = new StreamReader(infoModePath + "/" + filename + ".xml"))
                {
                    var preset = (Preset)serializer.Deserialize(reader);
                    return preset;
                }
            }
            catch { }
            return null;
        }

        public static void MakeFolderIfNonexistent()
        {
            DirectoryInfo di = Directory.CreateDirectory("BordercitiesPresets");
            DirectoryInfo dir = Directory.CreateDirectory(@"BordercitiesPresets/InfoModes/");
            
            Debug.Log(dir.FullName);
        }

        public static bool CheckIfExists(string infoMode)
        {
            string thePath = infoModePath + infoMode + @".xml";
            if (File.Exists(thePath))
                return true;
            else
                return false;
        }
    }
}