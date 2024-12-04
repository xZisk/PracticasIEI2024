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
using SimMetrics.Net.Metric;

namespace IEIPracticas
{
    class Prueba
    {
        public static void Main()
        {
            string databasePath = "./SQLite/dbproject.db";
            SQLiteHandler dbHandler = new SQLiteHandler(databasePath);
            dbHandler.OpenConnection();
            Start(dbHandler);
        }
        private static void Start(SQLiteHandler dbHandler)
        {
            Console.WriteLine("Elige la fuente de datos a procesar: 0->Exit 1->CSV(CV) 2->XML(CLE) 3->JSON(EUS) 4->Delete BD data");
            var caso = Console.ReadLine();
            if (caso.Equals("0"))
            {
                dbHandler.CloseConnection();
                return;
            }
            Task.Run(async () => await RunAsync(caso, dbHandler)).GetAwaiter().GetResult();
        }

        public static async Task RunAsync(string caso, SQLiteHandler dbHandler)
        {
            switch (caso) {
                case "1": 
                    await dbHandler.FilterAndInsertCSV(); break;
                case "2":
                    await dbHandler.FilterAndInsertXML(); break;
                case "3":
                    await dbHandler.FilterAndInsertJSON(); break;
                case "4":
                    dbHandler.DeleteData("DELETE FROM Monumento");
                    dbHandler.DeleteData("DELETE FROM Localidad");
                    dbHandler.DeleteData("DELETE FROM Provincia");
                    dbHandler.DeleteData("DELETE FROM sqlite_sequence"); 
                    break;
                default:
                    Console.WriteLine("Se ha introducido un valor no valido"); Start(dbHandler); return;
            }
            // Bloque de código de selección de todos los datos de la BD
            string selectQuery = "SELECT * FROM Monumento";
            dbHandler.QueryData(selectQuery);
            selectQuery = "SELECT * FROM Localidad";
            dbHandler.QueryData(selectQuery);
            selectQuery = "SELECT * FROM Provincia";
            dbHandler.QueryData(selectQuery);

            Start(dbHandler);
        }

    }
}
