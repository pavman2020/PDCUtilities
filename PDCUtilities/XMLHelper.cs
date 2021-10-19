using System;
using System.Xml.Serialization;

namespace PDCUtility
{
    public static class XMLHelper
    {
        public static object StripCommentsAndDeserialize(this System.Xml.Serialization.XmlSerializer oXmlSerializer, string strFullyPathedConfigFileName, Action<Exception> handleException = null)
        {
            ////// strip the COMMENTS out of the XML
            ////try
            ////{
            ////    // load document
            ////    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            ////    doc.Load(strFullyPathedConfigFileName);
            ////
            ////    // remove all comments
            ////    System.Xml.XmlNodeList l = doc.SelectNodes("//comment()");
            ////    foreach (System.Xml.XmlNode node in l) node.ParentNode.RemoveChild(node);
            ////
            ////    // store to memory stream and rewind
            ////    System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ////    doc.Save(ms);
            ////    ms.Seek(0, System.IO.SeekOrigin.Begin);
            ////
            ////    // deserialize using clean xml
            ////    return oXmlSerializer.Deserialize(System.Xml.XmlReader.Create(ms));
            ////}
            ////catch (Exception ex)
            ////{
            ////    if (null == handleException)
            ////        throw;
            ////
            ////    handleException?.Invoke(ex);
            ////}

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
    }
}