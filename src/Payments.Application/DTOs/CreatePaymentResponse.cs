namespace Payments.Application.DTOs;

public sealed class CreatePaymentResponse
{
    public Guid ExternalOperationId { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
