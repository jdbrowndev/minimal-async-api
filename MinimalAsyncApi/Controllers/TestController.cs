using Microsoft.AspNetCore.Mvc;
using MinimalAsyncApi.Jobs.Error;
using MinimalAsyncApi.Jobs.Fibonacci;
using MinimalAsyncApi.Jobs.LongRunning;
using MinimalAsyncApi.Jobs.RandomInt;
using MinimalAsyncApi.Services;

namespace MinimalAsyncApi.Controllers;

public class TestController : Controller
{
	private readonly IJobHostedService _jobService;

	public TestController(IJobHostedService jobService)
	{
		_jobService = jobService;
	}

	[HttpGet]
	public string GetRandomInt(int minValue, int maxValue, string webhookUrl = null)
	{
		var job = new RandomIntJob
		{
			MinValue = minValue,
			MaxValue = maxValue
		};

		var jobId = _jobService.Run(job, webhookUrl);

		return jobId;
	}

	[HttpGet]
	public string GetError(string webhookUrl = null)
	{
		var job = new ErrorJob();

		var jobId = _jobService.Run(job, webhookUrl);

		return jobId;
	}

	[HttpGet]
	public string GetLongRunning(string webhookUrl = null)
	{
		var job = new LongRunningJob();

		var jobId = _jobService.Run(job, webhookUrl);

		return jobId;
	}

	[HttpGet]
	public string GetFibonacciNumber(ulong index, string webhookUrl = null)
	{
		var job = new FibonacciJob { Index = index }; 

		var jobId = _jobService.Run(job, webhookUrl);

		return jobId;
	}
}
