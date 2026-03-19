using Payments.Application.DTOs;

namespace Payments.Application.Abstractions.Messaging;

public interface IRiskEvaluationRequestProducer
{
    Task ProduceAsync(RiskEvaluationRequestMessage message, CancellationToken cancellationToken);
}
