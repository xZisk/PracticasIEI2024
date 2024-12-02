using IEIPracticas.APIs_Scrapper;
using IEIPracticas.Extractores;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using Microsoft.Data.Sqlite;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
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
using System.Xml.Linq;

namespace IEIPracticas
{
    class Prueba
    {
        public static void Main()
        {
            Task.Run(async () => await RunAsync()).GetAwaiter().GetResult();
        }

        public static async Task RunAsync()
        {
            string databasePath = "./SQLite/dbproject.db";
            SQLiteHandler dbHandler = new SQLiteHandler(databasePath);
            dbHandler.OpenConnection();

            // Estas 4 lineas limpian las tablas de la BD para ir haciendo pruebas
            dbHandler.DeleteData("DELETE FROM Monumento");
            dbHandler.DeleteData("DELETE FROM Localidad");
            dbHandler.DeleteData("DELETE FROM Provincia");
            dbHandler.DeleteData("DELETE FROM sqlite_sequence");

            await dbHandler.FilterAndInsertCSV(); 
            await dbHandler.FilterAndInsertXML(); 
            await dbHandler.FilterAndInsertJSON();

            // Bloque de código de selección de todos los datos de la BD, para depuración
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
