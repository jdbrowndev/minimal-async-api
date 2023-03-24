using MinimalAsyncApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddJobServices();
builder.Services.AddSingleton<Random>(_ => new Random());
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
