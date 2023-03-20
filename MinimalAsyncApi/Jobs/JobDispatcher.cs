namespace MinimalAsyncApi.Jobs;

public interface IJobDispatcher
{
	Task<TResult> Dispatch<TJob, TResult>(TJob job, CancellationToken cancellationToken) where TJob : Job<TResult>;
}

public class JobDispatcher : IJobDispatcher
{
	private readonly IServiceProvider _serviceProvider;

	public JobDispatcher(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task<TResult> Dispatch<TJob, TResult>(TJob job, CancellationToken cancellationToken) where TJob : Job<TResult>
	{
		var runner = _serviceProvider.GetService<IJobRunner<TJob, TResult>>();
		if (runner == null)
			throw new ArgumentException($"Unable to retrieve job runner for {job.GetType().FullName}");

		var result = await runner.Run(job, cancellationToken);
		return result;
	}
}
