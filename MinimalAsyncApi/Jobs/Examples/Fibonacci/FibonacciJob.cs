namespace MinimalAsyncApi.Jobs.Examples.Fibonacci;

public class FibonacciJob : IJob<FibonacciJobResult>
{
    public string Index { get; init; }
}