
using Identity;
using Platform.Contracts.Modules;
using Platform.Persistence.DependencyInjection;
using Platform.Http;
using Serilog;
using Platform.Auth.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCustomSwagger();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);

builder.Services.AddModules(
    builder.Configuration,
    typeof(IdentityModule).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapModules();

app.MapGet("/health", () => "Healthy!");

app.Run();