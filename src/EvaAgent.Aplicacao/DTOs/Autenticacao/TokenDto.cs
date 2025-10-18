namespace EvaAgent.Aplicacao.DTOs.Autenticacao;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public string TipoToken { get; set; } = "Bearer";
    public DateTime Expiracao { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
