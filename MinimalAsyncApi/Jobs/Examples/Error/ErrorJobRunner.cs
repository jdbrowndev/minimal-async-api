namespace MinimalAsyncApi.Jobs.Examples.Error;

public class ErrorJobRunner : IJobRunner<ErrorJob, ErrorJobResult>
{
	public Task<ErrorJobResult> Run(ErrorJob job, CancellationToken cancellationToken)
	{
		throw new Exception("ErrorJob test exception");
	}
}
