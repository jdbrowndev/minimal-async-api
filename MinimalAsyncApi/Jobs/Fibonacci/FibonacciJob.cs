namespace MinimalAsyncApi.Jobs.Fibonacci;

public class FibonacciJob : IJob<FibonacciJobResult>
{
    public ulong Index { get; init; }
}