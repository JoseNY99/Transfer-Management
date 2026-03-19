namespace Payments.Application.DTOs;

public sealed class PaymentStatusResponse
{
    public Guid ExternalOperationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = default!;
}
