using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.DTOs;
using Payments.Application.Features.Payments.Commands.CreatePayment;
using Payments.Application.Features.Payments.Queries.GetPaymentByExternalOperationId;

namespace Payments.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new CreatePaymentCommand(request.CustomerId, request.ServiceProviderId, request.PaymentMethodId, request.Amount),
            cancellationToken);

        return CreatedAtAction(nameof(GetByExternalOperationId), new { externalOperationId = response.ExternalOperationId }, response);
    }

    [HttpGet("{externalOperationId:guid}")]
    [ProducesResponseType(typeof(PaymentStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByExternalOperationId(Guid externalOperationId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetPaymentByExternalOperationIdQuery(externalOperationId), cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }
}
