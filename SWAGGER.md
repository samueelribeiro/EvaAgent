# Documentação Swagger - EvaAgent API

## 📚 Visão Geral

O Swagger (OpenAPI) foi totalmente implementado para documentar a API do EvaAgent, fornecendo uma interface interativa para explorar e testar todos os endpoints disponíveis.

## 🚀 Acessar o Swagger

### URL de Acesso

Após iniciar a aplicação, o Swagger está disponível em:

```
http://localhost:5000/swagger
```

ou

```
https://localhost:5001/swagger
```

### Interface Swagger UI

A interface do Swagger UI fornece:
- ✅ Lista de todos os endpoints organizados por tags
- ✅ Documentação detalhada de cada endpoint
- ✅ Esquemas de request/response
- ✅ Exemplos de requisição
- ✅ Botão "Try it out" para testar diretamente
- ✅ Validação de entrada
- ✅ Códigos de resposta HTTP

## 📋 Endpoints Documentados

### 1. Health (Monitoramento)

#### `GET /api/Health`
- **Descrição**: Verifica status geral de saúde da aplicação
- **Retorno**: Status, timestamp, versão e conectividade do banco
- **Códigos HTTP**:
  - `200`: Aplicação saudável
  - `503`: Aplicação com problemas

#### `GET /api/Health/ready`
- **Descrição**: Readiness probe (Kubernetes)
- **Retorno**: Status de prontidão

#### `GET /api/Health/live`
- **Descrição**: Liveness probe (Kubernetes)
- **Retorno**: Status de vida

### 2. Webhook (Recepção de Mensagens)

#### `POST /api/Webhook/{espacoId}/mensagem`
- **Descrição**: Recebe mensagens de canais externos
- **Parâmetros**:
  - `espacoId` (path): ID do espaço (tenant)
  - `mensagem` (body): Dados da mensagem
- **Fluxo**:
  1. Identifica/cria receptor
  2. Cria/recupera conversa
  3. Pseudonimiza dados (LGPD)
  4. Resolve intenção
  5. Processa com agente
  6. Reverte pseudonimização
  7. Retorna resposta
- **Códigos HTTP**:
  - `200`: Sucesso
  - `400`: Erro no processamento

**Exemplo de Request**:
```json
{
  "canalTipo": "WhatsApp",
  "remetenteIdentificador": "5511999999999",
  "remetenteNome": "João Silva",
  "conteudo": "Qual foi o valor das vendas de hoje?",
  "recebidaEm": "2025-10-18T14:30:00Z"
}
```

#### `GET /api/Webhook/whatsapp/verify`
- **Descrição**: Verificação de webhook WhatsApp Business
- **Parâmetros**:
  - `hub_mode`: Modo do hub (deve ser "subscribe")
  - `hub_verify_token`: Token de verificação
  - `hub_challenge`: Challenge do Facebook
- **Códigos HTTP**:
  - `200`: Verificação bem-sucedida
  - `403`: Token inválido

#### `POST /api/Webhook/whatsapp/{espacoId}`
- **Descrição**: Recebe mensagens do WhatsApp Business API
- **Parâmetros**:
  - `espacoId` (path): ID do espaço
  - `payload` (body): Payload do WhatsApp

## 🔧 Configuração Implementada

### 1. Pacotes NuGet Adicionados

```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
<PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="7.2.0" />
```

### 2. Geração de XML Comments

Habilitado no `.csproj`:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

### 3. Configuração no Program.cs

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "EvaAgent API",
        Description = "API do Agente Multissistema - Plataforma Orquestradora de IA com conformidade LGPD",
        Contact = new OpenApiContact
        {
            Name = "Equipe EvaAgent",
            Email = "contato@evaagent.local"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Habilitar anotações
    options.EnableAnnotations();

    // Segurança JWT (para uso futuro)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```

### 4. Swagger UI Configurado

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api-docs/v1/swagger.json", "EvaAgent API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "EvaAgent API - Documentação";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
    options.EnableValidator();
});
```

## 📝 Anotações Swagger Utilizadas

### Controllers

```csharp
/// <summary>
/// Descrição do controller
/// </summary>
[SwaggerTag("Descrição da tag")]
public class MeuController : ControllerBase
```

### Endpoints

```csharp
/// <summary>
/// Descrição breve
/// </summary>
/// <remarks>
/// Descrição detalhada com markdown
/// </remarks>
/// <param name="parametro">Descrição do parâmetro</param>
/// <returns>Descrição do retorno</returns>
/// <response code="200">Descrição do código 200</response>
[HttpGet]
[SwaggerOperation(
    Summary = "Resumo",
    Description = "Descrição detalhada",
    OperationId = "NomeUnico",
    Tags = new[] { "Tag1", "Tag2" }
)]
[SwaggerResponse(200, "Sucesso", typeof(MeuDto))]
[SwaggerResponse(400, "Erro", typeof(ErrorDto))]
public IActionResult MeuEndpoint(
    [FromRoute, SwaggerParameter("Descrição", Required = true)] Guid id,
    [FromBody, SwaggerRequestBody("Descrição", Required = true)] MeuDto dto)
