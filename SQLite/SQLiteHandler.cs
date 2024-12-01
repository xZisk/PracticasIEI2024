using System;
using IEIPracticas.Extractores;
using IEIPracticas.Mappers;
using IEIPracticas.Models;
using IEIPracticas;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace SQLiteOperations
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

        public SqliteConnection getConnection() { return connection; }

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
                    Console.WriteLine($"{rowsAffected} fila(s) insertada(s).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar datos: {ex.Message}");
            }
        }

        // Método para eliminar datos
        public void DeleteData(string deleteQuery)
        {
            try
            {
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} fila(s) eliminada(s).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar datos: {ex.Message}");
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
                    Console.WriteLine("Resultados de la consulta:");
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
        //Metodo para comprobar si algo del nombre especificado existe en la tabla especificada
        public bool DoesItExist(string table, string nombre)
        {
            string query = $"SELECT COUNT(*) FROM {table} WHERE nombre = @Nombre";

            using (var command = new SqliteCommand(query, connection)) // Usa la conexión existente
            {
                command.Parameters.AddWithValue("@Nombre", nombre);
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }
        // Metodo para insertar un monumento
        private void InsertMonumento(Monumento monumento)
        {
            if (DoesItExist("Monumento", monumento.Nombre))
            {
                Console.WriteLine($"El monumento '{monumento.Nombre}' ya existe en la BD");
                return;
            };

            GetOrInsertProvinciaId(monumento.Provincia);
            int.TryParse(GetOrInsertLocalidadId(monumento.Localidad, monumento.Provincia).ToString(), out int idLocalidad);
            if (idLocalidad == 0)
            {
                Console.WriteLine($"Error: Monumento '{monumento.Nombre} no esta ligado a ninguna localidad valida, rechazado.");
                return;
            }

            string query = $"INSERT INTO Monumento " +
               $"(nombre, direccion, codigo_postal, longitud, latitud, descripcion, tipo, idLocalidad) " +
               $"VALUES ('{monumento.Nombre}', " +
               $"'{monumento.Direccion}', " +
               $"{monumento.CodigoPostal.ToString("D5")}, " +
               $"{monumento.Longitud.ToString(CultureInfo.InvariantCulture)}, " +
               $"{monumento.Latitud.ToString(CultureInfo.InvariantCulture)}, " +
               $"'{monumento.Descripcion}', " +
               $"'{GetEnumDescription(monumento.Tipo)}', " +
               $"{idLocalidad})";

            InsertData(query);
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
            string csvdoc = Extractor_csv.ConvertCsvToJson(".\\FFDD\\bienes_inmuebles_interes_cultural.csv");
            List<CSVMonumento> csvMonumentos = JsonSerializer.Deserialize<List<CSVMonumento>>(csvdoc);
            foreach (CSVMonumento csvMonumento in csvMonumentos)
            {
                var monumento = await CSVMapper.CSVMonumentoToMonumento(csvMonumento);
                if (monumento == null)
                {
                    Console.WriteLine($"Monumento inválido detectado: {csvMonumento.DENOMINACION}");
                    continue;
                }
                InsertMonumento(monumento);
            }
        }
        // Metodo para filtrar e invocar el metodo de InsertMonument(monumento) para el .json
        public async Task FilterAndInsertJSON()
        {
            string jsondoc = Extractor_json.LoadJsonAsString(".\\FFDD\\edificios.json");
            List<JSONMonumento> jsonMonumentos = JsonSerializer.Deserialize<List<JSONMonumento>>(jsondoc);
            foreach (JSONMonumento jsonMonumento in jsonMonumentos)
            {
                var monumento = await JSONMapper.JSONMonumentoToMonumento(jsonMonumento);
                if (monumento == null)
                {
                    Console.WriteLine($"Monumento inválido detectado: {jsonMonumento.documentName}");
                    continue;
                }
                InsertMonumento(monumento);
            }
        }
        // Metodo para filtrar e invocar el metodo de InsertMonument(monumento) para el .xml
        public async Task FilterAndInsertXML()
        {
            string xmldoc = Extractor_xml.ConvertXmlToJson(".\\FFDD\\monumentos.xml");
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(xmldoc);

            var monumentosArray = jsonObject.GetProperty("monumentos").GetProperty("monumento");

            List<XMLMonumento> xmlMonumentos = JsonSerializer.Deserialize<List<XMLMonumento>>(monumentosArray.ToString());

            foreach (XMLMonumento xmlMonumento in xmlMonumentos)
            {
                var monumento = await XMLMapper.XMLMonumentoToMonumento(xmlMonumento);

                if (monumento == null)
                {
                    Console.WriteLine($"Monumento inválido detectado: {xmlMonumento.nombre}");
                    continue;
                }
                InsertMonumento(monumento);
            }
        }
        // Metodo para filtrar e invocar el metodo de InsertMonument(monumento) para el .json
        /*public void FilterAndInsertJSON()
        {
            string jsondoc = Extractor_json.LoadJsonAsString(".\\FFDD\\edificios.json");
            List<JSONMonumento> jsonMonumentos = JsonSerializer.Deserialize<List<JSONMonumento>>(jsondoc);
            List<Monumento> monumentos = new List<Monumento>();
            foreach (JSONMonumento jsonMonumento in jsonMonumentos)
            {
                if (JSONmonumentoToMonumento(jsonMonumento) == null) { monumentos.Add(JSONmonumentoToMonumento(jsonMonumento)); }
            }
        }*/
        // Método para devolver la id de la localidad si existe y si no, agregarla
        public int GetOrInsertLocalidadId(string nombreLocalidad, string nombreProvincia)
        {
            if (string.IsNullOrEmpty(nombreLocalidad))
            {
                Console.WriteLine("Se ha intentado añadir la localidad pero carece de nombre, omitida.");
                return 0;
            }
            // Verificar si la localidad ya existe en la base de datos
            if (!DoesItExist("Localidad", nombreLocalidad))
            {
                // Insertar la nueva localidad
                string insertLocalidad = $"INSERT INTO Localidad (nombre, idProvincia) VALUES ('{nombreLocalidad}', '{GetOrInsertProvinciaId(nombreProvincia)}')";
                InsertData(insertLocalidad);
            }
            string queryIdLocalidad = $"SELECT idLocalidad FROM Localidad WHERE nombre = '{nombreLocalidad}'";
            var idLocalidad = GetId(queryIdLocalidad);
            return int.Parse(idLocalidad);
        }
        // Método para devolver la id de la localidad si existe y si no, agregarla
        public int GetOrInsertProvinciaId(string nombreProvincia)
        {
            if (string.IsNullOrEmpty(nombreProvincia))
            {
                Console.WriteLine("Se ha intentado añadir la provincia pero carece de nombre, omitida.");
                return 0;
            }
            // Verificar si la provincia ya existe en la base de datos
            if (!DoesItExist("Provincia", nombreProvincia))
            {
                // Insertar la nueva provincia
                string insertProvincia = $"INSERT INTO Provincia (nombre) VALUES ('{nombreProvincia}')";
                InsertData(insertProvincia);
            }
            string queryIdProvincia = $"SELECT idProvincia FROM Provincia WHERE nombre = '{nombreProvincia}'";
            var idProvincia = GetId(queryIdProvincia); // Asegúrate de obtener el ID correctamente
            return int.Parse(idProvincia);
        }
    }
}
