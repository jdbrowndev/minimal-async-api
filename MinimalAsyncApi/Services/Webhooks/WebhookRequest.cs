using MinimalAsyncApi.Services.Jobs;

namespace MinimalAsyncApi.Services.Webhooks;

public class WebhookRequest
{
	public IBackgroundJob Job { get; init; }
	public string WebhookUrl { get; init; }
}