```

### Modelos (DTOs)

```csharp
/// <summary>
/// Descrição do modelo
/// </summary>
public class MeuDto
{
    /// <summary>Descrição da propriedade</summary>
    /// <example>Valor de exemplo</example>
    public string Propriedade { get; set; }
}
```

## 🎯 Funcionalidades do Swagger UI

### 1. Try It Out
- Clique em "Try it out" em qualquer endpoint
- Preencha os parâmetros
- Clique em "Execute"
- Veja a resposta real da API

### 2. Modelos (Schemas)
- Visualize a estrutura de todos os DTOs
- Veja tipos de dados, obrigatoriedade e exemplos
- Copie exemplos para suas requisições

### 3. Autenticação (Futuro)
- Clique em "Authorize" no topo
- Insira seu token JWT
- Todas as requisições incluirão o token

### 4. Download da Especificação
- Baixe a especificação OpenAPI em JSON
- Use em ferramentas como Postman, Insomnia
- Gere código cliente automaticamente

## 📥 Exportar Especificação OpenAPI

### JSON
```
http://localhost:5000/api-docs/v1/swagger.json
```

### Uso com Outras Ferramentas

#### Postman
1. Abra Postman
2. Import > Link
3. Cole: `http://localhost:5000/api-docs/v1/swagger.json`

#### Insomnia
1. Abra Insomnia
2. Create > Import
3. Cole a URL do swagger.json

#### Geração de Cliente
```bash
# Instalar ferramenta
npm install -g @openapitools/openapi-generator-cli

# Gerar cliente C#
openapi-generator-cli generate \
  -i http://localhost:5000/api-docs/v1/swagger.json \
  -g csharp \
  -o ./cliente-gerado

# Gerar cliente TypeScript
openapi-generator-cli generate \
  -i http://localhost:5000/api-docs/v1/swagger.json \
  -g typescript-axios \
  -o ./cliente-ts
```

## 🔐 Segurança no Swagger

### Ambiente de Desenvolvimento
- Swagger habilitado por padrão
- Sem autenticação necessária
- Acesso local apenas

### Ambiente de Produção
Para desabilitar em produção, adicione:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

Ou proteja com autenticação:

```csharp
app.UseSwagger(options =>
{
    options.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        // Adicionar lógica de autenticação
    });
});
```

## 📊 Exemplos de Uso

### 1. Testar Health Check

1. Acesse `/swagger`
2. Expanda "Health"
3. Clique em `GET /api/Health`
4. Clique em "Try it out"
5. Clique em "Execute"
6. Veja a resposta

### 2. Enviar Mensagem via Webhook

1. Acesse `/swagger`
2. Expanda "Webhook"
3. Clique em `POST /api/Webhook/{espacoId}/mensagem`
4. Clique em "Try it out"
5. Preencha:
   - `espacoId`: Cole um GUID válido
   - Request body: Use o exemplo fornecido
6. Clique em "Execute"
7. Veja a resposta processada

## 🎨 Customizações Futuras

### Temas
```csharp
options.InjectStylesheet("/swagger-ui/custom.css");
```

### Logo Personalizado
```csharp
options.InjectJavascript("/swagger-ui/custom.js");
```

### Múltiplas Versões
```csharp
options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1.0.0" });
options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2.0.0" });
```

## 🐛 Troubleshooting

### Swagger não carrega
- Verifique se a aplicação está rodando
- Confirme a URL: `/swagger` (não `/swagger/index.html`)
- Verifique se `AddSwaggerGen()` está configurado

### XML Comments não aparecem
- Confirme `<GenerateDocumentationFile>true</GenerateDocumentationFile>` no `.csproj`
- Rebuild do projeto
- Verifique se o arquivo `.xml` está sendo gerado em `bin/Debug/net10.0/`

### Erros de validação
- Use `[Required]` para propriedades obrigatórias
- Use `[Range]` para validar intervalos
- Use `[StringLength]` para limitar tamanho

## 📚 Recursos Adicionais

- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Swagger UI Documentation](https://swagger.io/tools/swagger-ui/)

---

**Status**: ✅ Swagger totalmente implementado e funcional
**Versão**: v1.0.0
**Data**: 18 de Outubro de 2025
