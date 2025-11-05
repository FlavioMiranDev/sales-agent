using chatbot.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Controllers;

[Route("api/sales")]
[ApiController]
public class SalesController : ControllerBase
{
    private readonly IEmbeddingDataService _embeddingService;

    public SalesController(IEmbeddingDataService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    [HttpGet]
    public async Task<IActionResult> UpdateData()
    {
        await _embeddingService.UpdateDatabase();

        return Ok();
    }
}
