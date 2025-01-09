using Microsoft.AspNetCore.Mvc;
using IEIPracticas.Models;
using System.Text;
using IEIPracticas.SQLite;
using Microsoft.AspNetCore.Cors;

namespace Monuments.Controllers
{
    [ApiController]
    [Route("api/carga")]
    [EnableCors("AllowLocalhost3000")]
    public class CargaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _databasePath;
        public CargaController(HttpClient httpClient, IWebHostEnvironment env)
        {
            _httpClient = httpClient;

            // Ruta relativa al directorio donde está el proyecto principal, fuera de la carpeta 'Monuments'
            string projectRoot = Path.Combine(env.ContentRootPath, "..", "IEIPracticas", "SQLite");

            // Crear la ruta completa a la base de datos
            _databasePath = Path.Combine(projectRoot, "dbproject.db");

            _databasePath = Path.GetFullPath(_databasePath);
        }

        [HttpPost("{tipo}")]
        public async Task<IActionResult> UploadMonuments(string tipo)
        {
            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();
                switch (tipo)
                {
                    case "csv":
                        await dbHandler.FilterAndInsertCSV(); break;
                    case "xml": 
                        await dbHandler.FilterAndInsertXML(); break;
                    case "json":
                        await dbHandler.FilterAndInsertJSON(); break;
                    case "all":
                        await dbHandler.FilterAndInsertCSV();
                        await dbHandler.FilterAndInsertXML();
                        await dbHandler.FilterAndInsertJSON(); 
                        break;
                };
                dbHandler.CloseConnection();
                var result = new
                {
                    SuccessfulRecords = dbHandler.InsertedRecords,
                    RepairedRecords = dbHandler.RepairedRecords,
                    RejectedRecords = dbHandler.RejectedRecords
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving monuments: {ex.Message}");
                return StatusCode(500, $"Error saving monuments: {ex.Message}");
            }
        }

        [HttpDelete("clear")]
        public IActionResult DeleteAllData()
        {
            try
            {
                SQLiteHandler dbHandler = new SQLiteHandler(_databasePath);
                dbHandler.OpenConnection();
                var deleteResults = dbHandler.DeleteAllData();
                dbHandler.CloseConnection();
                return Ok(deleteResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
                return StatusCode(500, $"Error deleting data: {ex.Message}");
            }
        }
    }
}
