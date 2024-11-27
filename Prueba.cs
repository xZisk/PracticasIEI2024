using IEIPracticas.Extractores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas
{
    class Prueba
    {
        public static void Main()
        {
            Console.WriteLine(Extractor_csv.ConvertCsvToJson("..\\..\\..\\FFDD\\bienes_inmuebles_interes_cultural.csv"));
            Console.WriteLine(Extractor_json.LoadJsonAsString("..\\..\\..\\FFDD\\edificios.json"));
            Console.WriteLine(Extractor_xml.ConvertXmlToJson("..\\..\\..\\FFDD\\monumentos.xml"));
        }
    }
}
