﻿using MinimalAsyncApi.Jobs;

namespace MinimalAsyncApi.Services;

public interface IBackgroundJob
{
	public string Id { get; }
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
    public Job<TResult> Job { get; init; }
	public Task<TResult> Task { get; init; }
	public CancellationTokenSource CancellationTokenSource { get; init; }

	public bool IsCompleted => Task.IsCompleted;
	public bool IsCompletedSuccessfully => Task.IsCompletedSuccessfully;
	public bool IsFaulted => Task.IsFaulted;
	public bool IsCanceled => Task.IsCanceled;
	public object Result => Task.Result;
}