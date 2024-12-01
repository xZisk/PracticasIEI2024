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

            // string insertQueryMonumento = "INSERT INTO Monumento (nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo,idLocalidad) VALUES ('Cloacas Romanas', 'testdir',24700, 42.452992,-6.052759, 'testDescripcion','Puente',1 )";
            // dbHandler.InsertData(insertQueryMonumento);

            // Consultar datos
            

            //FilterAndInsertCSV(dbHandler);
            dbHandler.FilterAndInsertXML(dbHandler);
            string selectQuery = "SELECT * FROM Monumento";
            dbHandler.QueryData(selectQuery);
            selectQuery = "SELECT * FROM Localidad";
            dbHandler.QueryData(selectQuery);
            selectQuery = "SELECT * FROM Provincia";
            dbHandler.QueryData(selectQuery);

            dbHandler.CloseConnection();
        }
    }
}
