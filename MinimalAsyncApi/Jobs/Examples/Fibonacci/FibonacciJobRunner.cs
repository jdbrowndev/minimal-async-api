using System.Numerics;

namespace MinimalAsyncApi.Jobs.Examples.Fibonacci;

public class FibonacciJobRunner : IJobRunner<FibonacciJob, FibonacciJobResult>
{
	public Task<FibonacciJobResult> Run(FibonacciJob job, CancellationToken cancellationToken)
	{
		BigInteger index = BigInteger.Parse(job.Index);
		BigInteger prev = 0;
		BigInteger current = 1;

		if (index == 0)
			return Task.FromResult(new FibonacciJobResult { FibonacciNumber = prev.ToString() });
		if (index == 1)
			return Task.FromResult(new FibonacciJobResult { FibonacciNumber = current.ToString() });

		for (BigInteger i = 2; i <= index; i++)
		{
			if (cancellationToken.IsCancellationRequested)
				throw new TaskCanceledException();

			BigInteger tmp = current;
			current = prev + current;
			prev = tmp;
		}

		return Task.FromResult(new FibonacciJobResult { FibonacciNumber = current.ToString() });
	}
}