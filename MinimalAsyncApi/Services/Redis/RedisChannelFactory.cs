using StackExchange.Redis;

namespace MinimalAsyncApi.Services.Redis;

public interface IRedisChannelFactory
{
	RedisChannel GetCancelChannel();
}

public class RedisChannelFactory : IRedisChannelFactory
{
	public RedisChannel GetCancelChannel()
	{
		return new RedisChannel("cancel", RedisChannel.PatternMode.Auto);
	}
}
