using MinimalAsyncApi.Services.Models;
using MinimalAsyncApi.Services.Storage;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace MinimalAsyncApi.Services.Redis;

public class RedisJobStorage : IJobStorage
{
	private readonly IConnectionMultiplexer _redis;
	private readonly ConcurrentDictionary<string, IBackgroundJob> _jobs;

	public RedisJobStorage(IConnectionMultiplexer redis)
    {
		_redis = redis;
		_jobs = new ConcurrentDictionary<string, IBackgroundJob>();
	}

	public async Task<RedisJob> GetJob(string jobId)
	{
		var isLocal = _jobs.TryGetValue(jobId, out var backgroundJob);
		if (isLocal)
			return new RedisJob(backgroundJob);

		var db = _redis.GetDatabase();
		var json = await db.StringGetAsync(jobId);
		if (string.IsNullOrWhiteSpace(json))
			return null;

		var result = JsonSerializer.Deserialize<RedisJob>(json);
		return result;
	}

	public async Task<bool> SetJob(IBackgroundJob job)
	{
		_jobs.AddOrUpdate(job.Id, job, (k, v) => job);

		var db = _redis.GetDatabase();

		var redisJob = new RedisJob(job);
		var json = JsonSerializer.Serialize(redisJob);

		var result = await db.StringSetAsync(redisJob.Id, json, TimeSpan.FromHours(12));
		return result;
	}

	public async Task<bool> CancelJob(string jobId)
	{
		var exists = _jobs.TryGetValue(jobId, out var job);
		if (!exists || job.IsCompleted)
			return false;

		job.CancellationTokenSource.Cancel();

		// todo handle redis case

		return true;
	}

	public async Task CancelAllLocalJobs()
	{
		foreach (var key in _jobs.Keys)
		{
			await CancelJob(key);
		}
	}
}
