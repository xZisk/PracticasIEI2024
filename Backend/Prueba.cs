using IEIPracticas.APIs_Scrapper;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using Microsoft.Data.Sqlite;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
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
using IEIPracticas.SQLite;

namespace IEIPracticas
{
    class Prueba
    {
        private static readonly HttpClient client = new HttpClient();
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
            string apiCargaUrl = "http://localhost:5000/api/carga/";
            string apiBusquedaUrl = "http://localhost:5005/api/busqueda/";
            string responseMessage = "";
            switch (caso) {
                case "1":
                    responseMessage = await CallApiAsync(apiCargaUrl + "csv"); break;
                case "2":
                    responseMessage = await CallApiAsync(apiCargaUrl + "xml"); break;
                case "3":
                    responseMessage = await CallApiAsync(apiCargaUrl + "json"); break;
                case "4":
                    responseMessage = await DeleteAllDataAsync(apiCargaUrl); break;
                default:
                    Console.WriteLine("Se ha introducido un valor no valido"); Start(dbHandler); return;
            }
            // Bloque de código de selección de todos los datos de la BD
            responseMessage = await GetDBAsync(apiBusquedaUrl);
            Console.Write(responseMessage);
            Start(dbHandler);
        }
        private static async Task<string> GetDBAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return $"{await response.Content.ReadAsStringAsync()}";
                }
                else
                {
                    return $"Error al procesar los datos: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error en la llamada a la API: {ex.Message}";
            }
        }
        private static async Task<string> CallApiAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, null);
                if (response.IsSuccessStatusCode)
                {
                    return $"Datos procesados correctamente: {await response.Content.ReadAsStringAsync()}";
                }
                else
                {
                    return $"Error al procesar los datos: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error en la llamada a la API: {ex.Message}";
            }
        }

        private static async Task<string> DeleteAllDataAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return "Datos eliminados correctamente.";
                }
                else
                {
                    return $"Error al eliminar los datos: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error en la llamada a la API de eliminación: {ex.Message}";
            }
        }
    }
}
