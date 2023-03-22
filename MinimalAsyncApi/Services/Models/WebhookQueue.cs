using System.Threading.Channels;

namespace MinimalAsyncApi.Services.Models;

public interface IWebhookQueue
{
    Task QueueAsync(WebhookRequest workItem);
    Task<WebhookRequest> DequeueAsync(CancellationToken cancellationToken);
}

public class WebhookQueue : IWebhookQueue
{
    private readonly Channel<WebhookRequest> _queue;

    public WebhookQueue()
    {
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<WebhookRequest>(options);
    }

    public async Task QueueAsync(WebhookRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        await _queue.Writer.WriteAsync(request);
    }

    public async Task<WebhookRequest> DequeueAsync(CancellationToken cancellationToken)
    {
        var request = await _queue.Reader.ReadAsync(cancellationToken);
        return request;
    }
}