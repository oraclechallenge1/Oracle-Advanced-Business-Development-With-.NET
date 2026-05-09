using System.Reflection;
using System.Timers;
using HealthChecks.UI.Client;
using MedSave.Context;
using MedSave.Repositories;
using MedSave.Services.Manufacturer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MedSave.Repositories.Healthcare_Providers;
using MedSave.Repositories.Healthcare_Providers.Interfaces;
using MedSave.Services.HealthcareProviders;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
{
    options.ConnectionString = builder.Configuration["AzureMonitor:ConnectionString"]; 
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedSave API",
        Version = "v1",
        Description = "API RESTful da solução MedSave para gerenciamento de medicamentos em instuições de saúde."
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: connectionString, 
        name: "oracle-database", 
        failureStatus: HealthStatus.Degraded,
        tags: new[]{"db", "oracle", "sql"},
        timeout: TimeSpan.FromSeconds(10)
    )
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("Application is running"),
        tags: new[] { "api", "self" }
    );

builder.Services.AddHealthChecksUI(options =>
    {
        options.SetEvaluationTimeInSeconds(15);
        options.MaximumHistoryEntriesPerEndpoint(50);
        options.SetApiMaxActiveRequests(1);
        options.AddHealthCheckEndpoint("Health Check General", "/health");
        options.AddHealthCheckEndpoint("Health Check Application", "/health/application");
        options.AddHealthCheckEndpoint("Health Check Database", "/health/database");
    })
    .AddInMemoryStorage();

// ==============================
// DbContext
// ==============================
builder.Services.AddDbContext<MedSaveContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));


// ==============================
// Manufacturer Repositories
// ==============================
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<IAddressManufacturerRepository, AddressManufacturerRepository>();
builder.Services.AddScoped<IContactManufacturerRepository, ContactManufacturerRepository>();

// ==============================
// Healthcare Providers Repositories
// ==============================
builder.Services.AddScoped<IAddressStockRepository, AddressStockRepository>();
builder.Services.AddScoped<IHealthcareProvidersRepository, HealthcareProvidersRepository>();
builder.Services.AddScoped<IProviderTypeRepository, ProviderTypeRepository>();

// ==============================
// Services
// ==============================
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IHealthcareProvidersService, HealthcareProvidersService>();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/application", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("self"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/database", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MedSave API v1");
    c.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return Serilog.Events.LogEventLevel.Error;

        if (httpContext.Response.StatusCode >= 400)
            return Serilog.Events.LogEventLevel.Warning;

        return Serilog.Events.LogEventLevel.Information;
    };
});

app.MapControllers();

app.Run();

public partial class Program { }