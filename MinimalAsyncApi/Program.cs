using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Jobs.Error;
using MinimalAsyncApi.Jobs.Fibonacci;
using MinimalAsyncApi.Jobs.LongRunning;
using MinimalAsyncApi.Jobs.RandomInt;
using MinimalAsyncApi.Services;
using MinimalAsyncApi.Services.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IJobRunner<RandomIntJob, RandomIntJobResult>, RandomIntJobRunner>();
builder.Services.AddTransient<IJobRunner<ErrorJob, ErrorJobResult>, ErrorJobRunner>();
builder.Services.AddTransient<IJobRunner<LongRunningJob, LongRunningJobResult>, LongRunningJobRunner>();
builder.Services.AddTransient<IJobRunner<FibonacciJob, FibonacciJobResult>, FibonacciJobRunner>();

builder.Services.AddTransient<IJobDispatcher, JobDispatcher>();
builder.Services.AddSingleton<IJobHostedService, JobHostedService>();
builder.Services.AddHostedService(ctx => (JobHostedService) ctx.GetService<IJobHostedService>());
builder.Services.AddSingleton<IWebhookQueue, WebhookQueue>();
builder.Services.AddHostedService<WebhookHostedService>();

builder.Services.AddSingleton<IRandomService, RandomService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
