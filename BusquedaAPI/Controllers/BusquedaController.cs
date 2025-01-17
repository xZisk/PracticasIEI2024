using IEIPracticas.Models;
using IEIPracticas.SQLite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.AspNetCore.Annotations;
using MyProject.Models;


namespace Busqueda.Controllers
{
    [ApiController]
    [Route("api/busqueda")]
    [EnableCors("AllowLocalhost3000")]
    [SwaggerSchema("BusquedaAPI")]
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

        /// <summary>
        /// Obtiene todos los datos de la base de datos.
        /// </summary>
        /// <returns>Objeto con listas de Monumentos, Localidades y Provincias.</returns>
        [HttpGet("getAllDatabase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllDatabase()
        {
            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();

                // Consulta para obtener los monumentos
                string selectQuery = "SELECT * FROM Monumento";
                var monumentosData = dbHandler.GetData<Monumento>(selectQuery);

                // Consulta para obtener las localidades
                selectQuery = "SELECT * FROM Localidad";
                var localidadesData = dbHandler.GetData<Localidad>(selectQuery);

                // Consulta para obtener las provincias
                selectQuery = "SELECT * FROM Provincia";
                var provinciasData = dbHandler.GetData<Provincia>(selectQuery);

                dbHandler.CloseConnection();

                // Estructurar los datos en un objeto que contenga las tres listas
                var response = new
                {
                    monumentos = monumentosData,
                    localidades = localidadesData,
                    provincias = provinciasData
                };

                return Ok(response);  // Devolver la respuesta estructurada en formato JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving monuments: {ex.Message}");
            }
        }

        /// <summary>
        /// Filtra los monumentos dependiendo de los parametros introducidos.
        /// </summary>
        /// <returns>Objeto con listas de Monumentos</returns>
        /*
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
                var monumentosData = dbHandler.GetData<Monumento>(query, parameters);
                var localidadesData = dbHandler.GetData<Localidad>(query, parameters);
                var provinciasData = dbHandler.GetData<Provincia>(query, parameters);

                dbHandler.CloseConnection();

                var response = new
                {
                    monumentos = monumentosData,
                    localidades = localidadesData,
                    provincias = provinciasData
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }
        */
        [HttpGet("buscarMonumentos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                if (!string.IsNullOrWhiteSpace(localidad))
                {
                    Console.WriteLine("ENTRA EN LOCALIDAD");
                    query += $" AND l.nombre LIKE '{localidad}'";
                    Console.WriteLine("Query ahora mismo : "+ query);
                }

                if (!string.IsNullOrWhiteSpace(codigoPostal))
                {
                    Console.WriteLine("ENTRA EN CODIGOPOSTAL");
                    query += $" AND m.codigo_postal LIKE '{codigoPostal}'";
                    Console.WriteLine("Query ahora mismo : "+ query);
                }

                if (!string.IsNullOrWhiteSpace(provincia))
                {
                    Console.WriteLine("ENTRA EN PROVINCIA");
                    query += $" AND p.nombre LIKE '{provincia}'";
                    Console.WriteLine("Query ahora mismo : "+ query);
                }

                if (!string.IsNullOrWhiteSpace(tipo) && !tipo.Equals("all"))
                {
                    Console.WriteLine("ENTRA EN TIPO");
                    query += $" AND m.tipo LIKE '{tipo}'";
                    Console.WriteLine("Query ahora mismo : "+ query);

                }
                        

                // Ejecutar la consulta
                var monumentosData = dbHandler.GetData<Monumento>(query);
                var localidadesData = dbHandler.GetData<Localidad>(query);
                var provinciasData = dbHandler.GetData<Provincia>(query);

                dbHandler.CloseConnection();

                var response = new
                {
                    monumentos = monumentosData,
                    localidades = localidadesData,
                    provincias = provinciasData
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar monumentos: {ex.Message}");
            }
        }


    }
}
