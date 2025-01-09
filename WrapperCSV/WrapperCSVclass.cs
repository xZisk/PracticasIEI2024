using CsvHelper;
using CsvHelper.Configuration;
using WrapperCSV.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Linq;

class WrapperCSVclass
{
    public static string ConvertCsvToJson()
    {
        string csvFilePath = "./bienes_inmuebles_interes_cultural.csv";
        if (!File.Exists(csvFilePath))
        {
            throw new FileNotFoundException($"El archivo CSV no existe en la ruta: {csvFilePath}");
        }
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
