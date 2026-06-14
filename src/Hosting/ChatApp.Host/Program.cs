
using Identity;
using Platform.Auth.DependencyInjection;
using Platform.Contracts.Modules;
using Platform.Http;
using Platform.Persistence.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCustomSwagger();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddCorsConfiguration();
builder.Services.AddProblemDetails();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);

builder.Services.AddModules(
    builder.Configuration,
    typeof(IdentityModule).Assembly);

var app = builder.Build();

app.UseCors(CorsDependencyInjectionExtensions.PolicyName);

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapModules();

app.MapGet("/health", () => "Healthy!");

app.Run();