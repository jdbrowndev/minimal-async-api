using Microsoft.Extensions.Caching.Memory;
using MinimalAsyncApi.Jobs;

namespace MinimalAsyncApi.Services;

public interface IJobHostedService
{
	void Run<TResult>(Job<TResult> job);
	string GetStatus(string jobId);
	bool Cancel(string jobId);
	object GetResult(string jobId);
}

public class JobHostedService : IHostedService, IJobHostedService
{
	private readonly MemoryCache _jobs;
	private readonly HashSet<string> _jobIds;
	private readonly IJobDispatcher _jobDispatcher;
	private readonly ILogger<JobHostedService> _logger;

	public JobHostedService(IJobDispatcher jobDispatcher, ILogger<JobHostedService> logger)
	{
		_jobs = new MemoryCache(new MemoryCacheOptions());
		_jobIds = new HashSet<string>();
		_jobDispatcher = jobDispatcher;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("JobHostedService started");
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("JobHostedService stopping...");

		foreach (var id in _jobIds)
		{
			_jobs.TryGetValue<BackgroundJob>(id, out var job);
			if (job != null)
			{
				job.CancellationTokenSource.Cancel();
			}
		}

		_logger.LogInformation("JobHostedService stopped");
		_jobs.Dispose();

		return Task.CompletedTask;
	}

	public void Run<TResult>(Job<TResult> job)
	{
		var jobId = job.Id;
		var jobName = job.GetType().FullName;

		try
		{
			var cts = new CancellationTokenSource();
			var backgroundJob = new BackgroundJob
			{
				Job = job,
				Task = Task.Run(async () =>
				{
					try
					{
						var result = await _jobDispatcher.Dispatch(job, cts.Token);
						return (object)result;
					}
					catch (Exception e)
					{
						_logger.LogError(e, $"{jobName} ({jobId}) threw an unhandled exception");
						throw;
					}
				}),
				CancellationTokenSource = cts
			};

			_jobs.Set(jobId, backgroundJob, new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(DateTime.Now.AddHours(12))
				.RegisterPostEvictionCallback((key, value, reason, state) =>
				{
					var backgroundJob = (BackgroundJob)value;
					_jobIds.Remove(backgroundJob.Job.Id);
					backgroundJob.CancellationTokenSource.Cancel();
				})
			);
			_jobIds.Add(jobId);
		}
		catch (Exception e)
		{
			_logger.LogError(e, $"Failed to start job {jobName} ({jobId})");
			throw;
		}
	}

	public string GetStatus(string jobId)
	{
		_jobs.TryGetValue<BackgroundJob>(jobId, out var backgroundJob);

		if (backgroundJob == null)
		{
			return "Not Found";
		}
		else if (!backgroundJob.Task.IsCompleted)
		{
			return "Running";
		}
		else if (backgroundJob.Task.IsCanceled)
		{
			return "Cancelled";
		}
		else if (backgroundJob.Task.IsFaulted)
		{
			return "Error";
		}

		return "Done";
	}

	public bool Cancel(string jobId)
	{
		_jobs.TryGetValue<BackgroundJob>(jobId, out var backgroundJob);
		if (backgroundJob == null || backgroundJob.Task.IsCompleted)
			return false;

		backgroundJob.CancellationTokenSource.Cancel();
		return true;
	}

	public object GetResult(string jobId)
	{
		_jobs.TryGetValue<BackgroundJob>(jobId, out var backgroundJob);

		if (backgroundJob == null || !backgroundJob.Task.IsCompletedSuccessfully)
			return null;

		return backgroundJob.Task.Result;
	}
}
