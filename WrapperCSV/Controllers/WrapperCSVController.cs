using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WrapperCSV;

[ApiController]
[Route("api/wrapperCSV")]
public class WrapperCSVController : ControllerBase
{
    [HttpGet("processAndSend")]
    public IActionResult ProcessAndSend()
    {
        try
        {
            // Convertir CSV a JSON
            var json = WrapperCSVclass.ConvertCsvToJson();

            return Ok(json);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing CSV: {ex.Message}");
        }
    }
}