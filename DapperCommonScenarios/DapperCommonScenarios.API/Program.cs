using DapperCommonScenarios.API.Extensions;
using DapperCommonScenarios.API.Middleware;
using DapperCommonScenarios.API.Models;
using DapperCommonScenarios.API.Services.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

try
{
    Log.Information("{ProjectName} - Starting API Service", projectName);
    var builder = WebApplication.CreateBuilder(args);

    ConfigureServices(builder);

    var app = builder.Build();

    ConfigureMiddleware(app, app.Services);
    ConfigureEndpoints(app, app.Services);

    app.Run("http://*:9999/");

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "{ProjectName} - API Service: Host terminated unexpectedly", projectName);
    return -1;
}
finally
{
    Log.CloseAndFlush();
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Logging.AddSerilog();

    builder.Services.Configure<AppSettings>(builder.Configuration);

    builder.Services.AddWithValidation<AppSettings, AppSettingsValidator>();

    builder.Services.AddSingleton(resolver =>
        resolver.GetRequiredService<IOptions<AppSettings>>().Value);

    var appSettings = builder.Configuration.Get<AppSettings>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CORS",
            corsPolicyBuilder => corsPolicyBuilder.WithOrigins(appSettings.CorsOriginList)
                                                   .AllowAnyMethod()
                                                   .AllowAnyHeader()
                                                   .AllowCredentials());
    });

    builder.Services.AddScoped<BlogService>();

    builder.Services.AddSwaggerGen(swagger =>
    {
        swagger.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DapperCommonScenarios.API",
            Version = "v1"
        });
    });

    builder.Services
        .AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.UseMemberCasing();
        });
};

void ConfigureMiddleware(WebApplication app, IServiceProvider services)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DapperCommonScenarios.API V1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseExceptionHandlingMiddleware();
    app.UseCors("CORS");

    app.UseRouting();
}

void ConfigureEndpoints(IEndpointRouteBuilder app, IServiceProvider services)
{
    app.MapControllers();
}

