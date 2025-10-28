using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.RoATPService.Application.Api.AppStart;
using SFA.DAS.RoATPService.Application.Api.Extensions;
using SFA.DAS.RoATPService.Application.Api.Infrastructure;
using SFA.DAS.RoATPService.Application.Api.Middleware;
using SFA.DAS.RoATPService.Application.AppStart;
using SFA.DAS.RoATPService.Data.Extensions;
using SFA.DAS.Telemetry.Startup;

var builder = WebApplication.CreateBuilder(args);

IConfiguration _configuration = builder.Configuration.LoadConfiguration();
builder.Services.AddSingleton(_configuration);

builder.Services
    .AddAuthentication(_configuration)
    .AddRoatpDataContext(_configuration[ConfigurationConstants.SqlConnectionString])
    .AddOptions()
    .AddLogging()
    .AddApplicationInsightsTelemetry()
    .AddOpenTelemetry(_configuration)
    .AddTelemetryNotFoundAsSuccessfulResponse()
    .AddServiceRegistrations(_configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        //options.SchemaFilter<JsonPatchDocumentSchemaFilter>();
        options.SwaggerDoc(PolicyNames.Default, new OpenApiInfo { Title = "ROATP API" });
        options.CustomSchemaIds(type => type.FullName);
    });

builder.Services
    .AddMvc(options =>
    {
        if (_configuration.IsLocalEnvironment())
        {
            options.Filters.Add(new AllowAnonymousFilter());
        }
        else
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireRole(Roles.RoATPServiceInternalAPI)
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }
    })
    .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()))
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSwagger(c => c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{PolicyNames.Default}/swagger.json", PolicyNames.Default);
    options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
    });

app.UseHealthChecks("/ping",
    new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
    });

if (_configuration.IsLocalEnvironment())
{
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

await app.RunAsync();
