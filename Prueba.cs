using IEIPracticas.Extractores;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using SQLiteOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IEIPracticas
{
    class Prueba
    {
        public static void Main()
        {
            Console.WriteLine(Extractor_csv.ConvertCsvToJson(".\\FFDD\\bienes_inmuebles_interes_cultural.csv"));
            Console.WriteLine(Extractor_json.LoadJsonAsString(".\\FFDD\\edificios.json"));
            Console.WriteLine(Extractor_xml.ConvertXmlToJson(".\\FFDD\\monumentos.xml"));
        
            string databasePath = "./SQLite/dbproject.db";
            SQLiteHandler dbHandler = new SQLiteHandler(databasePath);
            dbHandler.OpenConnection();

            // Insertar datos
            string insertQueryProvincia = "INSERT INTO Provincia (idProvincia, nombre) VALUES (1, 'Valencia')";
            dbHandler.InsertData(insertQueryProvincia);

            string insertQueryLocalidad = "INSERT INTO Localidad (idLocalidad, nombre, idProvincia) VALUES (1, 'Valencia', 1)";
            dbHandler.InsertData(insertQueryLocalidad);

            string insertQueryMonumento = "INSERT INTO Monumento (idMonumento, nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo,idLocalidad) VALUES (1, 'Cloacas Romanas', 'testdir',24700, 42.452992,-6.052759, 'testDescripcion','Puente',1 )";
            dbHandler.InsertData(insertQueryMonumento);

            // Consultar datos
            string selectQuery = "SELECT * FROM Localidad";
            dbHandler.QueryData(selectQuery);

            // Eliminar datos
            //string deleteQuery = "DELETE FROM Provincia WHERE idProvincia = 1";
            //dbHandler.DeleteData(deleteQuery);

            //string deleteQuery2 = "DELETE FROM Localidad WHERE idLocalidad = 1";
            //dbHandler.DeleteData(deleteQuery2);

            //string deleteQuery3 = "DELETE FROM Monumento WHERE idMonumento = 1";
            //dbHandler.DeleteData(deleteQuery3);
            FilterAndInsertCSV();
            FilterAndInsertXML();
            FilterAndInsertJSON();

            dbHandler.CloseConnection();
        }
        private static void FilterAndInsertCSV()
        {
            string csvdoc = Extractor_csv.ConvertCsvToJson(".\\FFDD\\edificios.json");
            List<CSVMonumento> csvMonumentos = JsonSerializer.Deserialize<List<CSVMonumento>>(csvdoc);
            List<Monumento> monumentos = new List<Monumento>();
            foreach (CSVMonumento csvMonumento in csvMonumentos)
            {
                if(CSVmonumentoToMonumento(csvMonumento) == null) { monumentos.Add(CSVmonumentoToMonumento(csvMonumento)); } 
            }
        }
        private void FilterAndInsertXML()
        {
            string xmldoc = Extractor_xml.ConvertXmlToJson(".\\FFDD\\edificios.json");
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
        }
    }
}
