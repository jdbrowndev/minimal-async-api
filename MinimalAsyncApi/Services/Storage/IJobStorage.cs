﻿using MinimalAsyncApi.Services.Models;

namespace MinimalAsyncApi.Services.Storage;

public interface IJobStorage
{
	Task<RedisJob> GetJob(string jobId);
	Task<bool> SetJob(IBackgroundJob job);
	Task<bool> CancelJob(string jobId);
	Task CancelAllLocalJobs();
}