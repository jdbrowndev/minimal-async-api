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

		_jobService.Run<RandomIntJob, RandomIntJobResult>(job);

		return job.Id;
	}

	[HttpGet]
	public string GetError()
	{
		var job = new ErrorJob();

		_jobService.Run<ErrorJob, ErrorJobResult>(job);

		return job.Id;
	}

	[HttpGet]
	public string GetLongRunning()
	{
		var job = new LongRunningJob();

		_jobService.Run<LongRunningJob, LongRunningJobResult>(job);

		return job.Id;
	}
}
