using MinimalAsyncApi.Services.Models;

namespace MinimalAsyncApi.Services.Storage;

public class MemoryCacheJobStorage : IJobStorage
{
	public Task CancelAllLocalJobs()
	{
		throw new NotImplementedException();
	}

	public Task<bool> CancelJob(string jobId)
	{
		throw new NotImplementedException();
	}

	public Task<RedisJob> GetJob(string jobId)
	{
		throw new NotImplementedException();
	}

	public Task<bool> SetJob(IBackgroundJob job)
	{
		throw new NotImplementedException();
	}
}
