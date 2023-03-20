namespace MinimalAsyncApi.Jobs.RandomInt;

public class RandomIntJob : Job<RandomIntJobResult>
{
	public int MinValue { get; init; }
	public int MaxValue { get; init; }
}