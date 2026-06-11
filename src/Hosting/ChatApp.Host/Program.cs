
using Identity;
using Platform.Contracts.Modules;
using Platform.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddModules(
    builder.Configuration,
    typeof(IdentityModule).Assembly);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapModules();

app.MapGet("/", () => "Hello World!");

app.Run();