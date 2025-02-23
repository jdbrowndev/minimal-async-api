using MinimalAsyncApi.Services.Jobs;

namespace MinimalAsyncApi.Services.Webhooks;

public class WebhookHostedService : IHostedService
{
	private readonly IWebhookQueue _queue;
	private readonly ILogger<WebhookHostedService> _logger;
	private readonly CancellationTokenSource _cts;
	private Task _task;

	public WebhookHostedService(IWebhookQueue queue, ILogger<WebhookHostedService> logger)
	{
		_queue = queue;
		_logger = logger;
		_cts = new CancellationTokenSource();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("WebhookHostedService started");
		_task = ProcessQueue();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("WebhookHostedService stopping...");

		_cts.Cancel();

		_logger.LogInformation("WebhookHostedService stopped");
		return Task.CompletedTask;
	}

	private async Task ProcessQueue()
	{
		while (!_cts.IsCancellationRequested)
		{
			try
			{
				var request = await _queue.DequeueAsync(_cts.Token);
				await SendResult(request);
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"WebhookHostedService exception");
			}
		}
	}

	private async Task SendResult(WebhookRequest request)
	{
		var job = request.Job;
		var webhookUrl = request.WebhookUrl;

		var uri = new Uri(webhookUrl);
		if (!new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps }.Contains(uri.Scheme))
			throw new ArgumentException($"Webhook url {webhookUrl} for job {job.Name} ({job.Id}) must be HTTP/HTTPS");

		using var httpClient = new HttpClient();

		var result = new WebhookResult
		{
			JobId = job.Id,
			Status = GetStatus(job),
			Result = job.IsCompletedSuccessfully ? job.Result : null
		};
		using var response = await httpClient.PostAsJsonAsync(uri.AbsoluteUri, result, cancellationToken: _cts.Token);

		if (!response.IsSuccessStatusCode)
		{
			_logger.LogWarning("Webhook url {webhookUrl} for job {jobName} ({jobId}) returned an unsuccessful status code: {statusCode} {statusCodeName}", webhookUrl, job.Name, job.Id, response.StatusCode.ToString("D"), response.StatusCode);
		}
	}

	private static string GetStatus(IBackgroundJob job)
	{
		if (job.IsFaulted)
		{
			return "Error";
		}

		return "Done";
	}
}