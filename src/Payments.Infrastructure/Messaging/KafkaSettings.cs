namespace Payments.Infrastructure.Messaging;

public sealed class KafkaSettings
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = default!;
    public string RiskEvaluationRequestTopic { get; set; } = default!;
    public string RiskEvaluationResponseTopic { get; set; } = default!;
    public string ConsumerGroupId { get; set; } = default!;
}
