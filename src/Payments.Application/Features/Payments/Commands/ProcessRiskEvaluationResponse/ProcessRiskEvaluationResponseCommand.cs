using MediatR;

namespace Payments.Application.Features.Payments.Commands.ProcessRiskEvaluationResponse;

public sealed record ProcessRiskEvaluationResponseCommand(
    Guid ExternalOperationId,
    string Status) : IRequest;