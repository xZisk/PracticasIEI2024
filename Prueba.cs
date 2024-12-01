using IEIPracticas.Extractores;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using Microsoft.Data.Sqlite;
using SQLiteOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IEIPracticas
{
    class Prueba
    {
        int errores { get; set; }
        public static void Main()
        {
            Console.WriteLine(Extractor_csv.ConvertCsvToJson(".\\FFDD\\bienes_inmuebles_interes_cultural.csv"));
            Console.WriteLine(Extractor_json.LoadJsonAsString(".\\FFDD\\edificios.json"));
            Console.WriteLine(Extractor_xml.ConvertXmlToJson(".\\FFDD\\monumentos.xml"));

            string databasePath = "./SQLite/dbproject.db";
            SQLiteHandler dbHandler = new SQLiteHandler(databasePath);
            dbHandler.OpenConnection();

            dbHandler.DeleteData("DELETE FROM Monumento");
            dbHandler.DeleteData("DELETE FROM Localidad");
            dbHandler.DeleteData("DELETE FROM Provincia");

            // Insertar datos
            string insertQueryProvincia = "INSERT INTO Provincia (idProvincia, nombre) VALUES (1, 'Valencia')";
            dbHandler.InsertData(insertQueryProvincia);

            string insertQueryLocalidad = "INSERT INTO Localidad (idLocalidad, nombre, idProvincia) VALUES (1, 'Valencia', 1)";
            dbHandler.InsertData(insertQueryLocalidad);

            // string insertQueryMonumento = "INSERT INTO Monumento (nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo,idLocalidad) VALUES ('Cloacas Romanas', 'testdir',24700, 42.452992,-6.052759, 'testDescripcion','Puente',1 )";
            // dbHandler.InsertData(insertQueryMonumento);

            // Consultar datos
            string selectQuery = "SELECT * FROM Localidad";
            dbHandler.QueryData(selectQuery);

            //FilterAndInsertCSV(dbHandler);
            FilterAndInsertXML(dbHandler);
            selectQuery = "SELECT * FROM Monumento";
            dbHandler.QueryData(selectQuery);

            dbHandler.CloseConnection();
        }
        private static void InsertMonumento(Monumento monumento, SQLiteHandler dbHandler)
        {
            if (DoesMonumentExist(monumento.Nombre, dbHandler))
            {
                Console.WriteLine($"El monumento '{monumento.Nombre}' ya existe en la BD");
                return;
            };

            string query = $"INSERT INTO Monumento " +
               $"(nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo, idLocalidad) " +
               $"VALUES ('{monumento.Nombre}', " +
               $"'{monumento.Direccion}', " +
               $"{monumento.CodigoPostal.ToString("D5")}, " +
               $"{monumento.Longitud.ToString(CultureInfo.InvariantCulture)}, " +
               $"{monumento.Latitud.ToString(CultureInfo.InvariantCulture)}, " +
               $"'{monumento.Descripcion}', " +
               $"'{GetEnumDescription(monumento.Tipo)}', " +
               $"{monumento.IdLocalidad})";

            dbHandler.InsertData(query);
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute.Description;
        }
        private static void FilterAndInsertCSV(SQLiteHandler dbHandler)
        {
            string csvdoc = Extractor_csv.ConvertCsvToJson(".\\FFDD\\bienes_inmuebles_interes_cultural.csv");
            List<CSVMonumento> csvMonumentos = JsonSerializer.Deserialize<List<CSVMonumento>>(csvdoc);
            foreach (CSVMonumento csvMonumento in csvMonumentos)
            {
                var monumento = CSVMapper.CSVMonumentoToMonumento(csvMonumento);
                if (monumento == null)
                {
                    Console.WriteLine($"Monumento inválido detectado: {csvMonumento.DENOMINACION}");
                    continue;
                }
                InsertMonumento(monumento, dbHandler);
            }
        }
        private static void FilterAndInsertXML(SQLiteHandler dbHandler)
        {
            string xmldoc = Extractor_xml.ConvertXmlToJson(".\\FFDD\\monumentos.xml");
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(xmldoc);

            var monumentosArray = jsonObject.GetProperty("monumentos").GetProperty("monumento");

            List<XMLMonumento> xmlMonumentos = JsonSerializer.Deserialize<List<XMLMonumento>>(monumentosArray.ToString());

            foreach (XMLMonumento xmlMonumento in xmlMonumentos)
            {
                var monumento = XMLMapper.XMLMonumentoToMonumento(xmlMonumento);

                if (monumento == null)
                {
                    Console.WriteLine($"Monumento inválido detectado: {xmlMonumento.nombre}");
                    continue;
                }
                InsertMonumento(monumento, dbHandler);
            }

            /*private void FilterAndInsertJSON()
            {
                string jsondoc = Extractor_json.LoadJsonAsString(".\\FFDD\\edificios.json");
                List<JSONMonumento> jsonMonumentos = JsonSerializer.Deserialize<List<JSONMonumento>>(jsondoc);
                List<Monumento> monumentos = new List<Monumento>();
                foreach (JSONMonumento jsonMonumento in jsonMonumentos)
                {
                    if (JSONmonumentoToMonumento(jsonMonumento) == null) { monumentos.Add(JSONmonumentoToMonumento(jsonMonumento)); }
                }
            }*/
        }
        private static bool DoesMonumentExist(string nombre, SQLiteHandler dbHandler)
        {
            string query = "SELECT COUNT(*) FROM Monumento WHERE nombre = @Nombre";

            using (var command = new SqliteCommand(query, dbHandler.getConnection())) // Usa la conexión existente
            {
                command.Parameters.AddWithValue("@Nombre", nombre);
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }
    }
}
