using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("¡Hola, Mundo!");

             string csvFilePath = @".\FFDD\bienes_inmuebles_interes_cultural.csv";
             string xmlFilePath = @".\FFDD\monumentos.xml";
             string jsonFilePath = @".\FFDD\edificios.json";
              try
                {
                    //string jsonOutput = IEIPracticas.Extractores.Extractor_xml.ConvertXmlToJson(xmlFilePath);
                    //Console.WriteLine("El archivo XML ha sido convertido a JSON:");
                    //Console.WriteLine(jsonOutput);

                    string jsonOutput = IEIPracticas.Extractores.Extractor_csv.ConvertCsvToJson(csvFilePath);
                    Console.WriteLine("El archivo CSV ha sido convertido a JSON:");
                    Console.WriteLine(jsonOutput);

                    //string jsonOutput = IEIPracticas.Extractores.ExtractorJson.LoadJsonAsString(jsonFilePath);
                    //Console.WriteLine("El archivo JSON ha sido convertido a JSON:");
                    //Console.WriteLine(jsonOutput);

                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                }

        }
    }
}
