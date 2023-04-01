using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Jobs.Examples.Error;
using MinimalAsyncApi.Jobs.Examples.Fibonacci;
using MinimalAsyncApi.Services.Jobs;
using MinimalAsyncApi.Services.Redis;
using MinimalAsyncApi.Services.Webhooks;
using StackExchange.Redis;

namespace MinimalAsyncApi.Services;

public static class ServiceCollectionExtensions
{
	public static void AddMinimalAsyncApiServices(this IServiceCollection services, IConfiguration configuration)
	{
		// Add hosted services
		services.AddTransient<IJobDispatcher, JobDispatcher>();
		services.AddSingleton<IJobHostedService, JobHostedService>();
		services.AddHostedService(provider => (JobHostedService)provider.GetService<IJobHostedService>());
		services.AddHostedService<RedisSubscriptionHostedService>();
		services.AddSingleton<IWebhookQueue, WebhookQueue>();
		services.AddHostedService<WebhookHostedService>();

		// Add storage
		services.AddSingleton<IConnectionMultiplexer>(provider =>
		{
			var connectionString = configuration.GetValue<string>("Redis");
			return ConnectionMultiplexer.Connect(connectionString);
		});
		services.AddSingleton<IRedisJobStorage, RedisJobStorage>();

		// Add jobs
		services.AddTransient<IJobRunner<FibonacciJob, FibonacciJobResult>, FibonacciJobRunner>();
		services.AddTransient<IJobRunner<ErrorJob, ErrorJobResult>, ErrorJobRunner>();
	}
}