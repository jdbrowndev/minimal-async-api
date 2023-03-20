namespace MinimalAsyncApi.Jobs;

public interface IJobDispatcher
{
	Task<TResult> Dispatch<TResult>(IJob<TResult> job, CancellationToken cancellationToken);
}

public class JobDispatcher : IJobDispatcher
{
	private readonly IServiceProvider _serviceProvider;

	public JobDispatcher(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task<TResult> Dispatch<TResult>(IJob<TResult> job, CancellationToken cancellationToken)
	{
		var jobType = job.GetType();
		var runnerType = typeof(IJobRunner<,>).MakeGenericType(jobType, typeof(TResult));
		dynamic runner = _serviceProvider.GetService(runnerType);

		if (runner == null)
			throw new ArgumentException($"Unable to retrieve job runner for {jobType.FullName}");

		var result = await runner.Run((dynamic)job, cancellationToken);
		return result;
	}
}
