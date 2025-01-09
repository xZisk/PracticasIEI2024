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
    }
}
