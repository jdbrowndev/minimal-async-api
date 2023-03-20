using MinimalAsyncApi.Jobs;

namespace MinimalAsyncApi.Services;

public class BackgroundJob
{
	public IJob Job { get; set; }
	public Task<object> Task { get; set; }
	public CancellationTokenSource CancellationTokenSource { get; set; }
}
