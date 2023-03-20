namespace MinimalAsyncApi.Jobs.RandomInt;

public class RandomIntJob : IJob<RandomIntJobResult>
{
	public int MinValue { get; init; }
	public int MaxValue { get; init; }
}