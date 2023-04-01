using MinimalAsyncApi.Jobs;
using MinimalAsyncApi.Services.Redis;
using MinimalAsyncApi.Services.Webhooks;

namespace MinimalAsyncApi.Services.Jobs;

public interface IJobHostedService
{
	Task<string> Run<TResult>(IJob<TResult> job, string webhookUrl = null);
	Task<string> GetStatus(string jobId);
	Task<string> Cancel(string jobId);
	Task<object> GetResult(string jobId);
}

public class JobHostedService : IHostedService, IJobHostedService
{
	private readonly IRedisJobStorage _jobs;
	private readonly IJobDispatcher _jobDispatcher;
	private readonly IWebhookQueue _webhookQueue;
	private readonly ILogger<JobHostedService> _logger;

	public JobHostedService(IRedisJobStorage jobs, IJobDispatcher jobDispatcher, IWebhookQueue webhookQueue, ILogger<JobHostedService> logger)
	{
		_jobs = jobs;
		_jobDispatcher = jobDispatcher;
		_webhookQueue = webhookQueue;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("JobHostedService started");
		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("JobHostedService stopping...");

		await _jobs.CancelAllLocalJobs();

		_logger.LogInformation("JobHostedService stopped");
	}

	public async Task<string> Run<TResult>(IJob<TResult> job, string webhookUrl = null)
	{
		var jobId = Guid.NewGuid().ToString();
		var jobName = job.GetType().FullName;
		var cts = new CancellationTokenSource();

		var backgroundJob = new BackgroundJob<TResult>
		{
			Id = jobId,
			Name = jobName,
			Job = job,
			Task = Task.Run(async () =>
			{
				try
				{
					var jobDispatch = _jobDispatcher.Dispatch(job, cts.Token);
					var result = await jobDispatch.WaitAsync(TimeSpan.FromHours(12));
					return result;
				}
				catch (Exception e)
				{
					_logger.LogError(e, $"{jobName} ({jobId}) threw an unhandled exception");
					throw;
				}
			}),
			CancellationTokenSource = cts
		};
		backgroundJob.UpdateStorageTask = backgroundJob.Task.ContinueWith(_ => _jobs.SetJob(backgroundJob)).Unwrap();
		if (!string.IsNullOrWhiteSpace(webhookUrl))
			backgroundJob.WebhookTask = backgroundJob.Task.ContinueWith(_ => HandleWebhook(backgroundJob, webhookUrl), cancellationToken: cts.Token).Unwrap();

		await _jobs.SetJob(backgroundJob);

		return jobId;
	}

	public async Task<string> GetStatus(string jobId)
	{
		var backgroundJob = await _jobs.GetJob(jobId);

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

	public async Task<string> Cancel(string jobId)
	{
		var result = await _jobs.CancelJob(jobId);
		return result;
	}

	public async Task<object> GetResult(string jobId)
	{
		var backgroundJob = await _jobs.GetJob(jobId);
		return backgroundJob?.Result;
	}

	private async Task HandleWebhook(IBackgroundJob job, string webhookUrl)
	{
		try
		{
			await _webhookQueue.QueueAsync(new WebhookRequest { Job = job, WebhookUrl = webhookUrl });
		}
		catch (Exception e)
		{
			_logger.LogError(e, $"Failed to queue webhook request for {job.Name} ({job.Id})");
		}
	}
}
