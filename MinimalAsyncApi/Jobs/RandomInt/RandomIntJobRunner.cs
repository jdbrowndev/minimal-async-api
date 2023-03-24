namespace MinimalAsyncApi.Jobs.RandomInt;

public class RandomIntJobRunner : IJobRunner<RandomIntJob, RandomIntJobResult>
{
	private readonly Random _random;

	public RandomIntJobRunner(Random random)
	{
		_random = random;
	}

	public async Task<RandomIntJobResult> Run(RandomIntJob job, CancellationToken cancellationToken)
	{
		var randomInt = _random.Next(job.MinValue, job.MaxValue);

		// Wait a random amount of time to simulate an async operation
		await Task.Delay(_random.Next(3000, 9000), cancellationToken);

		var result = new RandomIntJobResult
		{
			Integer = randomInt
		};
		return result;
	}
}
