using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Jobs.Examples.Error;
using MinimalAsyncApi.Jobs.Examples.Fibonacci;
using MinimalAsyncApi.Services.Models;
using MinimalAsyncApi.Services.Redis;
using MinimalAsyncApi.Services.Storage;
using StackExchange.Redis;

namespace MinimalAsyncApi.Services;

public static class JobServiceCollectionExtensions
{
	public static void AddJobServices(this IServiceCollection services, IConfiguration configuration)
	{
		// Add hosted services
		services.AddTransient<IJobDispatcher, JobDispatcher>();
		services.AddSingleton<IJobHostedService, JobHostedService>();
		services.AddHostedService(provider => (JobHostedService)provider.GetService<IJobHostedService>());
		services.AddSingleton<IWebhookQueue, WebhookQueue>();
		services.AddHostedService<WebhookHostedService>();
		// todo hosted service to listen for cancellation over redis

		// Add storage
		services.AddSingleton<IConnectionMultiplexer>(provider =>
		{
			var connectionString = configuration.GetValue<string>("Redis");
			return ConnectionMultiplexer.Connect(connectionString);
		});
		services.AddSingleton<IJobStorage, RedisJobStorage>();

		// Add jobs
		services.AddTransient<IJobRunner<FibonacciJob, FibonacciJobResult>, FibonacciJobRunner>();
		services.AddTransient<IJobRunner<ErrorJob, ErrorJobResult>, ErrorJobRunner>();
	}
}