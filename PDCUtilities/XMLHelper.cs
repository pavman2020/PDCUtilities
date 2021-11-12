using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PDCUtility
{
    public static class XMLHelper
    {
        public static object StripCommentsAndDeserialize(this System.Xml.Serialization.XmlSerializer oXmlSerializer, string strFullyPathedConfigFileName, Action<Exception> handleException = null)
        {
            // strip the COMMENTS out of the XML
            try
            {
                var xmlReader = System.Xml.XmlReader.Create(strFullyPathedConfigFileName, new System.Xml.XmlReaderSettings { IgnoreComments = true });

                // deserialize using clean xml
                return oXmlSerializer.Deserialize(xmlReader);
            }
            catch (Exception ex)
            {
                if (null == handleException)
                    throw;

                handleException?.Invoke(ex);
            }

            return null;
        }

        public static string Serialize<T>(T dataToSerialize)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }

        public static T Deserialize<T>(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader);
        }

        public static T DeserializeFile<T>(string strFilename)
        {
            return Deserialize<T>(System.IO.File.ReadAllText(strFilename));
        }

        public static void Serialize(TextWriter writer, IDictionary dictionary)
        {
            List<Entry> entries = new List<Entry>(dictionary.Count);
            foreach (object key in dictionary.Keys)
            {
                entries.Add(new Entry(key, dictionary[key]));
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, entries);
        }

        public static void Deserialize(TextReader reader, IDictionary dictionary)
        {
            dictionary.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
            foreach (Entry entry in list)
            {
                dictionary[entry.Key] = entry.Value;
            }
        }

        public class Entry
        {
            public object Key;
            public object Value;

            public Entry()
            {
            }

            public Entry(object key, object value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}