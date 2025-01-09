using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WrapperXML;

[ApiController]
[Route("api/wrapperXML")]
public class WrapperXMLController : ControllerBase
{
    [HttpGet("processAndSend")]
    public IActionResult ProcessAndSend()
    {
        try
        {
            // Convertir XML a JSON
            var json = WrapperXMLclass.ConvertXMLToJson();

            return Ok(json);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing XML: {ex.Message}");
        }
    }
}