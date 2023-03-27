using MinimalAsyncApi.Jobs.Examples.Error;
using MinimalAsyncApi.Jobs.Examples.Fibonacci;
using MinimalAsyncApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddJobServices();
var app = builder.Build();

// Add Async API Endpoints

app.MapGet("/Async/GetStatus", (string jobId, IJobHostedService jobHostedService) => jobHostedService.GetStatus(jobId));
app.MapGet("/Async/GetResult", (string jobId, IJobHostedService jobHostedService) => jobHostedService.GetResult(jobId));
app.MapPut("/Async/Cancel", (string jobId, IJobHostedService jobHostedService) => jobHostedService.Cancel(jobId));

// Add Example Endpoints

app.MapGet("/Example/GetFibonacciNumber", (ulong index, string webhookUrl, IJobHostedService jobHostedService) => {
    var job = new FibonacciJob { Index = index }; 
    var jobId = jobHostedService.Run(job, webhookUrl);
    return jobId;
});

app.MapGet("/Example/GetError", (string webhookUrl, IJobHostedService jobHostedService) => {
    var job = new ErrorJob();
    var jobId = jobHostedService.Run(job, webhookUrl);
    return jobId;
});

app.Run();