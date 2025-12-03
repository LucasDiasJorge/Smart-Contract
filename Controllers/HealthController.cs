using Microsoft.AspNetCore.Mvc;
using Smart_Contract.Services;

namespace Smart_Contract.Controllers;

[ApiController]
[Route("api/grpc")]
public class HealthController : ControllerBase
{
    private readonly BlockchainGrpcClient _grpcClient;
    private readonly ILogger<HealthController> _logger;

    public HealthController(BlockchainGrpcClient grpcClient, ILogger<HealthController> logger)
    {
        _grpcClient = grpcClient;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealthAsync()
    {
        _logger.LogInformation("HTTP GET /api/grpc/health");
        bool isAvailable = await _grpcClient.IsServerAvailableAsync();
        return Ok(new { success = isAvailable });
    }
}
