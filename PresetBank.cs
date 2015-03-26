using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace Bordercities
{
    
    public class PresetBank
    {

        public string[] presetEntries;

        public static void Serialize(string filename, PresetBank presetBank)
        {
            var serializer = new XmlSerializer(typeof(PresetBank));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, presetBank);
            }
        }

        public static PresetBank Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(PresetBank));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    var presetBank = (PresetBank)serializer.Deserialize(reader);
                    return presetBank;
                }
            }
            catch { }
            return null;
        }

    }
}