using System.Text.Json;
using Confluent.Kafka;

namespace Risk.Worker.Producers;

public sealed class RiskEvaluationResponseProducer
{
    private readonly IConfiguration _configuration;

    public RiskEvaluationResponseProducer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ProduceAsync(Guid externalOperationId, string status, CancellationToken cancellationToken)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"]
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        var payload = JsonSerializer.Serialize(new
        {
            externalOperationId,
            status
        });

        await producer.ProduceAsync(
            _configuration["Kafka:RiskEvaluationResponseTopic"],
            new Message<string, string> { Key = externalOperationId.ToString(), Value = payload },
            cancellationToken);
    }
}
