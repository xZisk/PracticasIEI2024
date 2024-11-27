using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IEIPracticas.Extractores
{
    class Extractor_xml
    {
        public static string ConvertXmlToJson(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException($"El archivo XML no existe en la ruta: {xmlFilePath}");
            }

            string xmlContent = File.ReadAllText(xmlFilePath);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);
            return JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented); ;
        }
    }
}
