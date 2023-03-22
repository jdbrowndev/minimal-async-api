﻿using Microsoft.Extensions.Caching.Memory;
using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Services.Models;

namespace MinimalAsyncApi.Services;

public interface IJobHostedService
{
	string Run<TResult>(IJob<TResult> job, string webhookUrl = null);
	string GetStatus(string jobId);
	bool Cancel(string jobId);
	object GetResult(string jobId);
	TResult GetResult<TResult>(string jobId);
}

public class JobHostedService : IHostedService, IJobHostedService
{
	private readonly MemoryCache _jobs;
	private readonly HashSet<string> _jobIds;
	private readonly IJobDispatcher _jobDispatcher;
	private readonly IWebhookQueue _webhookQueue;
	private readonly ILogger<JobHostedService> _logger;

	public JobHostedService(IJobDispatcher jobDispatcher, IWebhookQueue webhookQueue, ILogger<JobHostedService> logger)
	{
		_jobs = new MemoryCache(new MemoryCacheOptions());
		_jobIds = new HashSet<string>();
		_jobDispatcher = jobDispatcher;
		_webhookQueue = webhookQueue;
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
			_jobs.TryGetValue<IBackgroundJob>(id, out var job);
			if (job != null)
			{
				job.CancellationTokenSource.Cancel();
			}
		}

		_logger.LogInformation("JobHostedService stopped");
		_jobs.Dispose();
		_jobIds.Clear();

		return Task.CompletedTask;
	}

	public string Run<TResult>(IJob<TResult> job, string webhookUrl = null)
	{
		var jobId = Guid.NewGuid().ToString();
		var jobName = job.Name;

		try
		{
			var cts = new CancellationTokenSource();

			BackgroundJob<TResult> backgroundJob = null;
			backgroundJob = new BackgroundJob<TResult>
			{
				Id = jobId,
				Name = jobName,
				Job = job,
				Task = Task.Run(async () =>
				{
					try
					{
						var result = await _jobDispatcher.Dispatch(job, cts.Token);
						return result;
					}
					catch (Exception e)
					{
						_logger.LogError(e, $"{jobName} ({jobId}) threw an unhandled exception");
						throw;
					}
					finally
					{
						if (!string.IsNullOrWhiteSpace(webhookUrl))
						{
							await _webhookQueue.QueueAsync(new WebhookRequest { Job = backgroundJob, WebhookUrl = webhookUrl });
						}
					}
				}),
				CancellationTokenSource = cts
			};

			_jobs.Set(jobId, backgroundJob, new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(DateTime.Now.AddHours(12))
				.RegisterPostEvictionCallback((key, value, reason, state) =>
				{
					var backgroundJob = (IBackgroundJob)value;
					_jobIds.Remove(backgroundJob.Id);
					backgroundJob.CancellationTokenSource.Cancel();
				})
			);
			_jobIds.Add(jobId);

			return jobId;
		}
		catch (Exception e)
		{
			_logger.LogError(e, $"Failed to start job {jobName} ({jobId})");
			throw;
		}
	}

	public string GetStatus(string jobId)
	{
		_jobs.TryGetValue<IBackgroundJob>(jobId, out var backgroundJob);

		if (backgroundJob == null)
		{
			return "Not Found";
		}
		else if (!backgroundJob.IsCompleted)
		{
			return "Running";
		}
		else if (backgroundJob.IsCanceled)
		{
			return "Canceled";
		}
		else if (backgroundJob.IsFaulted)
		{
			return "Error";
		}

		return "Done";
	}

	public bool Cancel(string jobId)
	{
		_jobs.TryGetValue<IBackgroundJob>(jobId, out var backgroundJob);
		if (backgroundJob == null || backgroundJob.IsCompleted)
			return false;

		backgroundJob.CancellationTokenSource.Cancel();
		return true;
	}

	public object GetResult(string jobId)
	{
		_jobs.TryGetValue<IBackgroundJob>(jobId, out var backgroundJob);

		if (backgroundJob == null || !backgroundJob.IsCompletedSuccessfully)
			return null;

		return backgroundJob.Result;
	}

	public TResult GetResult<TResult>(string jobId)
	{
		_jobs.TryGetValue<BackgroundJob<TResult>>(jobId, out var backgroundJob);

		if (backgroundJob == null || !backgroundJob.IsCompletedSuccessfully)
			return default;

		return backgroundJob.Task.Result;
	}
}
