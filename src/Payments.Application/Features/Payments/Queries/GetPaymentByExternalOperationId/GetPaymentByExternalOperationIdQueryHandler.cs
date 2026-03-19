using MediatR;
using Payments.Application.Abstractions.Persistence;
using Payments.Application.DTOs;

namespace Payments.Application.Features.Payments.Queries.GetPaymentByExternalOperationId;

public sealed class GetPaymentByExternalOperationIdQueryHandler : IRequestHandler<GetPaymentByExternalOperationIdQuery, PaymentStatusResponse?>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByExternalOperationIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentStatusResponse?> Handle(GetPaymentByExternalOperationIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByExternalOperationIdAsync(request.ExternalOperationId, cancellationToken);
        if (payment is null) return null;

        return new PaymentStatusResponse
        {
            ExternalOperationId = payment.ExternalOperationId,
            CreatedAt = payment.CreatedAt,
            Status = payment.Status.ToString().ToLowerInvariant()
        };
    }
}
