namespace Payments.Application.DTOs;

public sealed class CreatePaymentRequest
{
    public Guid CustomerId { get; set; }
    public Guid ServiceProviderId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
}
