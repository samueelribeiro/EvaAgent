using System.Security.Cryptography;
using System.Text;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Infra.Configuracoes;
using Microsoft.Extensions.Options;

namespace EvaAgent.Infra.Servicos.LGPD;

public class CryptoService : ICryptoService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public CryptoService(IOptions<CryptoOptions> options)
    {
        var cryptoOptions = options.Value;

        // Converte as chaves de string para bytes
        _key = Encoding.UTF8.GetBytes(cryptoOptions.Key.PadRight(32).Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes(cryptoOptions.IV.PadRight(16).Substring(0, 16));

        if (_key.Length != 32) // AES-256
            throw new ArgumentException("A chave deve ter 256 bits (32 bytes)");

        if (_iv.Length != 16) // AES block size
            throw new ArgumentException("O IV deve ter 128 bits (16 bytes)");
    }

    // Construtor alternativo para testes (aceita strings diretamente)
    public CryptoService(string key, string iv)
    {
        _key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16));

        if (_key.Length != 32) // AES-256
            throw new ArgumentException("A chave deve ter 256 bits (32 bytes)");

        if (_iv.Length != 16) // AES block size
            throw new ArgumentException("O IV deve ter 128 bits (16 bytes)");
    }

    public string Criptografar(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var textoBytes = Encoding.UTF8.GetBytes(texto);
        var cifrado = encryptor.TransformFinalBlock(textoBytes, 0, textoBytes.Length);

        return Convert.ToBase64String(cifrado);
    }

    public string Descriptografar(string textoCifrado)
    {
        if (string.IsNullOrEmpty(textoCifrado))
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var cifradoBytes = Convert.FromBase64String(textoCifrado);
        var decifrado = decryptor.TransformFinalBlock(cifradoBytes, 0, cifradoBytes.Length);

        return Encoding.UTF8.GetString(decifrado);
    }

    public string GerarHash(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var textoBytes = Encoding.UTF8.GetBytes(texto);
        var hashBytes = sha256.ComputeHash(textoBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerificarHash(string texto, string hash)
    {
        var hashCalculado = GerarHash(texto);
        return hashCalculado == hash;
    }
}
