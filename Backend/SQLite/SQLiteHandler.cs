using System;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using IEIPracticas;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using IEIPracticas.APIs_Scrapper;
using System.Data.Common;
using SimMetrics.Net.Metric;
using System.Text;

namespace IEIPracticas.SQLite
{
    public class SQLiteHandler
    {
        private readonly string connectionString;
        private SqliteConnection connection;

        // Constructor que inicializa la conexión
        public SQLiteHandler(string databasePath)
        {
            connectionString = $"Data Source={databasePath}";
        }

        // Método para abrir la conexión
        public void OpenConnection()
        {
            try
            {
                connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("Conexión abierta.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la conexión: {ex.Message}");
            }
        }

        // Método para cerrar la conexión
        public void CloseConnection()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Conexión cerrada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar la conexión: {ex.Message}");
            }
        }

        // Método para crear una tabla (si no existe)
        public void CreateTable(string createTableQuery)
        {
            try
            {
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Tabla creada o ya existe.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la tabla: {ex.Message}");
            }
        }

        // Método para insertar datos
        public void InsertData(string insertQuery)
        {
            try
            {
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Resultados de la ejecucion ('{insertQuery}'):");
                    Console.WriteLine($"{rowsAffected} fila(s) insertada(s).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar datos: {ex.Message}");
            }
        }


        // Método para eliminar datos
        public int DeleteData(string deleteQuery)
        {
            try
            {
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Resultados de la ejecucion ('{deleteQuery}'): {rowsAffected} fila(s) eliminada(s).");
                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar datos: {ex.Message}");
                return 0;
            }
        }

        // Método para consultar datos
        public void QueryData(string selectQuery)
        {
            try
            {
                using (var command = new SqliteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine($"Resultados de la consulta ('{selectQuery}'):");
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader.GetName(i)}: {reader.GetValue(i)} ");
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar datos: {ex.Message}");
            }
        }
        // Metodo para conseguir la Id de la consulta
        public string GetId(string getidQuery)
        {
            try
            {
                using (var command = new SqliteCommand(getidQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Si hay resultados
                    {
                        return reader.GetValue(0).ToString(); // Devuelve la id como string
                    }
                    return null; // Si no se encuentran resultados
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar datos: {ex.Message}");
                return null;
            }
        }
        //Metodo para comprobar si algo del nombre especificado o muy similar existe en la tabla especificada
        //Devuelve un valor booleano con el veredicto y el nombre a usar en la BD
        private (bool, string) EsNombreDuplicado(string table, string nuevoNombre)
        {
            string query = $"SELECT nombre FROM {table}";
            List<string> nombresExistentes = new List<string>();
            var lev = new Levenstein();
            double umbral = 0.85;

            using (var command = new SqliteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    nombresExistentes.Add(reader.GetValue(0).ToString());
                }
            }

            foreach (var nombre in nombresExistentes)
            {
                double similitud = lev.GetSimilarity(nombre, nuevoNombre);
                if (similitud == 1) return (true, nombre);
                if (similitud >= umbral)
                {
                    Console.WriteLine($"'{nuevoNombre}' es similar a '{nombre}' con una similitud del {similitud * 100:F2}%. Se asume error tipográfico y se rechaza.");
                    return (true, nombre);
                }
            }
            return (false, nuevoNombre);
        }
        // Metodo para insertar un monumento
        private void InsertMonumento(Monumento monumento)
        {
            GetOrInsertProvinciaId(monumento.Provincia);
            int.TryParse(GetOrInsertLocalidadId(monumento.Localidad, monumento.Provincia).ToString(), out int idLocalidad);
            if (idLocalidad == 0)
            {
                Console.WriteLine($"Error: Monumento '{monumento.Nombre}' no esta ligado a ninguna localidad valida, rechazado.");
                RejectedRecords.Add($"Nombre: {monumento.Nombre}, Error: No está ligado a ninguna localidad válida");
                return;
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string insertQuery = "INSERT INTO Monumento (nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo, idLocalidad) " +
                         "VALUES (@Nombre, @Direccion, @CodigoPostal, @Longitud, @Latitud, @Descripcion, @Tipo, @idLocalidad)";
                    using (var cmd = new SqliteCommand(insertQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", monumento.Nombre);
                        cmd.Parameters.AddWithValue("@Direccion", monumento.Direccion);
                        cmd.Parameters.AddWithValue("@CodigoPostal", monumento.CodigoPostal.ToString("D5"));
                        cmd.Parameters.AddWithValue("@Longitud", monumento.Longitud);
                        cmd.Parameters.AddWithValue("@Latitud", monumento.Latitud);
                        cmd.Parameters.AddWithValue("@Descripcion", monumento.Descripcion);
                        cmd.Parameters.AddWithValue("@Tipo", GetEnumDescription(monumento.Tipo));
                        cmd.Parameters.AddWithValue("@idLocalidad", idLocalidad);

                        // Ejecutar la consulta
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error al insertar datos: {ex.Message}");
                }
            }
        }
        // Metodo para conseguir la descripcion del tipo de monumento
        public string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute.Description;
        }
        // Metodo para filtrar e invocar el metodo de InsertMonument(monumento) para el .csv
        public async Task FilterAndInsertCSV()
        {
            var httpClient = new HttpClient();
            string wrapperApiUrl = "http://localhost:5001/api/wrapperCSV/processAndSend"; // URL de la API del wrapper

            // Llamada a la API del wrapper para obtener el JSON
            HttpResponseMessage response = await httpClient.GetAsync(wrapperApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: No se pudo obtener el JSON del wrapper. Código de estado: {response.StatusCode}");
                return;
            }
            string csvdoc = await response.Content.ReadAsStringAsync();
            var options = new ChromeOptions();
            // Configura Chrome para suprimir mensajes no críticos
            options.AddArgument("--log-level=3");
            options.AddArgument("--disable-logging");
            IWebDriver driver = new ChromeDriver(options);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            List<CSVMonumento> csvMonumentos = JsonSerializer.Deserialize<List<CSVMonumento>>(csvdoc);
            foreach (CSVMonumento csvMonumento in csvMonumentos)
            {
                if (EsNombreDuplicado("Monumento", csvMonumento.DENOMINACION).Item1)
                {
                    Console.WriteLine($"Error: El monumento '{csvMonumento.DENOMINACION}' ya existe en la BD, rechazado.");
                    RejectedRecords.Add($"Nombre: {csvMonumento.DENOMINACION}, Error: Ya existe en la BD");
                    continue;
                }

                if (string.IsNullOrEmpty(csvMonumento.UTMESTE) || string.IsNullOrEmpty(csvMonumento.UTMNORTE))
                {
                    Console.WriteLine($"Error: Coordenadas del monumento '{csvMonumento.DENOMINACION}' no válidas, rechazado.");
                    RejectedRecords.Add($"Nombre: {csvMonumento.DENOMINACION}, Error: Coordenadas no válidas");
                    continue;
                }
                (string Latitude, string Longitude) latlon = SeleniumScraper.Scraper(driver, csvMonumento.UTMESTE, csvMonumento.UTMNORTE);

                csvMonumento.UTMESTE = latlon.Latitude;
                csvMonumento.UTMNORTE = latlon.Longitude;
                var monumento = await CSVExt.CSVMonumentoToMonumento(csvMonumento);
                if (monumento == null)
                {
                    continue;
                }
                InsertMonumento(monumento);
                InsertedRecords = InsertedRecords + 1;
            }
            driver.Quit();

            // Agregar registros reparados y rechazados de CSVExt a las listas del handler
            RepairedRecords.AddRange(CSVExt.RepairedRecords);
            CSVExt.RepairedRecords.Clear();
            RejectedRecords.AddRange(CSVExt.RejectedRecords);
            CSVExt.RejectedRecords.Clear();
        }
        // Metodo para filtrar e invocar el metodo de InsertMonument(monumento) para el .json
        public async Task FilterAndInsertJSON()
        {
            var httpClient = new HttpClient();
            string wrapperApiUrl = "http://localhost:5003/api/wrapperJSON/processAndSend"; // URL de la API del wrapper

            // Llamada a la API del wrapper para obtener el JSON
            HttpResponseMessage response = await httpClient.GetAsync(wrapperApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: No se pudo obtener el JSON del wrapper. Código de estado: {response.StatusCode}");
                return;
            }
            string jsondoc = await response.Content.ReadAsStringAsync();
            List<JSONMonumento> jsonMonumentos = JsonSerializer.Deserialize<List<JSONMonumento>>(jsondoc);
            foreach (JSONMonumento jsonMonumento in jsonMonumentos)
            {
                if (EsNombreDuplicado("Monumento", jsonMonumento.documentName).Item1)
                {
                    Console.WriteLine($"Error: El monumento '{jsonMonumento.documentName}' ya existe en la BD, rechazado.");
                    RejectedRecords.Add($"Nombre: {jsonMonumento.documentName}, Error: Ya existe en la BD");
                    continue;
                }
                var monumento = await JSONExt.JSONMonumentoToMonumento(jsonMonumento);
                if (monumento == null)
                {
                    continue;
                }
                InsertMonumento(monumento);
                InsertedRecords = InsertedRecords + 1;
            }

            // Agregar registros reparados y rechazados de JSONExt a las listas del handler
            RepairedRecords.AddRange(JSONExt.RepairedRecords);
            JSONExt.RepairedRecords.Clear();
            RejectedRecords.AddRange(JSONExt.RejectedRecords);
            JSONExt.RejectedRecords.Clear();
        }
        // Metodo para filtrar e invocar el metodo de InsertMonument(monumento) para el .xml
        public async Task FilterAndInsertXML()
        {
            var httpClient = new HttpClient();
            string wrapperApiUrl = "http://localhost:5002/api/wrapperXML/processAndSend"; // URL de la API del wrapper

            // Llamada a la API del wrapper para obtener el JSON
            HttpResponseMessage response = await httpClient.GetAsync(wrapperApiUrl);
            response.EnsureSuccessStatusCode(); // Lanza una excepción si el código de estado no es exitoso

            string xmldoc = await response.Content.ReadAsStringAsync();
            // Deserializar el JSON recibido
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(xmldoc);
            var monumentosArray = jsonObject.GetProperty("monumentos").GetProperty("monumento").EnumerateArray();
            var xmlMonumentos = new List<XMLMonumento>();

            foreach (var monumentoElement in monumentosArray)
            {
                var xmlMonumento = JsonSerializer.Deserialize<XMLMonumento>(monumentoElement.GetRawText());
                xmlMonumentos.Add(xmlMonumento);
            }

            Console.WriteLine(xmldoc);

            foreach (XMLMonumento xmlMonumento in xmlMonumentos)
            {
                if (EsNombreDuplicado("Monumento", xmlMonumento.nombre).Item1)
                {
                    Console.WriteLine($"Error: El monumento '{xmlMonumento.nombre}' ya existe en la BD, rechazado");
                    RejectedRecords.Add($"Nombre: {xmlMonumento.nombre}, Error: Ya existe en la BD");
                    continue;
                }
                var monumento = await XMLExt.XMLMonumentoToMonumento(xmlMonumento);

                if (monumento == null)
                {
                    continue;
                }
                InsertMonumento(monumento);
                InsertedRecords = InsertedRecords + 1;
            }

            // Agregar registros reparados y rechazados de XMLExt a las listas del handler
            RepairedRecords.AddRange(XMLExt.RepairedRecords);
            XMLExt.RepairedRecords.Clear();
            RejectedRecords.AddRange(XMLExt.RejectedRecords);
            XMLExt.RejectedRecords.Clear();
        }
        // Método para devolver la id de la localidad si existe y si no, agregarla
        public int GetOrInsertLocalidadId(string nombreLocalidad, string nombreProvincia)
        {
            if (string.IsNullOrEmpty(nombreLocalidad))
            {
                Console.WriteLine("Se ha intentado añadir una localidad pero carece de nombre, omitida.");
                return 0;
            }
            (bool veredicto, string nombre) filtro = EsNombreDuplicado("Localidad", nombreLocalidad);
            if (!filtro.veredicto)
            {
                string insertLocalidad = "INSERT INTO Localidad (nombre, idProvincia) VALUES (@NombreLocalidad, @IdProvincia)";
                using (var command = new SqliteCommand(insertLocalidad, connection))
                {
                    command.Parameters.AddWithValue("@NombreLocalidad", filtro.nombre);
                    command.Parameters.AddWithValue("@IdProvincia", GetOrInsertProvinciaId(nombreProvincia));
                    command.ExecuteNonQuery();
                }
            }
            string queryIdLocalidad = $"SELECT idLocalidad FROM Localidad WHERE nombre = '{filtro.nombre}'";
            var idLocalidad = GetId(queryIdLocalidad);
            return int.Parse(idLocalidad);
        }

        // Método para devolver la id de la localidad si existe y si no, agregarla
        public int GetOrInsertProvinciaId(string nombreProvincia)
        {
            if (string.IsNullOrEmpty(nombreProvincia))
            {
                Console.WriteLine("Se ha intentado añadir una provincia pero carece de nombre, omitida.");
                return 0;
            }
            (bool veredicto, string nombre) filtro = EsNombreDuplicado("Provincia", nombreProvincia);
            if (!filtro.veredicto)
            {
                string insertProvincia = "INSERT INTO Provincia (nombre) VALUES (@NombreProvincia)";
                using (var command = new SqliteCommand(insertProvincia, connection))
                {
                    command.Parameters.AddWithValue("@NombreProvincia", filtro.nombre);
                    command.ExecuteNonQuery();
                }
            }
            string queryIdProvincia = $"SELECT idProvincia FROM Provincia WHERE nombre = '{filtro.nombre}'";
            var idProvincia = GetId(queryIdProvincia);
            return int.Parse(idProvincia);
        }

        // Metodo para borrar todo lo contenido en la base de datos
        public Dictionary<string, int> DeleteAllData()
        {
            var deleteResults = new Dictionary<string, int>();
            try
            {
                deleteResults["Monumento"] = DeleteData("DELETE FROM Monumento");
                deleteResults["Localidad"] = DeleteData("DELETE FROM Localidad");
                deleteResults["Provincia"] = DeleteData("DELETE FROM Provincia");
                deleteResults["sqlite_sequence"] = DeleteData("DELETE FROM sqlite_sequence");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
            }
            return deleteResults;
        }
        public string GetStringData(string selectQuery)
        {
            string response = "";
            try
            {
                using (var command = new SqliteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            response = response + ($"{reader.GetName(i)}: {reader.GetValue(i)} ");
                        }
                        response = response + "\n";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response = response + ($"Error al consultar datos: {ex.Message}");
                return response;
            }
        }

        public List<T> GetData<T>(string query)
        {
            var resultList = new List<T>();
            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = Activator.CreateInstance<T>();
                        
                        // Recorrer todas las columnas en el lector de datos
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var propertyName = char.ToUpper(columnName[0]) + columnName.Substring(1);
                            
                            // Buscar la propiedad que coincida, usando la capitalización correcta
                            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                            if ((property != null && reader[columnName] != DBNull.Value)  || columnName == "codigo_postal")
                            {
                                try
                                {
                                    var value = reader.GetValue(i);

                                    if (value != null)
                                    {
                                        if (propertyName.Equals("Codigo_postal")){
                                            if (result != null)
                                            {
                                                PropertyInfo propertyForCodigoPostal = result.GetType().GetProperty("CodigoPostal");
                                                if (propertyForCodigoPostal != null)
                                                {propertyForCodigoPostal.SetValue(result, Convert.ToInt32(value));}
                                            }
                                        }else if (propertyName.Equals("Tipo")){
                                            property.SetValue(result, GetEnumFromDescription(value.ToString()));
                                        } else {
                                            // Caso general
                                        property.SetValue(result, Convert.ChangeType(value, property.PropertyType));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error setting value for {propertyName}: {ex.Message}");
                                }
                            }
                        }
                        resultList.Add(result);
                    }
                }
            }
            return resultList;
        }

        public static Tipo GetEnumFromDescription(string description)
        {
            foreach (var field in typeof(Tipo).GetFields())
            {
                // Obtener el atributo DescriptionAttribute
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                
                if (attribute != null && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return (Tipo)field.GetValue(null);
                }
            }
            
            throw new ArgumentException($"No se encontró ningún valor del enumerado Tipo con la descripción '{description}'.");
        }



        //Cambiar nombre
        public List<T> GetData<T>(string query, Dictionary<string, object>? parameters = null)
        {
            var resultList = new List<T>();

            try
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    // Agregar parámetros a la consulta si existen
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var result = Activator.CreateInstance<T>();

                            // Recorrer todas las columnas en el lector de datos
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var propertyName = char.ToUpper(columnName[0]) + columnName.Substring(1);

                                // Buscar la propiedad que coincida, usando la capitalización correcta
                                var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                                if ((property != null && reader[columnName] != DBNull.Value) || columnName == "codigo_postal")
                                {
                                    try
                                    {
                                        var value = reader.GetValue(i);

                                        if (value != null)
                                        {
                                            if (propertyName.Equals("Codigo_postal"))
                                            {
                                                if (result != null)
                                                {
                                                    PropertyInfo propertyForCodigoPostal = result.GetType().GetProperty("CodigoPostal");
                                                    if (propertyForCodigoPostal != null)
                                                    {
                                                        propertyForCodigoPostal.SetValue(result, Convert.ToInt32(value));
                                                    }
                                                }
                                            }
                                            else if (propertyName.Equals("Tipo"))
                                            {
                                                property.SetValue(result, GetEnumFromDescription(value.ToString()));
                                            }
                                            else
                                            {
                                                // Caso general
                                                property.SetValue(result, Convert.ChangeType(value, property.PropertyType));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error setting value for {propertyName}: {ex.Message}");
                                    }
                                }
                            }
                            resultList.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar la consulta: {ex.Message}", ex);
            }

            return resultList;
        }


        public List<string> RepairedRecords { get; private set; } = new List<string>();
        public List<string> RejectedRecords { get; private set; } = new List<string>();
        public int InsertedRecords { get; private set;} = 0;
    }
}
