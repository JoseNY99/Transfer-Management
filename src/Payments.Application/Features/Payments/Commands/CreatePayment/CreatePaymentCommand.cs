using MediatR;
using Payments.Application.DTOs;

namespace Payments.Application.Features.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid CustomerId,
    Guid ServiceProviderId,
    int PaymentMethodId,
    decimal Amount) : IRequest<CreatePaymentResponse>;
