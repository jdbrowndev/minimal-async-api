namespace MinimalAsyncApi.Jobs.Fibonacci;

public class FibonacciJobRunner : IJobRunner<FibonacciJob, FibonacciJobResult>
{
    public Task<FibonacciJobResult> Run(FibonacciJob job, CancellationToken cancellationToken)
    {
        var index = job.Index;
        var prev = 0UL;
        var current = 1UL;

        if (index == 0)
            return Task.FromResult(new FibonacciJobResult { FibonacciNumber = prev });
        if (index == 1)
            return Task.FromResult(new FibonacciJobResult { FibonacciNumber = current });

        for (var i = 2UL; i <= index; i++)
        {
            var tmp = current;
            current = prev + current;
            prev = tmp;
        }

        return Task.FromResult(new FibonacciJobResult { FibonacciNumber = current });
    }
}