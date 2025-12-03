using Microsoft.AspNetCore.Mvc;
using Smart_Contract.Models;
using Smart_Contract.Services;

namespace Smart_Contract.Controllers;

[ApiController]
[Route("api/graphs/{graphId}/blocks")]
public class BlocksController : ControllerBase
{
    private readonly BlockchainService _service;
    private readonly ILogger<BlocksController> _logger;

    public BlocksController(BlockchainService service, ILogger<BlocksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Block>>> AddBlock(string graphId, [FromBody] AddBlockRequest request)
    {
        _logger.LogInformation("HTTP POST /api/graphs/{GraphId}/blocks", graphId);
        request.GraphId = graphId;
        ApiResponse<Block> result = await _service.AddBlockAsync(request);
        return result.Success
            ? Created($"/api/graphs/{graphId}/blocks/{result.Data?.Hash}", result)
            : BadRequest(result);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<ApiResponse<Block>>> GetLatestBlock(string graphId)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}/blocks/latest", graphId);
        ApiResponse<Block> result = await _service.GetLatestBlockAsync(graphId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<ApiResponse<Block>>> GetBlock(string graphId, string hash)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}/blocks/{Hash}", graphId, hash);
        ApiResponse<Block> result = await _service.GetBlockAsync(graphId, hash);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("range")]
    public async Task<ActionResult<ApiResponse<List<Block>>>> GetBlockRange(string graphId, [FromQuery] ulong startHeight, [FromQuery] ulong endHeight)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}/blocks/range - {Start}-{End}", graphId, startHeight, endHeight);
        ApiResponse<List<Block>> result = await _service.GetBlockRangeAsync(graphId, startHeight, endHeight);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
