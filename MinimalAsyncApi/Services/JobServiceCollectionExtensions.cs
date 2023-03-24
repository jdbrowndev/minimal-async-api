using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Jobs.Error;
using MinimalAsyncApi.Jobs.Fibonacci;
using MinimalAsyncApi.Jobs.LongRunning;
using MinimalAsyncApi.Jobs.RandomInt;
using MinimalAsyncApi.Services.Models;

namespace MinimalAsyncApi.Services;

public static class JobServiceCollectionExtensions
{
    public static void AddJobServices(this IServiceCollection services)
    {
        // add infrastructure
        services.AddTransient<IJobDispatcher, JobDispatcher>();
        services.AddSingleton<IJobHostedService, JobHostedService>();
        services.AddHostedService(ctx => (JobHostedService) ctx.GetService<IJobHostedService>());
        services.AddSingleton<IWebhookQueue, WebhookQueue>();
        services.AddHostedService<WebhookHostedService>();

        // add jobs
        services.AddTransient<IJobRunner<RandomIntJob, RandomIntJobResult>, RandomIntJobRunner>();
        services.AddTransient<IJobRunner<ErrorJob, ErrorJobResult>, ErrorJobRunner>();
        services.AddTransient<IJobRunner<LongRunningJob, LongRunningJobResult>, LongRunningJobRunner>();
        services.AddTransient<IJobRunner<FibonacciJob, FibonacciJobResult>, FibonacciJobRunner>();
    }
}