using Microsoft.AspNetCore.Mvc;
using WrapperJSON;

[ApiController]
[Route("api/wrapperJSON")]
public class WrapperJSONController : ControllerBase
{
    [HttpGet("processAndSend")]
    public IActionResult ProcessAndSend()
    {
        try
        {
            // Abrir JSON
            var json = WrapperJSONclass.LoadJSONAsString();

            return Ok(json);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing JSON: {ex.Message}");
        }
    }
}