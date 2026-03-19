using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Abstractions.Clock;
using Payments.Application.Abstractions.Messaging;
using Payments.Application.Abstractions.Persistence;
using Payments.Application.Abstractions.UnitOfWork;
using Payments.Infrastructure.Messaging;
using Payments.Infrastructure.Messaging.Consumers;
using Payments.Infrastructure.Messaging.Producers;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Persistence.Repositories;
using Payments.Infrastructure.Services;
using Payments.Infrastructure.UnitOfWork;

namespace Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IRiskEvaluationRequestProducer, RiskEvaluationRequestProducer>();
        services.AddHostedService<RiskEvaluationResponseConsumer>();

        return services;
    }
}
