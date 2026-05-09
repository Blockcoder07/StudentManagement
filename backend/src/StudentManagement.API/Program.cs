using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StudentManagement.API.Extensions;
using StudentManagement.API.Filters;
using StudentManagement.API.Middleware;
using StudentManagement.Application;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.Common.Responses;
using StudentManagement.Domain.Enums;
using StudentManagement.Infrastructure;
using StudentManagement.Infrastructure.Persistence.Seed;

WebApplicationBuilder objBuilder = WebApplication.CreateBuilder(args);

objBuilder.Host.UseSerilog((ctx, sp, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .ReadFrom.Services(sp)
    .Enrich.FromLogContext());

objBuilder.Services
    .AddInfrastructureServices(objBuilder.Configuration)
    .AddApplicationServices()
    .AddJwtAuthentication(objBuilder.Configuration)
    .AddAngularCors(objBuilder.Configuration)
    .AddSwaggerWithJwt();

objBuilder.Services.AddScoped<ValidationActionFilter>();

objBuilder.Services
    .AddControllers(opts =>
    {
        opts.Filters.AddService<ValidationActionFilter>();
    })
    .ConfigureApiBehaviorOptions(opts =>
    {
        opts.InvalidModelStateResponseFactory = ctx =>
        {
            List<string> lstErrors = ctx.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage))
                .ToList();

            ApiResponse objResponse = ApiResponse.Fail(
                ResponseMessage.ValidationFailed.GetDescription(),
                StatusCodes.Status400BadRequest,
                lstErrors);

            return new BadRequestObjectResult(objResponse);
        };
    });

objBuilder.Services.AddHealthChecks();

WebApplication objApp = objBuilder.Build();

objApp.UseSerilogRequestLogging();

objApp.UseMiddleware<GlobalExceptionMiddleware>();

if (objApp.Environment.IsDevelopment())
{
    objApp.UseSwagger();
    objApp.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
        opts.RoutePrefix = "swagger";
    });
}

objApp.UseHttpsRedirection();

objApp.UseCors(CorsServiceExtensions.StAngularClientPolicy);

objApp.UseAuthentication();
objApp.UseAuthorization();

objApp.MapControllers();

objApp.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        ApiResponse<object> objResponse = ApiResponse<object>.Ok(
            new { status = report.Status.ToString(), totalDuration = report.TotalDuration },
            "Service is healthy.");
        await ctx.Response.WriteAsJsonAsync(objResponse);
    }
});

await SeedDatabaseAsync(objApp);

await objApp.RunAsync();

static async Task SeedDatabaseAsync(WebApplication objApp)
{
    try
    {
        using IServiceScope objScope = objApp.Services.CreateScope();
        DatabaseSeeder objSeeder = objScope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await objSeeder.SeedAsync();
    }
    catch (Exception exMessage)
    {
        ILogger<Program> objLogger = objApp.Services.GetRequiredService<ILogger<Program>>();
        objLogger.LogError(exMessage, "Database seeding failed during startup.");
    }
}
