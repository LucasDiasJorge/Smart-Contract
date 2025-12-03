using Grpc.Net.Client;
using Blockchain;

namespace Smart_Contract.Services;

/// <summary>
/// Cliente gRPC para comunicação com o serviço de blockchain externo
/// </summary>
public class BlockchainGrpcClient : IDisposable
{
    private readonly ILogger<BlockchainGrpcClient> _logger;
    private readonly GrpcChannel _channel;
    private readonly Blockchain.BlockchainService.BlockchainServiceClient _client;
    private readonly string _serverAddress;

    public BlockchainGrpcClient(ILogger<BlockchainGrpcClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _serverAddress = configuration.GetValue<string>("GrpcSettings:BlockchainServerAddress") ?? "http://localhost:50051";
        
        _logger.LogInformation("Inicializando cliente gRPC para: {ServerAddress}", _serverAddress);
        
        _channel = GrpcChannel.ForAddress(_serverAddress);
        _client = new Blockchain.BlockchainService.BlockchainServiceClient(_channel);
        
        _logger.LogInformation("Cliente gRPC inicializado com sucesso");
    }

    /// <summary>
    /// Cria um novo grafo no servidor blockchain
    /// </summary>
    public async Task<Blockchain.CreateGraphResponse> CreateGraphAsync(string graphId, Blockchain.GraphType graphType, string description)
    {
        _logger.LogInformation("gRPC: Criando grafo - GraphId={GraphId}, Type={GraphType}", graphId, graphType);

        Blockchain.CreateGraphRequest request = new Blockchain.CreateGraphRequest
        {
            GraphId = graphId,
            GraphType = graphType,
            Description = description
        };

        try
        {
            Blockchain.CreateGraphResponse response = await _client.CreateGraphAsync(request);
            _logger.LogInformation("gRPC: Grafo criado - Success={Success}, Message={Message}", response.Success, response.Message);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao criar grafo");
            throw;
        }
    }

    /// <summary>
    /// Adiciona um novo bloco ao grafo
    /// </summary>
    public async Task<Blockchain.AddBlockResponse> AddBlockAsync(string graphId, string data, List<string>? crossReferences = null)
    {
        _logger.LogInformation("gRPC: Adicionando bloco - GraphId={GraphId}", graphId);

        Blockchain.AddBlockRequest request = new Blockchain.AddBlockRequest
        {
            GraphId = graphId,
            Data = data
        };

        if (crossReferences != null)
        {
            request.CrossReferences.AddRange(crossReferences);
        }

        try
        {
            Blockchain.AddBlockResponse response = await _client.AddBlockAsync(request);
            _logger.LogInformation("gRPC: Bloco adicionado - Success={Success}, Hash={Hash}", 
                response.Success, response.Block?.Hash ?? "N/A");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao adicionar bloco");
            throw;
        }
    }

    /// <summary>
    /// Obtém um bloco pelo hash
    /// </summary>
    public async Task<Blockchain.GetBlockResponse> GetBlockAsync(string graphId, string hash)
    {
        _logger.LogInformation("gRPC: Obtendo bloco - GraphId={GraphId}, Hash={Hash}", graphId, hash);

        Blockchain.GetBlockRequest request = new Blockchain.GetBlockRequest
        {
            GraphId = graphId,
            Hash = hash
        };

        try
        {
            Blockchain.GetBlockResponse response = await _client.GetBlockAsync(request);
            _logger.LogInformation("gRPC: Bloco obtido - Success={Success}", response.Success);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao obter bloco");
            throw;
        }
    }

    /// <summary>
    /// Obtém o último bloco do grafo
    /// </summary>
    public async Task<Blockchain.GetBlockResponse> GetLatestBlockAsync(string graphId)
    {
        _logger.LogInformation("gRPC: Obtendo último bloco - GraphId={GraphId}", graphId);

        Blockchain.GetLatestBlockRequest request = new Blockchain.GetLatestBlockRequest
        {
            GraphId = graphId
        };

        try
        {
            Blockchain.GetBlockResponse response = await _client.GetLatestBlockAsync(request);
            _logger.LogInformation("gRPC: Último bloco obtido - Success={Success}, Hash={Hash}", 
                response.Success, response.Block?.Hash ?? "N/A");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao obter último bloco");
            throw;
        }
    }

