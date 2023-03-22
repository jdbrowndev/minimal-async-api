namespace MinimalAsyncApi.Jobs;

public interface IJob<TResult>
{
    public string Name => GetType().FullName;
}