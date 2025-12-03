using Microsoft.OpenApi.Models;
using Smart_Contract.Services;

namespace Smart_Contract;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Configura logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // Registra serviços
        builder.Services.AddSingleton<BlockchainGrpcClient>();
        builder.Services.AddSingleton<BlockchainService>();

        // Controllers + Swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Blockchain Contacts API",
                Version = "v1",
                Description = "REST facade for the blockchain gRPC service"
            });
        });

        WebApplication app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blockchain Contacts API v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
