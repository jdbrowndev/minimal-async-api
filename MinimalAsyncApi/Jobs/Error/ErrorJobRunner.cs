namespace MinimalAsyncApi.Jobs.Error;

public class ErrorJobRunner : IJobRunner<ErrorJob, ErrorJobResult>
{
	public async Task<ErrorJobResult> Run(ErrorJob job, CancellationToken cancellationToken)
	{
		await Task.Delay(5000, cancellationToken);

		throw new Exception("Test exception");
	}
}
