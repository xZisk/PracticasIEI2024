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
        public static async Task Main()
        {
            Console.WriteLine(Extractor_csv.ConvertCsvToJson(".\\FFDD\\bienes_inmuebles_interes_cultural.csv"));
            Console.WriteLine(Extractor_json.LoadJsonAsString(".\\FFDD\\edificios.json"));
            Console.WriteLine(Extractor_xml.ConvertXmlToJson(".\\FFDD\\monumentos.xml"));

            string databasePath = "./SQLite/dbproject.db";
            SQLiteHandler dbHandler = new SQLiteHandler(databasePath);
            dbHandler.OpenConnection();

            // Estas 3 lineas limpian las tablas de la BD para ir haciendo pruebas
            dbHandler.DeleteData("DELETE FROM Monumento");
            dbHandler.DeleteData("DELETE FROM Localidad");
            dbHandler.DeleteData("DELETE FROM Provincia");

            //await dbHandler.FilterAndInsertCSV(); // Funciona sin scrapper ni API
            //await dbHandler.FilterAndInsertXML(); // Funciona todo menos la API
            await dbHandler.FilterAndInsertJSON();

            // Bloque de codigo de seleccion de todos los datos de la BD, para depuracion
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
