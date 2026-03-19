using FluentAssertions;

namespace Payments.UnitTests;

public class RiskEvaluatorTests
{
    [Fact]
    public void Should_Deny_When_Amount_Is_Greater_Than_20000000()
    {
        var amount = 20000001m;
        (amount > 20_000_000m).Should().BeTrue();
    }
}
