using Microsoft.AspNetCore.Mvc;
using Smart_Contract.Models;
using Smart_Contract.Services;

namespace Smart_Contract.Controllers;

[ApiController]
[Route("api/graphs/{graphId}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly BlockchainService _service;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(BlockchainService service, ILogger<ContactsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Contact>>>> ListContacts(string graphId)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}/contacts", graphId);
        ApiResponse<List<Contact>> result = await _service.ListContactsAsync(graphId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<ApiResponse<Contact>>> GetContact(string graphId, string hash)
    {
        _logger.LogInformation("HTTP GET /api/graphs/{GraphId}/contacts/{Hash}", graphId, hash);
        ApiResponse<Contact> result = await _service.GetContactFromBlockAsync(graphId, hash);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
