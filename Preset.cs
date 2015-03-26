using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace Bordercities
{
    
    public class Preset
    {
        private const string presetPath = "BordercitiesPresets";

        public string presetName;
        
        public EdgeDetection.EdgeDetectMode edgeMode;
        public float sensNorm;
        public float sensDepth;
        public float edgeExpo;
        public float edgeSamp;
        public float edgeOnly;
        public float toneMapBoost;
        public float toneMapGamma;
        public bool subViewOnly;
        public bool autoEdge;

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

        public static void MakeFolderIfNonexistent()
        {
            DirectoryInfo di = Directory.CreateDirectory("BordercitiesPresets");
        }
    }
}