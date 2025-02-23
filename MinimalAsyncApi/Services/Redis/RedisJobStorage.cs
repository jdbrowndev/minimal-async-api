using MinimalAsyncApi.Services.Jobs;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace MinimalAsyncApi.Services.Redis;

public interface IRedisJobStorage
{
	Task<SerializableJob> GetJob(string jobId);
	Task<bool> SetJob(IBackgroundJob job);
	Task<string> CancelJob(string jobId, bool localOnly = false);
	Task CancelAllLocalJobs();
}

public class RedisJobStorage : IRedisJobStorage
{
	private readonly IConnectionMultiplexer _redis;
	private readonly RedisChannel _cancelChannel;
	private readonly ConcurrentDictionary<string, IBackgroundJob> _jobs;
	private readonly JsonSerializerOptions _serializerOptions;

	public RedisJobStorage(IConnectionMultiplexer redis, IRedisChannelFactory channelFactory)
	{
		_redis = redis;
		_cancelChannel = channelFactory.GetCancelChannel();
		_jobs = new ConcurrentDictionary<string, IBackgroundJob>();
		_serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
	}

	public async Task<SerializableJob> GetJob(string jobId)
	{
		var isLocal = _jobs.TryGetValue(jobId, out var backgroundJob);
		if (isLocal)
			return new SerializableJob(backgroundJob);

		var db = _redis.GetDatabase();
		var json = await db.StringGetAsync(jobId);
		if (string.IsNullOrWhiteSpace(json))
			return null;

		var result = JsonSerializer.Deserialize<SerializableJob>(json, _serializerOptions);
		return result;
	}

	public async Task<bool> SetJob(IBackgroundJob job)
	{
		_jobs.AddOrUpdate(job.Id, job, (k, v) => job);

		var db = _redis.GetDatabase();

		var redisJob = new SerializableJob(job);
		var json = JsonSerializer.Serialize(redisJob, _serializerOptions);

		var result = await db.StringSetAsync(redisJob.Id, json, TimeSpan.FromHours(12));
		return result;
	}

	public async Task<string> CancelJob(string jobId, bool localOnly = false)
	{
		var isLocal = _jobs.TryGetValue(jobId, out var job);
		if (isLocal)
		{
			if (job.IsCompleted)
				return "Completed";
			else
			{
				job.CancellationTokenSource.Cancel();
				return "Canceled";
			}
		}

		if (localOnly)
			return "Not Local";

		var subscriber = _redis.GetSubscriber();
		await subscriber.PublishAsync(_cancelChannel, jobId);

		return "Cancellation Requested";
	}

	public async Task CancelAllLocalJobs()
	{
		foreach (var key in _jobs.Keys)
		{
			await CancelJob(key);
		}
	}
}
