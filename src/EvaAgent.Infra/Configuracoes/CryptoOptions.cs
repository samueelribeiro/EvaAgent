namespace EvaAgent.Infra.Configuracoes;

public class CryptoOptions
{
    public const string SectionName = "Crypto";

    public string Key { get; set; } = string.Empty;
    public string IV { get; set; } = string.Empty;
}
