using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Extractores
{
    class Extractor_csv
    {
        static string ConvertCsvToJson(string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
            {
                throw new FileNotFoundException($"El archivo JSON no existe en la ruta: {csvFilePath}");
            }
            var csvLines = File.ReadAllLines(csvFilePath);

            var headers = csvLines[0].Split(',');
            var records = new List<Dictionary<string, string>>();

            // Procesa cada fila, excepto la primera (encabezados)
            for (int i = 1; i < csvLines.Length; i++)
            {
                var values = csvLines[i].Split(',');
                var record = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    // Asocia cada valor con su encabezado correspondiente
                    record[headers[j]] = j < values.Length ? values[j] : "";
                }

                records.Add(record);
            }

            // Convierte la lista de objetos a formato JSON
            return JsonConvert.SerializeObject(records, Formatting.Indented);
        }
    }
}
