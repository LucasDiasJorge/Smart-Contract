using System.Text.Json;
using Smart_Contract.Models;

namespace Smart_Contract.Services;

/// <summary>
/// Serviço que abstrai o backend gRPC de blockchain de contatos
/// </summary>
public class BlockchainService
{
    private readonly ILogger<BlockchainService> _logger;
    private readonly BlockchainGrpcClient _grpcClient;

    public BlockchainService(ILogger<BlockchainService> logger, BlockchainGrpcClient grpcClient)
    {
        _logger = logger;
        _grpcClient = grpcClient;
    }

    /// <summary>
    /// Cria um novo grafo no backend gRPC
    /// </summary>
    public async Task<ApiResponse<GraphInfo>> CreateGraphAsync(CreateGraphRequest request)
    {
        _logger.LogInformation("[gRPC] Criando grafo: {GraphId}", request.GraphId);

        try
        {
            Blockchain.GraphType grpcType = MapToGrpcGraphType(request.GraphType);
            Blockchain.CreateGraphResponse response = await _grpcClient.CreateGraphAsync(
                request.GraphId,
                grpcType,
                request.Description);

            GraphInfo? graphInfo = null;
            if (response.Success)
            {
                graphInfo = await FetchGraphDetailsAsync(request.GraphId);
            }

            return new ApiResponse<GraphInfo>
            {
                Success = response.Success,
                Message = response.Message,
                Data = graphInfo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao criar grafo");
            return Failure<GraphInfo>("Falha ao criar grafo via gRPC");
        }
    }

    /// <summary>
    /// Adiciona um novo bloco com contato ao grafo remoto
    /// </summary>
    public async Task<ApiResponse<Block>> AddBlockAsync(AddBlockRequest request)
    {
        _logger.LogInformation("[gRPC] Adicionando bloco ao grafo: {GraphId}", request.GraphId);

        try
        {
            if (string.IsNullOrEmpty(request.Contact.Id))
            {
                request.Contact.Id = Guid.NewGuid().ToString();
            }
            request.Contact.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string contactData = JsonSerializer.Serialize(request.Contact);
            Blockchain.AddBlockResponse response = await _grpcClient.AddBlockAsync(
                request.GraphId,
                contactData,
                request.CrossReferences);

            return new ApiResponse<Block>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Block != null ? MapBlock(response.Block) : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao adicionar bloco");
            return Failure<Block>("Falha ao adicionar bloco via gRPC");
        }
    }

    /// <summary>
    /// Obtém um bloco por hash
    /// </summary>
    public async Task<ApiResponse<Block>> GetBlockAsync(string graphId, string hash)
    {
        _logger.LogInformation("[gRPC] Buscando bloco: GraphId={GraphId}, Hash={Hash}", graphId, hash);

        try
        {
            Blockchain.GetBlockResponse response = await _grpcClient.GetBlockAsync(graphId, hash);
            return new ApiResponse<Block>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Block != null ? MapBlock(response.Block) : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao buscar bloco");
            return Failure<Block>("Falha ao buscar bloco via gRPC");
        }
    }

    /// <summary>
    /// Obtém o último bloco de um grafo
    /// </summary>
    public async Task<ApiResponse<Block>> GetLatestBlockAsync(string graphId)
    {
        _logger.LogInformation("[gRPC] Buscando último bloco: GraphId={GraphId}", graphId);

        try
        {
            Blockchain.GetBlockResponse response = await _grpcClient.GetLatestBlockAsync(graphId);
            return new ApiResponse<Block>
            {
                Success = response.Success,
                Message = response.Message,
                Data = response.Block != null ? MapBlock(response.Block) : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao buscar último bloco");
            return Failure<Block>("Falha ao buscar último bloco via gRPC");
        }
    }

    /// <summary>
    /// Obtém informações detalhadas do grafo
    /// </summary>
    public async Task<ApiResponse<GraphInfo>> GetGraphInfoAsync(string graphId)
    {
        _logger.LogInformation("[gRPC] Buscando informações do grafo: {GraphId}", graphId);

        try
        {
            GraphInfo? info = await FetchGraphDetailsAsync(graphId);
            bool success = info != null;
            return new ApiResponse<GraphInfo>
            {
                Success = success,
                Message = success ? "Informações do grafo encontradas" : "Grafo não encontrado",
                Data = info
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao buscar info de grafo");
            return Failure<GraphInfo>("Falha ao buscar informações do grafo via gRPC");
        }
    }

    /// <summary>
    /// Lista todos os grafos cadastrados
    /// </summary>
    public async Task<ApiResponse<List<GraphInfo>>> ListGraphsAsync()
    {
        _logger.LogInformation("[gRPC] Listando grafos");

        try
        {
            Blockchain.ListGraphsResponse response = await _grpcClient.ListGraphsAsync();
            List<GraphInfo> data = response.Graphs
                .Select(MapGraphInfo)
                .ToList();

            return new ApiResponse<List<GraphInfo>>
            {
                Success = true,
                Message = $"Total de {data.Count} grafo(s) encontrado(s)",
                Data = data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao listar grafos");
            return Failure<List<GraphInfo>>("Falha ao listar grafos via gRPC");
        }
    }

    /// <summary>
    /// Verifica a integridade de um grafo
    /// </summary>
    public async Task<VerifyGraphResponse> VerifyGraphAsync(string graphId)
    {
        _logger.LogInformation("[gRPC] Verificando grafo: {GraphId}", graphId);

        try
        {
            Blockchain.VerifyGraphResponse response = await _grpcClient.VerifyGraphAsync(graphId);
            return new VerifyGraphResponse
            {
                Success = response.Success,
                IsValid = response.IsValid,
                Message = response.Message,
                Errors = response.Errors.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao verificar grafo");
            return new VerifyGraphResponse
            {
                Success = false,
                IsValid = false,
                Message = "Falha ao verificar grafo via gRPC"
            };
        }
    }

    /// <summary>
    /// Executa validação cruzada de todos os grafos
    /// </summary>
    public async Task<CrossValidateResponse> CrossValidateGraphsAsync()
    {
        _logger.LogInformation("[gRPC] Validando todos os grafos");

        try
        {
            Blockchain.CrossValidateResponse response = await _grpcClient.CrossValidateGraphsAsync();
            return new CrossValidateResponse
            {
                Success = response.Success,
                AllValid = response.AllValid,
                Message = response.Message,
                GraphStatuses = response.GraphStatuses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC na validação cruzada");
            return new CrossValidateResponse
            {
                Success = false,
                AllValid = false,
                Message = "Falha ao validar grafos via gRPC",
                GraphStatuses = new Dictionary<string, bool>()
            };
        }
    }

    /// <summary>
    /// Obtém blocos por range de altura
    /// </summary>
    public async Task<ApiResponse<List<Block>>> GetBlockRangeAsync(string graphId, ulong startHeight, ulong endHeight)
    {
        _logger.LogInformation("[gRPC] Buscando blocos: GraphId={GraphId}, Start={Start}, End={End}",
            graphId, startHeight, endHeight);

        try
        {
            Blockchain.GetBlockRangeResponse response = await _grpcClient.GetBlockRangeAsync(graphId, startHeight, endHeight);
            List<Block> blocks = response.Blocks.Select(MapBlock).ToList();

            return new ApiResponse<List<Block>>
            {
                Success = response.Success,
                Message = response.Success ? $"{blocks.Count} bloco(s) encontrado(s)" : "Falha ao buscar blocos",
                Data = response.Success ? blocks : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao buscar blocos por range");
            return Failure<List<Block>>("Falha ao buscar blocos via gRPC");
        }
    }

    /// <summary>
    /// Obtém o contato armazenado em um bloco específico
    /// </summary>
    public async Task<ApiResponse<Contact>> GetContactFromBlockAsync(string graphId, string hash)
    {
        _logger.LogInformation("[gRPC] Extraindo contato: GraphId={GraphId}, Hash={Hash}", graphId, hash);

        ApiResponse<Block> blockResponse = await GetBlockAsync(graphId, hash);
        if (!blockResponse.Success || blockResponse.Data == null)
        {
            return new ApiResponse<Contact>
            {
                Success = false,
                Message = blockResponse.Message
            };
        }

        try
        {
            Contact? contact = JsonSerializer.Deserialize<Contact>(blockResponse.Data.Data);
            if (contact == null)
            {
                return Failure<Contact>("Dados do contato inválidos");
            }

            return new ApiResponse<Contact>
            {
                Success = true,
                Message = "Contato encontrado",
                Data = contact
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar contato");
            return Failure<Contact>("Erro ao processar dados do contato");
        }
    }

    /// <summary>
    /// Lista todos os contatos gravados em um grafo
    /// </summary>
    public async Task<ApiResponse<List<Contact>>> ListContactsAsync(string graphId)
    {
        _logger.LogInformation("[gRPC] Listando contatos: GraphId={GraphId}", graphId);

        try
        {
            GraphInfo? info = await FetchGraphDetailsAsync(graphId);
            if (info == null)
            {
                return new ApiResponse<List<Contact>>
                {
                    Success = false,
                    Message = "Grafo não encontrado"
                };
            }

            ulong totalBlocks = info.TotalBlocks;
            if (totalBlocks <= 1)
            {
                return new ApiResponse<List<Contact>>
                {
                    Success = true,
                    Message = "Nenhum contato encontrado",
                    Data = new List<Contact>()
                };
            }

            Blockchain.GetBlockRangeResponse response = await _grpcClient.GetBlockRangeAsync(graphId, 1, totalBlocks);
            List<Contact> contacts = new List<Contact>();

            foreach (Blockchain.Block block in response.Blocks)
            {
                try
                {
                    Contact? contact = JsonSerializer.Deserialize<Contact>(block.Data);
                    if (contact != null)
                    {
                        contacts.Add(contact);
                    }
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Contato inválido no bloco: {Hash}", block.Hash);
                }
            }

            return new ApiResponse<List<Contact>>
            {
                Success = true,
                Message = $"{contacts.Count} contato(s) encontrado(s)",
                Data = contacts
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro gRPC ao listar contatos");
            return Failure<List<Contact>>("Falha ao listar contatos via gRPC");
        }
    }

    private async Task<GraphInfo?> FetchGraphDetailsAsync(string graphId)
    {
        Blockchain.GetGraphInfoResponse response = await _grpcClient.GetGraphInfoAsync(graphId);
        if (!response.Success)
        {
            return null;
        }

        return new GraphInfo
        {
            GraphId = response.GraphId,
            GraphType = MapFromGrpcGraphType(response.GraphType),
            TotalBlocks = response.TotalBlocks,
            Description = string.Empty,
            LatestHash = response.LatestHash,
            CreatedAt = response.CreatedAt,
            IsValid = response.IsValid
        };
    }

    private static GraphInfo MapGraphInfo(Blockchain.GraphInfo info)
    {
        return new GraphInfo
        {
            GraphId = info.GraphId,
            GraphType = MapFromGrpcGraphType(info.GraphType),
            TotalBlocks = info.TotalBlocks,
            Description = info.Description,
            LatestHash = string.Empty,
            CreatedAt = 0,
            IsValid = true
        };
    }

    private static Block MapBlock(Blockchain.Block block)
    {
        return new Block
        {
            Hash = block.Hash,
            PreviousHash = block.PreviousHash,
            Timestamp = block.Timestamp,
            Nonce = block.Nonce,
            Data = block.Data,
            Height = block.Height,
            GraphId = block.GraphId,
            CrossReferences = block.CrossReferences.ToList()
        };
    }

    private static Blockchain.GraphType MapToGrpcGraphType(GraphType type)
    {
        return type switch
        {
            GraphType.Transaction => Blockchain.GraphType.Transaction,
            GraphType.Identity => Blockchain.GraphType.Identity,
            GraphType.Asset => Blockchain.GraphType.Asset,
            GraphType.Audit => Blockchain.GraphType.Audit,
            GraphType.Custom => Blockchain.GraphType.Custom,
            _ => Blockchain.GraphType.Custom
        };
    }

    private static GraphType MapFromGrpcGraphType(Blockchain.GraphType type)
    {
        return type switch
        {
            Blockchain.GraphType.Transaction => GraphType.Transaction,
            Blockchain.GraphType.Identity => GraphType.Identity,
            Blockchain.GraphType.Asset => GraphType.Asset,
            Blockchain.GraphType.Audit => GraphType.Audit,
            Blockchain.GraphType.Custom => GraphType.Custom,
            _ => GraphType.Custom
        };
    }

    private static ApiResponse<T> Failure<T>(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}
