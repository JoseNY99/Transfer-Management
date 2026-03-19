using FluentAssertions;
using Moq;
using Payments.Application.Abstractions.Clock;
using Payments.Application.Abstractions.Messaging;
using Payments.Application.Abstractions.Persistence;
using Payments.Application.Abstractions.UnitOfWork;
using Payments.Application.Features.Payments.Commands.CreatePayment;
using Payments.Domain.Entities;

namespace Payments.UnitTests;

public class CreatePaymentCommandHandlerTests
{
    [Fact]
    public async Task Should_Create_Payment_And_Publish_Message()
    {
        var repository = new Mock<IPaymentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var clock = new Mock<IDateTimeProvider>();
        var producer = new Mock<IRiskEvaluationRequestProducer>();

        clock.Setup(x => x.UtcNow).Returns(new DateTime(2026, 03, 18, 12, 0, 0, DateTimeKind.Utc));

        var handler = new CreatePaymentCommandHandler(
            repository.Object,
            unitOfWork.Object,
            clock.Object,
            producer.Object);

        var result = await handler.Handle(new CreatePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 1000m), CancellationToken.None);

        result.Status.Should().Be("evaluating");
        repository.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        producer.Verify(x => x.ProduceAsync(It.IsAny<Payments.Application.DTOs.RiskEvaluationRequestMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
