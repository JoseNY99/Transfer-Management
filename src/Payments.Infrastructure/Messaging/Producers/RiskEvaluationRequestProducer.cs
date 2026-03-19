using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Payments.Application.Abstractions.Messaging;
using Payments.Application.DTOs;

namespace Payments.Infrastructure.Messaging.Producers;

public sealed class RiskEvaluationRequestProducer : IRiskEvaluationRequestProducer
{
    private readonly KafkaSettings _settings;

    public RiskEvaluationRequestProducer(IOptions<KafkaSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task ProduceAsync(RiskEvaluationRequestMessage message, CancellationToken cancellationToken)
    {
        var config = new ProducerConfig { BootstrapServers = _settings.BootstrapServers };
        using var producer = new ProducerBuilder<string, string>(config).Build();

        var payload = JsonSerializer.Serialize(message);

        await producer.ProduceAsync(
            _settings.RiskEvaluationRequestTopic,
            new Message<string, string> { Key = message.ExternalOperationId.ToString(), Value = payload },
            cancellationToken);
    }
}
