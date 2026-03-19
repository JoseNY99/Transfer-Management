using FluentAssertions;
using Moq;
using Payments.Application.Abstractions.Clock;
using Payments.Application.Abstractions.Persistence;
using Payments.Application.Abstractions.UnitOfWork;
using Payments.Application.Features.Payments.Commands.ProcessRiskEvaluationResponse;
using Payments.Domain.Entities;

namespace Payments.UnitTests;

public class ProcessRiskEvaluationResponseCommandHandlerTests
{
    [Fact]
    public async Task Should_Mark_Accepted()
    {
        var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), 1, 1000m, DateTime.UtcNow);
        var repo = new Mock<IPaymentRepository>();
        var uow = new Mock<IUnitOfWork>();
        var clock = new Mock<IDateTimeProvider>();

        repo.Setup(x => x.GetByExternalOperationIdAsync(payment.ExternalOperationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        var handler = new ProcessRiskEvaluationResponseCommandHandler(repo.Object, uow.Object, clock.Object);

        await handler.Handle(new ProcessRiskEvaluationResponseCommand(payment.ExternalOperationId, "accepted"), CancellationToken.None);

        payment.Status.ToString().ToLowerInvariant().Should().Be("accepted");
    }
}
