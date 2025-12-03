using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Smart_Contract.Models;

/// <summary>
/// Representa um bloco na blockchain de contatos
/// </summary>
public class Block
{
    public string Hash { get; set; } = string.Empty;
    public string PreviousHash { get; set; } = string.Empty;
    public long Timestamp { get; set; }
    public ulong Nonce { get; set; }
    public string Data { get; set; } = string.Empty;
    public ulong Height { get; set; }
    public string GraphId { get; set; } = string.Empty;
    public List<string> CrossReferences { get; set; } = new List<string>();

    /// <summary>
    /// Calcula o hash do bloco usando SHA256
    /// </summary>
    public string CalculateHash()
    {
        string rawData = $"{PreviousHash}{Timestamp}{Nonce}{Data}{Height}{GraphId}";
        
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        
        StringBuilder builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        
        return builder.ToString();
    }
}
