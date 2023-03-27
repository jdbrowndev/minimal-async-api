namespace MinimalAsyncApi.Jobs.Examples.Fibonacci;

public class FibonacciJob : IJob<FibonacciJobResult>
{
    public ulong Index { get; init; }
}