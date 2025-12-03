using Microsoft.AspNetCore.Mvc;
using Smart_Contract.Models;
using Smart_Contract.Services;

namespace Smart_Contract.Controllers;

[ApiController]
[Route("api/graphs")]
public class GraphsController : ControllerBase
{
    private readonly BlockchainService _service;
    private readonly ILogger<GraphsController> _logger;

    public GraphsController(BlockchainService service, ILogger<GraphsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GraphInfo>>> CreateGraph([FromBody] CreateGraphRequest request)
    {
        _logger.LogInformation("HTTP POST /api/graphs - GraphId={GraphId}", request.GraphId);
        ApiResponse<GraphInfo> result = await _service.CreateGraphAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<GraphInfo>>>> ListGraphs()
    {
        _logger.LogInformation("HTTP GET /api/graphs");
        ApiResponse<List<GraphInfo>> result = await _service.ListGraphsAsync();
        return Ok(result);
    }

    [HttpGet("{graphId}")]
    public async Task<ActionResult<ApiResponse<GraphInfo>>> GetGraph(string graphId)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}", graphId);
        ApiResponse<GraphInfo> result = await _service.GetGraphInfoAsync(graphId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{graphId}/verify")]
    public async Task<ActionResult<VerifyGraphResponse>> VerifyGraph(string graphId)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}/verify", graphId);
        VerifyGraphResponse result = await _service.VerifyGraphAsync(graphId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("validate/all")]
    public async Task<ActionResult<CrossValidateResponse>> CrossValidateGraphs()
    {
        _logger.LogInformation("HTTP GET /api/graphs/validate/all");
        CrossValidateResponse result = await _service.CrossValidateGraphsAsync();
        return Ok(result);
    }
}
