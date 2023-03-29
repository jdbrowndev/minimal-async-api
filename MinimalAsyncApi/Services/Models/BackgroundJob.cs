using MinimalAsyncApi.Jobs;

namespace MinimalAsyncApi.Services.Models;

public interface IBackgroundJob
{
	public string Id { get; }
	public string Name { get; }
	public CancellationTokenSource CancellationTokenSource { get; }

	public bool IsCompleted { get; }
	public bool IsCompletedSuccessfully { get; }
	public bool IsFaulted { get; }
	public bool IsCanceled { get; }
	public object Result { get; }
}

public class BackgroundJob<TResult> : IBackgroundJob
{
    public string Id { get; init; }
	public string Name { get; init; }
    public IJob<TResult> Job { get; init; }
	public Task<TResult> Task { get; set; }
	public Task UpdateStorageTask { get; set; }
	public Task WebhookTask { get; set; }
	public CancellationTokenSource CancellationTokenSource { get; init; }

	public bool IsCompleted => Task.IsCompleted;
	public bool IsCompletedSuccessfully => Task.IsCompletedSuccessfully;
	public bool IsFaulted => Task.IsFaulted;
	public bool IsCanceled => Task.IsCanceled;
	public object Result => Task.Result;
}
