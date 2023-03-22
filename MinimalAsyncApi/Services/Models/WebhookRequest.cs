namespace MinimalAsyncApi.Services.Models;

public class WebhookRequest
{
    public IBackgroundJob Job { get; init; }
    public string WebhookUrl { get; init; }
}