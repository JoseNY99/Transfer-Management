using System.Text.Json;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payments.Application.DTOs;
using Payments.Application.Features.Payments.Commands.ProcessRiskEvaluationResponse;

namespace Payments.Infrastructure.Messaging.Consumers;

public sealed class RiskEvaluationResponseConsumer : BackgroundService
{
    private readonly KafkaSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RiskEvaluationResponseConsumer> _logger;

    public RiskEvaluationResponseConsumer(
        IOptions<KafkaSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<RiskEvaluationResponseConsumer> logger)
    {
        _settings = settings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_settings.RiskEvaluationResponseTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(TimeSpan.FromSeconds(2));
                if (result?.Message?.Value is null)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var message = JsonSerializer.Deserialize<RiskEvaluationResponseMessage>(
                    result.Message.Value,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (message is null)
                    continue;

                _logger.LogInformation(
                    "Kafka response received. ExternalOperationId: {ExternalOperationId}, Status: {Status}",
                    message.ExternalOperationId,
                    message.Status);

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(
                    new ProcessRiskEvaluationResponseCommand(
                        message.ExternalOperationId,
                        message.Status),
                    stoppingToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogWarning(ex, "Kafka not ready yet for risk evaluation response consumer");
                await Task.Delay(3000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming risk evaluation response");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}