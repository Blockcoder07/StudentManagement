using Microsoft.OpenApi.Models;

namespace StudentManagement.API.Extensions;

public static class SwaggerServiceExtensions
{
    #region Public Methods

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection objServices)
    {
        objServices.AddEndpointsApiExplorer();
        objServices.AddSwaggerGen(opts =>
        {
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Student Management API",
                Version = "v1",
                Description = "Production-ready Student Management Web API secured with JWT bearer tokens.",
                Contact = new OpenApiContact { Name = "Student Management Team" }
            });

            OpenApiSecurityScheme objJwtScheme = new()
            {
                Name = "Authorization",
                Description = "Enter your JWT token below. The 'Bearer ' prefix is added automatically.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };

            opts.AddSecurityDefinition(objJwtScheme.Reference.Id, objJwtScheme);
            opts.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { objJwtScheme, Array.Empty<string>() }
            });
        });

        return objServices;
    }

    #endregion
}
