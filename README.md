# Smart-Contract — Blockchain de Contatos (POC)

API REST que atua como *proxy* para um serviço gRPC de blockchain (definido por `blockchain.proto`), armazenando blocos de contatos. Projeto simples para POC com logs informativos e mapeamento entre tipos Protobuf e modelos locais.

**Principais recursos**
- Endpoints REST que chamam um backend gRPC (configurável) seguindo `blockchain.proto`.
- Cliente gRPC encapsulado em `Services/BlockchainGrpcClient.cs`.
- Serviço central `Services/BlockchainService.cs` que delega operações ao backend gRPC.
- Arquivo de testes HTTP: `Smart-Contract.http` (para VS Code REST Client ou similares).
- Swagger/OpenAPI ativado em modo Development.

## Pré-requisitos
- .NET 10 SDK (versão de preview usada no projeto)
- Servidor gRPC compatível rodando (por padrão `http://localhost:8888`)
- (Opcional) VS Code com extensão REST Client para usar `Smart-Contract.http`

## Configuração
Edite o endereço do servidor gRPC em `appsettings.json` (ou `appsettings.Development.json`):

```json
"GrpcSettings": {
  "BlockchainServerAddress": "http://localhost:8888"
}
```

## Como rodar (PowerShell)
1. Restaurar e compilar:

```powershell
cd "..\Smart-Contract"
dotnet build
```

2. Executar a API:

```powershell
dotnet run
```

Por padrão a aplicação escuta em `http://localhost:5258` (veja logs no terminal).

## Swagger / OpenAPI
Enquanto `ASPNETCORE_ENVIRONMENT` estiver em `Development`, a aplicação registra o OpenAPI e expõe a UI. Acesse:

- Swagger UI (se em Development): `http://localhost:5258/openapi` (mapOpenApi usado no projeto)

> Se preferir abrir automaticamente o Swagger, execute a aplicação no ambiente Development e abra o navegador no endereço acima.

## Endpoints REST (resumo)
A API REST é uma camada sobre o backend gRPC. Rotas principais:

- `GET /api/grpc/health` — Verifica disponibilidade do servidor gRPC configurado
- `POST /api/graphs` — Criar novo grafo (chama gRPC CreateGraph)
- `GET /api/graphs` — Listar grafos (chama gRPC ListGraphs)
- `GET /api/graphs/{graphId}` — Obter info do grafo (gRPC GetGraphInfo)
- `GET /api/graphs/{graphId}/verify` — Verificar integridade do grafo (gRPC VerifyGraph)
- `GET /api/graphs/validate/all` — Validar todos os grafos (gRPC CrossValidateGraphs)
- `POST /api/graphs/{graphId}/blocks` — Adicionar bloco (contato) (gRPC AddBlock)
- `GET /api/graphs/{graphId}/blocks/latest` — Último bloco (gRPC GetLatestBlock)
- `GET /api/graphs/{graphId}/blocks/{hash}` — Obter bloco por hash (gRPC GetBlock)
- `GET /api/graphs/{graphId}/blocks/range?startHeight=X&endHeight=Y` — Blocos por range (gRPC GetBlockRange)
- `GET /api/graphs/{graphId}/contacts` — Listar contatos (usa GetBlockRange internamente)
- `GET /api/graphs/{graphId}/contacts/{hash}` — Contato a partir do hash do bloco

Consulte `Smart-Contract.http` para exemplos prontos de requisições.

## Testando com `Smart-Contract.http`
- Abra `Smart-Contract.http` no VS Code (com REST Client) e execute as requisições na ordem desejada.
- Primeiro crie um grafo (`POST /api/graphs`), depois adicione blocos e consulte.

## gRPC / Protobuf

Protobuf (Protocol Buffers) é um mecanismo de serialização de dados eficiente, multiplataforma e de código aberto do Google, usado para estruturar dados de forma compacta e rápida para comunicação entre sistemas (RPC, gRPC) ou armazenamento, definindo a estrutura em arquivos .proto, compilando-os para gerar código em diversas linguagens e trocando dados em formato binário, muito mais leve e rápido que XML ou JSON. 

O arquivo `blockchain.proto` contém a definição do serviço e mensagens. O projeto está configurado para gerar classes cliente a partir do `.proto` (veja `Smart-Contract.csproj` com `Protobuf Include="blockchain.proto" GrpcServices="Client"`).

Se modificar o `.proto`, será necessário reconstruir o projeto para regenerar as classes.

## Logs
- A aplicação usa logging por console com nível `Information` por padrão.
- O serviço `BlockchainService` e o cliente `BlockchainGrpcClient` gravam logs informativos e erros para facilitar debug.

## Dicas e troubleshooting
- Se o gRPC backend não estiver disponível, os endpoints REST retornarão erro/Problem com mensagem do erro.
- Valide a `GrpcSettings.BlockchainServerAddress` se conectar a um servidor remoto ou em porta diferente.
- Para desenvolvimento local, defina `ASPNETCORE_ENVIRONMENT=Development` antes de `dotnet run` para habilitar OpenAPI:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run
```


## Próximos passos sugeridos
- Opcional: migrar rotas do `Program.cs` para controllers na pasta `Controllers` (se desejar organização por MVC).
- Adicionar testes automatizados (unit/integration) e CI.
- Adicionar autenticação/SSL para calls gRPC em produção.

---
