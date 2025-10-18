using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Agentes;

public class AgenteFinanceiro : IAgenteEspecialista
{
    private readonly ILogger<AgenteFinanceiro> _logger;
    private readonly IProvedorIA _provedorIA;

    public string Nome => "Agente Financeiro";

    public AgenteFinanceiro(
        ILogger<AgenteFinanceiro> logger,
        IProvedorIA provedorIA)
    {
        _logger = logger;
        _provedorIA = provedorIA;
    }

    public async Task<string> ProcessarMensagemAsync(
        string mensagem,
        Guid conversaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processando mensagem financeira para conversa {ConversaId}",
            conversaId);

        var contexto = @"Você é um assistente financeiro especializado.
Você tem acesso a informações de vendas, boletos, contas a receber e pagar.
Responda de forma clara e objetiva sobre questões financeiras.
Se necessário, solicite mais informações para executar ações.";

        var resposta = await _provedorIA.GerarRespostaAsync(
            mensagem,
            contexto,
            cancellationToken);

        return resposta;
    }

    public Task<bool> PodeProcessarAsync(
        string mensagem,
        CancellationToken cancellationToken = default)
    {
        var palavrasChave = new[] {
            "venda", "vendas", "saldo", "financeiro", "boleto",
            "pagamento", "cobrança", "fatura", "receber", "pagar",
            "dinheiro", "valor", "custo", "receita", "despesa"
        };

        var mensagemLower = mensagem.ToLowerInvariant();
        var podeProcessar = palavrasChave.Any(p => mensagemLower.Contains(p));

        return Task.FromResult(podeProcessar);
    }
}
