using Microsoft.AspNetCore.Mvc;
using MinimalAsyncApi.Jobs.Error;
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
	public string GetRandomInt(int minValue, int maxValue)
	{
		var job = new RandomIntJob
		{
			MinValue = minValue,
			MaxValue = maxValue
		};

		var jobId = _jobService.Run(job);

		return jobId;
	}

	[HttpGet]
	public string GetError()
	{
		var job = new ErrorJob();

		var jobId = _jobService.Run(job);

		return jobId;
	}

	[HttpGet]
	public string GetLongRunning()
	{
		var job = new LongRunningJob();

		var jobId = _jobService.Run(job);

		return jobId;
	}
}
