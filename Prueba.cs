using IEIPracticas.Extractores;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using SQLiteOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            FilterAndInsertCSV(dbHandler);
            selectQuery = "SELECT * FROM Monumento";
            dbHandler.QueryData(selectQuery);

            dbHandler.CloseConnection();
        }
        private static void InsertMonumento(Monumento monumento, SQLiteHandler dbHandler)
        {
            string query = $"INSERT INTO Monumento " +
               $"(nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo, idLocalidad) " +
               $"VALUES ('{monumento.Nombre}', " +
               $"'{monumento.Direccion}', " +
               $"{monumento.CodigoPostal}, " +
               $"{monumento.Longitud}, " +
               $"{monumento.Latitud}, " +
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
        /*private void FilterAndInsertXML()
        {
            string xmldoc = Extractor_xml.ConvertXmlToJson(".\\FFDD\\monumentos.xml");
            List<XMLMonumento> xmlMonumentos = JsonSerializer.Deserialize<List<XMLMonumento>>(xmldoc);
            List<Monumento> monumentos = new List<Monumento>();
            foreach (XMLMonumento xmlMonumento in xmlMonumentos)
            {
                if (XMLmonumentoToMonumento(xmlMonumento) == null) { monumentos.Add(XMLmonumentoToMonumento(xmlMonumento)); } 
            }
        }
        private void FilterAndInsertJSON()
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
}
