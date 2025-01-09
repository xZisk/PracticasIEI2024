using Microsoft.AspNetCore.Mvc;
using WrapperCSV;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/wrapperCSV")]
public class WrapperCSVController : ControllerBase
{
    /// <summary>
    /// Process CSV and send as JSON.
    /// </summary>
    /// <returns>Processed JSON data.</returns>
    [HttpGet("processAndSend")]
    [SwaggerOperation(Summary = "Process CSV and send as JSON", Description = "Converts CSV data to JSON format and returns it.")]
    [SwaggerResponse(200, "OK - Sends a JSON item of the CSV data")]
    [SwaggerResponse(500, "Internal Server Error - Error processing CSV: 'exception message'")]
    public IActionResult ProcessAndSend()
    {
        try
        {
            // Convertir CSV a JSON
            var json = WrapperCSVclass.ConvertCsvToJson();
            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error processing CSV: {ex.Message}" });
        }
    }
}