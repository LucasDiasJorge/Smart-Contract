namespace Smart_Contract.Models;

/// <summary>
/// Representa um contato armazenado no bloco
/// </summary>
public class Contact
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public long CreatedAt { get; set; }
}
