namespace Payments.Application.DTOs;

public sealed class RiskEvaluationRequestMessage
{
    public Guid ExternalOperationId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}
