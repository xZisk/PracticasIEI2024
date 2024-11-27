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
        public static string convertCsvToJson(string csvFilePath)
        {
            // Leer todas las líneas del archivo CSV
            var csvLines = File.ReadAllLines(csvFilePath);

            // Verificar que el archivo no esté vacío
            if (csvLines.Length == 0)
                throw new Exception("El archivo CSV está vacío.");

            // Separar el encabezado usando el delimitador correcto
            var headers = csvLines[0].Split(';');

            // Lista para almacenar los objetos generados a partir del CSV
            var dataList = new List<Dictionary<string, string>>();

            // Procesar cada línea después del encabezado
            for (int i = 1; i < csvLines.Length; i++)
            {
                var values = csvLines[i].Split(';'); // Separar por punto y coma
                var dataObject = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    // Asignar el valor correspondiente al encabezado
                    dataObject[headers[j]] = values.Length > j ? values[j] : string.Empty;
                }

                dataList.Add(dataObject);
            }

            // Convertir la lista de objetos a JSON
            return JsonConvert.SerializeObject(dataList, Formatting.Indented);
        }
    }
}
