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
	private readonly JsonSerializerOptions _serializerOptions;

	public RedisJobStorage(IConnectionMultiplexer redis)
    {
		_redis = redis;
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

	public async Task<bool> CancelJob(string jobId)
	{
		var isLocal = _jobs.TryGetValue(jobId, out var job);
		if (isLocal)
		{
			if (job.IsCompleted)
				return false;
			else
				job.CancellationTokenSource.Cancel();
		}
		else
		{
			// todo handle Redis case
		}

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
