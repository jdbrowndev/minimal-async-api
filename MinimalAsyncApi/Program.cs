using MinimalAsyncApi.Jobs.Examples.Error;
using MinimalAsyncApi.Jobs.Examples.Fibonacci;
using MinimalAsyncApi.Services;
using MinimalAsyncApi.Services.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddMinimalAsyncApiServices(builder.Configuration);
var app = builder.Build();

// Add Async API endpoints

app.MapGet("/Async/GetStatus", (string jobId, IJobHostedService jobHostedService) => jobHostedService.GetStatus(jobId));
app.MapGet("/Async/GetResult", (string jobId, IJobHostedService jobHostedService) => jobHostedService.GetResult(jobId));
app.MapPut("/Async/Cancel", (string jobId, IJobHostedService jobHostedService) => jobHostedService.Cancel(jobId));

// Add Example API endpoints

app.MapGet("/Example/GetFibonacciNumber", async (ulong index, string webhookUrl, IJobHostedService jobHostedService) => {
    var job = new FibonacciJob { Index = index }; 
    var jobId = await jobHostedService.Run(job, webhookUrl);
    return jobId;
});

app.MapGet("/Example/GetError", async (string webhookUrl, IJobHostedService jobHostedService) => {
    var job = new ErrorJob();
    var jobId = await jobHostedService.Run(job, webhookUrl);
    return jobId;
});

app.Run();