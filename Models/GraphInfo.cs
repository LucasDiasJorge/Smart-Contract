namespace Smart_Contract.Models;

/// <summary>
/// Tipos de grafos suportados
/// </summary>
public enum GraphType
{
    Transaction = 0,
    Identity = 1,
    Asset = 2,
    Audit = 3,
    Custom = 4
}

/// <summary>
/// Informações sobre um grafo da blockchain
/// </summary>
public class GraphInfo
{
    public string GraphId { get; set; } = string.Empty;
    public GraphType GraphType { get; set; }
    public ulong TotalBlocks { get; set; }
    public string Description { get; set; } = string.Empty;
    public string LatestHash { get; set; } = string.Empty;
    public long CreatedAt { get; set; }
    public bool IsValid { get; set; }
}
