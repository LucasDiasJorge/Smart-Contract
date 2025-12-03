namespace Smart_Contract.Models;

/// <summary>
/// Response padrão para operações
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

/// <summary>
/// Response para verificação de grafo
/// </summary>
public class VerifyGraphResponse
{
    public bool Success { get; set; }
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();
}

/// <summary>
/// Response para validação cruzada
/// </summary>
public class CrossValidateResponse
{
    public bool Success { get; set; }
    public bool AllValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, bool> GraphStatuses { get; set; } = new Dictionary<string, bool>();
}
