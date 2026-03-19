using MediatR;
using Payments.Application.Abstractions.Clock;
using Payments.Application.Abstractions.Persistence;
using Payments.Application.Abstractions.UnitOfWork;

namespace Payments.Application.Features.Payments.Commands.ProcessRiskEvaluationResponse;

public sealed class ProcessRiskEvaluationResponseCommandHandler : IRequestHandler<ProcessRiskEvaluationResponseCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ProcessRiskEvaluationResponseCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(ProcessRiskEvaluationResponseCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByExternalOperationIdAsync(
            request.ExternalOperationId,
            cancellationToken);

        if (payment is null)
            return;

        if (request.Status.Equals("accepted", StringComparison.OrdinalIgnoreCase))
            payment.MarkAccepted(_dateTimeProvider.UtcNow);
        else
            payment.MarkDenied(_dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}