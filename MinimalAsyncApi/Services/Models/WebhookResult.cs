namespace MinimalAsyncApi.Services.Models;

public class WebhookResult
{
    public string Status { get; init; }
    public object Result { get; init; }
}