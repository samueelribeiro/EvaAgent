namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface ICryptoService
{
    string Criptografar(string texto);
    string Descriptografar(string textoCifrado);
    string GerarHash(string texto);
    bool VerificarHash(string texto, string hash);
}
