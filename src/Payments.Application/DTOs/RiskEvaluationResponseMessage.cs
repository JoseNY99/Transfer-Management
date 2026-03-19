namespace Payments.Application.DTOs;

public sealed class RiskEvaluationResponseMessage
{
    public Guid ExternalOperationId { get; set; }
    public string Status { get; set; } = default!;
}
