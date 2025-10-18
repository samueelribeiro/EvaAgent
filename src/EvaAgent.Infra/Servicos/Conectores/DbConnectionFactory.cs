using System.Data;
using EvaAgent.Dominio.Enums;
using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using MySqlConnector;

namespace EvaAgent.Infra.Servicos.Conectores;

/// <summary>
/// Factory para criar conexões com diferentes bancos de dados
/// </summary>
public class DbConnectionFactory
{
    /// <summary>
    /// Cria uma conexão IDbConnection baseada no tipo de banco
    /// </summary>
    public IDbConnection CriarConexao(TipoBancoDados tipo, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string não pode ser vazia", nameof(connectionString));

        return tipo switch
        {
            TipoBancoDados.SqlServer => new SqlConnection(connectionString),
            TipoBancoDados.PostgreSQL => new NpgsqlConnection(connectionString),
            TipoBancoDados.Oracle => new OracleConnection(connectionString),
            TipoBancoDados.MySQL => new MySqlConnection(connectionString),
            _ => throw new NotSupportedException($"Tipo de banco '{tipo}' não é suportado")
        };
    }

    /// <summary>
    /// Testa se a conexão está funcionando
    /// </summary>
    public async Task<bool> TestarConexaoAsync(
        TipoBancoDados tipo,
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = CriarConexao(tipo, connectionString);
            await Task.Run(() => connection.Open(), cancellationToken);
            return connection.State == ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }
}
