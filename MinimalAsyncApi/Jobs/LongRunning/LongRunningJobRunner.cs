using MinimalAsyncApi.Jobs;

namespace MinimalAsyncApi.Jobs.LongRunning;

public class LongRunningJobRunner : IJobRunner<LongRunningJob, LongRunningJobResult>
{
	public async Task<LongRunningJobResult> Run(LongRunningJob job, CancellationToken cancellationToken)
	{
		await Task.Delay(3600000, cancellationToken); // 1 hour

		return new LongRunningJobResult
		{
			Message = "Long running job is done"
		};
	}
}
