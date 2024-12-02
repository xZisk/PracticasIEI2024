using CsvHelper;
using CsvHelper.Configuration;
using IEIPracticas.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Linq;

class Extractor_csv
{
    public static string ConvertCsvToJson(string csvFilePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
        };

        // Leer el archivo CSV
        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<CSVMonumento>().ToList();

            // Convertir a JSON
            return JsonConvert.SerializeObject(records, Formatting.Indented);
        }
    }
}
