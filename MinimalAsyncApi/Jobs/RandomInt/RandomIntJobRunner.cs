using MinimalAsyncApi.Services;

namespace MinimalAsyncApi.Jobs.RandomInt;

public class RandomIntJobRunner : IJobRunner<RandomIntJob, RandomIntJobResult>
{
	private readonly IRandomService _randomService;

	public RandomIntJobRunner(IRandomService randomService)
	{
		_randomService = randomService;
	}

	public async Task<RandomIntJobResult> Run(RandomIntJob job, CancellationToken cancellationToken)
	{
		var random = _randomService.Get();
		var randomInt = random.Next(job.MinValue, job.MaxValue);

		// Wait a random amount of time to simulate an async operation
		await Task.Delay(random.Next(3000, 9000), cancellationToken);

		var result = new RandomIntJobResult
		{
			Integer = randomInt
		};
		return result;
	}
}
