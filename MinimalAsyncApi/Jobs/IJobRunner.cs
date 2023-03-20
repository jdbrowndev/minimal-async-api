namespace MinimalAsyncApi.Jobs;

public interface IJobRunner<TJob, TResult> where TJob : IJob<TResult>
{
	public Task<TResult> Run(TJob job, CancellationToken cancellationToken);
}