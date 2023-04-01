namespace MinimalAsyncApi.Services.Webhooks;

public class WebhookResult
{
	public string JobId { get; init; }
	public string Status { get; init; }
	public object Result { get; init; }
}