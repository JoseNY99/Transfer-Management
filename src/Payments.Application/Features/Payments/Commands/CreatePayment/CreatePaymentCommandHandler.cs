using MediatR;
using Payments.Application.Abstractions.Clock;
using Payments.Application.Abstractions.Messaging;
using Payments.Application.Abstractions.Persistence;
using Payments.Application.Abstractions.UnitOfWork;
using Payments.Application.DTOs;
using Payments.Domain.Entities;

namespace Payments.Application.Features.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRiskEvaluationRequestProducer _producer;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IRiskEvaluationRequestProducer producer)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _producer = producer;
    }

    public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment(
            request.CustomerId,
            request.ServiceProviderId,
            request.PaymentMethodId,
            request.Amount,
            _dateTimeProvider.UtcNow);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _producer.ProduceAsync(new RiskEvaluationRequestMessage
        {
            ExternalOperationId = payment.ExternalOperationId,
            CustomerId = payment.CustomerId,
            Amount = payment.Amount
        }, cancellationToken);

        return new CreatePaymentResponse
        {
            ExternalOperationId = payment.ExternalOperationId,
            Status = payment.Status.ToString().ToLowerInvariant(),
            CreatedAt = payment.CreatedAt
        };
    }
}
