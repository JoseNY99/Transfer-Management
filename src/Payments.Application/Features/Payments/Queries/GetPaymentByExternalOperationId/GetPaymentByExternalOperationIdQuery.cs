using MediatR;
using Payments.Application.DTOs;

namespace Payments.Application.Features.Payments.Queries.GetPaymentByExternalOperationId;

public sealed record GetPaymentByExternalOperationIdQuery(Guid ExternalOperationId) : IRequest<PaymentStatusResponse?>;
