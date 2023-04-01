using MinimalAsyncApi.Services.Storage;
using StackExchange.Redis;

namespace MinimalAsyncApi.Services;

public class RedisSubscriptionHostedService : IHostedService
{
	private readonly IConnectionMultiplexer _redis;
	private readonly IRedisJobStorage _jobStorage;
	private readonly ILogger<RedisSubscriptionHostedService> _logger;
	private ISubscriber _subscriber;
	private Task _subscribeTask;

	public RedisSubscriptionHostedService(IConnectionMultiplexer redis, IRedisJobStorage jobStorage, ILogger<RedisSubscriptionHostedService> logger)
    {
		_redis = redis;
		_jobStorage = jobStorage;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("RedisSubscriptionHostedService started");

		_subscriber = _redis.GetSubscriber();
		_subscribeTask = _subscriber.SubscribeAsync("cancel", async (channel, message) => {
			try
			{
				await _jobStorage.CancelJob((string)message, localOnly: true);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "RedisSubscriptionHostedService subscribe exception");
			}
		});

		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("RedisSubscriptionHostedService stopping...");

		await _subscriber.UnsubscribeAllAsync();
		await _subscribeTask;

		_logger.LogInformation("RedisSubscriptionHostedService stopped");
	}
}
