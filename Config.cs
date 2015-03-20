using System.IO;
using System.Xml.Serialization;

namespace Bordercities
{

    public class Config
    {
        public EdgeDetection.EdgeDetectMode edgeMode;
        public bool edgeEnabled;
        public float sensNorm;
        public float sensDepth;
        public float edgeExpo;
        public float edgeSamp;
        public float edgeOnly;

        public bool bloomEnabled;
        public float bloomThresh;
        public float bloomIntens;
        public float bloomBlurSize;

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