using Microsoft.EntityFrameworkCore;
using Risk.Worker.Consumers;
using Risk.Worker.Persistence;
using Risk.Worker.Producers;
using Risk.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<RiskReadDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RiskEvaluator>();
builder.Services.AddScoped<RiskEvaluationResponseProducer>();
builder.Services.AddHostedService<RiskEvaluationRequestConsumer>();

var app = builder.Build();
app.Run();
