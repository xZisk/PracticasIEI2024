using System;
using Microsoft.Data.Sqlite;

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

        public SqliteConnection getConnection() { return  connection; }

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
    }
}
