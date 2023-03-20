namespace MinimalAsyncApi.Jobs;

public abstract class Job<TResult> : IJob
{
	public string Id { get; private set; }

	public Job()
	{
		Id = Guid.NewGuid().ToString();
	}
}
