namespace MinimalAsyncApi.Services;

public interface IRandomService
{
	Random Get();
}

public class RandomService : IRandomService
{
	private readonly Random _random;

	public RandomService()
	{
		_random = new Random();
	}

	public Random Get()
	{
		return _random;
	}
}
