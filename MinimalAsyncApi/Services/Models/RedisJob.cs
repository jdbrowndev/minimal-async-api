namespace MinimalAsyncApi.Services.Models;

public class RedisJob
{
    public RedisJob()
    {
    }

	public RedisJob(IBackgroundJob job)
	{
		Id = job.Id;
		Name = job.Name;
		IsCompleted = job.IsCompleted;
		IsCompletedSuccessfully = job.IsCompletedSuccessfully;
		IsFaulted = job.IsFaulted;
		IsCanceled = job.IsCanceled;
		Result = job.IsCompletedSuccessfully ? job.Result : null;
	}

    public string Id { get; init; }
    public string Name { get; init; }
	public bool IsCompleted { get; init; }
	public bool IsCompletedSuccessfully { get; init; }
	public bool IsFaulted { get; init; }
	public bool IsCanceled { get; init; }
	public object Result { get; init; }
}
