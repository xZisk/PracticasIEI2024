using Microsoft.AspNetCore.Mvc;
using WrapperJSON;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/wrapperJSON")]
public class WrapperJSONController : ControllerBase
{
    /// <summary>
    /// Process JSON and send.
    /// </summary>
    /// <returns>Processed JSON data.</returns>
    [HttpGet("processAndSend")]
    [SwaggerOperation(Summary = "Process JSON and send", Description = "Loads JSON data and returns it.")]
    [SwaggerResponse(200, "OK - Sends a JSON item")]
    [SwaggerResponse(500, "Internal Server Error - Error processing JSON: 'exception message'")]
    public IActionResult ProcessAndSend()
    {
        try
        {
            // Abrir JSON
            var json = WrapperJSONclass.LoadJSONAsString();
            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error processing JSON: {ex.Message}" });
        }
    }
}