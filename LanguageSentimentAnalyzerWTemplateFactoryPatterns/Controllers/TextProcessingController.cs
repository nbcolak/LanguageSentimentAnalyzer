using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Annotations;
using TranslateSentimentProcessorWTemplateFactoryPatterns.Factory;

namespace TranslateSentimentProcessorWTemplateFactoryPatterns.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TextProcessingController : ControllerBase
{
    private readonly TextProcessingFactory _factory;

    public TextProcessingController(TextProcessingFactory factory)
    {
        _factory = factory;
    }

    [HttpPost]
    public IActionResult ProcessText([FromBody] TextProcessingRequest request)
    {
        try
        {
            var processor = _factory.CreateProcessor(request.TranslateService, request.SentimentService);

            processor.ProcessText(request.Text);

            return Ok("Ok. Look at the console");
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Invalid request: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
public class TextProcessingRequest
{
    public string Text { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [SwaggerSchema("Select translation service (Google = 0, Azure = 1)")]
    public ServiceType TranslateService { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [SwaggerSchema("Select sentiment service (Google = 0, Azure = 1)")]
    public ServiceType SentimentService { get; set; }
}