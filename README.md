# minimal-async-api

This project implements an example asynchronous API in ASP.NET Core. In this context, asynchronous is not referring to C# async/await (though it is used), but to building an API capable of executing long-running jobs and returning their results asynchronously. Results can be accessed by supplying a webhook callback or by polling job status.

Project features include:

- ASP.NET Core implementation using the Minimal API for endpoints and the `IHostedService` interface for background tasks
- Horizontal API scaling via Redis state storage
- Endpoints for running example jobs, retrieving a job's status and result, and cancelling a job
- Webhook support to POST job results to a callback
- `docker-compose.yml` file to run an example container deployment using nginx, 3 API services, and Redis

# Build

Run `docker-compose build`

# Run

Run `docker-compose up -d`

# Stop

Run `docker-compose down`
