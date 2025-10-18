using System.Text.RegularExpressions;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Entidades.LGPD;

namespace EvaAgent.Infra.Servicos.LGPD;

public class PseudonimizadorService : IPseudonimizadorService
{
    private readonly IRepositorioBase<RegistroPseudonimizacao> _repositorio;
    private readonly ICryptoService _cryptoService;

    // Regex patterns para detecção de dados sensíveis
    private static readonly Regex CpfRegex = new(@"\b\d{3}\.?\d{3}\.?\d{3}-?\d{2}\b", RegexOptions.Compiled);
    private static readonly Regex CnpjRegex = new(@"\b\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2}\b", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled);
    private static readonly Regex TelefoneRegex = new(@"\b(?:\+55\s?)?(?:\(?\d{2}\)?\s?)?\d{4,5}-?\d{4}\b", RegexOptions.Compiled);

    // Pattern simples para nomes próprios (maiúscula seguida de minúsculas)
    private static readonly Regex NomeRegex = new(@"\b[A-ZÀ-Ú][a-zà-ú]+(?:\s[A-ZÀ-Ú][a-zà-ú]+)+\b", RegexOptions.Compiled);

    public PseudonimizadorService(
        IRepositorioBase<RegistroPseudonimizacao> repositorio,
        ICryptoService cryptoService)
    {
        _repositorio = repositorio;
        _cryptoService = cryptoService;
    }

    public async Task<string> PseudonimizarAsync(
        string texto,
        Guid? conversaId = null,
        Guid? solicitacaoIAId = null)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return texto;

        var textoPseudonimizado = texto;
        var dadosSensiveis = new List<(string Valor, string Tipo)>();

        // Detectar CPF
        foreach (Match match in CpfRegex.Matches(texto))
        {
            dadosSensiveis.Add((match.Value, "CPF"));
        }

        // Detectar CNPJ
        foreach (Match match in CnpjRegex.Matches(texto))
        {
            dadosSensiveis.Add((match.Value, "CNPJ"));
        }

        // Detectar Email
        foreach (Match match in EmailRegex.Matches(texto))
        {
            dadosSensiveis.Add((match.Value, "Email"));
        }

        // Detectar Telefone
        foreach (Match match in TelefoneRegex.Matches(texto))
        {
            dadosSensiveis.Add((match.Value, "Telefone"));
        }

        // Detectar Nomes (heurística simples)
        foreach (Match match in NomeRegex.Matches(texto))
        {
            // Filtra palavras comuns que não são nomes
            if (!match.Value.Contains("Brasil") &&
                !match.Value.Contains("José Silva") && // Exemplo de exclusão
                match.Value.Split().Length >= 2) // Pelo menos nome e sobrenome
            {
                dadosSensiveis.Add((match.Value, "Nome"));
            }
        }

        // Pseudonimizar cada dado sensível
        foreach (var (valor, tipo) in dadosSensiveis.Distinct())
        {
            var guid = Guid.NewGuid();
            var valorCifrado = _cryptoService.Criptografar(valor);
            var valorHash = _cryptoService.GerarHash(valor);

            var registro = new RegistroPseudonimizacao
            {
                Guid = guid,
                ValorOriginalHash = valorHash,
                ValorCifrado = valorCifrado,
                TipoDado = tipo,
                ConversaId = conversaId,
                SolicitacaoIAId = solicitacaoIAId,
                PseudonimizadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddHours(24) // Expira em 24h
            };

            await _repositorio.AdicionarAsync(registro);

            // Substituir no texto
            textoPseudonimizado = textoPseudonimizado.Replace(valor, $"{{{guid}}}");
        }

        return textoPseudonimizado;
    }

    public async Task<string> ReverterPseudonimizacaoAsync(
        string textoPseudonimizado,
        Guid? conversaId = null,
        Guid? solicitacaoIAId = null)
    {
        if (string.IsNullOrWhiteSpace(textoPseudonimizado))
            return textoPseudonimizado;

        var textoRevertido = textoPseudonimizado;

        // Encontrar todos os GUIDs no texto
        var guidPattern = new Regex(@"\{([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\}");
        var matches = guidPattern.Matches(textoPseudonimizado);

        foreach (Match match in matches)
        {
            var guidStr = match.Groups[1].Value;
            if (Guid.TryParse(guidStr, out var guid))
            {
                // Buscar registro
                var registros = await _repositorio.BuscarAsync(r => r.Guid == guid);
                var registro = registros.FirstOrDefault();

                if (registro != null)
                {
                    var valorOriginal = _cryptoService.Descriptografar(registro.ValorCifrado);
                    textoRevertido = textoRevertido.Replace($"{{{guid}}}", valorOriginal);

                    // Marcar como revertido
                    registro.RevertidoEm = DateTime.UtcNow;
                    await _repositorio.AtualizarAsync(registro);
                }
            }
        }

        return textoRevertido;
    }

    public async Task<Dictionary<string, string>> ObterMapaPseudonimizacaoAsync(
        Guid? conversaId = null,
        Guid? solicitacaoIAId = null)
    {
        var query = await _repositorio.BuscarAsync(r =>
            (conversaId == null || r.ConversaId == conversaId) &&
            (solicitacaoIAId == null || r.SolicitacaoIAId == solicitacaoIAId));

        var mapa = new Dictionary<string, string>();

        foreach (var registro in query)
        {
            var valorOriginal = _cryptoService.Descriptografar(registro.ValorCifrado);
            mapa[registro.Guid.ToString()] = valorOriginal;
        }

        return mapa;
    }

    public async Task LimparPseudonimizacoesExpiradasAsync()
    {
        var expirados = await _repositorio.BuscarAsync(r =>
            r.ExpiraEm.HasValue && r.ExpiraEm.Value < DateTime.UtcNow);

        foreach (var registro in expirados)
        {
            await _repositorio.RemoverAsync(registro.Id);
        }
    }
}
