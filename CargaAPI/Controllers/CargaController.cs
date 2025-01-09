using Microsoft.AspNetCore.Mvc;
using IEIPracticas.SQLite;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.AspNetCore.Annotations;

namespace Monuments.Controllers
{
    [ApiController]
    [Route("api/carga")]
    [EnableCors("AllowLocalhost3000")]
    [SwaggerSchema("CargaAPI")]
    public class CargaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _databasePath;
        public CargaController(HttpClient httpClient, IWebHostEnvironment env)
        {
            _httpClient = httpClient;

            // Ruta relativa al directorio donde está el proyecto principal, fuera de la carpeta 'Monuments'
            string projectRoot = Path.Combine(env.ContentRootPath, "..", "Backend", "SQLite");

            // Crear la ruta completa a la base de datos
            _databasePath = Path.Combine(projectRoot, "dbproject.db");

            _databasePath = Path.GetFullPath(_databasePath);
        }

        /// <summary>
        /// Upload monuments data.
        /// </summary>
        /// <param name="tipo">The type of data to upload (csv, xml, json, all).</param>
        /// <returns>Result of the upload operation.</returns>
        [HttpPost("{tipo}")]
        [SwaggerOperation(Summary = "Upload monuments data", Description = "Uploads monuments data from CSV, XML, JSON, or all formats.")]
        [SwaggerResponse(200, "OK - Data uploaded successfully.")]
        [SwaggerResponse(400, "Bad Request - Invalid data type.")]
        [SwaggerResponse(500, "Internal Server Error - Error saving monuments.")]
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
                    default:
                        return BadRequest(new { message = "Invalid data type. Use 'csv', 'xml', 'json', or 'all'." });
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
                return StatusCode(500, new { message = $"Error saving monuments: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete all data.
        /// </summary>
        /// <returns>Result of the delete operation.</returns>
        [HttpDelete("clear")]
        [SwaggerOperation(Summary = "Delete all data", Description = "Deletes all data from the database.")]
        [SwaggerResponse(200, "OK - Data deleted successfully.")]
        [SwaggerResponse(500, "Internal Server Error - Error deleting data.")]
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
                return StatusCode(500, new { message = $"Error deleting data: {ex.Message}" });
            }
        }
    }
}