    /// <summary>
    /// Obtém informações do grafo
    /// </summary>
    public async Task<Blockchain.GetGraphInfoResponse> GetGraphInfoAsync(string graphId)
    {
        _logger.LogInformation("gRPC: Obtendo info do grafo - GraphId={GraphId}", graphId);

        Blockchain.GetGraphInfoRequest request = new Blockchain.GetGraphInfoRequest
        {
            GraphId = graphId
        };

        try
        {
            Blockchain.GetGraphInfoResponse response = await _client.GetGraphInfoAsync(request);
            _logger.LogInformation("gRPC: Info do grafo obtida - Success={Success}, TotalBlocks={TotalBlocks}", 
                response.Success, response.TotalBlocks);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao obter info do grafo");
            throw;
        }
    }

    /// <summary>
    /// Lista todos os grafos
    /// </summary>
    public async Task<Blockchain.ListGraphsResponse> ListGraphsAsync()
    {
        _logger.LogInformation("gRPC: Listando grafos");

        Blockchain.ListGraphsRequest request = new Blockchain.ListGraphsRequest();

        try
        {
            Blockchain.ListGraphsResponse response = await _client.ListGraphsAsync(request);
            _logger.LogInformation("gRPC: Grafos listados - Total={Total}", response.Graphs.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao listar grafos");
            throw;
        }
    }

    /// <summary>
    /// Verifica a integridade do grafo
    /// </summary>
    public async Task<Blockchain.VerifyGraphResponse> VerifyGraphAsync(string graphId)
    {
        _logger.LogInformation("gRPC: Verificando grafo - GraphId={GraphId}", graphId);

        Blockchain.VerifyGraphRequest request = new Blockchain.VerifyGraphRequest
        {
            GraphId = graphId
        };

        try
        {
            Blockchain.VerifyGraphResponse response = await _client.VerifyGraphAsync(request);
            _logger.LogInformation("gRPC: Verificação concluída - IsValid={IsValid}", response.IsValid);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao verificar grafo");
            throw;
        }
    }

    /// <summary>
    /// Valida todos os grafos
    /// </summary>
    public async Task<Blockchain.CrossValidateResponse> CrossValidateGraphsAsync()
    {
        _logger.LogInformation("gRPC: Validando todos os grafos");

        Blockchain.CrossValidateRequest request = new Blockchain.CrossValidateRequest();

        try
        {
            Blockchain.CrossValidateResponse response = await _client.CrossValidateGraphsAsync(request);
            _logger.LogInformation("gRPC: Validação cruzada concluída - AllValid={AllValid}", response.AllValid);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro na validação cruzada");
            throw;
        }
    }

    /// <summary>
    /// Obtém blocos por range de altura
    /// </summary>
    public async Task<Blockchain.GetBlockRangeResponse> GetBlockRangeAsync(string graphId, ulong startHeight, ulong endHeight)
    {
        _logger.LogInformation("gRPC: Obtendo blocos por range - GraphId={GraphId}, Start={Start}, End={End}", 
            graphId, startHeight, endHeight);

        Blockchain.GetBlockRangeRequest request = new Blockchain.GetBlockRangeRequest
        {
            GraphId = graphId,
            StartHeight = startHeight,
            EndHeight = endHeight
        };

        try
        {
            Blockchain.GetBlockRangeResponse response = await _client.GetBlockRangeAsync(request);
            _logger.LogInformation("gRPC: Blocos obtidos - Total={Total}", response.Blocks.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Erro ao obter blocos por range");
            throw;
        }
    }

    /// <summary>
    /// Verifica se o servidor está acessível
    /// </summary>
    public async Task<bool> IsServerAvailableAsync()
    {
        _logger.LogInformation("gRPC: Verificando disponibilidade do servidor");

        try
        {
            await ListGraphsAsync();
            _logger.LogInformation("gRPC: Servidor disponível");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "gRPC: Servidor indisponível");
            return false;
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("gRPC: Encerrando conexão com o servidor");
        _channel.Dispose();
    }
}
