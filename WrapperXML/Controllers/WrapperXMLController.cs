using Microsoft.AspNetCore.Mvc;
using WrapperXML;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/wrapperXML")]
[SwaggerSchema("WrapperXML")]
public class WrapperXMLController : ControllerBase
{
    /// <summary>
    /// Process XML and send as JSON.
    /// </summary>
    /// <returns>Processed JSON data.</returns>
    [HttpGet("processAndSend")]
    [SwaggerOperation(Summary = "Process XML and send as JSON", Description = "Converts XML data to JSON format and returns it.")]
    [SwaggerResponse(200, "OK - Sends a JSON item of the XML data")]
    [SwaggerResponse(500, "Internal Server Error - Error processing XML: 'exception message'")]
    public IActionResult ProcessAndSend()
    {
        try
        {
            // Convertir XML a JSON
            var json = WrapperXMLclass.ConvertXMLToJson();
            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error processing XML: {ex.Message}" });
        }
    }
}