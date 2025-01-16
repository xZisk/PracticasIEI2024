using IEIPracticas.Models;
using IEIPracticas.SQLite;
using Microsoft.AspNetCore.Mvc;

namespace Busqueda.Controllers
{
    [ApiController]
    [Route("api/busqueda")]
    public class BusquedaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _databasePath;
        private string response = "Resultados de la busqueda:\n";
        public BusquedaController(HttpClient httpClient, IWebHostEnvironment env)
        {
            _httpClient = httpClient;

            // Ruta relativa al directorio donde est� el proyecto principal, fuera de la carpeta 'Monuments'
            string projectRoot = Path.Combine(env.ContentRootPath, "..", "Backend", "SQLite");

            // Crear la ruta completa a la base de datos
            _databasePath = Path.Combine(projectRoot, "dbproject.db");

            _databasePath = Path.GetFullPath(_databasePath);
        }
        [HttpGet]
        public IActionResult GetAllDatabase()
        {
            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();
                string selectQuery = "SELECT * FROM Monumento";
                response = response + dbHandler.GetStringData(selectQuery);
                selectQuery = "SELECT * FROM Localidad";
                response = response + dbHandler.GetStringData(selectQuery);
                selectQuery = "SELECT * FROM Provincia";
                response = response + dbHandler.GetStringData(selectQuery);
                dbHandler.CloseConnection();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving monuments: {ex.Message}");
            }
        }

        //////////////////creo q esto no hará falta si al final no usamos se borra y ya////////////////////////////
        /*
        //Filtrar Monumento por localidad
        [HttpGet("monumentoPorLocalidad")]
        public IActionResult GetMonumentsByLocalidad([FromQuery] string localidad)
        {
            if (string.IsNullOrWhiteSpace(localidad))
            {
                return BadRequest("El parámetro 'localidad' no puede estar vacío.");
            }

            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();

                // Consulta para obtener los monumentos asociados a la localidad
                 string query = @"
                    SELECT m.*
                    FROM Monumento m
                    INNER JOIN Localidad l ON m.idLocalidad = l.idLocalidad
                    WHERE l.nombre LIKE @Localidad";

                // Ejecutar la consulta con el parámetro localidad
                var parameters = new Dictionary<string, object>
                {
                    { "@Localidad", $"%{localidad}%" }
                };
                var resultados = dbHandler.GetStringData(query, parameters);
                dbHandler.CloseConnection();

                string response = $"Resultados de la búsqueda para la localidad '{localidad}':\n";
                response += resultados;

                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }

        //Filtrar monumento por código postal
        [HttpGet("monumentoPorCodigoPostal")]
        public IActionResult GetMonumentsByCodigoPostal([FromQuery] string codigoPostal)
        {
            if (string.IsNullOrWhiteSpace(codigoPostal))
            {
                return BadRequest("El parámetro 'codigoPostal' no puede estar vacío.");
            }

            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();

                // Consulta para obtener los monumentos asociados al código postal
                string query = @"
                    SELECT *
                    FROM Monumento
                    WHERE codigo_postal LIKE @CodigoPostal";

                // Ejecutar la consulta con el parámetro código postal
                var parameters = new Dictionary<string, object>
                {
                    { "@CodigoPostal", $"%{codigoPostal}%" }
                };
                var resultados = dbHandler.GetStringData(query, parameters);

                dbHandler.CloseConnection();

                // Construir la respuesta en texto plano
                string response = $"Resultados de la búsqueda para el código postal '{codigoPostal}':\n";
                response += resultados;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }

        //Filtrar por Provincia
        [HttpGet("monumentoPorProvincia")]
        public IActionResult GetMonumentsByProvincia([FromQuery] string provincia)
        {
            if (string.IsNullOrWhiteSpace(provincia))
            {
                return BadRequest("El parámetro 'provincia' no puede estar vacío.");
            }

            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();

                // Consulta para obtener los monumentos asociados a la provincia
                string query = @"
                    SELECT m.*
                    FROM Monumento m
                    INNER JOIN Localidad l ON m.idLocalidad = l.idLocalidad
                    INNER JOIN Provincia p ON l.idProvincia = p.idProvincia
                    WHERE p.nombre LIKE @Provincia";

                // Ejecutar la consulta con el parámetro provincia
                var parameters = new Dictionary<string, object>
                {
                    { "@Provincia", $"%{provincia}%" }
                };
                var resultados = dbHandler.GetStringData(query, parameters);

                dbHandler.CloseConnection();

                // Construir la respuesta en texto plano
                string response = $"Resultados de la búsqueda para la provincia '{provincia}':\n";
                response += resultados;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }

        //Filtrar por tipo de monumento
        [HttpGet("monumentoPorTipo")]
        public IActionResult GetMonumentsByTipo([FromQuery] string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return BadRequest("El parámetro 'tipo' no puede estar vacío.");
            }

            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();

                // Consulta para obtener los monumentos asociados al tipo
                string query = @"
                    SELECT *
                    FROM Monumento
                    WHERE tipo LIKE @Tipo";

                // Ejecutar la consulta con el parámetro tipo
                var parameters = new Dictionary<string, object>
                {
                    { "@Tipo", $"%{tipo}%" }
                };
                var resultados = dbHandler.GetStringData(query, parameters);

                dbHandler.CloseConnection();

                // Construir la respuesta en texto plano
                string response = $"Resultados de la búsqueda para el tipo '{tipo}':\n";
                response += resultados;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }*/

        [HttpGet("buscarMonumentos")]
        public IActionResult BuscarMonumentos(
            [FromQuery] string? localidad, 
            [FromQuery] string? codigoPostal, 
            [FromQuery] string? provincia, 
            [FromQuery] string? tipo)
        {
            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();

                // Construir la consulta base
                string query = @"
                    SELECT m.*
                    FROM Monumento m
                    LEFT JOIN Localidad l ON m.idLocalidad = l.idLocalidad
                    LEFT JOIN Provincia p ON l.idProvincia = p.idProvincia
                    WHERE 1 = 1";

                // Crear parámetros dinámicos
                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(localidad))
                {
                    query += " AND l.nombre LIKE @Localidad";
                    parameters["@Localidad"] = $"%{localidad}%";
                }

                if (!string.IsNullOrWhiteSpace(codigoPostal))
                {
                    query += " AND m.codigo_postal LIKE @CodigoPostal";
                    parameters["@CodigoPostal"] = $"%{codigoPostal}%";
                }

                if (!string.IsNullOrWhiteSpace(provincia))
                {
                    query += " AND p.nombre LIKE @Provincia";
                    parameters["@Provincia"] = $"%{provincia}%";
                }

                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    query += " AND m.tipo LIKE @Tipo";
                    parameters["@Tipo"] = $"%{tipo}%";
                }

                // Ejecutar la consulta
                var resultados = dbHandler.GetStringData(query, parameters);

                dbHandler.CloseConnection();

                // Construir la respuesta en texto plano
                string response = "Resultados de la búsqueda:\n";
                response += resultados;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }


    }
}
