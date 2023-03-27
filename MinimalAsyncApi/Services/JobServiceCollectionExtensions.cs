using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Jobs.Examples.Error;
using MinimalAsyncApi.Jobs.Examples.Fibonacci;
using MinimalAsyncApi.Services.Models;

namespace MinimalAsyncApi.Services;

public static class JobServiceCollectionExtensions
{
    public static void AddJobServices(this IServiceCollection services)
    {
        // Add infrastructure
        services.AddTransient<IJobDispatcher, JobDispatcher>();
        services.AddSingleton<IJobHostedService, JobHostedService>();
        services.AddHostedService(ctx => (JobHostedService) ctx.GetService<IJobHostedService>());
        services.AddSingleton<IWebhookQueue, WebhookQueue>();
        services.AddHostedService<WebhookHostedService>();

        // Add jobs
        services.AddTransient<IJobRunner<FibonacciJob, FibonacciJobResult>, FibonacciJobRunner>();
        services.AddTransient<IJobRunner<ErrorJob, ErrorJobResult>, ErrorJobRunner>();
    }
}