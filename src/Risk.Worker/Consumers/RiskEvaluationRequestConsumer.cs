using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Risk.Worker.Producers;
using Risk.Worker.Services;

namespace Risk.Worker.Consumers;

public sealed class RiskEvaluationRequestConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RiskEvaluationRequestConsumer> _logger;

    public RiskEvaluationRequestConsumer(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<RiskEvaluationRequestConsumer> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = _configuration["Kafka:ConsumerGroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_configuration["Kafka:RiskEvaluationRequestTopic"]);

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

                var data = JsonSerializer.Deserialize<RiskEvaluationRequestKafkaMessage>(
                    result.Message.Value,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (data is null)
                    continue;

                using var scope = _serviceProvider.CreateScope();
                var evaluator = scope.ServiceProvider.GetRequiredService<RiskEvaluator>();
                var producer = scope.ServiceProvider.GetRequiredService<RiskEvaluationResponseProducer>();

                var status = await evaluator.EvaluateAsync(data.CustomerId, data.Amount, stoppingToken);
                await producer.ProduceAsync(data.ExternalOperationId, status, stoppingToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogWarning(ex, "Kafka not ready yet for risk evaluation request consumer");
                await Task.Delay(3000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing risk evaluation request");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }

    private sealed class RiskEvaluationRequestKafkaMessage
    {
        public Guid ExternalOperationId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}