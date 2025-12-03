namespace Smart_Contract.Models;

/// <summary>
/// Request para adicionar um bloco com contato
/// </summary>
public class AddBlockRequest
{
    public string GraphId { get; set; } = string.Empty;
    public Contact Contact { get; set; } = new Contact();
    public List<string> CrossReferences { get; set; } = new List<string>();
}

/// <summary>
/// Request para criar um novo grafo
/// </summary>
public class CreateGraphRequest
{
    public string GraphId { get; set; } = string.Empty;
    public GraphType GraphType { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Request para obter blocos por range
/// </summary>
public class GetBlockRangeRequest
{
    public string GraphId { get; set; } = string.Empty;
    public ulong StartHeight { get; set; }
    public ulong EndHeight { get; set; }
}
