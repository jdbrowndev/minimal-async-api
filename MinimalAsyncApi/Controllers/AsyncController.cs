using Microsoft.AspNetCore.Mvc;
using MinimalAsyncApi.Services;

namespace MinimalAsyncApi.Controllers;

public class AsyncController : Controller
{
	private readonly IJobHostedService _jobService;

	public AsyncController(IJobHostedService jobService)
	{
		_jobService = jobService;
	}

	[HttpGet]
	public string GetStatus(string jobId)
	{
		var status = _jobService.GetStatus(jobId);
		return status;
	}

	[HttpGet]
	public object GetResult(string jobId)
	{
		var result = _jobService.GetResult(jobId);
		return result;
	}

	[HttpPut]
	public bool Cancel(string jobId)
	{
		var result = _jobService.Cancel(jobId);
		return result;
	}
}
