using Payments.Domain.Common;
using Payments.Domain.Enums;

namespace Payments.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid ExternalOperationId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ServiceProviderId { get; private set; }
    public int PaymentMethodId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }

    private Payment() { }

    public Payment(Guid customerId, Guid serviceProviderId, int paymentMethodId, decimal amount, DateTime createdAtUtc)
    {
        Id = Guid.NewGuid();
        ExternalOperationId = Guid.NewGuid();
        CustomerId = customerId;
        ServiceProviderId = serviceProviderId;
        PaymentMethodId = paymentMethodId;
        Amount = amount;
        Status = PaymentStatus.Evaluating;
        CreatedAt = createdAtUtc;
    }

    public void MarkAccepted(DateTime updatedAtUtc)
    {
        Status = PaymentStatus.Accepted;
        UpdatedAt = updatedAtUtc;
    }

    public void MarkDenied(DateTime updatedAtUtc)
    {
        Status = PaymentStatus.Denied;
        UpdatedAt = updatedAtUtc;
    }
}
