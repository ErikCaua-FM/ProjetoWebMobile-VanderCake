using VanderCake.Models;

namespace VanderCake.Contracts;

public class LoginResposta
{
    public string AccessToken { get; set; } = "";
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public Vendedor Vendedor { get; set; } = new();
}
