using Chat;
using Identity;
using Platform.Akka;
using Platform.Auth.DependencyInjection;
using Platform.Contracts.Modules;
using Platform.Http;
using Platform.Persistence.DependencyInjection;
using Platform.Realtime;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Logging.AddFilter(
    "Microsoft.AspNetCore.SignalR",
    LogLevel.Debug);

builder.Logging.AddFilter(
    "Microsoft.AspNetCore.Http.Connections",
    LogLevel.Debug);

builder.Services.AddCustomSwagger();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCorsConfiguration();

builder.Services.AddAkkaPlatform(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddPlatformSignalR();

builder.Services.AddModules(
    builder.Configuration,
    typeof(IdentityModule).Assembly,
    typeof(ChatModule).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors(CorsDependencyInjectionExtensions.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapModules();
app.MapPlatformSignalR();

app.MapGet("/health", () => "Healthy!");

app.Run();